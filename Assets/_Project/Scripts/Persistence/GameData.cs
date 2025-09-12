using System;
using _Project.Scripts.Utils;
using UnityEngine.Serialization;

namespace _Project.Scripts.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string GameLevel;
    }
}