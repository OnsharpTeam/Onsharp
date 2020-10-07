using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onsharp.Commands;
using Onsharp.Native;
using Onsharp.Updater;
using Onsharp.Utils;

namespace Onsharp.Plugins
{
    internal class PluginManager : IPluginManager
    {
        public List<Plugin> Plugins { get; }
        
        internal List<PluginDomain> Domains { get; private set; }

        internal PluginLoadContext Context { get; private set; }

        private bool _isLoaded;

        internal PluginManager()
        {
            Plugins = new List<Plugin>();
            Domains = new List<PluginDomain>();
            Context = new PluginLoadContext();
            ReloadLibs();
            Reload();
        }

        /// <summary>
        /// Adds a package to the server config object if not existing.
        /// </summary>
        /// <param name="name"></param>
        private void AddPackage(string name)
        {
            JArray array = Bridge.ServerConfig["packages"] as JArray;
            if (array == null) return;
            if (array.Any(t => t.Value<string>() == name)) return;
            array.Add(name);
            Onset.StartPackage(name);
        }

        public IReadOnlyList<Plugin> GetAllPlugins()
        {
            return Plugins.AsReadOnly();
        }

        internal void IteratePlugins(Action<Plugin> callback)
        {
            lock (Plugins)
            {
                for (int i = Plugins.Count - 1; i >= 0; i--)
                {
                    Plugin plugin = Plugins[i];
                    callback.Invoke(plugin);
                }
            }
        }

        internal PluginDomain GetDomain(string id)
        {
            lock (Domains)
            {
                return Domains.SelectFirst(domain => domain.Plugin.Meta.Id == id);
            }
        }

        public Plugin GetPlugin(string id)
        {
            lock (Plugins)
            {
                for (int i = Plugins.Count - 1; i >= 0; i--)
                {
                    Plugin plugin = Plugins[i];
                    if (plugin.Meta.Id == id)
                    {
                        return plugin;
                    }
                }
            }

            return null;
        }

        public void Start(string name)
        {
            string path = Path.Combine(Bridge.PluginsPath, name + ".dll");
            CreateDomain(path)?.Start();
        }

        public void Reload()
        {
            if (_isLoaded)
            {
                Unload();
                Context = new PluginLoadContext();
            }

            _isLoaded = true;
            bool reloadAfter = false;
            List<PluginDomain> domainCache = new List<PluginDomain>();
            foreach (string pluginPath in Directory.GetFiles(Bridge.PluginsPath))
            {
                if(!Path.GetExtension(pluginPath).ToLower().Contains("dll")) continue;
                PluginDomain domain = new PluginDomain(this, pluginPath);
                domain.Initialize();
                if (domain.Plugin.State == PluginState.Failed)
                    continue;
                if (domain.UpdatingData != null)
                {
                    if (!Bridge.Config.KeepPluginsUpdated)
                    {
                        Bridge.Logger.Warn("There is an update available for {PLUGIN} ({OLD} -> {NEW})!",
                            domain.Plugin.Display, domain.Plugin.Meta.Version, domain.UpdatingData.Version);
                    }
                    else
                    {
                        Bridge.Logger.Warn("There is an update available for {PLUGIN} ({OLD} -> {NEW})! Starting updater...",
                            domain.Plugin.Display, domain.Plugin.Meta.Version, domain.UpdatingData.Version);
                        AutoUpdater updater = new AutoUpdater(domain);
                        updater.Start();
                        reloadAfter = true;
                    }
                }

                if (domain.PackageProvider != null)
                {
                    domain.Plugin.Logger.Info("Found a package provider! Exporting package...");
                    domain.PackageProvider.Export(Path.Combine(Bridge.PackagePath, domain.PackageProvider.Name));
                    AddPackage(domain.PackageProvider.Name);
                    domain.Plugin.Logger.Info("Package {NAME} successfully provided!", domain.PackageProvider.Name);
                }
                
                domainCache.Add(domain);
            }

            if (reloadAfter)
            {
                Reload();
                return;
            }

            File.WriteAllText(Path.Combine(Bridge.ServerPath, "server_config.json"),
                Bridge.ServerConfig.ToString(Formatting.Indented));
            
            PrioritizedList list = new PrioritizedList(domainCache);
            foreach (PluginDomain handle in domainCache)
            {
                if (handle.Plugin.Meta.Dependencies.Length > 0)
                {
                    foreach (string parent in handle.Plugin.Meta.Dependencies)
                    {
                        list.Increment(handle, list.GetRise(parent));
                    }
                }
            }

            domainCache = list.Convert();
            Domains = new List<PluginDomain>();
            foreach (PluginDomain domain in domainCache)
            {
                if (domain.Plugin.Meta.Id == "_global" || domain.Plugin.Meta.Name == "_global")
                {
                    Bridge.Logger.Fatal(
                        "Could not load plugin {ID} on path \"{PATH}\" because either the id or the name has is a blacklisted word!",
                        domain.Plugin.Meta.Id, domain.Path);
                    return;
                }

                Plugin otherPlugin = GetPlugin(domain.Plugin.Meta.Id);
                if (otherPlugin != null)
                {
                    Bridge.Logger.Warn(
                        "Could not loading plugin {ID} on path \"{PATH}\" because the id is already occupied by the plugin {ID2} on the path {PATH2}!",
                        domain.Plugin.Meta.Id, domain.Path, otherPlugin.Meta.Id, otherPlugin.FilePath);
                    continue;
                }
                
                Domains.Add(domain);
                domain.Start();
            }
        }

