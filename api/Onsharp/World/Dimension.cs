using System;

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