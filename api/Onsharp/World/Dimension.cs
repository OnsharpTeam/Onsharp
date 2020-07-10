using System;
using Onsharp.Entities;
using Onsharp.Native;

namespace Onsharp.World
{
    public class Dimension : IEquatable<Dimension>
    {
        private readonly Server _server;
        
        internal uint Value { get; }

        internal Dimension(Server server, uint value)
        {
            _server = server;
            Value = value;
        }

        /// <summary>
        /// Creates a NPC in this dimension.
        /// </summary>
        /// <param name="pos">The position of the NPC</param>
        /// <param name="heading">The yaw rotation of the NPC</param>
        /// <returns>The wrapped NPC object</returns>
        public NPC CreateNPC(Vector pos, double heading)
        {
            NPC npc = _server.CreateNPC(Onset.CreateNPC(pos.X, pos.Y, pos.Z, heading));
            npc.SetDimension(Value);
            return npc;
        }
        
        /// <summary>
        /// Creates a door in this dimension.
        /// </summary>
        /// <param name="model">The model of the door</param>
        /// <param name="pos">The position of the door</param>
        /// <param name="yaw">The yaw of the door</param>
        /// <param name="enableInteract">True enables the interaction with this door when pressing 'E'</param>
        /// <returns>The wrapped door object</returns>
        public Door CreateDoor(int model, Vector pos, double yaw, bool enableInteract = true)
        {
            Door door = _server.CreateDoor(Onset.CreateDoor(model, pos.X, pos.Y, pos.Z, yaw, enableInteract));
            door.SetDimension(Value);
            return door;
        }

        /// <summary>
        /// Creates an explosion in the current dimension.
        /// </summary>
        /// <param name="id">The id of the explosion (between 1 and 20)</param>
        /// <param name="pos">The position where the explosion will spawn</param>
        /// <param name="soundEnabled">If the explosion sound is enabled</param>
        /// <param name="camShakeRadius">How much the camera should shake</param>
        /// <param name="radialForce">The radial force of the explosion</param>
        /// <param name="damageRadius">The radius in which is explosion causes damage</param>
        /// <returns>True, if the explosion could be successfully spawned</returns>
        /// <exception cref="ArgumentException">If the id is out of the id range</exception>
        public bool CreateExplosion(byte id, Vector pos, bool soundEnabled = true, double camShakeRadius = 1000,
            double radialForce = 500_000, double damageRadius = 600)
        {
            if(id < 1 || id > 20)
                throw new ArgumentException("The id can't be greater than 20 and less than 1!");
            return Onset.CreateExplosion(id, pos.X, pos.Y, pos.Z, Value, soundEnabled, camShakeRadius, radialForce,
                damageRadius);
        }

        public bool Equals(Dimension other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Dimension) obj);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }

        public static bool operator ==(Dimension left, Dimension right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Dimension left, Dimension right)
        {
            return !Equals(left, right);
        }
    }
}