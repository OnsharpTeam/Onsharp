using System;
using System.Reflection;

namespace Onsharp.Events
{
    /// <summary>
    /// A remote event is getting triggered by the client side. If a client-side scripts of a player
    /// wants to send the server some data, it triggers a remote event by specifying the name and sending
    /// objects as parameters with it. The method handler than gets notified. The first parameter
    /// of every handler is a player.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RemoteEvent : Attribute
    {
        /// <summary>
        /// The name of the remote event this handler is listening to.
        /// </summary>
        public string Name { get; }
        
        private MethodInfo _handler;
        private object _owner;

        /// <summary>
        /// All parameters for the handling method.
        /// </summary>
        internal ParameterInfo[] Parameters => _handler.GetParameters();

        public RemoteEvent(string name)
        {
            Name = name;
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
    }
}