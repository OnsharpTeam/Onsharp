using System;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Text3D : LifelessEntity
    {
        public Text3D(int id) : base(id, "Text3D")
        {
        }

        public override void SetPosition(double x, double y, double z)
        {
            throw new NotImplementedException("Text3D does not implement Positioning");
        }

        public override void SetPosition(Vector vector)
        {
            throw new NotImplementedException("Text3D does not implement Positioning");
        }

        public override Vector GetPosition()
        {
            throw new NotImplementedException("Text3D does not implement Positioning");
        }
    }
}