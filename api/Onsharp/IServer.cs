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
    }
}