using System;
using System.Reflection;

namespace Onsharp.Events
{
    /// <summary>
    /// A server event is event called from the server when specified things happen in the world of Onset.
    /// Marking a method with this attribute marks them as handler for the specified event.
    /// When the event gets fired, the marked method gets called. Therefore the class must be registered
    /// via <see cref="IServer.RegisterServerEvents"/> or <see cref="IServer.RegisterServerEvents{T}"/>.
    /// If an event handler is returning false, the specified event is getting cancelled. The returning value is optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerEvent : Attribute
    {
        /// <summary>
        /// The value indicating the type which will be handled by this handler.
        /// </summary>
        public EventType Type { get; }
        
        /// <summary>
        /// The specification value is only for custom events.
        /// </summary>
        internal string Specification { get; }
        
        private MethodInfo _handler;
        private object _owner;

        /// <summary>
        /// The default constructor. This constructor is for default event listening.
        /// </summary>
        public ServerEvent(EventType type)
        {
            Type = type;
        }

        /// <summary>
        /// The construct which offers the possibility to listen to custom events.
        /// </summary>
        /// <param name="name">The name of the custom event</param>
        public ServerEvent(string name)
        {
            Specification = name;
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