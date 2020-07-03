
namespace Onsharp.Native
{
    /// <summary>
    /// The config for the native runtime of Onsharp.
    /// </summary>
    internal class RuntimeConfig
    {
        /// <summary>
        /// Whether the Onsharp runtime is in debug mode or not.
        /// </summary>
        public bool IsDebug { get; set; } = false;

        /// <summary>
        /// Whether Onsharp should keep itself and the plugins updated or not.
        /// </summary>
        public bool KeepUpdated { get; set; } = true;
    }
}