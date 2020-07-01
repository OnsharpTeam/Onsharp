using System;
using System.Reflection;
using Onsharp.Native;

namespace Onsharp.Converters
{
    /// <summary>
    /// This is the basic converter which is the default one and converts the rest of the types which are not given.
    /// </summary>
    internal class BasicConverter : Converter
    {
        private static readonly Type IntType = typeof(int);
        private static readonly Type LongType = typeof(long);
        private static readonly Type ShortType = typeof(short);
        private static readonly Type ByteType = typeof(byte);
        private static readonly Type CharType = typeof(char);
        private static readonly Type FloatType = typeof(float);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type DecimalType = typeof(decimal);
        private static readonly Type BoolType = typeof(bool);

        public BasicConverter() : base(null, Convert)
        {
        }

        internal override bool IsHandlerFor(ParameterInfo parameter)
        {
            return true;
        }

        private static object Convert(string givenObject, Type wantedType)
        {
            if (wantedType == IntType)
            {
                return int.Parse(givenObject);
            }
            if (wantedType == LongType)
            {
                return long.Parse(givenObject);
            }
            if (wantedType == ShortType)
            {
                return short.Parse(givenObject);
            }
            if (wantedType == ByteType)
            {
                return byte.Parse(givenObject);
            }
            if (wantedType == CharType)
            {
                return char.Parse(givenObject);
            }
            if (wantedType == FloatType)
            {
                return float.Parse(givenObject);
            }
            if (wantedType == DoubleType)
            {
                return double.Parse(givenObject);
            }
            if (wantedType == DecimalType)
            {
                return decimal.Parse(givenObject);
            }
            if (wantedType == BoolType)
            {
                return bool.Parse(givenObject);
            }

            return givenObject;
        }
    }
}