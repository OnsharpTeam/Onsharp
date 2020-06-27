using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Onsharp.Plugin
{
    /// <summary>
    /// The domain load context loads the plugin and its assemblies.
    /// </summary>
    internal class DomainLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly string _pluginPath;

        internal DomainLoadContext(string pluginPath)
        {
            _pluginPath = pluginPath;
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }

        public Assembly Load()
        {
            return LoadFromAssemblyName(new AssemblyName(_pluginPath));
        }
    }
}