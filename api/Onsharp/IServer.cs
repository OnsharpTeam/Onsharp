using System;
using System.Collections.Generic;
using Onsharp.Entities;
using Onsharp.Entities.Factory;
using Onsharp.Enums;
using Onsharp.Interop;
using Onsharp.Native;
using Onsharp.World;
using Object = Onsharp.Entities.Object;

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
        /// The name of the server.
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// The tick rate of the server.
        /// </summary>
        double TickRate { get; }
        
        /// <summary>
        /// The count of the maximum value of players which can enter the server.
        /// </summary>
        int MaxPlayers { get; }
        
        /// <summary>
        /// The current statistics of the network.
        /// </summary>
        NetworkStats NetworkStats { get; }
        
        /// <summary>
        /// Returns the wrapped dimension from the given id. 
        /// </summary>
        /// <param name="val">The dimension id</param>
        Dimension this[uint val] { get; }
        
        /// <summary>
        /// A list containing all players currently on the server.
        /// </summary>
        IReadOnlyList<Player> Players { get; }
        
        /// <summary>
        /// A list containing all doors currently on the server.
        /// </summary>
        IReadOnlyList<Door> Doors { get; }
        
        /// <summary>
        /// A list containing all NPCs currently on the server.
        /// </summary>
        IReadOnlyList<NPC> NPCs { get; }
        
        /// <summary>
        /// A list containing all objects currently on the server.
        /// </summary>
        IReadOnlyList<Object> Objects { get; }
        
        /// <summary>
        /// A list containing all pickups currently on the server.
        /// </summary>
        IReadOnlyList<Pickup> Pickups { get; }
        
        /// <summary>
        /// A list containing all 3d texts currently on the server.
        /// </summary>
        IReadOnlyList<Text3D> Text3Ds { get; }
        
        /// <summary>
        /// A list containing all vehicles currently on the server.
        /// </summary>
        IReadOnlyList<Vehicle> Vehicles { get; }
        
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

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Events.RemoteEvent"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// </summary>
        /// <param name="owner">The owner object owning the marked methods</param>
        void RegisterRemoteEvents(object owner);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Events.RemoteEvent"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// The difference to the other method is that in this method no owner object is created,
        /// instead only the static methods are registered as handlers. 
        /// </summary>
        /// <typeparam name="T">The type which will be searched through</typeparam>
        void RegisterRemoteEvents<T>();

        /// <summary>
        /// Calls a custom event on this server with the given arguments. If the event gets cancelled, this event is returning false.
        /// </summary>
        /// <param name="name">The name of the custom event</param>
        /// <param name="args">The arguments of the custom event. Onsharp Entities are valid but only in the single form. Lists or something like that are not allowed in combination</param>
        /// <returns>False, if the event gets cancelled</returns>
        bool CallEvent(string name, params object[] args);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Commands.Command"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// </summary>
        /// <param name="owner">The owner object owning the marked methods</param>
        void RegisterCommands(object owner);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Commands.Command"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// The difference to the other method is that in this method no owner object is created,
        /// instead only the static methods are registered as handlers. 
        /// </summary>
        /// <typeparam name="T">The type which will be searched through</typeparam>
        void RegisterCommands<T>();

        /// <summary>
        /// Imports the package from the given name and thus provides access to the functions exported there.
        /// </summary>
        /// <param name="packageName">The name of the wanted package</param>
        /// <returns>The imported package pipeline</returns>
        LuaPackage ImportPackage(string packageName);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Interop.LuaExport"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// </summary>
        /// <param name="owner">The owner object owning the marked methods</param>
        void RegisterExportable(object owner);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Interop.LuaExport"/> marked methods and registers them.
        /// <see cref="IEntryPoint"/> classes will be registered automatically.
        /// The difference to the other method is that in this method no owner object is created,
        /// instead only the static methods are registered as handlers. 
        /// </summary>
        /// <typeparam name="T">The type which will be searched through</typeparam>
        void RegisterExportable<T>();

        /// <summary>
        /// Shuts down the server.
        /// </summary>
        void ShutdownServer();

        /// <summary>
        /// Returns the wrapped dimension object to the given id.
        /// </summary>
        /// <param name="val">The id of the dimension</param>
        /// <returns>The wrapped dimension object</returns>
        Dimension GetDimension(uint val);

        /// <summary>
        /// Creates a door in the global dimension.
        /// </summary>
        /// <param name="model">The model of the door</param>
        /// <param name="pos">The position of the door</param>
        /// <param name="yaw">The yaw of the door</param>
        /// <param name="enableInteract">True enables the interaction with this door when pressing 'E'</param>
        /// <returns>The wrapped door object</returns>
        Door CreateDoor(int model, Vector pos, double yaw, bool enableInteract = true);

        /// <summary>
        /// Creates a NPC in this dimension.
        /// </summary>
        /// <param name="pos">The position of the NPC</param>
        /// <param name="heading">The yaw rotation of the NPC</param>
        /// <returns>The wrapped NPC object</returns>
        NPC CreateNPC(Vector pos, double heading);

        /// <summary>
        /// Creates an object in this dimension.
        /// </summary>
        /// <param name="model">The model of the object</param>
        /// <param name="pos">The position of the object</param>
        /// <param name="rot">The rotation of the object</param>
        /// <param name="scale">The scale of the object</param>
        /// <returns>The wrapped object</returns>
        Object CreateObject(int model, Vector pos, Vector rot = null, Vector scale = null);

        /// <summary>
        /// Creates a pickup in this dimension.
        /// </summary>
        /// <param name="model">The model of the pickup</param>
        /// <param name="pos">The position of the pickup</param>
        /// <returns>The wrapped pickup object</returns>
        Pickup CreatePickup(int model, Vector pos);

        /// <summary>
        /// Creates a 3D text in this dimension.
        /// </summary>
        /// <param name="text">The content of the 3D text</param>
        /// <param name="size">The size of the 3D text</param>
        /// <param name="pos">The position of the 3D text</param>
        /// <param name="rot">The rotation of the 3D text</param>
        /// <returns>The wrapped 3D text object</returns>
        Text3D CreateText3D(string text, int size, Vector pos, Vector rot = null);

        /// <summary>
        /// Creates a vehicle in this dimension.
        /// </summary>
        /// <param name="model">The mode of the vehicle</param>
        /// <param name="pos">The position of the vehicle</param>
        /// <param name="heading">The heading of the vehicle</param>
        /// <returns>The wrapped vehicle object</returns>
        Vehicle CreateVehicle(VehicleModel model, Vector pos, double heading = 0);
        
        /// <summary>
        /// Adds the given callback to the task queue of the main thread. When the task queue is getting proceed
        /// the callback is getting invoked from the main thread.
        /// This method should be used from other threads than the main thread if interaction with the wrapped api
        /// is wanted.
        /// </summary>
        /// <param name="callback">The callback which is added to the queue</param>
        void InvokeMainThread(Action callback);
    }
}