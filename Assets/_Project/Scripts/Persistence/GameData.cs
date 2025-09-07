using System;
using UnityEngine.Serialization;

namespace _Project.Scripts.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string GameLevel;
        [FormerlySerializedAs("basePersistenceData")] [FormerlySerializedAs("TestObjectPersistenceData")] public BaseGameObjectPersData baseGameObjectPersData;
    }
}