using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.General.Patterns.Singleton
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject
        where T : ScriptableObject
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
#if UNITY_EDITOR
                    // Trong Editor thì tìm bằng AssetDatabase cho tiện
                    string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                    if (guids.Length > 0)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                        instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    }
#else
                // Runtime thì load từ Resources
                instance = Resources.Load<T>(typeof(T).Name);
#endif
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                // Không nên xóa, chỉ cảnh báo thôi
                Debug.LogWarning($"Duplicate {typeof(T).Name} detected: {name}");
            }
        }
    }
}