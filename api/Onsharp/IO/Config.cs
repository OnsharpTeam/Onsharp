using System;

namespace Onsharp.IO
{
    /// <summary>
    /// A class marked as a config, will be transformed into a TOML file and saved to the the data folder of the plugin.
    /// When the server starts it gets loaded in the data storage of the plugin and can be retrieved from there at any time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Config : Attribute
    {
        /// <summary>
        /// The name of the config file.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The path to the folder folder, if needed.
        /// </summary>
        public string SubPath { get; }

        public Config(string name, string subPath = "")
        {
            Name = name;
            SubPath = subPath;
        }
    }
}