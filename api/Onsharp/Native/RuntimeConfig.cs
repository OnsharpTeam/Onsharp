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
        /// Whether the lazy mover is active or not.
        /// </summary>
        public bool LazyMoverActive { get; set; } = false;

        /// <summary>
        /// Whether the metrics system is enabled on this server or not.
        /// </summary>
        public bool MetricsEnabled { get; set; } = true;
    }
}