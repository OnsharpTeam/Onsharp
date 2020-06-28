using System.Collections.Generic;
using Onsharp.Entities;
using Onsharp.Entities.Factory;
using Onsharp.Plugin;

namespace Onsharp
{
    internal class Server : IServer
    {
        #region Native
        
        internal PluginDomain Owner { get; }
        
        #region Entity Pools
        internal EntityPool PlayerPool { get; }
        #endregion
        
        #region Entity Factories
        private IEntityFactory<Player> PlayerFactory { get; set; }
        #endregion
        #endregion

        public IReadOnlyList<Player> Players => PlayerPool.CastEntities<Player>();

        internal Server(PluginDomain owner)
        {
            Owner = owner;
            PlayerFactory = new PlayerFactory();
            PlayerPool = new EntityPool();
        }

        public void OverrideEntityFactory<T>(IEntityFactory<T> factory) where T : Entity
        {
            if (typeof(Player).IsAssignableFrom(typeof(T)))
            {
                PlayerFactory = (IEntityFactory<Player>) factory;
            }
        }

        internal Player CreatePlayer(uint id)
        {
            return PlayerPool.GetEntity(id, () => PlayerFactory.Create(id));
        }
    }
}