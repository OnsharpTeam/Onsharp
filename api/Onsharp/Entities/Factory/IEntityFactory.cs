namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// An entity factory creates an instance of an entity.
    /// Every entity must have a constructor with just the session id as parameter.
    /// The factory creates the entity and puts it into the pool for later use.
    /// </summary>
    /// <typeparam name="T">The type of entity the factory handles</typeparam>
    public interface IEntityFactory<out T> where T : Entity
    {
        /// <summary>
        /// Creates the wanted entity for the given id.
        /// </summary>
        /// <param name="id">The id of the new entity</param>
        /// <returns>The created entity</returns>
        T Create(int id);
    }
}