using System;

namespace Onsharp.World
{
    public class Dimension : IEquatable<Dimension>
    {
        private readonly uint _value;

        internal Dimension(uint value)
        {
            _value = value;
        }

        public bool Equals(Dimension other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Dimension) obj);
        }

        public override int GetHashCode()
        {
            return (int) _value;
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