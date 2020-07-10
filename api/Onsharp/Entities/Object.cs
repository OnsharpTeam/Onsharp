using System;
using Onsharp.Enums;
using Onsharp.Native;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Object : LifelessEntity
    {
        /// <summary>
        /// The rotation of the object.
        /// </summary>
        public Vector Rotation
        {
            get
            {
                double x = 0;
                double y = 0;
                double z = 0;
                Onset.GetObjectRotation(Id, ref x, ref y, ref z);
                Vector vector = new Vector(x, y, z);
                vector.SyncCallback = () => Rotation = vector;
                return vector;
            }
            set => Onset.SetObjectRotation(Id, value.X, value.Y, value.Z);
        }
        
        /// <summary>
        /// The scale of the object.
        /// </summary>
        public Vector Scale
        {
            get
            {
                double x = 0;
                double y = 0;
                double z = 0;
                Onset.GetObjectScale(Id, ref x, ref y, ref z);
                Vector vector = new Vector(x, y, z);
                vector.SyncCallback = () => Scale = vector;
                return vector;
            }
            set => Onset.SetObjectScale(Id, value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Whether the object is attached to anything or not.
        /// </summary>
        public bool IsAttached => Onset.IsObjectAttached(Id);

        /// <summary>
        /// Whether the object is moving or not.
        /// </summary>
        public bool IsMoving => Onset.IsObjectMoving(Id);

        /// <summary>
        /// The model of the object.
        /// </summary>
        public int Model
        {
            get => Onset.GetObjectModel(Id);
            set => Onset.SetObjectModel(Id, value);
        }
        
        public Object(int id) : base(id, "Object")
        {
        }

        /// <summary>
        /// Checks if the object is streamed in for the player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>True, if the object is streamed in for the player</returns>
        public bool IsStreamedIn(Player player)
        {
            return Onset.IsStreamedIn(EntityName, player.Id, Id);
        }

        /// <summary>
        /// Set the specified stream distance for the object. The distance can not be greater than the global stream distance
        /// </summary>
        /// <param name="distance">The distance to be set</param>
        public void SetStreamDistance(double distance)
        {
            Onset.SetObjectStreamDistance(Id, distance);
        }

        /// <summary>
        /// Attaches the object to the given entity.
        /// </summary>
        /// <param name="target">The entity on which the object is attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Entity target, Vector pos, Vector rot, string socketName = "")
        {
            AttachType type = Bridge.GetTypeByEntity(target);
            if(type == AttachType.None) throw new ArgumentException("The target entity is not a valid attach target entity!");
            Onset.SetObjectAttached(Id, (int) type, target.Id, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, socketName);
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

        /// <summary>
        /// Detaches the object from whatever its attach to.
        /// </summary>
        public void Detach()
        {
            Onset.SetObjectDetached(Id);
        }

        /// <summary>
        /// Gets the current attachment info of the object.
        /// </summary>
        /// <returns>The current attachment info</returns>
        public AttachmentInfo GetAttachmentInfo()
        {
            int typeId = 0;
            int entity = 0;
            Onset.GetObjectAttachmentInfo(Id, ref typeId, ref entity);
            AttachType type = (AttachType) typeId;
            return new AttachmentInfo(type, Owner.CreateAttachEntity(type, entity));
        }

        /// <summary>
        /// Forces the object to move to a position by the given speed.
        /// </summary>
        /// <param name="pos">The target position</param>
        /// <param name="speed">The speed of the moving</param>
        public void MoveTo(Vector pos, double speed = 160)
        {
            Onset.SetObjectMoveTo(Id, pos.X, pos.Y, pos.Z, speed);
        }

        /// <summary>
        /// Stops the object from moving.
        /// </summary>
        public void StopMoving()
        {
            Onset.StopObjectMove(Id);
        }

        /// <summary>
        /// Sets the rotation axis of the object.
        /// </summary>
        public void SetRotationAxis(Vector pos)
        {
            Onset.SetObjectRotateAxis(Id, pos.X, pos.Y, pos.Z);
        }
    }
}