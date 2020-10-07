using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Onsharp.Commands;
using Onsharp.Entities;
using Onsharp.Entities.Factory;
using Onsharp.Enums;
using Onsharp.Events;
using Onsharp.Interop;
using Onsharp.Native;
using Onsharp.Plugins;
using Onsharp.World;
using Object = Onsharp.Entities.Object;

namespace Onsharp
{
    internal class Server : IServer
    {
        #region Entity Types

        private static readonly Type EntityType = typeof(Entity);
        private static readonly Type PlayerType = typeof(Player);
        private static readonly Type DoorType = typeof(Door);
        private static readonly Type NPCType = typeof(NPC);
        private static readonly Type ObjectType = typeof(Object);
        private static readonly Type PickupType = typeof(Pickup);
        private static readonly Type Text3DType = typeof(Text3D);
        private static readonly Type VehicleType = typeof(Vehicle);

        #endregion
        
        internal PluginDomain Owner { get; }

        public string Name
        {
            get => Bridge.PtrToString(Onset.GetServerName());
            set => Onset.SetServerName(value);
        }

        public double TickRate => Onset.GetServerTickRate();

        public int MaxPlayers => Onset.GetMaxPlayers();

        public NetworkStats NetworkStats => Bridge.GetNetworkStats(0);

        public Dimension this[uint val] => GetDimension(val);

        public IReadOnlyList<Player> Players => PlayerPool.CastEntities<Player>();
        
        public IReadOnlyList<Door> Doors => DoorPool.CastEntities<Door>();
        
        public IReadOnlyList<NPC> NPCs => NPCPool.CastEntities<NPC>();
        
        public IReadOnlyList<Object> Objects => ObjectPool.CastEntities<Object>();
        
        public IReadOnlyList<Pickup> Pickups => PickupPool.CastEntities<Pickup>();
        
        public IReadOnlyList<Text3D> Text3Ds => Text3DPool.CastEntities<Text3D>();
        
        public IReadOnlyList<Vehicle> Vehicles => VehiclePool.CastEntities<Vehicle>();
        
        internal EntityPool PlayerPool { get; }    

        private IEntityFactory<Player> PlayerFactory { get; set; }
        
        internal EntityPool DoorPool { get; }    

        private IEntityFactory<Door> DoorFactory { get; set; }
        
        internal EntityPool NPCPool { get; }    

        private IEntityFactory<NPC> NPCFactory { get; set; }
        
        internal EntityPool ObjectPool { get; }    

        private IEntityFactory<Object> ObjectFactory { get; set; }
        
        internal EntityPool PickupPool { get; }    

        private IEntityFactory<Pickup> PickupFactory { get; set; }
        
        internal EntityPool Text3DPool { get; }    

        private IEntityFactory<Text3D> Text3DFactory { get; set; }
        
        internal EntityPool VehiclePool { get; }    

        private IEntityFactory<Vehicle> VehicleFactory { get; set; }
        
        private List<ServerEvent> ServerEvents { get; }
        
        private List<RemoteEvent> RemoteEvents { get; }
       
        private List<LuaExport> Exportables { get; }
        
        private List<Dimension> Dimensions { get; }

        private readonly CommandManager _commandManager;
        private readonly Dimension _globalDim;
        private readonly ConcurrentQueue<Action> _taskQueue;

        internal Server(PluginDomain owner)
        {
            Owner = owner;
            _globalDim = new Dimension(this, 0);
            _taskQueue = new ConcurrentQueue<Action>();
            Dimensions = new List<Dimension>();
            PlayerFactory = new PlayerFactory();
            PlayerPool = new EntityPool(this, null, CreatePlayer);
            DoorFactory = new DoorFactory();
            DoorPool = new EntityPool(this, "Doors", CreateDoor);
            NPCFactory = new NPCFactory();
            NPCPool = new EntityPool(this, "NPC", CreateNPC);
            ObjectFactory = new ObjectFactory();
            ObjectPool = new EntityPool(this, "Objects", CreateObject);
            PickupFactory = new PickupFactory();
            PickupPool = new EntityPool(this, "Pickups", CreatePickup);
            Text3DFactory = new Text3DFactory();
            Text3DPool = new EntityPool(this, "Text3D", CreateText3D);
            VehicleFactory = new VehicleFactory();
            VehiclePool = new EntityPool(this, "Vehicles", CreateVehicle);
            ServerEvents = new List<ServerEvent>();
            RemoteEvents = new List<RemoteEvent>();
            Exportables = new List<LuaExport>();
            _commandManager = new CommandManager(this);
        }

