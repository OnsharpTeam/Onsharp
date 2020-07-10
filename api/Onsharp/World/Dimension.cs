using System;
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