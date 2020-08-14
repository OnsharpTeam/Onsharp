using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Onsharp.Utils;

namespace Onsharp.Commands
{
    /// <summary>
    /// Methods marked with this command attribute are marked as command handlers.
    /// Each handling method must have a <see cref="Onsharp.Entities.Player"/> as first argument
    /// followed by the arguments of the command. Arguments can be optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Command : Attribute
    {
        /// <summary>
        /// The name of the command defining the chat root.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// The description of the command describing what the command is doing.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// The permission needed to run this command. Can be wildcarded.
        /// </summary>
        public string Permission { get; set; }
        
        /// <summary>
        /// The aliases of the command. An alias is an optional other name instead of the given one.
        /// </summary>
        public string[] Aliases { get; }
      
        /// <summary>
        /// The usage of the command.
        /// </summary>
        internal Usage Usage { get; private set; }
        
        /// <summary>
        /// The command names which can be used.
        /// </summary>
        internal string CommandText { get; private set; }
        
        private MethodInfo _handler;
        private object _owner;

        /// <summary>
        /// The main constructor which offers every parameter.
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="description">The description of the command</param>
        /// <param name="permission">The permission which is needed in order to run the command</param>
        /// <param name="aliases">The aliases of the command</param>
        public Command(string name, string description, string permission, params string[] aliases)
        {
            Permission = permission;
            Description = description;
            Aliases = aliases;
            SetCommandName(name);
        }
        
        /// <summary>
        /// The constructor which offers the minimum of parameters.
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="aliases">The aliases of the command</param>
        public Command(string name, params string[] aliases) : this(name, "No description found", null, aliases)
        {
        }

        /// <summary>
        /// Sets the name of this command and generates the command text.
        /// </summary>
        internal void SetCommandName(string name)
        {
            Name = name;
            StringBuilder builder = new StringBuilder();
            builder.Append("/" + name);
            foreach (string alias in Aliases)
            {
                builder.Append(", /" + alias);
            }

            CommandText = builder.ToString();
        }
        
        /// <summary>
        /// Sets the handling data for this server event.
        /// </summary>
        /// <param name="owner">The owner of the handler, if static, the owner is null</param>
        /// <param name="handler">The handling method as handler</param>
        internal void SetHandler(object owner, MethodInfo handler)
        {
            _handler = handler;
            _owner = owner;
            Usage = RuntimeUtils.GetUsage(_handler, 1);
        }

        /// <summary>
        /// Fires the server event handling method with the given arguments and returns if the cancel state as bool, if given.
        /// </summary>
        /// <param name="args">The events arguments passed to the handler</param>
        /// <returns>False if the event should be cancelled</returns>
        internal bool FireEvent(object[] args)
        {
            return _handler.Invoke(_owner, args) as bool? ?? true;
        }

        /// <summary>
        /// Returns an array of parameters of the handling method.
        /// </summary>
        /// <returns>All parameters</returns>
        internal ParameterInfo[] GetParameters()
        {
            return _handler.GetParameters();
        }
    }
}