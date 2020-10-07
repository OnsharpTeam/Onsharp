using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Threading;
using Onsharp.IO;
using Onsharp.Modules;
using Onsharp.Native;
using Onsharp.Updater;

namespace Onsharp.Plugins
{
    /// <summary>
    /// The plugin domain is controlling its wrapped plugin.
    /// It also manages the underlying assembly and interacts with it, if needed.
    /// </summary>
    internal class PluginDomain
    {
        private static readonly Type EntryPointType = typeof(EntryPoint);
        private static readonly Type PluginType = typeof(Plugin);
        private Assembly _assembly;
        
        /// <summary>
        /// The current instance of the plugin manager.
        /// </summary>
        internal PluginManager PluginManager { get; }
        
        /// <summary>
        /// If there is an update available, this property contains the updating data.
        /// </summary>
        internal UpdatingData UpdatingData { get; private set; }
        
        /// <summary>
        /// The path to the plugins library.
        /// </summary>
        internal string Path { get; }

        /// <summary>
        /// All entry points of the plugin which is owned by this domain.
        /// </summary>
        internal List<EntryPoint> EntryPoints { get; private set; }

        /// <summary>
        /// the plugin main class of the plugin which is owned by this domain.
        /// </summary>
        internal Plugin Plugin { get; private set; }
        
        internal I18n I18n { get; private set; }

        /// <summary>
        /// The server owned by this domain and the plugin.
        /// </summary>
        internal Server Server { get; private set; }
        
        /// <summary>
        /// The provider of the package, if set.
        /// </summary>
        internal PackageProvider PackageProvider { get; private set; }

        internal PluginDomain(PluginManager pluginManager, string path)
        {
            Path = path;
            PluginManager = pluginManager;
        }
        
