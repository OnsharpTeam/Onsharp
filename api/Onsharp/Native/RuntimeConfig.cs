
using System;

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
        /// Whether Onsharp should keep the plugins updated or not.
        /// </summary>
        public bool KeepPluginsUpdated { get; set; } = true;

        /// <summary>
        /// The timeout how long the console command manager should wait until start to take input.
        /// </summary>
        [Obsolete("The console input is not getting overwatched by the console manager but by the Onset server itself.")]
        public int ConsoleInputTimeout { get; set; } = 3000;

        /// <summary>
        /// Whether the lazy mover is active or not.
        /// </summary>
        public bool LazyMoverActive { get; set; } = false;
    }
}