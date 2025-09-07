using System;
using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Persistence
{
    [Serializable]
    public class BaseGameObjectPersData : IPersistenceSavable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public Vector3 position;
        public Quaternion rotation;
        [FormerlySerializedAs("lossyScale")] public Vector3 scale;
    }

    public static class BaseGameObjectPersDataExtensions
    {
        public static void SetByTransform(
            this BaseGameObjectPersData persData, Transform transform)
        {
            persData.position = transform.position;
            persData.rotation = transform.rotation;
            persData.scale = transform.localScale;
        }

        public static void SetForTransform(this BaseGameObjectPersData persData, Transform transform)
        {
            transform.position = persData.position;
            transform.rotation = persData.rotation;
            transform.localScale = persData.scale;
        }
    }
}