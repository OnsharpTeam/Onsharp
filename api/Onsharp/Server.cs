using System.Collections.Generic;
using System.Reflection;
using Onsharp.Entities;
using Onsharp.Entities.Factory;
using Onsharp.Events;
using Onsharp.Plugin;

namespace Onsharp
{
    internal class Server : IServer
    {
        internal PluginDomain Owner { get; }
        
        internal EntityPool PlayerPool { get; }        

        public IReadOnlyList<Player> Players => PlayerPool.CastEntities<Player>();
        
        private IEntityFactory<Player> PlayerFactory { get; set; }
        
        private List<ServerEvent> ServerEvents { get; }

        internal Server(PluginDomain owner)
        {
            Owner = owner;
            PlayerFactory = new PlayerFactory();
            PlayerPool = new EntityPool();
            ServerEvents = new List<ServerEvent>();
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

        internal Player CreatePlayer(uint id)
        {
            return PlayerPool.GetEntity(id, () => PlayerFactory.Create(id));
        }
    }
}