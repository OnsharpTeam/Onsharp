using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Onsharp.Native;

namespace Onsharp.Plugin
{
    internal class PluginManager : IPluginManager
    {
        public List<IPlugin> Plugins { get; }
        
        internal List<PluginDomain> Domains { get; }

        internal PluginManager()
        {
            Plugins = new List<IPlugin>();
            Domains = new List<PluginDomain>();
            ReloadLibs();
            List<PluginDomain> domainCache = new List<PluginDomain>();
            foreach (string pluginPath in Directory.GetFiles(Bridge.PluginsPath))
            {
                PluginDomain domain = new PluginDomain(this, pluginPath);
                domain.Initialize();
                domainCache.Add(domain);
            }

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

                IPlugin otherPlugin = GetPlugin(domain.Plugin.Meta.Id);
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

        public IPlugin GetPlugin(string id)
        {
            for (int i = Plugins.Count - 1; i >= 0; i--)
            {
                IPlugin plugin = Plugins[i];
                if (plugin.Meta.Id == id)
                {
                    return plugin;
                }
            }

            return null;
        }

        public void Start(string name)
        {
            string path = Path.Combine(Bridge.PluginsPath, name + ".dll");
            CreateDomain(path)?.Start();
        }

        public void Stop(IPlugin plugin)
        {
            //TODO check if this is not longer a dependency
            ForceStop(plugin);
        }
        
        internal void ForceStop(IPlugin plugin)
        {
            PluginDomain domain = GetDomain(plugin);
            if (domain != null)
            {
                domain.Stop(true);
            }
            else
            {
                Bridge.Logger.Fatal("No domain found for plugin {ID}!", plugin.Meta.Id);
            }
        }

        public void Restart(IPlugin plugin)
        {
            PluginDomain domain = GetDomain(plugin);
            if (domain != null)
            {
                domain.Stop(false);
                domain.Initialize();
                domain.Start();
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
                    Assembly.LoadFile(libPath);
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
                if (GetPlugin(domain.Plugin.Meta.Id) != null)
                {
                    //TODO warning that there is a duplicate plugin id
                    return null;
                }
                
                Domains.Add(domain);
                return domain;
            }
        }

        private PluginDomain GetDomain(IPlugin plugin)
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