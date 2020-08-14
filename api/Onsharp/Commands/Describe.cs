using System;

namespace Onsharp.Commands
{
    /// <summary>
    /// The describe attribute is allowing to add description to parameters which is needed for the parameter generating for the command api.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class Describe : Attribute
    {
        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The constructor which is offering the description.
        /// </summary>
        /// <param name="description">The description of the parameter</param>
        public Describe(string description)
        {
            Description = description;
        }
    }
}