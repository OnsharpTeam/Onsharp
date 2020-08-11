using Onsharp.Modules;
using Onsharp.Native;
using Onsharp.Plugins;

namespace Onsharp
{
    /// <summary>
    /// This interface is the entry point for the Onsharp api.
    /// There is no maximum number of classes that may implement this interface.
    /// </summary>
    public abstract class EntryPoint
    {
        /// <summary>
        /// The manager which manages all the plugin instances running currently.
        /// </summary>
        public IPluginManager PluginManager { get; set; }
        
        /// <summary>
        /// The own server instance associated to the plugin.
        /// </summary>
        public IServer Server { get; set; }
        
        /// <summary>
        /// The current Onsharp runtime instance running in the background.
        /// </summary>
        public IRuntime Runtime { get; set; }
        
        /// <summary>
        /// The i18n associated to this plugin.
        /// </summary>
        public I18n I18n { get; set; }
    }
}