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
        
        internal EntityPool PlayerPool { get; }        

        public IReadOnlyList<Player> Players => PlayerPool.CastEntities<Player>();
        
        private IEntityFactory<Player> PlayerFactory { get; set; }
        
        private List<ServerEvent> ServerEvents { get; }
        
        private List<RemoteEvent> RemoteEvents { get; }

        internal Server(PluginDomain owner)
        {
            Owner = owner;
            PlayerFactory = new PlayerFactory();
            PlayerPool = new EntityPool();
            ServerEvents = new List<ServerEvent>();
            RemoteEvents = new List<RemoteEvent>();
        }

        public void OverrideEntityFactory<T>(IEntityFactory<T> factory) where T : Entity
        {
            if (typeof(Player).IsAssignableFrom(typeof(T)))
            {
                PlayerFactory = (IEntityFactory<Player>) factory;
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
        internal Player CreatePlayer(uint id)
        {
            return PlayerPool.GetEntity(id, () => PlayerFactory.Create(id));
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
            }
        }
    }
}