        /// <summary>
        /// Returns the wrapped dimension object of the given dimension value from the pool.
        /// If no dimension object is found, a new one is getting created.
        /// </summary>
        /// <param name="id">The id of the dimension</param>
        /// <returns>The wrapped dimension</returns>
        internal Dimension CreateDimension(uint id)
        {
            if (id == 0) return _globalDim;
            lock (Dimensions)
            {
                for (int i = Dimensions.Count - 1; i >= 0; i--)
                {
                    Dimension dimension = Dimensions[i];
                    if (dimension.Value == id)
                    {
                        return dimension;
                    }
                }

                Dimension newDim = new Dimension(this, id);
                Dimensions.Add(newDim);
                return newDim;
            }
        }

        public void OverrideEntityFactory<T>(IEntityFactory<T> factory) where T : Entity
        {
            if (typeof(Player).IsAssignableFrom(typeof(T)))
            {
                PlayerFactory = (IEntityFactory<Player>) factory;
            }
            else if (typeof(Door).IsAssignableFrom(typeof(T)))
            {
                DoorFactory = (IEntityFactory<Door>) factory;
            }
            else if (typeof(NPC).IsAssignableFrom(typeof(T)))
            {
                NPCFactory = (IEntityFactory<NPC>) factory;
            }
            else if (typeof(Object).IsAssignableFrom(typeof(T)))
            {
                ObjectFactory = (IEntityFactory<Object>) factory;
            }
            else if (typeof(Pickup).IsAssignableFrom(typeof(T)))
            {
                PickupFactory = (IEntityFactory<Pickup>) factory;
            }
            else if (typeof(Text3D).IsAssignableFrom(typeof(T)))
            {
                Text3DFactory = (IEntityFactory<Text3D>) factory;
            }
            else if (typeof(Vehicle).IsAssignableFrom(typeof(T)))
            {
                VehicleFactory = (IEntityFactory<Vehicle>) factory;
            }
        }

        /// <summary>
        /// Returns a player which fits to the given check.
        /// </summary>
        /// <param name="check">The check which needs to fit to the wanted player</param>
        /// <returns>The wanted player or null</returns>
        internal Player GetPlayerBy(Predicate<Player> check)
        {
            IReadOnlyList<Player> players = Players;
            for (int i = players.Count - 1; i >= 0; i--)
            {
                Player player = players[i];
                if (check.Invoke(player))
                {
                    return player; 
                }
            }

            return null;
        }

        public void RegisterServerEvents(object owner)
        {
            lock (ServerEvents)
            {
                foreach (MethodInfo method in owner.GetType().GetRuntimeMethods())
                {
                    if(method.IsStatic) continue;
                    ServerEvent @event = method.GetCustomAttribute<ServerEvent>();
                    if (@event == null) continue;
                    @event.SetHandler(owner, method);
                    ServerEvents.Add(@event);
                }
            }
        }

        public void RegisterServerEvents<T>()
        {
            lock (ServerEvents)
            {
                foreach (MethodInfo method in typeof(T).GetRuntimeMethods())
                {
                    if(!method.IsStatic) continue;
                    ServerEvent @event = method.GetCustomAttribute<ServerEvent>();
                    if (@event == null) continue;
                    @event.SetHandler(null, method);
                    ServerEvents.Add(@event);
                }
            }
        }

        public void RegisterRemoteEvents<T>()
        {
            lock (RemoteEvents)
            {
                foreach (MethodInfo method in typeof(T).GetRuntimeMethods())
                {
                    if(!method.IsStatic) continue;
                    RemoteEvent @event = method.GetCustomAttribute<RemoteEvent>();
                    if (@event == null) continue;
                    Onset.RegisterRemoteEvent(Owner.Plugin.Meta.Id, @event.Name);
                    @event.SetHandler(null, method);
                    RemoteEvents.Add(@event);
                }
            }
        }

        public void RegisterRemoteEvents(object owner)
        {
            lock (RemoteEvents)
            {
                foreach (MethodInfo method in owner.GetType().GetRuntimeMethods())
                {
                    if(method.IsStatic) continue;
                    RemoteEvent @event = method.GetCustomAttribute<RemoteEvent>();
                    if (@event == null) continue;
                    Onset.RegisterRemoteEvent(Owner.Plugin.Meta.Id, @event.Name);
                    @event.SetHandler(owner, method);
                    RemoteEvents.Add(@event);
                }
            }
        }

        internal bool CallEventUnsafely(string name, params object[] args)
        {
            bool flag = true;
            lock (ServerEvents)
            {
                foreach (ServerEvent @event in ServerEvents)
                {
                    if (@event.Type == EventType.Custom && @event.Specification == name)
                    {
                        if (!@event.FireEvent(args))
                            flag = false;
                    }
                }
            }

            return flag;
        }

