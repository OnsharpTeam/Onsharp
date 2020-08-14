using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Onsharp.Commands;

namespace Onsharp.Utils
{
    /// <summary>
    /// A class which is only used for some runtime utility functionality.
    /// </summary>
    internal static class RuntimeUtils
    {
        /// <summary>
        /// Returns a parameter string of the given parameters from the given method info.
        /// </summary>
        /// <param name="info">The method with the given parameters</param>
        /// <param name="offset">The offset where to start</param>
        /// <returns>The string containing the parameters</returns>
        internal static Usage GetUsage(MethodInfo info, int offset = 0)
        {
            List<Usage.Parameter> list = new List<Usage.Parameter>();
            StringBuilder builder = new StringBuilder();
            foreach (ParameterInfo parameter in info.GetParameters())
            {
                if (offset > 0)
                {
                    offset--;
                    continue;
                }
                
                builder.Append(" <" + parameter.Name + (parameter.IsOptional ? "?" : "") + ">");
                string description = GetParameterDescription(parameter) ?? "No description found";
                list.Add(new Usage.Parameter(parameter.Name, parameter.ParameterType.Name, description));
            }
            
            return new Usage(builder.ToString(), list);
        }

        private static string GetParameterDescription(ParameterInfo info)
        {
            return info.GetCustomAttribute<Describe>()?.Description;
        }
    }
}