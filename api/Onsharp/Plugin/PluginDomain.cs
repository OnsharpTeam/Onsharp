using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Onsharp.IO;
using Onsharp.Native;

namespace Onsharp.Plugin
{
    /// <summary>
    /// The plugin domain is controlling its wrapped plugin.
    /// It also manages the underlying assembly and interacts with it, if needed.
    /// </summary>
    internal class PluginDomain : AssemblyLoadContext
    {
        private static readonly Type EntryPointType = typeof(IEntryPoint);
        private static readonly Type PluginType = typeof(IPlugin);
        private readonly DomainLoadContext _context;
        private readonly PluginManager _pluginManager;
        private Assembly _assembly;
        
        /// <summary>
        /// The path to the plugins library.
        /// </summary>
        internal string Path { get; }
        
        /// <summary>
        /// All entry points of the plugin which is owned by this domain.
        /// </summary>
        internal List<IEntryPoint> EntryPoints { get; private set; }

        /// <summary>
        /// the plugin main class of the plugin which is owned by this domain.
        /// </summary>
        internal IPlugin Plugin { get; private set; }

        internal PluginDomain(PluginManager pluginManager, string path)
        {
            Path = path;
            _pluginManager = pluginManager;
            _context = new DomainLoadContext(path);
        }

        /// <summary>
        /// Initializes the plugin and loads its assembly but does not start it.
        /// To start the plugin use <see cref="Start"/>
        /// </summary>
        internal void Initialize()
        {
            EntryPoints = new List<IEntryPoint>();
            _assembly = _context.Load();
            foreach (Type type in _assembly.GetExportedTypes())
            {
                if (PluginType.IsAssignableFrom(type))
                {
                    PluginMeta meta = type.GetCustomAttribute<PluginMeta>();
                    if (meta == null)
                    {
                        Bridge.Logger.Fatal(
                            "Onsharp found a plugin class {CLASS} in the plugin on path \"{PATH}\" which does not have a meta descriptor!",
                            type.FullName, Path);
                    }
                    else
                    {
                        Plugin = TryCreatePlugin(type);
                        if (Plugin != null)
                        {
                            Plugin.Data = new DataStorage(Plugin);
                            Plugin.FilePath = Path;
                            Plugin.Meta = meta;
                            Plugin.Logger = new Logger(Plugin.Display, meta.IsDebug);
                            Plugin.State = PluginState.Unknown;
                            EntryPoints.Add(Plugin);
                        }
                        else
                        {
                            Bridge.Logger.Fatal(
                                "Onsharp tried to instantiate the plugin class {CLASS} in the plugin on path \"{PATH}\" but failed! Does the class have a default constructor?",
                                type.FullName, Path);
                        }
                    }
                }
                else if (EntryPointType.IsAssignableFrom(type))
                {
                    EntryPoints.Add((IEntryPoint) Activator.CreateInstance(type));
                }
            }

            ChangePluginState(PluginState.Loaded);
        }

        private IPlugin TryCreatePlugin(Type type)
        {
            try
            {
                return (IPlugin) Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Starts the plugin and calls its start callback.
        /// </summary>
        internal void Start()
        {
            try
            {
                Plugin.Logger.Info("Starting plugin {NAME}...", Plugin.Display);
                foreach (IEntryPoint entryPoint in EntryPoints)
                {
                    entryPoint.PluginManager = Bridge.PluginManager;
                }
                
                Plugin.OnStart();
                lock (_pluginManager.Plugins)
                    _pluginManager.Plugins.Add(Plugin);
                ChangePluginState(PluginState.Started);
                Plugin.Logger.Info("Plugin {NAME} successfully started!", Plugin.Display);
            }
            catch (Exception ex)
            {
                Plugin.Logger.Error(ex, "Plugin {NAME} failed to start!", Plugin.Display);
            }
        }

        /// <summary>
        /// Stops the plugin and unloads it.
        /// </summary>
        /// <param name="completely">If true, the domain is getting unregistered and later disposed</param>
        internal void Stop(bool completely)
        {
            try
            {
                Plugin.Logger.Warn("Stopping plugin {NAME}...", Plugin.Display);
                Plugin.OnStop();
                Unload();
                lock (_pluginManager.Plugins)
                    _pluginManager.Plugins.Remove(Plugin);
                ChangePluginState(PluginState.Stopped);
                Plugin.Logger.Info("Plugin {NAME} successfully stopped!", Plugin.Display);
            }
            catch (Exception ex)
            {
                Plugin.Logger.Error(ex, "Plugin {NAME} failed to stop!", Plugin.Display);
            }

            if (!completely) return;
            lock (_pluginManager.Domains)
            {
                _pluginManager.Domains.Remove(this);
            }
        }

        private void ChangePluginState(PluginState state)
        {
            //TODO call plugin state changed event
            Plugin.State = state;
        }
    }
}