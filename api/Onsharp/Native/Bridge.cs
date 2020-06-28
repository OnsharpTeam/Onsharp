using System;
using System.IO;
using System.Runtime.InteropServices;
using Nett;
using Onsharp.IO;
using Onsharp.Plugin;

namespace Onsharp.Native
{
    internal static class Bridge
    {
        internal const string DllName = "onsharp-runtime";

        /// <summary>
        /// The path of the server software running this runtime.
        /// </summary>
        internal static string ServerPath { get; private set; }
        
        /// <summary>
        /// The path to the runtime folder.
        /// </summary>
        internal static string AppPath { get; private set; }
        
        /// <summary>
        /// The path to the third party libraries folder.
        /// </summary>
        internal static string LibsPath { get; private set; }
        
        /// <summary>
        /// The path to the folder containing all plugin files.
        /// </summary>
        internal static string PluginsPath { get; private set; }
        
        /// <summary>
        /// The path to the folder containing data to the plugins or the runtime itself.
        /// </summary>
        internal static string DataPath { get; private set; }
        
        /// <summary>
        /// The path to the folder containing logs to the plugins or the runtime itself.
        /// </summary>
        internal static string LogPath { get; private set; }
        
        /// <summary>
        /// The config of the current Onsharp runtime.
        /// </summary>
        internal static RuntimeConfig Config { get; private set; }
        
        /// <summary>
        /// The logger of the Onsharp runtime.
        /// </summary>
        internal static ILogger Logger { get; private set; }
        
        internal static PluginManager PluginManager { get; private set; }
        
        /// <summary>
        /// Gets called by the native runtime when Onsharp should load itself.
        /// <param name="appPath">The path to the server given from the coreclr host</param>
        /// </summary>
        internal static void Load(string appPath)
        {
            ServerPath = appPath;
            AppPath = Path.Combine(ServerPath, "onsharp");
            Directory.CreateDirectory(AppPath);
            LibsPath = Path.Combine(AppPath, "libs");
            Directory.CreateDirectory(LibsPath);
            PluginsPath = Path.Combine(AppPath, "plugins");
            Directory.CreateDirectory(PluginsPath);
            LogPath = Path.Combine(AppPath, "logs");
            Directory.CreateDirectory(LogPath);
            DataPath = Path.Combine(AppPath, "data");
            Directory.CreateDirectory(DataPath);
            string configPath = Path.Combine(DataPath, "global.toml");
            if (File.Exists(configPath))
            {
                Config = Toml.ReadFile<RuntimeConfig>(configPath);
            }
            else
            {
                Config = new RuntimeConfig();
                Toml.WriteFile(Config, configPath);
            }
            
            Logger = new Logger("Onsharp", Config.IsDebug, "_global");
            if(Config.IsDebug) Logger.Warn("{DEBUG}-Mode is currently active!", "DEBUG");
            PluginManager = new PluginManager();
        }

        /// <summary>
        /// Gets called by the native runtime when Onsharp should unload itself.
        /// </summary>
        internal static void Unload()
        {
            Logger.Warn("Stopping bridge...");
            for (int i = PluginManager.Plugins.Count - 1; i >= 0; i--)
            {
                IPlugin plugin = PluginManager.Plugins[i];
                PluginManager.ForceStop(plugin);
            }
            
            Logger.Info("Onsharp successfully stopped!");
        }

        /// <summary>
        /// Just a placeholder: Maybe this will be replaced by another technique.
        /// </summary>
        internal static bool ExecuteEvent(string name, string json)
        {
            return false;
        }

        /// <summary>
        /// Converts a pointer to a string.
        /// </summary>
        /// <param name="ptr">The pointer to be converted</param>
        /// <returns>The converted string</returns>
        internal static string PtrToString(IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr);
        }
    }
}