using Onsharp.IO;

namespace Onsharp.Plugins
{
    /// <summary>
    /// This interface defines the main plugin class. Only one class is allowed to implement this interface.
    /// The plugin interface inherited the <see cref="IEntryPoint"/> interface and all its functionality.
    /// </summary>
    public abstract class Plugin : IEntryPoint
    {
        /// <summary>
        /// The meta of this plugin.
        /// </summary>
        public PluginMeta Meta { get; internal set; }
        
        /// <summary>
        /// The current state of this plugin.
        /// </summary>
        public PluginState State { get; internal set; }
        
        /// <summary>
        /// The path to the library file containing this plugin.
        /// </summary>
        public string FilePath { get; internal set; }
        
        /// <summary>
        /// The interface for interacting with the data storage which belongs to this plugin.
        /// </summary>
        public IDataStorage Data { get; internal set; }
        
        /// <summary>
        /// The specialized logger for this plugin.
        /// </summary>
        public ILogger Logger { get; internal set; }

        /// <summary>
        /// The display name of this plugin. Either the name or the plugin id.
        /// </summary>
        public string Display => string.IsNullOrEmpty(Meta.Name) ? Meta.Id : Meta.Name;

        /// <summary>
        /// Gets called when the plugin is getting enabled.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Gets called when the plugin is getting disabled.
        /// </summary>
        public abstract void OnStop();

        public IPluginManager PluginManager { get; set; }
        
        public IServer Server { get; set; }
    }
}