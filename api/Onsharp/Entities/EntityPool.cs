using System;
using System.Collections.Generic;
using System.Linq;
using Onsharp.Native;

namespace Onsharp.Entities
{
    /// <summary>
    /// The entity pool manages all entity instances created belonging to the given plugin.
    /// </summary>
    internal class EntityPool
    {
        private readonly List<Entity> _entities;

        public EntityPool()
        {
            _entities = new List<Entity>();
        }

        internal bool Validate(Entity entity)
        {
            if (Onset.IsEntityValid(Convert.ToInt64(entity.Id), entity.Name))
                return true;
            RemoveEntity(entity);
            return false;
        }

        internal void RemoveEntity(Entity entity)
        {
            lock (_entities)
                _entities.Remove(entity);
        }

        internal T GetEntity<T>(uint id, Func<T> creator) where T : Entity
        {
            lock (_entities)
            {
                for (int i = _entities.Count - 1; i >= 0; i--)
                {
                    Entity entity = _entities[i];
                    if (entity.Id == id)
                    {
                        return (T) entity;
                    }
                }

                T newEntity = creator.Invoke();
                newEntity.Pool = this;
                _entities.Add(newEntity);
                return newEntity;
            }
        }
        
        internal IReadOnlyList<T> CastEntities<T>() where T : Entity
        {
            lock (_entities)
            {
                return _entities.Cast<T>().ToList().AsReadOnly();
            }
        }
    }
}