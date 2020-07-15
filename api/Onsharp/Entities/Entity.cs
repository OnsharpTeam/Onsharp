using Onsharp.Enums;
using Onsharp.Native;
using Onsharp.World;

namespace Onsharp.Entities
{
    /// <summary>
    /// The base class of every entity in the world of Onset.
    /// Everything which can be placed in the world is an entity. 
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// The session id of the entity in the world. The id is given by the server and is unique.
        /// The id will change on restart.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Checks if the entity is valid or not.
        /// </summary>
        public bool IsValid => Pool.Validate(this);

        /// <summary>
        /// The dimension of the entity.
        /// </summary>
        public Dimension Dimension
        {
            get => Owner.GetDimension(Onset.GetEntityDimension(EntityName, Id));
            set => Onset.SetEntityDimension(EntityName, Id, value.Value);
        }

        /// <summary>
        /// The entity name of the entity which will be needed for calling specific native methods.
        /// </summary>
        internal string EntityName { get; }
        
        internal Server Owner { get; set; }
        
        internal EntityPool Pool { get; set; }

        internal Entity(int id, string entityName)
        {
            Id = id;
            EntityName = entityName;
        }

        /// <summary>
        /// Sets the current position of this entity to the given vector.
        /// </summary>
        /// <param name="vector">The vector to be set</param>
        public virtual void SetPosition(Vector vector)
        {
            SetPosition(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Sets the current position of the entity to the given axis values.
        /// </summary>
        /// <param name="x">The x axis value</param>
        /// <param name="y">The y axis value</param>
        /// <param name="z">The z axis value</param>
        public virtual void SetPosition(double x, double y, double z)
        {
            Onset.SetEntityPosition(Id, EntityName, x, y, z);
        }
        
        /// <summary>
        /// Gets the current position of this entity.
        /// </summary>
        /// <returns>The position as vector</returns>
        public virtual Vector GetPosition()
        {
            double x = 0, y = 0, z = 0;
            Onset.GetEntityPosition(Id, EntityName, ref x, ref y, ref z);
            Vector vector = new Vector(x, y, z);
            vector.SyncCallback = () => SetPosition(vector);
            return vector;
        }

        /// <summary>
        /// Sets the dimension of this entity by the given dimension id.
        /// </summary>
        /// <param name="id">The id of the wanted dimension</param>
        public void SetDimension(uint id)
        {
            Dimension = Owner[id];
        }

        /// <summary>
        /// Sets the property value for the given name of the entity.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value to be set to</param>
        /// <param name="sync">Whether the value should be synced to the clients or not</param>
        public void SetPropertyValue(string name, object value, bool sync = false)
        {
            NativeValue nVal = Bridge.CreateNValue(value);
            Onset.SetPropertyValue(EntityName, Id, name, nVal.NativePtr, sync);
        }

        /// <summary>
        /// Gets the property value for the given name of the entity.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <typeparam name="T">The wanted return type</typeparam>
        /// <returns>The casted value</returns>
        public T GetPropertyValue<T>(string name)
        {
            return (T) new NativeValue(Onset.GetPropertyValue(EntityName, Id, name)).GetValue();
        }
    }
}