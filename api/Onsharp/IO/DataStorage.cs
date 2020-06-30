using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nett;
using Onsharp.Native;
using Onsharp.Plugins;

namespace Onsharp.IO
{
    internal class DataStorage : IDataStorage
    {
        private readonly List<object> _cache;
        private readonly string _path;

        public DirectoryInfo Directory { get; }

        internal DataStorage(Plugin plugin)
        {
            _path = Path.Combine(Bridge.DataPath, plugin.Display);
            Directory = System.IO.Directory.CreateDirectory(_path);
            _cache = new List<object>();
        }

        public T Retrieve<T>(T @default = default)
        {
            lock (_cache)
            {
                for (int i = _cache.Count - 1; i >= 0; i--)
                {
                    object obj = _cache[i];
                    if (obj is T t)
                    {
                        return t;
                    }
                }
            }

            return @default;
        }

        public T Config<T>()
        {
            T config = Retrieve<T>();
            if (config != null)
            {
                return config;
            }

            var configMeta = typeof(T).GetCustomAttribute<Config>();
            if (configMeta == null) return default;
            string path = Path.Combine(_path,
                string.IsNullOrEmpty(configMeta.SubPath)
                    ? configMeta.Name + ".toml"
                    : Path.Combine(configMeta.SubPath, configMeta.Name + ".toml"));
            if (!File.Exists(path))
            {
                config = Activator.CreateInstance<T>();
                Toml.WriteFile(config, path);
                lock (_cache)
                    _cache.Add(config);
                return config;
            }

            var readConfig = Toml.ReadFile<T>(path);
            lock(_cache)
                _cache.Add(readConfig);
            return readConfig;
        }
    }
}