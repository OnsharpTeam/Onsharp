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
        /// The entity name of the entity which will be needed for calling specific native methods.
        /// </summary>
        internal string EntityName { get; }
        
        internal EntityPool Pool { get; set; }

        internal Entity(int id, string entityName)
        {
            Id = id;
            EntityName = entityName;
        }
    }
}