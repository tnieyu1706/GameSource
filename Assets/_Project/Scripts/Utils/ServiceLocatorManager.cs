using System.Collections.Generic;

namespace _Project.Scripts.Utils
{
    public abstract class ServiceLocatorManager<TKey, TValue>
    {
        private Dictionary<TKey, TValue> serviceLocator;

        public Dictionary<TKey, TValue> GetServiceLocator =>
            serviceLocator ?? (serviceLocator = new Dictionary<TKey, TValue>());

        public abstract TValue GetService(TKey key);
    }
}