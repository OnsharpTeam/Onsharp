using System.Collections.Generic;

namespace Onsharp.Commands
{
    internal class Usage
    {
        internal string Text { get; }

        internal List<Parameter> Parameters { get; }

        public Usage(string text, List<Parameter> parameters)
        {
            Text = text;
            Parameters = parameters;
        }

        internal class Parameter
        {
            internal string Name { get; }
            
            internal string Type { get; }
            
            internal string Description { get; }

            public Parameter(string name, string type, string description)
            {
                Name = name;
                Type = type;
                Description = description;
            }
        }
    }
}