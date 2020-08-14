using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Onsharp.Native
{
    /// <summary>
    /// The runtime represents the whole Onsharp instance and its functionality.
    /// </summary>
    public interface IRuntime
    {
        /// <summary>
        /// The version of the currently running instance of the game.
        /// </summary>
        int GameVersion { get; }
        
        /// <summary>
        /// The version of the game as string.
        /// </summary>
        string GameVersionString { get; }
        
        /// <summary>
        /// The servers uptime in seconds.
        /// </summary>
        double UptimeSeconds { get; }
        
        /// <summary>
        /// The servers uptime in milliseconds.
        /// </summary>
        double UptimeMillis { get; }
        
        /// <summary>
        /// The delta time in seconds.
        /// </summary>
        double DeltaSeconds { get; }
        
        /// <summary>
        /// Calls a custom event on all current running plugin instances. If the event gets cancelled, this event is returning false.
        /// </summary>
        /// <param name="name">The name of the custom event</param>
        /// <param name="args">The arguments of the custom event. Onsharp Entities are valid but only in the single form. Lists or something like that are not allowed in combination</param>
        /// <returns>False, if the event gets cancelled</returns>
        bool CallEvent(string name, params object[] args);

        /// <summary>
        /// Disables the refreshing of the entity pools when retrieving all elements of the pools.
        /// The disabling is only recommended if only Onsharp will be used for server-side scripting because than the
        /// refreshing process is unnecessary.
        /// </summary>
        void DisableEntityPoolRefreshing();

        /// <summary>
        /// Registers a new converter in the system.
        /// </summary>
        /// <param name="convertProcess">The process which will convert the given string into its typed object</param>
        /// <typeparam name="T">The type which gets handled by this converter</typeparam>
        void RegisterConverter<T>(Func<string, Type, object> convertProcess);

        /// <summary>
        /// Registers a completely custom converter. If you need to override the handle check, extend from the <see cref="Converter"/> class and override it.
        /// With this method you can than register the converter.
        /// </summary>
        /// <param name="converter">The custom converter</param>
        void RegisterCustomConverter(Converter converter);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Commands.ConsoleCommand"/> marked methods and registers them.
        /// <see cref="EntryPoint"/> classes will be registered automatically.
        /// </summary>
        /// <param name="owner">The owner object owning the marked methods</param>
        /// <param name="specifier">The specifier is a back up plan, when a command name is occupied</param>
        void RegisterConsoleCommands(object owner, string specifier);

        /// <summary>
        /// Searches through the class of the given owner objects for <see cref="Commands.ConsoleCommand"/> marked methods and registers them.
        /// <see cref="EntryPoint"/> classes will be registered automatically.
        /// The difference to the other method is that in this method no owner object is created,
        /// instead only the static methods are registered as handlers. 
        /// </summary>
        /// <param name="specifier">The specifier is a back up plan, when a command name is occupied</param>
        /// <typeparam name="T">The type which will be searched through</typeparam>
        void RegisterConsoleCommands<T>(string specifier);

        /// <summary>
        /// Starts a lua package on the server.
        /// </summary>
        /// <param name="packageName">The name of the package which should be started</param>
        void StartPackage(string packageName);
        
        /// <summary>
        /// Stops a lua package currently running on the server.
        /// </summary>
        /// <param name="packageName">The name of the package which should be stopped</param>
        void StopPackage(string packageName);

        /// <summary>
        /// Checks if the lua package by the given name is currently running on the server.
        /// </summary>
        /// <param name="packageName">The name of the package which should be checked</param>
        /// <returns>True, if the package is started</returns>
        bool IsPackageStarted(string packageName);

        /// <summary>
        /// Returns a list containing all lua packages currently running on the server.
        /// </summary>
        /// <returns>A list containing all packages names</returns>
        List<string> GetAllPackages();
    }
}