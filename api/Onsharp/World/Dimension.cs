using System;
using Onsharp.Entities;
using Onsharp.Enums;
using Onsharp.Native;
using Object = Onsharp.Entities.Object;

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
        /// Creates a vehicle in this dimension.
        /// </summary>
        /// <param name="model">The mode of the vehicle</param>
        /// <param name="pos">The position of the vehicle</param>
        /// <param name="heading">The heading of the vehicle</param>
        /// <returns>The wrapped vehicle object</returns>
        public Vehicle CreateVehicle(VehicleModel model, Vector pos, double heading = 0)
        {
            Vehicle vehicle = _server.CreateVehicle(Onset.CreateVehicle((int) model, pos.X, pos.Y, pos.Z, heading));
            vehicle.SetDimension(Value);
            return vehicle;
        }

        /// <summary>
        /// Creates a 3D text in this dimension.
        /// </summary>
        /// <param name="text">The content of the 3D text</param>
        /// <param name="size">The size of the 3D text</param>
        /// <param name="pos">The position of the 3D text</param>
        /// <param name="rot">The rotation of the 3D text</param>
        /// <returns>The wrapped 3D text object</returns>
        public Text3D CreateText3D(string text, int size, Vector pos, Vector rot = null)
        {
            rot ??= Vector.Empty;
            Text3D text3d =
                _server.CreateText3D(Onset.CreateText3D(text, size, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z));
            text3d.InternalText = text;
            text3d.SetDimension(Value);
            return text3d;
        }

        /// <summary>
        /// Creates a pickup in this dimension.
        /// </summary>
        /// <param name="model">The model of the pickup</param>
        /// <param name="pos">The position of the pickup</param>
        /// <returns>The wrapped pickup object</returns>
        public Pickup CreatePickup(int model, Vector pos)
        {
            Pickup pickup = _server.CreatePickup(Onset.CreatePickup(model, pos.X, pos.Y, pos.Z));
            pickup.SetDimension(Value);
            return pickup;
        }

        /// <summary>
        /// Creates an object in this dimension.
        /// </summary>
        /// <param name="model">The model of the object</param>
        /// <param name="pos">The position of the object</param>
        /// <param name="rot">The rotation of the object</param>
        /// <param name="scale">The scale of the object</param>
        /// <returns>The wrapped object</returns>
        public Object CreateObject(int model, Vector pos, Vector rot = null, Vector scale = null)
        {
            rot ??= Vector.Empty;
            scale ??= Vector.One;
            Object obj = _server.CreateObject(Onset.CreateObject(model, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z,
                scale.X, scale.Y, scale.Z));
            obj.SetDimension(Value);
            return obj;
        }

        /// <summary>
        /// Creates a NPC in this dimension.
        /// </summary>
        /// <param name="pos">The position of the NPC</param>
        /// <param name="heading">The yaw rotation of the NPC</param>
        /// <returns>The wrapped NPC object</returns>
        public NPC CreateNPC(Vector pos, double heading = 0)
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