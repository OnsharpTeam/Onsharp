using System;
using Onsharp.Modules;
using Onsharp.Native;

namespace Onsharp.Plugins
{
    /// <summary>
    /// The meta attribute offers basic information about the plugin
    /// and its functionality.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginMeta : Attribute
    {
        /// <summary>
        /// The id of the plugin. The id must be unique, otherwise the plugin won't load.
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// The name of the plugin. The name is optional. If the name is set, all displays will replaced with the name.
        /// If no name is set, the display name is the id.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The version of the plugin. 
        /// </summary>
        public string Version { get; }
        
        /// <summary>
        /// The author of the plugin.
        /// </summary>
        public string Author { get; }
        
        /// <summary>
        /// If the plugin has dependencies which needs to be loaded before this plugin gets enabled,
        /// the id of the dependency can be entered here.
        /// </summary>
        public string[] Dependencies { get; }
        
        /// <summary>
        /// Whether the plugin is in debug mode or not.
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// The mode of the i18n module. Default: <see cref="Modules.I18n.Mode.Internal"/>.
        /// </summary>
        public I18n.Mode I18n { get; set; } = Modules.I18n.Mode.Internal;
        
        /// <summary>
        /// The type defining a package provider for this plugin. The type must extend <see cref="Native.PackageProvider"/> and need a default constructor.
        /// </summary>
        public Type PackageProvider { get; set; }

        /// <summary>
        /// The api version which uses the plugin. If the version is lower the current runtime version, the plugin won't be loaded in order to prevent further issues.
        /// By default its set to the current runtime api version. If you want to prevent users to use your plugin which the wrong api version you can manually set it.
        /// </summary>
        public int ApiVersion { get; set; } = Bridge.ApiVersion;

        /// <summary>
        /// This constructor offers all properties. If you want to set them, this is the constructor you need.
        /// </summary>
        /// <param name="id">The id of the plugin</param>
        /// <param name="name">The display name of the plugin</param>
        /// <param name="version">The version of the plugin</param>
        /// <param name="author">The author of the plugin</param>
        /// <param name="dependencies">The dependencies the plugin has</param>
        public PluginMeta(string id, string name, string version, string author, params string[] dependencies)
        {
            Id = id;
            Name = name;
            Version = version;
            Author = author;
            Dependencies = dependencies;
        }

        /// <summary>
        /// The recommended constructor. Here you can set a recommended portion of properties.
        /// The name is completely optional and just for displaying, so its not set here.
        /// </summary>
        /// <param name="id">The id of the plugin</param>
        /// <param name="version">The version of the plugin</param>
        /// <param name="author">The author of the plugin</param>
        /// <param name="dependencies">The dependencies the plugin has</param>
        public PluginMeta(string id, string version, string author, params string[] dependencies) : this(id, null, version, author, dependencies)
        {
        }

        /// <summary>
        /// This is the minimal required constructor. It offers all the required properties which needs to be set.
        /// The author and the name are not set.
        /// </summary>
        /// <param name="id">The id of the plugin</param>
        /// <param name="version">The version of the plugin</param>
        /// <param name="dependencies">The dependencies the plugin has</param>
        public PluginMeta(string id, string version, params string[] dependencies) : this(id, version, "Some Guy", dependencies)
        {
        }
    }
}