        /// <summary>
        /// Initializes the plugin and loads its assembly but does not start it.
        /// To start the plugin use <see cref="Start"/>
        /// </summary>
        internal void Initialize()
        {
            Bridge.Logger.Info("Loading plugin on path \"{PATH}\"...", Path);
            Server = new Server(this);
            EntryPoints = new List<EntryPoint>();
            _assembly = PluginManager.Context.Load(Path);
            if (_assembly == null)
            {
                ChangePluginState(PluginState.Failed);
                Bridge.Logger.Fatal(
                    "Could not finish the load process of the plugin on path \"{PATH}\": An assembly is already loaded in the context!", Path);
                return;
            }
            
            foreach (Type type in _assembly.GetExportedTypes())
            {
                if (PluginType.IsAssignableFrom(type))
                {
                    PluginMeta meta = type.GetCustomAttribute<PluginMeta>();
                    if (meta == null)
                    {
                        ChangePluginState(PluginState.Failed);
                        Bridge.Logger.Fatal(
                            "Onsharp found a plugin class {CLASS} in the plugin on path \"{PATH}\" which does not have a meta descriptor!",
                            type.FullName, Path);
                        return;
                    }

                    if (string.Equals(meta.Id, "native", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Bridge.Logger.Fatal(
                            "Onsharp found a plugin class {CLASS} in the plugin on path \"{PATH}\" which has native as plugin id which is not allowed!",
                            type.FullName, Path);
                        return;
                    }

                    AutoUpdaterAttribute updateAttribute = type.GetCustomAttribute<AutoUpdaterAttribute>();
                    if (updateAttribute != null)
                    {
                        UpdatingData = AutoUpdater.RetrieveData(updateAttribute.Url);
                        if (meta.Version == UpdatingData.Version)
                        {
                            UpdatingData = null;
                        }
                    }

                    Plugin = TryCreatePlugin(type);
                    if (Plugin != null)
                    {
                        Plugin.Meta = meta;
                        Plugin.FilePath = Path;
                        Plugin.Data = new DataStorage(Plugin);
                        Plugin.Logger = new Logger(Plugin.Display, meta.IsDebug);
                        Plugin.State = PluginState.Unknown;
                        EntryPoints.Add(Plugin);

                        PackageProvider = TryCreatePackageProvider(meta.PackageProvider);
                        if (PackageProvider == null) continue;
                        PackageProvider.Author ??= meta.Author;
                        PackageProvider.Version ??= meta.Version;
                        PackageProvider.Name ??= Plugin.Display;
                        PackageProvider.Name = Regex.Replace(PackageProvider.Name, "[^0-9A-Za-z]", "");
                    }
                    else
                    {
                        ChangePluginState(PluginState.Failed);
                        Bridge.Logger.Fatal(
                            "Onsharp tried to instantiate the plugin class {CLASS} in the plugin on path \"{PATH}\" but failed! Does the class have a default constructor?",
                            type.FullName, Path);
                        return;
                    }
                }
                else if (EntryPointType.IsAssignableFrom(type))
                {
                    EntryPoint entryPoint = (EntryPoint) Activator.CreateInstance(type);
                    if (entryPoint == null)
                    {
                        Bridge.Logger.Fatal(
                            "Could not instantiate the entry class {CLASS} in the plugin on path \"{PATH}\"! Does it has a default constructor?",
                            type.FullName, Path);
                        continue;    
                    }
                    
                    EntryPoints.Add(entryPoint);
                }
            }

            if (Plugin == null)
            {
                ChangePluginState(PluginState.Failed);
                Bridge.Logger.Fatal(
                    "Could not finish the load process of the plugin on path \"{PATH}\": There is no valid plugin main class!", Path);
                return;
            }

            if (Plugin.Meta.ApiVersion < Bridge.ApiVersion)
            {
                ChangePluginState(PluginState.Failed);
                Bridge.Logger.Fatal(
                    "The plugin failed on the api version check! The plugin {PLUGIN} uses the api v{V1} but your runtime runs with v{VR}, the runtime version is too new!",
                    Plugin.Display, Plugin.Meta.ApiVersion, Bridge.ApiVersion);
                return;
            }

            I18n = Plugin.Meta.I18n == I18n.Mode.Disabled ? null : new I18n(Plugin.Logger, _assembly, Plugin);
            Server.Inject();
            foreach (EntryPoint entryPoint in EntryPoints)
            {
                entryPoint.Server = Server;
                entryPoint.PluginManager = Bridge.PluginManager;
                entryPoint.I18n = I18n;
                entryPoint.Runtime = Bridge.Runtime;
                entryPoint.Runtime.RegisterConsoleCommands(entryPoint, Plugin.Meta.Id);
                entryPoint.Server.RegisterExportable(entryPoint);
                entryPoint.Server.RegisterRemoteEvents(entryPoint);
                entryPoint.Server.RegisterServerEvents(entryPoint);
                entryPoint.Server.RegisterCommands(entryPoint);
            }

            ChangePluginState(PluginState.Loaded);
        }

        private Plugin TryCreatePlugin(Type type)
        {
            try
            {
                return (Plugin) Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        private PackageProvider TryCreatePackageProvider(Type type)
        {
            if (type == null) return null;
            try
            {
                return (PackageProvider) Activator.CreateInstance(type);
            }
            catch(Exception e)
            {
                Bridge.Logger.Error(e, "Could not create package provider {T} because of an error!", type.FullName);
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
                Plugin.Logger.Info("Initializing plugin {NAME} v{VERSION} by {AUTHOR}...", Plugin.Display,
                        Plugin.Meta.Version, Plugin.Meta.Author);
                Plugin.OnInitialize();
                I18n.Initialize();
                Plugin.Logger.Info("Starting...");
                Plugin.OnStart();
                lock (PluginManager.Plugins)
                    PluginManager.Plugins.Add(Plugin);
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
        internal void Stop()
        {
            try
            {
                Plugin.Logger.Warn("Stopping plugin {NAME}...", Plugin.Display);
                Plugin.OnStop();
                lock (PluginManager.Plugins)
                    PluginManager.Plugins.Remove(Plugin);
                ChangePluginState(PluginState.Stopped);
                Plugin.Logger.Info("Plugin {NAME} successfully stopped!", Plugin.Display);
            }
            catch (Exception ex)
            {
                Plugin.Logger.Error(ex, "Plugin {NAME} failed to stop!", Plugin.Display);
            }

            lock (PluginManager.Domains)
            {
                PluginManager.Domains.Remove(this);
            }
        }

        private void ChangePluginState(PluginState state)
        {
            Plugin.State = state;
            Bridge.Logger.Debug("Plugin {ID} changed state to {STATE}", Path, Enum.GetName(typeof(PluginState), state));
        }
    }
}