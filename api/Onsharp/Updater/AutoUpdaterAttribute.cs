using System;

namespace Onsharp.Updater
{
    /// <summary>
    /// The auto updating feature is an optional feature for plugins. The feature allows the
    /// developer of the plugin to update the plugins of them automatically.
    /// It works as follows: When the plugin gets initialized the plugin manager looks for the main class
    /// and if the main class has this attribute. If the main class has the attribute it takes the url and loads the
    /// updating data from this url. After comparing the versions the update downloads the needed files from the url
    /// in the updating data, unloads the domain, extracts the downloaded zip file, puts the files at the right places
    /// and cleans up the environment.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoUpdaterAttribute : Attribute
    {
        /// <summary>
        /// The url to the updating data. Look up the wiki, to see the needed format of this JSON data.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The default constructor which takes the url to the updating data.
        /// </summary>
        public AutoUpdaterAttribute(string url)
        {
            Url = url;
        }
    }
}