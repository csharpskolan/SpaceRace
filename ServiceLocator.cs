using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceRace
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void AddService<T>(T service)
        {
            var type = typeof(T);
            if (!services.ContainsKey(type))
            {
                services.Add(type, service);
            }
        }

        public static T GetService<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                return (T)services[type];
            }
            throw new InvalidOperationException($"Service of type {type} not found.");
        }
    }
}
