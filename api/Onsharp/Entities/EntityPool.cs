using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Onsharp.Native;

namespace Onsharp.Entities
{
    /// <summary>
    /// The entity pool manages all entity instances created belonging to the given plugin.
    /// </summary>
    internal class EntityPool
    {
        private List<Entity> _entities;
        private readonly string _entityName;
        private readonly Func<int, Entity> _creator;
        private readonly Server _server;
        
        public EntityPool(Server server, string entityName, Func<int, Entity> creator)
        {
            _server = server;
            _entityName = entityName;
            _creator = creator;
            _entities = new List<Entity>();
        }

        internal bool Validate(Entity entity)
        {
            if (Onset.IsEntityValid(entity.Id, entity.EntityName))
                return true;
            RemoveEntity(entity);
            return false;
        }

        internal void RemoveEntity(Entity entity)
        {
            lock (_entities)
                _entities.Remove(entity);
        }

        internal T GetEntity<T>(int id, Func<T> creator) where T : Entity
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
                newEntity.Owner = _server;
                _entities.Add(newEntity);
                return newEntity;
            }
        }
        
        internal IReadOnlyList<T> CastEntities<T>() where T : Entity
        {
            if (Bridge.IsEntityRefreshingEnabled && _entityName != null)
            {
                int len = 0;
                IntPtr ptr = Onset.GetEntities(_entityName, ref len);
                int[] entities = new int[len];
                Marshal.Copy(ptr, entities, 0, len);
                List<Entity> newEntities = new List<Entity>();
                foreach (int entityId in entities)
                {
                    newEntities.Add(_creator.Invoke(entityId));
                }

                lock (_entities)
                    _entities = newEntities;
                Onset.ReleaseIntArray(ptr);
            }
            
            lock (_entities)
            {
                return _entities.Cast<T>().ToList().AsReadOnly();
            }
        }
    }
}