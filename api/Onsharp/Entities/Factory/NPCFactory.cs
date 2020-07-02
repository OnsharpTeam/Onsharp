namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="NPC"/> objects.
    /// </summary>
    internal class NPCFactory : IEntityFactory<NPC>
    {
        public NPC Create(int id)
        {
            return new NPC(id);
        }
    }
}