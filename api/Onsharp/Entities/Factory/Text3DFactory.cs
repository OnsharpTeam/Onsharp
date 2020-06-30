namespace Onsharp.Entities.Factory
{
    /// <summary>
    /// The factory which manages the creation of <see cref="Text3D"/> objects.
    /// </summary>
    internal class Text3DFactory : IEntityFactory<Text3D>
    {
        public Text3D Create(long id)
        {
            return new Text3D(id);
        }
    }
}