using System;
using Onsharp.IO;
using Onsharp.Metrics;
using Onsharp.Native;

namespace Onsharp.Plugins
{
    /// <summary>
    /// This interface defines the main plugin class. Only one class is allowed to implement this interface.
    /// The plugin interface inherited the <see cref="EntryPoint"/> interface and all its functionality.
    /// </summary>
    public abstract class Plugin : EntryPoint
    {
        /// <summary>
        /// The meta of this plugin.
        /// </summary>
        public PluginMeta Meta { get; internal set; }
        
        /// <summary>
        /// The current state of this plugin.
        /// </summary>
        public PluginState State { get; internal set; }
        
        /// <summary>
        /// The path to the library file containing this plugin.
        /// </summary>
        public string FilePath { get; internal set; }
        
        /// <summary>
        /// The interface for interacting with the data storage which belongs to this plugin.
        /// </summary>
        public IDataStorage Data { get; internal set; }
        
        /// <summary>
        /// The specialized logger for this plugin.
        /// </summary>
        public ILogger Logger { get; internal set; }

        /// <summary>
        /// The display name of this plugin. Either the name or the plugin id.
        /// </summary>
        public string Display => string.IsNullOrEmpty(Meta.Name) ? Meta.Id : Meta.Name;

        /// <summary>
        /// Gets called when the plugin is getting enabled.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Gets called when the plugin is getting disabled.
        /// </summary>
        public abstract void OnStop();

        /// <summary>
        /// Gets called when the plugin is initialized. This process happens before start and before the i18n is finished but after the plugins are sorted and ordered.
        /// Use this event to add languages manually to the i18n module.
        /// </summary>
        public virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Enables the metrics system for this plugin. In order to use the metrics system, every plugin needs a metrics
        /// secret generated on https://onsharp.eternitylife.de/metrics.
        /// The secret in combination with the plugin id allowing the underlying metrics client to communicate with
        /// the remote metrics API.
        /// After the connection could be successfully established to the metrics API, a connection gets wrapped in a
        /// <see cref="IMetrics"/> client in order to allow simple access.
        /// If the connection failed or something was configured wrong or the MetricsEnabled flag in the runtime config is disabled,
        /// the process will fail and the method returns null.
        /// </summary>
        /// <param name="secret">The secret for the metrics API</param>
        /// <returns>The <see cref="IMetrics"/> client or null on failure</returns>
        protected IMetrics EnableMetrics(string secret)
        {
            try
            {
                if (!Bridge.Config.MetricsEnabled) return null;
                MetricsClient client = new MetricsClient(Meta.Id, secret);
                client.Connect();
                return client;
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error occurred while trying to enable the metrics system!");
                return null;
            }
        }
    }
}