        public bool CallEvent(string name, params object[] args)
        {
            bool flag = true;
            RepairCustomEventArgs(args);
            lock (ServerEvents)
            {
                foreach (ServerEvent @event in ServerEvents)
                {
                    if (@event.Type == EventType.Custom && @event.Specification == name)
                    {
                        if (!@event.FireEvent(args))
                            flag = false;
                    }
                }
            }

            return flag;
        }

        internal void Inject()
        {
            _commandManager.RegisterCommands(Bridge.Runtime, "native");
        }

        public void RegisterCommands(object owner)
        {
            _commandManager.RegisterCommands(owner, Owner.Plugin.Meta.Id);
        }

        public void RegisterCommands<T>()
        {
            _commandManager.RegisterCommands<T>(Owner.Plugin.Meta.Id);
        }

        public LuaPackage ImportPackage(string packageName)
        {
            Onset.ImportPackage(packageName);
            return new LuaPackage(Owner.Plugin.Meta.Id, packageName);
        }

        public void RegisterExportable(object owner)
        {
            lock (Exportables)
            {
                foreach (MethodInfo method in owner.GetType().GetRuntimeMethods())
                {
                    if(method.IsStatic) continue;
                    LuaExport export = method.GetCustomAttribute<LuaExport>();
                    if (export == null) continue;
                    export.SetHandler(owner, method);
                    Exportables.Add(export);
                }
            }
        }

        public void RegisterExportable<T>()
        {
            lock (Exportables)
            {
                foreach (MethodInfo method in typeof(T).GetRuntimeMethods())
                {
                    if(!method.IsStatic) continue;
                    LuaExport export = method.GetCustomAttribute<LuaExport>();
                    if (export == null) continue;
                    export.SetHandler(null, method);
                    Exportables.Add(export);
                }
            }
        }

        public void ShutdownServer()
        {
            Onset.ShutdownServer();
        }

        public Dimension GetDimension(uint val)
        {
            return CreateDimension(val);
        }

        public Door CreateDoor(int model, Vector pos, double yaw, bool enableInteract = true)
        {
            return _globalDim.CreateDoor(model, pos, yaw, enableInteract);
        }

        public NPC CreateNPC(Vector pos, double heading)
        {
            return _globalDim.CreateNPC(pos, heading);
        }

        public Object CreateObject(int model, Vector pos, Vector rot = null, Vector scale = null)
        {
            return _globalDim.CreateObject(model, pos, rot, scale);
        }

        public Pickup CreatePickup(int model, Vector pos)
        {
            return _globalDim.CreatePickup(model, pos);
        }

        public Text3D CreateText3D(string text, int size, Vector pos, Vector rot = null)
        {
            return _globalDim.CreateText3D(text, size, pos, rot);
        }

        public Vehicle CreateVehicle(VehicleModel model, Vector pos, double heading = 0)
        {
            return _globalDim.CreateVehicle(model, pos, heading);
        }

        public void InvokeMainThread(Action callback)
        {                
            _taskQueue.Enqueue(callback);
        }

        internal object FireExportable(string funcName, object[] args)
        {
            lock (Exportables)
            {
                for (int i = Exportables.Count - 1; i >= 0; i--)
                {
                    LuaExport export = Exportables[i];
                    if (export.Name == funcName)
                    {
                        return export.Execute(args);
                    }
                }

                return null;
            }
        }

        private Entity CreateTypedEntity(int id, Type type)
        {
            if (type == DoorType)
            {
                return CreateDoor(id);
            }
            
            if (type == NPCType)
            {
                return CreateNPC(id);
            }
            
            if (type == ObjectType)
            {
                return CreateObject(id);
            }
            
            if (type == PickupType)
            {
                return CreatePickup(id);
            }
            
            if (type == PlayerType)
            {
                return CreatePlayer(id);
            }
            
            if (type == Text3DType)
            {
                return CreateText3D(id);
            }
            
            if (type == VehicleType)
            {
                return CreateVehicle(id);
            }
            
            return type == NPCType ? CreateNPC(id) : null;
        }

        internal void FireRemoteEvent(string name, int player, object[] nArgs)
        {
            lock (RemoteEvents)
            {
                for (int i = RemoteEvents.Count - 1; i >= 0; i--)
                {
                    RemoteEvent @event = RemoteEvents[i];
                    if (@event.Name == name)
                    {
                        ParameterInfo[] parameters = @event.Parameters;
                        object[] args = new object[nArgs.Length + 1];
                        args[0] = CreatePlayer(player);
                        for (int j = 0; j < nArgs.Length; j++)
                        {
                            ParameterInfo parameter = parameters[j + 1];
                            object raw = nArgs[j];
                            if (EntityType.IsAssignableFrom(parameter.ParameterType) && raw is int id)
                            {
                                raw = CreateTypedEntity(id, parameter.ParameterType);
                            }
                            
                            args[j + 1] = raw;
                        }

                        @event.FireEvent(args);
                        break;
                    }
                }
            }
        }

