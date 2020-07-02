namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Object"/> objects.
    /// </summary>
    internal class ObjectFactory : IEntityFactory<Object>
    {
        public Object Create(int id)
        {
            return new Object(id);
        }
    }
}