using System;
using System.Reflection;
using System.Runtime.Loader;
using Onsharp.Native;

namespace Onsharp.Plugins
{
    /// <summary>
    /// The domain load context loads the plugin and its assemblies.
    /// </summary>
    internal class DomainLoadContext : AssemblyLoadContext
    {
        private readonly string _pluginPath;

        internal DomainLoadContext(string pluginPath) : base(true)
        {
            _pluginPath = pluginPath;
        }

        public Assembly Load()
        {
            return LoadFromAssemblyPath(_pluginPath);
        }
    }
}