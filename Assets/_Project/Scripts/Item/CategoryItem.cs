using System;
using AwesomeAttributes;
using UnityEngine;

#if UNITY_EDITOR
using _Project.Scripts.General.StaticHelpers;
#endif

namespace _Project.Scripts.Item
{
    [Serializable]
    [CreateAssetMenu(fileName = "Category", menuName = "Scriptable Objects/Item/Category")]
    public class CategoryItem : ScriptableObject, IBaseTypeData
    {
        [SerializeField, Readonly] private int categoryId;
        [SerializeField] private string categoryName;

        public int TypeId
        {
            get => categoryId;
            set => categoryId = value;
        }

        public string Name => categoryName;

        void Awake()
        {
            categoryId = IdentifySystemHelper.GenerateId(this);

#if UNITY_EDITOR
            categoryName = MyAssetHelper.GetAssetName(this);
#endif
        }

        public override string ToString()
        {
            return $"Category: {Name}, Id: {TypeId}";
        }
    }
}