        internal void FireCommand(int player, string name, string line)
        {
            _commandManager.ExecuteCommand(name, line, player);
        }

        internal bool CallEvent(EventType type, params object[] eventArgs)
        {
            bool flag = true;
            lock (ServerEvents)
            {
                foreach (ServerEvent @event in ServerEvents)
                {
                    if (@event.Type == type)
                    {
                        if (!@event.FireEvent(eventArgs))
                            flag = false;
                    }
                }
            }

            if (type == EventType.GameTick)
            {
                while (_taskQueue.TryDequeue(out Action callback))
                {
                    callback.Invoke();
                }
            }

            return flag;
        }

        /// <summary>
        /// Creates an entity which will fit the needed attach type.
        /// </summary>
        /// <param name="type">The attach type to which the entity needs to fit</param>
        /// <param name="id">The id of the entity</param>
        /// <returns>The wrapped entity which was attached or null if none was hit</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal Entity CreateAttachEntity(AttachType type, int id)
        {
            switch (type)
            {
                case AttachType.None:
                    return null;
                case AttachType.Player:
                    return CreatePlayer(id);
                case AttachType.Vehicle:
                    return CreateVehicle(id);
                case AttachType.Object:
                    return CreateObject(id);
                case AttachType.NPC:
                    return CreateNPC(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Creates an entity which will fit the needed hit type.
        /// </summary>
        /// <param name="type">The hit type to which the entity needs to fit.</param>
        /// <param name="id">The id of the entity.</param>
        /// <returns>The wrapped entity which was hit or null if none was hit.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal Entity CreateHitEntity(HitType type, int id)
        {
            switch (type)
            {
                case HitType.Player:
                    return CreatePlayer(id);
                case HitType.Vehicle:
                    return CreateVehicle(id);
                case HitType.NPC:
                    return CreateNPC(id);
                case HitType.Object:
                    return CreateObject(id);
                case HitType.Landscape:
                case HitType.Air:
                case HitType.Water:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Creates a wrapped player object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped player object</returns>
        internal Player CreatePlayer(int id)
        {
            return PlayerPool.GetEntity(id, () => PlayerFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped door object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped door object</returns>
        internal Door CreateDoor(int id)
        {
            return DoorPool.GetEntity(id, () => DoorFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped NPC object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped NPC object</returns>
        internal NPC CreateNPC(int id)
        {
            return NPCPool.GetEntity(id, () => NPCFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped object</returns>
        internal Object CreateObject(int id)
        {
            return ObjectPool.GetEntity(id, () => ObjectFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped pickup object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped pickup object</returns>
        internal Pickup CreatePickup(int id)
        {
            return PickupPool.GetEntity(id, () => PickupFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped 3d text object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped 3d text object</returns>
        internal Text3D CreateText3D(int id)
        {
            return Text3DPool.GetEntity(id, () => Text3DFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped vehicle object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped vehicle object</returns>
        internal Vehicle CreateVehicle(int id)
        {
            return VehiclePool.GetEntity(id, () => VehicleFactory.Create(id));
        }

        /// <summary>
        /// The arguments passed to the custom event can be corrupted because of a different type. To prevent
        /// failures we will iterate through every argument and if its an Onsharp Entity, we will rewrap it
        /// and fix the wrong typing.
        /// </summary>
        /// <param name="args">The raw arguments</param>
        private void RepairCustomEventArgs(object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                object rawArg = args[i];
                if (rawArg is Player player)
                {
                    args[i] = CreatePlayer(player.Id);
                }
                else if (rawArg is Door door)
                {
                    args[i] = CreateDoor(door.Id);
                }
                else if (rawArg is NPC npc)
                {
                    args[i] = CreateNPC(npc.Id);
                }
                else if (rawArg is Object obj)
                {
                    args[i] = CreateObject(obj.Id);
                }
                else if (rawArg is Pickup pickup)
                {
                    args[i] = CreatePickup(pickup.Id);
                }
                else if (rawArg is Text3D text)
                {
                    args[i] = CreateText3D(text.Id);
                }
                else if (rawArg is Vehicle vehicle)
                {
                    args[i] = CreateVehicle(vehicle.Id);
                }
            }
        }
    }
}