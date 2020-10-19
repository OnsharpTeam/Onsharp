using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Nett;
using Newtonsoft.Json.Linq;
using Onsharp.Commands;
using Onsharp.Converters;
using Onsharp.Entities;
using Onsharp.Enums;
using Onsharp.Events;
using Onsharp.Interop;
using Onsharp.IO;
using Onsharp.Modules;
using Onsharp.Plugins;
using Onsharp.Updater;
using Onsharp.Utils;
using Onsharp.World;
using Object = Onsharp.Entities.Object;
using Timer = Onsharp.Threading.Timer;

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
        internal const string DllName = "../../plugins/onsharp-runtime";
        
        /// <summary>
        /// The current version of Onsharp running.
        /// </summary>
        internal static readonly Version Version = new Version(1, 1, 4);

        /// <summary>
        /// The current api version of onsharp.
        /// </summary>
        internal const int ApiVersion = 1;

        /// <summary>
        /// A list containing all the events which needs as first argument a player, so called player events.
        /// </summary>
        private static readonly List<EventType> PlayerEvents = new List<EventType>
        {
            EventType.PlayerChat, EventType.PlayerChatCommand, EventType.PlayerJoin, EventType.PlayerQuit, EventType.PlayerPickupHit,
            EventType.NPCStreamIn, EventType.NPCStreamOut, EventType.PlayerEnterVehicle, EventType.PlayerLeaveVehicle, EventType.PlayerStateChange,
            EventType.VehicleStreamIn, EventType.VehicleStreamOut, EventType.PlayerDamage, EventType.PlayerDeath, EventType.PlayerInteractDoor,
            EventType.PlayerStreamIn, EventType.PlayerStreamOut, EventType.PlayerServerAuth, EventType.PlayerSteamAuth, EventType.PlayerDownloadFile,
            EventType.PlayerWeaponShot, EventType.PlayerSpawn, EventType.PlayerChangeDimension
        };

        /// <summary>
        /// The path of the server software running this runtime.
        /// </summary>
        internal static string ServerPath { get; set; }
        
        /// <summary>
        /// The path to the runtime folder.
        /// </summary>
        internal static string AppPath { get; set; }
        
        /// <summary>
        /// The path to the temp folder.
        /// </summary>
        internal static string TempPath { get; private set; }
        
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
        /// The path to the folder containing all LUA packages for Onset.
        /// </summary>
        internal static string PackagePath { get; private set; }
        
        /// <summary>
        /// The JSON object of the Onset server config.
        /// </summary>
        internal static JObject ServerConfig { get; private set; }
        
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
        /// A list containing all admins on this server.
        /// </summary>
        internal static List<long> Admins { get; private set; }
        
        /// <summary>
        /// A list which contains occupied command names.
        /// </summary>
        internal static List<string> OccupiedCommandNames { get; private set; }
        
        /// <summary>
        /// A list which contains occupied console command names.
        /// </summary>
        internal static List<string> OccupiedConsoleCommandNames { get; private set; }
        
        /// <summary>
        /// The current console manager running in the background.
        /// </summary>
        internal static ConsoleManager ConsoleManager { get; set; }
        
        /// <summary>
        /// All converters which are registered in the runtime.
        /// </summary>
        private static List<Converter> Converters { get; } = new List<Converter>
        {
            new EnumConverter(), new PlayerConverter()
        };
        
        /// <summary>
        /// The path to the file containing all admins.
        /// </summary>
        private static string AdminsFile { get; set; }

        private static readonly Converter DefaultConverter = new BasicConverter();

        /// <summary>
        /// Gets called by the native runtime when Onsharp should load itself.
        /// <param name="appPath">The path to the server given from the coreclr host</param>
        /// </summary>
        internal static void Load(string appPath)
        {
            try
            {
                OccupiedCommandNames = new List<string>();
                OccupiedConsoleCommandNames = new List<string>();
                ServerPath = appPath;
                AppPath = Path.Combine(ServerPath, "onsharp");
                Directory.CreateDirectory(AppPath);
                LibsPath = Path.Combine(AppPath, "libs");
                Directory.CreateDirectory(LibsPath);
                PluginsPath = Path.Combine(AppPath, "plugins");
                Directory.CreateDirectory(PluginsPath);
                TempPath = Path.Combine(AppPath, "tmp");
                Directory.CreateDirectory(TempPath);
                LogPath = Path.Combine(AppPath, "logs");
                Directory.CreateDirectory(LogPath);
                DataPath = Path.Combine(AppPath, "data");
                Directory.CreateDirectory(DataPath);
                PackagePath = Path.Combine(ServerPath, "packages");
                Directory.CreateDirectory(PackagePath);
                ServerConfig = JObject.Parse(File.ReadAllText(Path.Combine(ServerPath, "server_config.json")));
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

                AdminsFile = Path.Combine(DataPath, "admins.json");
                if (File.Exists(AdminsFile))
                {
                    Admins = Json.FromJson<List<long>>(File.ReadAllText(AdminsFile));
                }
                else
                {
                    Admins = new List<long>();
                    File.WriteAllText(AdminsFile, Json.ToJson(Admins));
                }
                
                Logger = new Logger("Onsharp", Config.IsDebug, "_global");
                if(Config.IsDebug) Logger.Warn("{DEBUG}-Mode is currently active!", "DEBUG");
                ConsoleManager = new ConsoleManager();
                Runtime = new Bridge();
                ConsoleManager.Reset();
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
            
            PluginManager.Unload();
            Logger.Info("Onsharp successfully stopped!");
        }

        /// <summary>
        /// Gets called when the half script is loaded and the runtime entries can be loaded.
        /// </summary>
        internal static void InitRuntimeEntries()
        {
            try
            {
                if (RuntimeUpdater.IsUpdateAvailable(out string newVersion))
                {
                    Logger.Fatal("The update v{VERSION} for the Onsharp Runtime is available!", newVersion);
                }
                
                LazyMover.Start();
                PluginManager = new PluginManager();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "The init of the runtime ran into an error!");
            }
        }

        /// <summary>
        /// Returns true, if the given console command name is occupied or not.
        /// </summary>
        /// <param name="name">The name to be checked</param>
        internal static bool IsConsoleCommandOccupied(string name)
        {
            lock (OccupiedConsoleCommandNames)
            {
                return OccupiedConsoleCommandNames.ContainsIgnoreCase(name);
            }
        }

        /// <summary>
        /// Occupies a the given console command name.
        /// </summary>
        internal static void OccupyConsoleCommand(string name)
        {
            lock (OccupiedConsoleCommandNames)
            {
                OccupiedConsoleCommandNames.Add(name);
            }
        }

        /// <summary>
        /// Returns true, if the given command name is occupied or not.
        /// </summary>
        /// <param name="name">The name to be checked</param>
        internal static bool IsCommandOccupied(string name)
        {
            lock (OccupiedCommandNames)
            {
                return OccupiedCommandNames.ContainsIgnoreCase(name);
            }
        }

        /// <summary>
        /// Occupies a the given command name.
        /// </summary>
        internal static void OccupyCommand(string name)
        {
            lock (OccupiedCommandNames)
            {
                OccupiedCommandNames.Add(name);
            }
        }

        /// <summary>
        /// Saves the admins to a file.
        /// </summary>
        private static void SaveAdmins()
        {
            try
            {
                File.WriteAllText(AdminsFile, Json.ToJson(Admins));
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error occurred while saving admins file!");
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
                    int typeId = System.Convert.ToInt32(args[0]);
                    if (typeId == 7 || typeId == 6) return true;
                    EventType type = (EventType) typeId;

                    bool consoleBreak = false;
                    bool flag = true;
                    PluginManager.IteratePlugins(plugin =>
                    {
                        if (plugin == null || plugin.State == PluginState.Failed) return;
                        PluginDomain domain = PluginManager.GetDomain(plugin);
                        if (domain == null)
                        {
                            Logger.Fatal("Could not get plugin domain for loaded plugin {PLUGIN}!", plugin.Display);
                            return;
                        }

                        object[] objArgs = ParseEventArgs(domain, type, args);
                        if (!domain.Server.CallEvent(type, objArgs))
                            flag = false;

                        if (type == EventType.PlayerQuit)
                        {
                            domain.Server.PlayerPool.RemoveEntity((Player) objArgs[0]);
                        }
                        else if (type == EventType.PlayerSteamAuth)
                        {
                            Player player = (Player) objArgs[0];
                            lock (Admins)
                            {
                                player.IsAdmin = Admins.Contains(player.SteamID);
                            }
                        }
                        else if (type == EventType.ConsoleInput)
                        {
                            if (consoleBreak) return;
                            string input = (string) objArgs[0];
                            if (ConsoleManager.PollInput(input, true))
                            {
                                consoleBreak = true;
                                return;
                            }
                            
                            consoleBreak = ConsoleManager.PollInput(input);
                        }
                    });
            
                    return flag;
                }

                if (key == "call-remote")
                {
                    int player = System.Convert.ToInt32(args[0]);
                    string pluginId = (string) args[1];
                    string name = (string) args[2];
                    object[] remoteArgs = new object[args.Length - 3];
                    for (int i = 3; i < args.Length; i++)
                    {
                        remoteArgs[i - 3] = args[i];
                    }
                    
                    Plugin plugin = PluginManager.GetPlugin(pluginId);
                    if (plugin != null)
                    {
                        PluginManager.GetDomain(plugin)?.Server.FireRemoteEvent(name, player, remoteArgs);
                    }

                    return null;
                }

                if (key == "call-command")
                {
                    string pluginId = (string) args[0];
                    int player = System.Convert.ToInt32(args[1]);
                    string name = (string) args[2];
                    string line = (string) args[3];
                    if (pluginId == "native")
                    {
                        
                        return null;
                    }
                    
                    Plugin plugin = PluginManager.GetPlugin(pluginId);
                    if (plugin != null)
                    {
                        PluginManager.GetDomain(plugin)?.Server.FireCommand(player, name, line);
                    }

                    return null;
                }

                if (key == "call-timer")
                {
                    string id = (string) args[0];
                    Timer.CallTimer(id);
                    return null;
                }

                if (key == "call-delay")
                {
                    string id = (string) args[0];
                    Timer.CallDelay(id);
                    return null;
                }

                if (key == "interop")
                {
                    string pluginId = (string) args[0];
                    string funcName = (string) args[1];
                    object[] @params = new object[args.Length - 2];
                    for (int i = 2; i < args.Length; i++)
                    {
                        @params[i - 2] = args[i];
                    }
                    
                    Plugin plugin = PluginManager.GetPlugin(pluginId);
                    return plugin != null ? PluginManager.GetDomain(plugin)?.Server.FireExportable(funcName, @params) : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    "An error occurred while handling a call {CALLNAME} from the native side! The data which was excepted is the following when debug enabled!",
                    key);
                Logger.Debug("Data for the Call:\n{DATA}", Json.ToJson(args, Json.Flag.Pretty));
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
            return new NativeValue(ptr).GetValue() as string;
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
        /// Converts the given string list to an object parameter fitting array using only the <see cref="BasicConverter"/>.
        /// </summary>
        /// <param name="objects">The strings which will be converted</param>
        /// <param name="wantedTypes">The parameters which are wanted</param>
        /// <param name="withOptional">If optional parameters are allowed or not</param>
        /// <returns>The converted object array or null if something failed</returns>
        internal static object[] ConvertBasic(List<string> objects, ParameterInfo[] wantedTypes, bool withOptional)
        {
            try
            {
                object[] arr = new object[wantedTypes.Length];
                for (int i = 0; i < wantedTypes.Length; i++)
                {
                    ParameterInfo wantedType = wantedTypes[i];
                    if (withOptional && wantedType.IsOptional && i >= objects.Count)
                    {
                        arr[i] = Type.Missing;
                        continue;
                    }
                    
                    arr[i] = DefaultConverter.Handle(objects[i], wantedType, null);
                }

                return arr;
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error occurred in converting process!");
                return null;
            }
        }

        /// <summary>
        /// Gets the attach type of the given entity. If the entity is invalid <see cref="AttachType.None"/> is returned.
        /// </summary>
        /// <param name="entity">The wanted attach target entity</param>
        /// <returns>The associated attach type</returns>
        internal static AttachType GetTypeByEntity(Entity entity)
        {
            return entity switch
            {
                Player _ => AttachType.Player,
                Vehicle _ => AttachType.Vehicle,
                Object _ => AttachType.Object,
                NPC _ => AttachType.NPC,
                _ => AttachType.None
            };
        }

        /// <summary>
        /// Returns the network stats from the wanted source.
        /// </summary>
        /// <param name="source">The source can either be 0 and less, so its the server, or greater 0 than its a player</param>
        /// <returns>The current network stats</returns>
        internal static NetworkStats GetNetworkStats(int source)
        {
            int totalPacketLoss = 0;
            int lastSecondPacketLoss = 0;
            int messagesInResendBuffer = 0;
            int bytesInResendBuffer = 0;
            int bytesSend = 0;
            int bytesReceived = 0;
            int bytesResend = 0;
            int totalBytesSend = 0;
            int totalBytesReceived = 0;
            bool isLimitedByCongestionControl = false;
            bool isLimitedByOutgoingBandwidthLimit = false;
            Onset.GetNetworkStats(source, ref totalPacketLoss, ref lastSecondPacketLoss,
                ref messagesInResendBuffer,
                ref bytesInResendBuffer, ref bytesSend, ref bytesReceived, ref bytesResend,
                ref totalBytesSend,
                ref totalBytesReceived, ref isLimitedByCongestionControl,
                ref isLimitedByOutgoingBandwidthLimit);
            return new NetworkStats(totalPacketLoss, lastSecondPacketLoss,
                messagesInResendBuffer,
                bytesInResendBuffer, bytesSend, bytesReceived, bytesResend,
                totalBytesSend,
                totalBytesReceived, isLimitedByCongestionControl,
                isLimitedByOutgoingBandwidthLimit);
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
                        try
                        {
                            arr[i] = converter.Handle(objects[i - 1], wantedType, server);
                        }
                        catch (Exception e)
                        {
                            server.Owner.Plugin.Logger.Error(e,
                                "An error occurred with converter {CONV} when converting {VAL} to {TYPE}!",
                                converter.GetType().FullName, objects[i - 1], wantedType.ParameterType.FullName);
                        }
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

            if (val is LuaTable t)
            {
                return t.NVal;
            }
            
            return new NativeValue(Onset.CreateNValue());
        }

        /// <summary>
        /// Parses the given argument array from the bridge caller into event args which than can be passed to the event handler.
        /// WARNING: The first argument of the given array is the event type. Don't use it.
        /// </summary>
        private static object[] ParseEventArgs(PluginDomain owner, EventType type, object[] args)
        {
            Player player = IsPlayerEvent(type) ? owner.Server.CreatePlayer((int) args[1]) : null;
            switch (type)
            {
                case EventType.PlayerQuit:
                    return new object[] {player};
                case EventType.PlayerChat:
                    return new[] {player, args[2]};
                case EventType.PlayerChatCommand:
                    return new[] {player, args[2], args[3]};
                case EventType.PlayerJoin:
                    return new object[] {player};
                case EventType.PlayerPickupHit:
                    return new object[] {player, owner.Server.CreatePickup((int) args[2])};
                case EventType.GameTick:
                    return new[]{args[1]};
                case EventType.ClientConnectionRequest:
                    return new[]{args[1], args[2]};
                case EventType.NPCReachTarget:
                    return new object[] {owner.Server.CreateNPC((int) args[1])};
                case EventType.NPCDamage:
                    return new object[] {owner.Server.CreateNPC((int) args[1]), (DamageType) (int) args[2], (double) args[3]};
                case EventType.NPCSpawn:
                    return new object[] {owner.Server.CreateNPC((int) args[1])};
                case EventType.NPCDeath:
                    return new object[] {owner.Server.CreateNPC((int) args[1])};
                case EventType.NPCStreamIn:
                    return new object[] {player, owner.Server.CreateNPC((int) args[2])};
                case EventType.NPCStreamOut:
                    return new object[] {player, owner.Server.CreateNPC((int) args[2])};
                case EventType.PlayerEnterVehicle:
                    return new object[] {player, owner.Server.CreateVehicle((int) args[2]), (int) args[3]};
                case EventType.PlayerLeaveVehicle:
                    return new object[] {player, owner.Server.CreateVehicle((int) args[2]), (int) args[3]};
                case EventType.PlayerStateChange:
                    return new object[] {player, (PlayerState) (int) args[2], (PlayerState) (int) args[3]};
                case EventType.VehicleRespawn:
                    return new object[] {owner.Server.CreateVehicle((int) args[1])};
                case EventType.VehicleStreamIn:
                    return new object[] {player, owner.Server.CreateVehicle((int) args[2])};
                case EventType.VehicleStreamOut:
                    return new object[] {player, owner.Server.CreateVehicle((int) args[2])};
                case EventType.PlayerServerAuth:
                    return new object[] {player};
                case EventType.PlayerSteamAuth:
                    return new object[] {player};
                case EventType.PlayerDownloadFile:
                    return new object[] {player, (string) args[2], (string) args[3]};
                case EventType.PlayerStreamIn:
                    return new object[] {player, owner.Server.CreatePlayer((int) args[2])};
                case EventType.PlayerStreamOut:
                    return new object[] {player, owner.Server.CreatePlayer((int) args[2])};
                case EventType.PlayerSpawn:
                    return new object[] {player};
                case EventType.PlayerDeath:
                    return new object[] {player, owner.Server.CreatePlayer((int) args[2])};
                case EventType.PlayerWeaponShot:
                    HitType hitType = (HitType) (int) args[3];
                    return new object[]
                    {
                        player, (Weapon) (int) args[2], hitType, owner.Server.CreateHitEntity(hitType, (int) args[4]),
                        new Vector((double) args[5], (double) args[6], (double) args[7]),
                        new Vector((double) args[8], (double) args[9], (double) args[10]),
                        new Vector((double) args[11], (double) args[12], (double) args[13])
                    };
                case EventType.PlayerDamage:
                    return new object[] {player, (DamageType) (int) args[2], (double) args[3]};
                case EventType.PlayerInteractDoor:
                    return new object[] {player, owner.Server.CreateDoor((int) args[2]), (bool) args[3]};
                case EventType.VehicleDamage:
                    return new object[] {owner.Server.CreateVehicle((int) args[1]), (double) args[2], (int) args[3], (double) args[4]};
                case EventType.Custom:
                    return null;
                case EventType.PlayerPreCommand:
                    return null;
                case EventType.ConsoleInput:
                    return new[] {args[1]};
                case EventType.DoorDestroyed:
                    return new object[] {owner.Server.CreateDoor((int) args[1])};
                case EventType.NPCDestroyed:
                    return new object[] {owner.Server.CreateNPC((int) args[1])};
                case EventType.ObjectDestroyed:
                    return new object[] {owner.Server.CreateObject((int) args[1])};
                case EventType.PickupDestroyed:
                    return new object[] {owner.Server.CreatePickup((int) args[1])};
                case EventType.Text3DDestroyed:
                    return new object[] {owner.Server.CreateText3D((int) args[1])};
                case EventType.PlayerChangeDimension:
                    return new object[] {player, owner.Server.CreateDimension((uint) args[2]), owner.Server.CreateDimension((uint) args[3])};
                case EventType.VehicleChangeDimension:
                    return new object[] {owner.Server.CreateVehicle((int) args[1]), owner.Server.CreateDimension((uint) args[2]), owner.Server.CreateDimension((uint) args[3])};
                case EventType.Text3DChangeDimension:
                    return new object[] {owner.Server.CreateText3D((int) args[1]), owner.Server.CreateDimension((uint) args[2]), owner.Server.CreateDimension((uint) args[3])};
                case EventType.PickupChangeDimension:
                    return new object[] {owner.Server.CreatePickup((int) args[1]), owner.Server.CreateDimension((uint) args[2]), owner.Server.CreateDimension((uint) args[3])};
                case EventType.ObjectChangeDimension:
                    return new object[] {owner.Server.CreateObject((int) args[1]), owner.Server.CreateDimension((uint) args[2]), owner.Server.CreateDimension((uint) args[3])};
                case EventType.NPCChangeDimension:
                    return new object[] {owner.Server.CreateNPC((int) args[1]), owner.Server.CreateDimension((uint) args[2]), owner.Server.CreateDimension((uint) args[3])};
                case EventType.ObjectCreated:
                    return new object[] {owner.Server.CreateObject((int) args[1])};
                case EventType.VehicleCreated:
                    return new object[] {owner.Server.CreateVehicle((int) args[1])};
                case EventType.Text3DCreated:
                    return new object[] {owner.Server.CreateText3D((int) args[1])};
                case EventType.PickupCreated:
                    return new object[] {owner.Server.CreatePickup((int) args[1])};
                case EventType.NPCCreated:
                    return new object[] {owner.Server.CreateNPC((int) args[1])};
                case EventType.DoorCreated:
                    return new object[] {owner.Server.CreateDoor((int) args[1])};
                case EventType.ObjectStopMoving:
                    return new object[] {owner.Server.CreateObject((int) args[1])};
                default:
                    return null;
            }
        }

        public int GameVersion => Onset.GetGameVersion();
        
        public string GameVersionString => PtrToString(Onset.GetGameVersionAsString());

        public double UptimeSeconds => Onset.GetTimeSeconds();

        public double UptimeMillis => Onset.GetTheTickCount();

        public double DeltaSeconds => Onset.GetDeltaSeconds();

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

        [Obsolete("Since entities get created when the proper event got fired, there is no need for refreshing with the LUA side")]
        public void DisableEntityPoolRefreshing()
        {
        }

        public void RegisterConverter<T>(Func<string, Type, object> convertProcess)
        {
            Converters.Add(new Converter(typeof(T), convertProcess));
        }

        public void RegisterCustomConverter(Converter converter)
        {
            Converters.Add(converter);
        }

        public void RegisterConsoleCommands(object owner, string specifier = null)
        {
            ConsoleManager.Register(owner, specifier);
        }

        public void RegisterConsoleCommands<T>(string specifier = null)
        {
            ConsoleManager.Register<T>(specifier);
        }

        public void StartPackage(string packageName)
        {
            Onset.StartPackage(packageName);
        }

        public void StopPackage(string packageName)
        {
            Onset.StopPackage(packageName);
        }

        public bool IsPackageStarted(string packageName)
        {
            return Onset.IsPackageStarted(packageName);
        }

        public List<string> GetAllPackages()
        {
            LuaTable table = new LuaTable(Onset.GetAllPackages());
            List<string> list = new List<string>();
            foreach (object key in table.Keys)
            {
                list.Add(table[key] as string);
            }

            return list;
        }

        [ConsoleCommand("help", "Shows all console commands and how to use them")]
        public void OnHelpConsoleCommand([Describe("The page number with the next 5 commands.")] int page = 1)
        {
            ConsoleManager.PrintCommands(page);
        }

        [ConsoleCommand("reload", "Reloads all plugins")]
        public void OnReloadConsoleCommand()
        {
            PluginManager.ReloadLibs();
            PluginManager.Reload();
        }

        [ConsoleCommand("exit", "Stops the server")]
        public void OnExitConsoleCommand()
        {
            Onset.ShutdownServer();
        }
        
        [Command("help", "Shows all commands and how to use them", Permission = "onset.commands.help")]
        public void OnHelpCommand(Player player, [Describe("The page number with the next 5 commands.")] int page = 1)
        {
            CommandInfo.PrintCommands(player, page);
        }

        [ConsoleCommand("makeadmin", "Makes a player to an admin")]
        public void OnMakeAdminConsoleCommand([Describe("The target steamID64 which should be made admin")] long target)
        {
            Admins.Add(target);
            SaveAdmins();
            Logger.Info(target + " is now admin!");
        }

        [ConsoleCommand("remadmin", "Makes a player to an admin")]
        public void OnRemoveAdminConsoleCommand([Describe("The target steamID64 which should be removed from admin")] long target)
        {
            Admins.Remove(target);
            SaveAdmins();
            Logger.Info(target + " is no longer admin!");
        }

        [Command("makeadmin", "Makes a player to an admin", Permission = "onset.commands.makeadmin")]
        public void OnMakeAdminCommand(Player player, [Describe("The target which should be made admin")] Player target)
        {
            Admins.Add(target.SteamID);
            SaveAdmins();
            player.SendColoredMessage("~7/~" + target.Name + " is now admin!");
        }

        [Command("remadmin", "Makes a player to an admin", Permission = "onset.commands.remadmin")]
        public void OnRemoveAdminCommand(Player player, [Describe("The target which should be removed from admin")] Player target)
        {
            Admins.Remove(target.SteamID);
            SaveAdmins();
            player.SendColoredMessage("~7/~" + target.Name + " is no longer admin!");
        }
    }
}