using System.Collections.Generic;

namespace Onsharp.Plugins
{
    /// <summary>
    /// The manager which manages every plugin running on the server.
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Returns a list with all plugins which are currently managed by this plugin manager.
        /// </summary>
        IReadOnlyList<Plugin> GetAllPlugins();

        /// <summary>
        /// Gets the plugin of the wanted plugin meta id.
        /// </summary>
        /// <param name="id">The wanted plugin meta id</param>
        /// <returns>The plugin class of the wanted plugin</returns>
        Plugin GetPlugin(string id);

        /// <summary>
        /// Loads and starts the plugin on the given path.
        /// </summary>
        /// <param name="name">The name of the plugin file in the plugins folder</param>
        void Start(string name);

        /// <summary>
        /// Stops the given plugin and unloads it completely.
        /// Stopping a plugin which is dependency of another running plugin, will cancel the stopping process due to a dependency lock.
        /// </summary>
        /// <param name="plugin">The plugin to be stopped</param>
        void Stop(Plugin plugin);

        /// <summary>
        /// Restarts the plugin: Stops the plugin and unloads it. After the plugin is unloaded, the plugin gets loaded again and started.
        /// </summary>
        /// <param name="plugin"></param>
        void Restart(Plugin plugin);

        /// <summary>
        /// Reloads the libraries folder. When reloading the folder, every library will be reinitialized by the .NET runtime.
        /// </summary>
        void ReloadLibs();
    }
}