        internal void Unload()
        {
            Context.UnloadAndClean();
            Plugins.Clear();
            Domains.Clear();
            CommandInfo.Reset();
            Bridge.OccupiedCommandNames.Clear();
            Bridge.OccupiedConsoleCommandNames.Clear();
            Bridge.ConsoleManager.Reset();
            
            _isLoaded = false;
        }

        internal void ForceStop(Plugin plugin)
        {
            PluginDomain domain = GetDomain(plugin);
            if (domain != null)
            {
                domain.Stop();
            }
            else
            {
                Bridge.Logger.Fatal("No domain found for plugin {ID}!", plugin.Meta.Id);
            }
        }

        public void ReloadLibs()
        {
            Bridge.Logger.Debug("Reloading third-party libraries...");
            int reloadedCount = 0;
            foreach (string libPath in Directory.GetFiles(Bridge.LibsPath))
            {
                try
                {
                    Assembly.LoadFrom(libPath);
                    reloadedCount++;
                }
                catch(Exception ex)
                {
                    Bridge.Logger.Error(ex, "Could not reload third-party library!");
                }
            }
            Bridge.Logger.Info("{COUNT} third-party libraries were reloaded!", reloadedCount);
        }

        private PluginDomain CreateDomain(string path)
        {
            lock (Domains)
            {
                for (int i = Domains.Count - 1; i >= 0; i--)
                {
                    PluginDomain other = Domains[i];
                    if (other.Path == path)
                        return other;
                }
                
                PluginDomain domain = new PluginDomain(this, path);
                domain.Initialize();
                if (domain.Plugin.State == PluginState.Failed)
                    return null;
                Plugin otherPlugin = GetPlugin(domain.Plugin.Meta.Id);
                if (otherPlugin != null)
                {
                    Bridge.Logger.Warn(
                        "Could not loading plugin {ID} on path \"{PATH}\" because the id is already occupied by the plugin {ID2} on the path {PATH2}!",
                        domain.Plugin.Meta.Id, domain.Path, otherPlugin.Meta.Id, otherPlugin.FilePath);
                    return null;
                }
                
                Domains.Add(domain);
                return domain;
            }
        }

        internal PluginDomain GetDomain(Plugin plugin)
        {
            lock (Domains)
            {
                for (int i = Domains.Count - 1; i >= 0; i--)
                {
                    PluginDomain other = Domains[i];
                    if (other.Plugin == plugin)
                        return other;
                }
            }

            return null;
        }

        private class PrioritizedList
        {
            private List<PrioritizedItem> Items { get; }

            internal PrioritizedList(List<PluginDomain> input)
            {
                Items = new List<PrioritizedItem>();
                foreach (PluginDomain handle in input)
                {
                    Items.Add(new PrioritizedItem(handle));
                }
            }

            internal void Increment(PluginDomain handle, int value)
            {
                foreach (PrioritizedItem item in Items)
                {
                    if (item.Handle.Plugin.Meta.Id == handle.Plugin.Meta.Id)
                    {
                        item.Value += value;
                    }
                    else if (item.Handle.Plugin.Meta.Dependencies.Contains(handle.Plugin.Meta.Id))
                    {
                        Increment(item.Handle, value + 1);
                    }
                }
            }

            internal int GetRise(string parent)
            {
                foreach (PrioritizedItem item in Items)
                {
                    if (item.Handle.Plugin.Meta.Id == parent)
                    {
                        return item.Value + 1;
                    }
                }
                return 1;
            }

            internal List<PluginDomain> Convert()
            {
                return Items.OrderBy(o => o.Value).Select(item => item.Handle).ToList();
            }
        }

        private class PrioritizedItem
        {
            internal int Value { get; set; }

            internal PluginDomain Handle { get; }

            internal PrioritizedItem(PluginDomain handle)
            {
                Value = 0;
                Handle = handle;
            }
        }
    }
}