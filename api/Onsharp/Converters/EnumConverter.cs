using System;
using System.Reflection;
using Onsharp.Native;

namespace Onsharp.Converters
{
    /// <summary>
    /// The enum converter is a special converter because it does not use the default checking method for the handle check.
    /// </summary>
    internal class EnumConverter : Converter
    {
        internal EnumConverter() : base(null, Convert)
        {
        }

        internal override bool IsHandlerFor(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsEnum;
        }

        private static object Convert(string val, Type type)
        {
            return Enum.Parse(type, val);
        }
    }
}