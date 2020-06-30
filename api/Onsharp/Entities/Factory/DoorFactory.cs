namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Door"/> objects.
    /// </summary>
    internal class DoorFactory : IEntityFactory<Door>
    {
        public Door Create(long id)
        {
            return new Door(id);
        }
    }
}