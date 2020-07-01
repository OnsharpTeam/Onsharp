using System;
using System.Reflection;

namespace Onsharp.Native
{
    /// <summary>
    /// Converters are mechanisms which take care of the conversion of certain types from a string.
    /// They are used among other things in the area of the command API. 
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// The type the converter is handling.
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// The function which handles the converting process.
        /// </summary>
        private readonly Func<string, Type, object> _process;

        public Converter(Type type, Func<string, Type, object> process)
        {
            _type = type;
            _process = process;
        }

        /// <summary>
        /// Checks if this converter can handle the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter which should be handled</param>
        /// <returns>True, if the convert can handle the parameter</returns>
        internal virtual bool IsHandlerFor(ParameterInfo parameter)
        {
            return parameter.ParameterType == _type;
        }

        /// <summary>
        /// Converts the given value into the wanted type of the parameter.
        /// </summary>
        /// <param name="value">The value of which should be converted</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="server">The server which is getting handled from</param>
        /// <returns>The converted string as object</returns>
        internal virtual object Handle(string value, ParameterInfo parameter, Server server)
        {
            return _process.Invoke(value, parameter.ParameterType);
        }
    }
}