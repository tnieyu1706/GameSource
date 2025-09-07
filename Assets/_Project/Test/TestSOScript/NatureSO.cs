using System;
using UnityEngine;

namespace _Project.Test.TestSOScript
{
    public interface INature
    {
        string NatureName { get; }
    }
    
    [Serializable]
    public class Nature : INature
    {
        [SerializeField] private string natureName;
        public string NatureName => natureName;
    }

    [CreateAssetMenu(fileName = "New Nature", menuName = "TestSO/Nature")]
    public class NatureSO : ScriptableObject
    {
        public Nature nature;
    }
}