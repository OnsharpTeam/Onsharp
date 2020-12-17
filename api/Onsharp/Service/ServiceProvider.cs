using System;
using System.Collections.Generic;

namespace Onsharp.Service
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        internal ServiceProvider()
        {
            _services = new Dictionary<Type, object>();
        }
        
        public T Get<T>() where T : IService
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
                return (T) _services[type];
            return default;
        }

        public void Provide<T>(T service) where T : IService
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
                _services.Remove(type);
            _services.Add(type, service);
        }
    }
}