using System;
using System.Reflection;

namespace Onsharp.Commands
{
    /// <summary>
    /// Methods marked with this command attribute are marked as command handlers.
    /// Console commands are coming from the console input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommand : Attribute
    {
        /// <summary>
        /// The name of the command defining the chat root.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The description of the command.
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// The usage of this command.
        /// The usage defines only the arguments after the name.
        /// </summary>
        public string Usage { get; }
        
        private MethodInfo _handler;
        private object _owner;

        /// <summary>
        /// The main constructor requiring the commands name.
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="usage">The usage of the command</param>
        /// <param name="description">The description of the command</param>
        public ConsoleCommand(string name, string usage, string description)
        {
            Name = name;
            Usage = usage;
            Description = description;
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