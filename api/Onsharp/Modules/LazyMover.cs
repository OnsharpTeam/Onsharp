using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Onsharp.Native;
using Onsharp.Plugins;

namespace Onsharp.Modules
{
    /// <summary>
    /// The lazy mover module is a module which provides the server with a pre sorter for DLLs.
    /// Lazy mover creates a folder where a dev can drop all its files, the mover than will check if the
    /// DLL is a plugin - which than gets moved to the plugins folder - or a third-party library.
    /// Other files will be ignored.
    /// </summary>
    internal static class LazyMover
    {
        private static readonly string DirPath = Path.Combine(Bridge.ServerPath, "lazy");
        
        /// <summary>
        /// Starts the lazy mover.
        /// </summary>
        internal static void Start()
        {
            if(!Bridge.Config.LazyMoverActive) return;
            Bridge.Logger.Info("LazyMover initialized!");
            Directory.CreateDirectory(DirPath);
            foreach (string path in Directory.GetFiles(DirPath))
            {
                if (Path.GetExtension(path).ToLower().EndsWith("dll"))
                    TryMove(path);
            }
        }

        private static void TryMove(string path)
        {
            try
            {
                bool flag = IsFileAPlugin(path);
                Bridge.Logger.Debug("LazyDrop found ({PATH}) its a {MODE}!", path, flag ? "PLUGIN" : "LIB");
                string dest = Path.Combine(flag ? Bridge.PluginsPath : Bridge.LibsPath, Path.GetFileName(path));
                if(File.Exists(dest))
                    File.Delete(dest);
                File.Move(path, dest, true);
            }
            catch (Exception ex)
            {
                Bridge.Logger.Debug("An error occurred while lazy move {PATH}: \n{EX}", path, ex.ToString());
            }
        }

        private static bool IsFileAPlugin(string path)
        {
            try
            {
                AssemblyLoadContext context = new AssemblyLoadContext("cached_check_" + path, true);
                bool flag = false;
                using (FileStream stream = File.OpenRead(path))
                {
                    Assembly assembly = context.LoadFromStream(stream);
                    foreach (Type type in assembly.ExportedTypes)
                    {
                        if (typeof(Plugin).IsAssignableFrom(type))
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                context.Unload();
                return flag;
            }
            catch
            {
                return false;
            }
        }
    }
}