namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Pickup"/> objects.
    /// </summary>
    internal class PickupFactory : IEntityFactory<Pickup>
    {
        public Pickup Create(int id)
        {
            return new Pickup(id);
        }
    }
}