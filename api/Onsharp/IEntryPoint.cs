using Onsharp.Plugin;

namespace Onsharp
{
    /// <summary>
    /// This interface is the entry point for the Onsharp api.
    /// There is no maximum number of classes that may implement this interface.
    /// </summary>
    public interface IEntryPoint
    {
        /// <summary>
        /// The manager which manages all the plugin instances running currently.
        /// </summary>
        IPluginManager PluginManager { get; set; }
        
        /// <summary>
        /// The own server instance associated to the plugin.
        /// </summary>
        IServer Server { get; set; }
    }
}