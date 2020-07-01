using System;

namespace Onsharp.Native
{
    /// <summary>
    /// The runtime represents the whole Onsharp instance and its functionality.
    /// </summary>
    public interface IRuntime
    {
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
    }
}