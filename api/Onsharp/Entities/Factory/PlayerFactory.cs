namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Player"/> objects.
    /// </summary>
    internal class PlayerFactory : IEntityFactory<Player>
    {
        public Player Create(int id)
        {
            return new Player(id);
        }
    }
}