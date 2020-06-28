using System.Collections.Generic;
using Onsharp.Entities;
using Onsharp.Entities.Factory;

namespace Onsharp
{
    /// <summary>
    /// The interface represents the server in all its functionality.
    /// The interaction and manipulation of the system can be called from here.
    /// Every plugin has its own instance for the server. 
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// A list containing all players currently on the server.
        /// </summary>
        IReadOnlyList<Player> Players { get; }
        
        /// <summary>
        /// Overrides the build in version of the existing entity factory.
        /// </summary>
        /// <param name="factory">The new factory which overrides the old one</param>
        /// <typeparam name="T"></typeparam>
        void OverrideEntityFactory<T>(IEntityFactory<T> factory) where T : Entity;

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Events.ServerEvent"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// </summary>
        /// <param name="owner">The owner object owning the marked methods</param>
        void RegisterServerEvents(object owner);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Events.ServerEvent"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// The difference to the other method is that in this method no owner object is created,
        /// instead only the static methods are registered as handlers. 
        /// </summary>
        /// <typeparam name="T">The type which will be searched through</typeparam>
        void RegisterServerEvents<T>();
    }
}