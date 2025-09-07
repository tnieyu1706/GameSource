using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.General.Patterns.UnityTechnique
{
    /// <summary>
    /// ScriptableObject - CacheData.
    /// Contain 1 class Serializable to config.
    /// Data - Can Runtime Generate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CacheEntityScriptable<T> : ScriptableObject
    {
        public T entity;
    }
}