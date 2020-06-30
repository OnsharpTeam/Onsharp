namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Object"/> objects.
    /// </summary>
    internal class ObjectFactory : IEntityFactory<Object>
    {
        public Object Create(long id)
        {
            return new Object(id);
        }
    }
}