using System;
using Newtonsoft.Json;

namespace Onsharp.World
{
    /// <summary>
    /// This class represents an euclidean vector and gives some functionality.
    /// </summary>
    public class Vector : IEquatable<Vector>
    {
        /// <summary>
        /// An empty vector which has 0 on every axis.
        /// </summary>
        public static readonly Vector Empty = new Vector();

        /// <summary>
        /// An vector containing only 1 on every axis.
        /// </summary>
        public static readonly Vector One = new Vector(1, 1, 1);

        /// <summary>
        /// For internal use only: This callback gets triggered on change of one of the axis
        /// and will force a sync of this vector.
        /// </summary>
        internal Action SyncCallback { get; set; }
        
        /// <summary>
        /// The x-value of the vector.
        /// </summary>
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                SyncCallback?.Invoke();
            }
        }

        private double _x;

        /// <summary>
        /// The y-value of the vector.
        /// </summary>
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                SyncCallback?.Invoke();
            }
        }

        private double _y;
        
        /// <summary>
        /// The z-value of the vector.
        /// </summary>
        public double Z
        {
            get => _z;
            set
            {
                _z = value;
                SyncCallback?.Invoke();
            }
        }

        private double _z;

        /// <summary>
        /// The overridden addition operator.
        /// Adds the vector v2 onto the vector v1.
        /// </summary>
        /// <returns>The sum vector</returns>
        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1).Add(v2);
        }

        /// <summary>
        /// The overridden subtraction operator.
        /// Subtracts the vector v2 from the vector v1.
        /// </summary>
        /// <returns>The diff vector</returns>
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1).Subtract(v2);
        }

        /// <summary>
        /// The overridden division operator.
        /// Divides the vector v2 from the vector v1.
        /// </summary>
        /// <returns>The quotient vector</returns>
        public static Vector operator /(Vector v1, Vector v2)
        {
            return new Vector(v1).Divide(v2);
        }

        /// <summary>
        /// The overridden multiplication operator.
        /// Multiplies the vector v2 with the vector v1.
        /// </summary>
        /// <returns>The product vector</returns>
        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(v1).Multiply(v2);
        }

        /// <summary>
        /// The overridden modulo operator.
        /// The modulo calculation is like this: v1 % v2
        /// </summary>
        public static Vector operator %(Vector v1, Vector v2)
        {
            return new Vector(v1).Mod(v2);
        }

        public static bool operator ==(Vector left, Vector right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Vector left, Vector right)
        {
            return !Equals(left, right);
        }

        public Vector(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(Vector v) : this(v.X, v.Y, v.Z)
        {
        }

        [JsonConstructor]
        public Vector()
        {
        }

        /// <summary>
        /// Adds the given value on every value of this vector.
        /// </summary>
        /// <param name="v">The value to be added</param>
        /// <returns>This vector</returns>
        public Vector Add(double v)
        {
            X += v;
            Y += v;
            Z += v;
            return this;
        }

        /// <summary>
        /// Subtracts the given value from every value of this vector.
        /// </summary>
        /// <param name="v">The value to be subtracted</param>
        /// <returns>This vector</returns>
        public Vector Subtract(double v)
        {
            X -= v;
            Y -= v;
            Z -= v;
            return this;
        }

        /// <summary>
        /// Multiplies the given value with every value of this vector.
        /// </summary>
        /// <param name="v">The value to be multiplied</param>
        /// <returns>This vector</returns>
        public Vector Multiply(double v)
        {
            X *= v;
            Y *= v;
            Z *= v;
            return this;
        }

        /// <summary>
        /// Divides the given value from every value of this vector.
        /// </summary>
        /// <param name="v">The value to be divided</param>
        /// <returns>This vector</returns>
        public Vector Divide(double v)
        {
            X /= v;
            Y /= v;
            Z /= v;
            return this;
        }

        /// <summary>
        /// Applies module to all values of this vector.
        /// </summary>
        /// <param name="v">The modulo value</param>
        /// <returns>This vector</returns>
        public Vector Mod(double v)
        {
            X %= v;
            Y %= v;
            Z %= v;
            return this;
        }

        /// <see cref="Add(double)"/>
        public Vector Add(Vector o)
        {
            return Add(o.X, o.Y, o.Z);
        }

        /// <see cref="Subtract(double)"/>
        public Vector Subtract(Vector o)
        {
            return Subtract(o.X, o.Y, o.Z);
        }

        /// <see cref="Multiply(double)"/>
        public Vector Multiply(Vector o)
        {
            return Multiply(o.X, o.Y, o.Z);
        }

        /// <see cref="Divide(double)"/>
        public Vector Divide(Vector o)
        {
            return Divide(o.X, o.Y, o.Z);
        }

        /// <see cref="Mod(double)"/>
        public Vector Mod(Vector o)
        {
            return Mod(o.X, o.Y, o.Z);
        }

        /// <see cref="Add(double)"/>
        public Vector Add(double x, double y, double z)
        {
            X += x;
            Y += y;
            Z += z;
            return this;
        }

        /// <see cref="Subtract(double)"/>
        public Vector Subtract(double x, double y, double z)
        {
            X -= x;
            Y -= y;
            Z -= z;
            return this;
        }

        /// <see cref="Multiply(double)"/>
        public Vector Multiply(double x, double y, double z)
        {
            X *= x;
            Y *= y;
            Z *= z;
            return this;
        }

        /// <see cref="Divide(double)"/>
        public Vector Divide(double x, double y, double z)
        {
            X /= x;
            Y /= y;
            Z /= z;
            return this;
        }

        /// <see cref="Mod(double)"/>
        public Vector Mod(double x, double y, double z)
        {
            X %= x;
            Y %= y;
            Z %= z;
            return this;
        }

        /// <summary>
        /// Calculates the distance to the given vector.
        /// </summary>
        /// <param name="o">The other vector</param>
        /// <returns>The distance in float</returns>
        public double DistanceTo(Vector o)
        {
            Vector direction = Direction(this, o);
            return Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
        }

        /// <summary>
        /// Calculates the distance to the given vector.
        /// Different from <see cref="DistanceTo"/> is that it only uses X and Y.
        /// </summary>
        /// <param name="o">The other vector</param>
        /// <returns>The distance in float</returns>
        public double DistanceTo2D(Vector o)
        {
            Vector direction = Direction(this, o);
            return Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
        }

        /// <summary>
        /// Calculates the length of this vector.
        /// </summary>
        /// <returns>The length as double</returns>
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Calculates the length of this vector.
        /// Different from <see cref="Length"/> is that it only uses X and Y.
        /// </summary>
        /// <returns>The length as double</returns>
        public double Length2D()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// Normalizes the vector (divides the vector with the vector's length).
        /// </summary>
        /// <returns>This normalized vector</returns>
        public Vector Normalize()
        {
            return Divide(Length());
        }

        /// <summary>
        /// Normalizes the vector (divides the vector with the vector's length).
        /// Different from <see cref="Normalize"/> is that it only uses X and Y.
        /// </summary>
        /// <returns>This normalized vector</returns>
        public Vector Normalize2D()
        {
            double dist = Length2D();
            return Divide(dist, dist, 1);
        }

        public bool Equals(Vector other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Vector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Turns this vector into a direction vector which is going TO the given vector.
        /// </summary>
        /// <param name="o">The vector which defines the point where this vector is going to</param>
        /// <returns></returns>
        public Vector To(Vector o)
        {
            Vector direction = Direction(this, o);
            X = direction.X;
            Y = direction.Y;
            Z = direction.Z;
            return this;
        }

        /// <summary>
        /// Turns this vector into a direction vector which is coming FROM the given vector.
        /// </summary>
        /// <param name="o">The vector which defines the point where this vector is coming from</param>
        /// <returns></returns>
        public Vector From(Vector o)
        {
            Vector direction = Direction(o, this);
            X = direction.X;
            Y = direction.Y;
            Z = direction.Z;
            return this;
        }

        /// <summary>
        /// Interpolates the current vector with the other given vector by the given sensitivity value.
        /// </summary>
        /// <param name="o">The other vector</param>
        /// <param name="by">The sensitivity value</param>
        /// <returns></returns>
        public Vector Lerp(Vector o, double by)
        {
            X = Lerp(X, o.X, by);
            Y = Lerp(Y, o.Y, by);
            Z = Lerp(Z, o.Z, by);
            return this;
        }

        /// <summary>
        /// Calculates a direction vector between the given vectors.
        /// </summary>
        /// <param name="from">The starting vector where the vector will be coming from</param>
        /// <param name="to">The end vector where the vector will be going to</param>
        /// <returns></returns>
        public static Vector Direction(Vector from, Vector to)
        {
            return new Vector(to.X - from.X, to.Y - from.Y, to.Z - from.Z);
        }

        /// <summary>
        /// Calculates an interpolation of the two values by the given sensitivity value.
        /// </summary>
        /// <param name="d1">The first value</param>
        /// <param name="d2">The second value</param>
        /// <param name="by">The sensitivity value</param>
        /// <returns>The interpolated value</returns>
        public static double Lerp(double d1, double d2, double by)
        {
            return d1 * (1 - by) + d2 * by;
        }

        /// <summary>
        /// Calculates an interpolation of the two vectors by the given sensitivity value.
        /// </summary>
        /// <param name="v1">The first value</param>
        /// <param name="v2">The second value</param>
        /// <param name="by">The sensitivity value</param>
        /// <returns>The interpolated value</returns>
        public static Vector Lerp(Vector v1, Vector v2, double by)
        {
            return new Vector(Lerp(v1.X, v2.X, by), Lerp(v1.Y, v2.Y, by), Lerp(v1.Z, v2.Z, by));
        }
    }
}