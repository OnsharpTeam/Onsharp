using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Onsharp.Plugins
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly List<string> _pathsLoaded;
        
        internal PluginLoadContext() : base(null, true)
        {
            _pathsLoaded = new List<string>();
        }

        public Assembly Load(string path)
        {
            if (_pathsLoaded.Contains(path))
            {
                return null;
            }
            
            _pathsLoaded.Add(path);
            using Stream stream = File.OpenRead(path);
            return LoadFromStream(stream);
        }

        public void UnloadAndClean()
        {
            _pathsLoaded.Clear();
            Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}