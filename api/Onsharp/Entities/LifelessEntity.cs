using Onsharp.Native;

namespace Onsharp.Entities
{
    /// <summary>
    /// A lifeless entity is an entity which is not controlled by any player.
    /// </summary>
    public abstract class LifelessEntity : Entity
    {
        protected LifelessEntity(int id, string entityName) : base(id, entityName)
        {
        }

        /// <summary>
        /// Destroys this entity and removes it from the cache.
        /// </summary>
        public void Destroy()
        {
            Pool.RemoveEntity(this);
            Onset.DestroyEntity(EntityName, Id);
        }
    }
}