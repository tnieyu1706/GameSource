using _Project.Scripts.General.Patterns.Singleton;
using UnityEngine;

namespace _Project.Scripts.Utils
{
    [SingletonFactory]
    public class WaitForSecondsServiceLocator : ServiceLocatorManager<float, WaitForSeconds>
    {
        public override WaitForSeconds GetService(float key)
        {
            if (GetServiceLocator.ContainsKey(key))
            {
                return GetServiceLocator[key];
            }
            
            WaitForSeconds wait = new WaitForSeconds(key);
            GetServiceLocator.Add(key, wait);
            return wait;
        }
    }
}