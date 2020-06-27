using Onsharp.IO;

namespace Onsharp.Plugin
{
    /// <summary>
    /// This interface defines the main plugin class. Only one class is allowed to implement this interface.
    /// The plugin interface inherited the <see cref="IEntryPoint"/> interface and all its functionality.
    /// </summary>
    public interface IPlugin : IEntryPoint
    {
        /// <summary>
        /// The meta of this plugin.
        /// </summary>
        PluginMeta Meta { get; set; }
        
        /// <summary>
        /// The current state of this plugin.
        /// </summary>
        PluginState State { get; set; }
        
        /// <summary>
        /// The path to the library file containing this plugin.
        /// </summary>
        string FilePath { get; set; }
        
        /// <summary>
        /// The interface for interacting with the data storage which belongs to this plugin.
        /// </summary>
        IDataStorage Data { get; set; }
        
        /// <summary>
        /// The specialized logger for this plugin.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// The display name of this plugin. Either the name or the plugin id.
        /// </summary>
        string Display => string.IsNullOrEmpty(Meta.Name) ? Meta.Id : Meta.Name;

        /// <summary>
        /// Gets called when the plugin is getting enabled.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Gets called when the plugin is getting disabled.
        /// </summary>
        void OnStop();
    }
}