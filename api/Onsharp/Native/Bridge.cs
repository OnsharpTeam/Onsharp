using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Nett;
using Onsharp.Converters;
using Onsharp.Entities;
using Onsharp.Events;
using Onsharp.IO;
using Onsharp.Plugins;

namespace Onsharp.Native
{
    /// <summary>
    /// The bridge managed the direct contact from the c++ runtime
    /// and the base runtime functionality and data.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal class Bridge : IRuntime
    {
        /// <summary>
        /// The name of the c++ runtime dll.
        /// </summary>
        internal const string DllName = "onsharp-runtime";
        
        /// <summary>
        /// A list containing all the events which needs as first argument a player, so called player events.
        /// </summary>
        private static readonly List<EventType> PlayerEvents = new List<EventType>
        {
            EventType.PlayerChat, EventType.PlayerChatCommand, EventType.PlayerJoin, EventType.PlayerQuit, EventType.PlayerPickupHit,
            EventType.NPCStreamIn, EventType.NPCStreamOut, EventType.PlayerEnterVehicle, EventType.PlayerLeaveVehicle, EventType.PlayerStateChange,
            EventType.VehicleStreamIn, EventType.VehicleStreamOut, EventType.PlayerDamage, EventType.PlayerDeath, EventType.PlayerInteractDoor,
            EventType.PlayerStreamIn, EventType.PlayerStreamOut, EventType.PlayerServerAuth, EventType.PlayerSteamAuth, EventType.PlayerDownloadFile,
            EventType.PlayerWeaponShot, EventType.PlayerSpawn
        };

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
        
        /// <summary>
        /// The current plugin manager instance managing all the plugins
        /// </summary>
        internal static PluginManager PluginManager { get; private set; }
        
        /// <summary>
        /// The current wrapped runtime instance for the bridge.
        /// </summary>
        internal static Bridge Runtime { get; private set; }
        
        /// <summary>
        /// All converters which are registered in the runtime.
        /// </summary>
        private static List<Converter> Converters { get; } = new List<Converter>
        {
            new EnumConverter(), new PlayerConverter()
        };

        private static readonly Converter DefaultConverter = new BasicConverter();

        /// <summary>
        /// The flag defining if the entity refreshing of the pools is enabled.
        /// If true, the pool gets refreshed if its getting accessed for retrieving all elements.
        /// Turning it off is recommended if Onsharp is the only scripting environment running in Onset,
        /// because than the management of every entity is managed by Onsharp.
        /// </summary>
        internal static bool IsEntityRefreshingEnabled => Runtime._isEntityRefreshingEnabled;

        private bool _isEntityRefreshingEnabled = true;
        
        /// <summary>
        /// Gets called by the native runtime when Onsharp should load itself.
        /// <param name="appPath">The path to the server given from the coreclr host</param>
        /// </summary>
        internal static void Load(string appPath)
        {
            try
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
                Runtime = new Bridge();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "The loading of the runtime ran into an error!");
            }
        }

        /// <summary>
        /// Gets called by the native runtime when Onsharp should unload itself.
        /// </summary>
        internal static void Unload()
        {
            Logger.Warn("Stopping bridge...");
            for (int i = PluginManager.Plugins.Count - 1; i >= 0; i--)
            {
                Plugin plugin = PluginManager.Plugins[i];
                PluginManager.ForceStop(plugin);
            }
            
            Logger.Info("Onsharp successfully stopped!");
        }

        /// <summary>
        /// Gets called when the half script is loaded and the runtime entries can be loaded.
        /// </summary>
        internal static void InitRuntimeEntries()
        {
            try
            {
                PluginManager = new PluginManager();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "The init of the runtime ran into an error!");
            }
        }

        /// <summary>
        /// This method gets called from the native side and is the interaction interface from the pipeline to the dotnet runtime.
        /// </summary>
        /// <param name="key">The key which defines what the reason is, the native side is calling</param>
        /// <param name="nArgsPtr">The arguments which are passed to the dotnet runtime</param>
        /// <param name="len">The length of the nArgs data which got passed</param>
        /// <returns>If wanted, some data as NVal</returns>
        internal static IntPtr CallBridge(string key, IntPtr nArgsPtr, int len)
        {
            IntPtr[] nArgs = new IntPtr[len];
            Marshal.Copy(nArgsPtr, nArgs, 0, len);
            object[] args = new object[len];
            for (int i = 0; i < len; i++)
            {
                args[i] = new NativeValue(nArgs[i]).GetValue();
                if (key == "call-command")
                {
                    Logger.Debug("{NV}", args[i]);
                }
            }

            return CreateNValue(HandleCalling(key, args)).NativePtr;
        }

        /// <summary>
        /// Handles the incoming calling from the native side.
        /// </summary>
        private static object HandleCalling(string key, object[] args)
        {
            try
            {
                if (key == "call-event")
                {
                    EventType type = (EventType) (int) args[0];
                    bool flag = true;
                    PluginManager.IteratePlugins(plugin =>
                    {
                        PluginDomain domain = PluginManager.GetDomain(plugin);
                        if (domain == null)
                        {
                            Logger.Fatal("Could not get plugin domain for loaded plugin {PLUGIN}!", plugin.Display);
                            return;
                        }

                        if (!domain.Server.CallEvent(type, ParseEventArgs(domain, type, args)))
                            flag = false;
                    });
            
                    return flag;
                }

                if (key == "call-remote")
                {
                    int player = (int) args[0];
                    string pluginId = (string) args[1];
                    string name = (string) args[2];
                    object[] remoteArgs = new object[args.Length - 3];
                    for (int i = 3; i < args.Length; i++)
                    {
                        remoteArgs[i - 2] = args[i];
                    }
                    
                    Plugin plugin = PluginManager.GetPlugin(pluginId);
                    if (plugin != null)
                    {
                        PluginManager.GetDomain(plugin)?.Server.FireRemoteEvent(name, player, remoteArgs);
                    }
                }

                if (key == "call-command")
                {
                    string pluginId = (string) args[0];
                    Logger.Debug("cc pluginId: " + pluginId);
                    int player = (int) args[1];
                    Logger.Debug("cc player: " + player);
                    string name = (string) args[2];
                    Logger.Debug("cc name: " + name);
                    string line = (string) args[3];
                    Logger.Debug("cc line: " + line);
                    Plugin plugin = PluginManager.GetPlugin(pluginId);
                    if (plugin != null)
                    {
                        PluginManager.GetDomain(plugin)?.Server.FireCommand(player, name, line);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while handling a call from the native side!");
            }
            
            return null;
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
        
        /// <summary>
        /// Returns a boolean whether the given event is a player event - so needs a player as the first argument - or not.
        /// </summary>
        /// <param name="type">The event type</param>
        /// <returns>True if it is a player event</returns>
        internal static bool IsPlayerEvent(EventType type)
        {
            return PlayerEvents.Contains(type);
        }

        /// <summary>
        /// Finds the right converter which can handle this given parameter.
        /// </summary>
        /// <param name="parameter">The parameter to be handled</param>
        /// <returns>The converter which handles the parameter</returns>
        private static Converter FindConverter(ParameterInfo parameter)
        {
            for (int i = Converters.Count - 1; i >= 0; i--)
            {
                Converter converter = Converters[i];
                if (converter.IsHandlerFor(parameter))
                {
                    return converter;
                }
            }

            return DefaultConverter;
        }
        
        /// <summary>
        /// Converts the given string list to an object parameter fitting array.
        /// </summary>
        /// <param name="server">The server instance from where the process is requested</param>
        /// <param name="objects">The strings which will be converted</param>
        /// <param name="wantedTypes">The parameters which are wanted</param>
        /// <param name="invoker">The player which executes the command</param>
        /// <param name="withOptional">If optional parameters are allowed or not</param>
        /// <returns>The converted object array or null if something failed</returns>
        internal static object[] Convert(Server server, List<string> objects, ParameterInfo[] wantedTypes, Player invoker, bool withOptional = false)
        {
            try
            {
                object[] arr = new object[wantedTypes.Length];
                arr[0] = invoker;
                for (int i = 1; i < wantedTypes.Length; i++)
                {
                    ParameterInfo wantedType = wantedTypes[i];
                    if (withOptional && wantedType.IsOptional && (i - 1) >= objects.Count)
                    {
                        arr[i] = Type.Missing;
                        continue;
                    }
                    
                    Converter converter = FindConverter(wantedType);
                    if (converter != null)
                    {
                        arr[i] = converter.Handle(objects[i - 1], wantedType, server);
                    }
                }

                return arr;
            }
            catch (Exception e)
            {
                server.Owner.Plugin.Logger.Error(e, "An error occurred in converting process!");
                return null;
            }
        }
        
        /// <summary>
        /// Creates a new instance of native value by the given value object.
        /// </summary>
        /// <param name="val">The value object of for the native value</param>
        /// <returns>The native value instance</returns>
        internal static NativeValue CreateNValue(object val)
        {
            if (val == null)
            {
                return new NativeValue(Onset.CreateNValue());
            }

            if (val is string s)
            {
                return new NativeValue(Onset.CreateNValue(s));
            }

            if (val is int i)
            {
                return new NativeValue(Onset.CreateNValue(i));
            }

            if (val is double d)
            {
                return new NativeValue(Onset.CreateNValue(d));
            }

            if (val is bool b)
            {
                return new NativeValue(Onset.CreateNValue(b));
            }

            return new NativeValue(Onset.CreateNValue());
        }

        /// <summary>
        /// Parses the given argument array from the bridge caller into event args which than can be passed to the event handler.
        /// WARNING: The first argument of the given array is the event type. Don't use it.
        /// </summary>
        private static object[] ParseEventArgs(PluginDomain owner, EventType type, object[] args)
        {
            switch (type)
            {
                case EventType.PlayerQuit:
                    break;
                case EventType.PlayerChat:
                    break;
                case EventType.PlayerChatCommand:
                    break;
                case EventType.PlayerJoin:
                    break;
                case EventType.PlayerPickupHit:
                    break;
                case EventType.PackageStart:
                    break;
                case EventType.PackageStop:
                    break;
                case EventType.GameTick:
                    break;
                case EventType.ClientConnectionRequest:
                    return new[]{args[1], args[2]};
                case EventType.NPCReachTarget:
                    break;
                case EventType.NPCDamage:
                    break;
                case EventType.NPCSpawn:
                    break;
                case EventType.NPCDeath:
                    break;
                case EventType.NPCStreamIn:
                    break;
                case EventType.NPCStreamOut:
                    break;
                case EventType.PlayerEnterVehicle:
                    break;
                case EventType.PlayerLeaveVehicle:
                    break;
                case EventType.PlayerStateChange:
                    break;
                case EventType.VehicleRespawn:
                    break;
                case EventType.VehicleStreamIn:
                    break;
                case EventType.VehicleStreamOut:
                    break;
                case EventType.PlayerServerAuth:
                    break;
                case EventType.PlayerSteamAuth:
                    break;
                case EventType.PlayerDownloadFile:
                    break;
                case EventType.PlayerStreamIn:
                    break;
                case EventType.PlayerStreamOut:
                    break;
                case EventType.PlayerSpawn:
                    break;
                case EventType.PlayerDeath:
                    break;
                case EventType.PlayerWeaponShot:
                    break;
                case EventType.PlayerDamage:
                    break;
                case EventType.PlayerInteractDoor:
                    break;
                default:
                    return null;
            }
            return null;
        }

        public bool CallEvent(string name, params object[] args)
        {
            bool flag = true;
            PluginManager.IteratePlugins(plugin =>
            {
                PluginDomain domain = PluginManager.GetDomain(plugin);
                if (domain == null)
                {
                    Logger.Fatal("Could not get plugin domain for loaded plugin {PLUGIN}!", plugin.Display);
                    return;
                }

                if (!domain.Server.CallEvent(name, args))
                    flag = false;
            });
            return flag;
        }

        public void DisableEntityPoolRefreshing()
        {
            _isEntityRefreshingEnabled = false;
        }

        public void RegisterConverter<T>(Func<string, Type, object> convertProcess)
        {
            Converters.Add(new Converter(typeof(T), convertProcess));
        }

        public void RegisterCustomConverter(Converter converter)
        {
            Converters.Add(converter);
        }
    }
}