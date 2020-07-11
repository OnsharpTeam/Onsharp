using System;
using System.Net;
using Newtonsoft.Json;
using Onsharp.Native;

namespace Onsharp.Updater
{
    /// <summary>
    /// The class which manages the runtime updating process
    /// </summary>
    internal static class RuntimeUpdater
    {
        private const string UpdateUrl = "https://eternitylife.de/onsharp_update.json";
        private static Data _lastData;
        
        /// <summary>
        /// Checks if a runtime update is available.
        /// </summary>
        /// <param name="version">The version of the new update, if one is available</param>
        /// <returns>True, if an update is available</returns>
        internal static bool IsUpdateAvailable(out string version)
        {
            version = null;
            try
            {
                using WebClient client = new WebClient();
                _lastData = JsonConvert.DeserializeObject<Data>(client.DownloadString(UpdateUrl));
                version = _lastData.Version;
                return Version.Parse(_lastData.Version) > Bridge.Version;
            }
            catch (Exception ex)
            {
                Bridge.Logger.Error(ex, "An error occurred while reading updating data from MainServer!");
                return false;
            }
        }

        private class Data
        {
            [JsonProperty("version")]
            internal string Version { get; set; }

            [JsonProperty("timestamp")]
            internal string TimeStamp { get; set; }
            
            [JsonProperty("file")]
            internal string File { get; set; }
        }
    }
}