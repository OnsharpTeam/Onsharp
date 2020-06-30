using System.Collections.Generic;
using System.Reflection;
using Onsharp.Entities;
using Onsharp.Entities.Factory;
using Onsharp.Events;
using Onsharp.Plugins;

namespace Onsharp
{
    internal class Server : IServer
    {
        internal PluginDomain Owner { get; }    

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

        internal Server(PluginDomain owner)
        {
            Owner = owner;
            PlayerFactory = new PlayerFactory();
            PlayerPool = new EntityPool("Players", id =>
            {
                CreatePlayer(id);
            });
            DoorFactory = new DoorFactory();
            DoorPool = new EntityPool("Doors", id =>
            {
                CreateDoor(id);
            });
            NPCFactory = new NPCFactory();
            NPCPool = new EntityPool("NPC", id =>
            {
                CreateNPC(id);
            });
            ObjectFactory = new ObjectFactory();
            ObjectPool = new EntityPool("Objects", id =>
            {
                CreateObject(id);
            });
            PickupFactory = new PickupFactory();
            PickupPool = new EntityPool("Pickups", id =>
            {
                CreatePickup(id);
            });
            Text3DFactory = new Text3DFactory();
            Text3DPool = new EntityPool("Text3D", id =>
            {
                CreateText3D(id);
            });
            VehicleFactory = new VehicleFactory();
            VehiclePool = new EntityPool("Vehicles", id =>
            {
                CreateVehicle(id);
            });
            ServerEvents = new List<ServerEvent>();
            RemoteEvents = new List<RemoteEvent>();
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
                    @event.SetHandler(owner, method);
                    RemoteEvents.Add(@event);
                }
            }
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

        internal void FireRemoteEvent(string name, uint player, object[] args)
        {
            //TODO adding calling of remote events
        }

        internal bool CallEvent(EventType type, object[] eventArgs)
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

            return flag;
        }

        /// <summary>
        /// Creates a wrapped player object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped player object</returns>
        internal Player CreatePlayer(long id)
        {
            return PlayerPool.GetEntity(id, () => PlayerFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped door object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped door object</returns>
        internal Door CreateDoor(long id)
        {
            return DoorPool.GetEntity(id, () => DoorFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped NPC object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped NPC object</returns>
        internal NPC CreateNPC(long id)
        {
            return NPCPool.GetEntity(id, () => NPCFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped object</returns>
        internal Object CreateObject(long id)
        {
            return ObjectPool.GetEntity(id, () => ObjectFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped pickup object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped pickup object</returns>
        internal Pickup CreatePickup(long id)
        {
            return PickupPool.GetEntity(id, () => PickupFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped 3d text object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped 3d text object</returns>
        internal Text3D CreateText3D(long id)
        {
            return Text3DPool.GetEntity(id, () => Text3DFactory.Create(id));
        }

        /// <summary>
        /// Creates a wrapped vehicle object or returns the existing associated instance of it.
        /// </summary>
        /// <param name="id">The session id of the entity</param>
        /// <returns>The wrapped vehicle object</returns>
        internal Vehicle CreateVehicle(long id)
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