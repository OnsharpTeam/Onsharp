using System;
using System.Reflection;

namespace Onsharp.Events
{
    /// <summary>
    /// A server event is event called from the server when specified things happen in the world of Onset.
    /// Marking a method with this attribute marks them as handler for the specified event.
    /// When the event gets fired, the marked method gets called. Therefore the class must be registered
    /// via <see cref="IServer.RegisterServerEvents"/> or <see cref="IServer.RegisterServerEvents{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerEvent : Attribute
    {
        /// <summary>
        /// The value indicating the type which will be handled by this handler.
        /// </summary>
        public EventType Type { get; }
        
        private MethodInfo _handler;
        private object _owner;

        /// <summary>
        /// The default constructor.
        /// </summary>
        public ServerEvent(EventType type)
        {
            Type = type;
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