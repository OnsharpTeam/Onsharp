using System.Collections.Generic;

namespace Onsharp.Exceptions
{
    /// <summary>
    /// This exception gets thrown when a plugin is getting stopped but there are other plugins
    /// which still needs the stopping plugin as a dependency.
    /// </summary>
    public class PluginNeededAsDependencyException : PluginException
    {
        /// <summary>
        /// The plugin id of the plugin which is being stopped.
        /// </summary>
        public string StoppingPlugin { get; }
        
        /// <summary>
        /// A list of plugin ids containing plugins which still needs the stopping plugin.
        /// </summary>
        public List<string> Dependencies { get; }        
        
        internal PluginNeededAsDependencyException(string stoppingPlugin, List<string> dependencies) 
            : base("The stopping plugin (" + stoppingPlugin + ") is locked due to dependency usage of " + dependencies + " other plugins")
        {
            StoppingPlugin = stoppingPlugin;
            Dependencies = dependencies;
        }
    }
}