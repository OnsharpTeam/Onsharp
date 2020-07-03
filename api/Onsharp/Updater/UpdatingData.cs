using Newtonsoft.Json;

namespace Onsharp.Updater
{
    /// <summary>
    /// The updating data model for the fitting JSON data from the update url.
    /// </summary>
    internal class UpdatingData
    {
        /// <summary>
        /// The version of the current available files.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
        
        /// <summary>
        /// An url to the changelog if there is one available.
        /// </summary>
        [JsonProperty("changelog")]
        public string Changelog { get; set; }
        
        /// <summary>
        /// An url to a zip file containing the new files.
        /// </summary>
        [JsonProperty("files")]
        public string Files { get; set; }
        
        /// <summary>
        /// The name of the file which is the plugin library.
        /// </summary>
        [JsonProperty("plugin_file")]
        public string PluginFile { get; set; }
    }
}