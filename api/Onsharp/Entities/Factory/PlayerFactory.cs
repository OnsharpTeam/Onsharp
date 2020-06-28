namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Player"/> objects.
    /// </summary>
    internal class PlayerFactory : IEntityFactory<Player>
    {
        public Player Create(uint id)
        {
            return new Player(id);
        }
    }
}