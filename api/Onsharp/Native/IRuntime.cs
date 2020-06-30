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
    }
}