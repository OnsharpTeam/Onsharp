using Onsharp.World;

namespace Onsharp.Entities
{
    public class Vehicle : LifelessEntity
    {
        public Vehicle(int id) : base(id, "Vehicle")
        {
        }
        
        /// <summary>
        /// Attaches the given object to the entity.
        /// </summary>
        /// <param name="obj">The object which gets attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Object obj, Vector pos, Vector rot, string socketName = "")
        {
            obj.Attach(this, pos, rot, socketName);
        }
        
        /// <summary>
        /// Attaches the given object to the entity.
        /// </summary>
        /// <param name="text3d">The object which gets attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Text3D text3d, Vector pos, Vector rot, string socketName = "")
        {
            text3d.Attach(this, pos, rot, socketName);
        }
    }
}