using System.Collections.Generic;
using Onsharp.Exceptions;

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
        /// Reloads all plugins. If new plugins were added to the plugins folder, the newly added plugins will be loaded, too.
        /// </summary>
        void Reload();
        /// <summary>
        /// Reloads the libraries folder. When reloading the folder, every library will be reinitialized by the .NET runtime.
        /// </summary>
        void ReloadLibs();
    }
}