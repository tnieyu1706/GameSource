using System;
using System.Collections.Generic;

namespace _Project.Scripts.Item
{
    public class IdentifyManager
    {
        public Dictionary<int, UnityEngine.Object> Identifiers = new();
        public int CurrentId = -1;
    }
    /// <summary>
    /// Static class Manager. (not Runtime)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class IdentifySystemHelper
    {
        internal static readonly Dictionary<Type, IdentifyManager> Registries = new();

        public static int GenerateId<T>(T obj) where T : UnityEngine.Object
        {
            var type = typeof(T);
            if (!Registries.ContainsKey(type))
                Registries[type] = new IdentifyManager();

            var manager = Registries[type];

            // Nếu đã có thì trả lại id cũ
            foreach (var kv in manager.Identifiers)
                if (kv.Value == obj)
                    return kv.Key;

            int id = ++manager.CurrentId;
            manager.Identifiers[id] = obj;
            return id;
        }

        public static UnityEngine.Object GetItem(Type type, int id)
        {
            return Registries.TryGetValue(type, out var manager) && manager.Identifiers.ContainsKey(id)
                ? manager.Identifiers[id]
                : null;
        }

        public static void RemoveIdentify(Type type, int id)
        {
            if (Registries.TryGetValue(type, out var manager))
                manager.Identifiers.Remove(id);
        }
        
    }
}