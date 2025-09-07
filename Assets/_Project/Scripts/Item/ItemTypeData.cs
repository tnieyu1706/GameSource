using System;
using System.Collections.Generic;
using _Project.Scripts.General.Patterns.Singleton;
using AwesomeAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Item
{
    public enum ItemType
    {
        Ingredient,
        Production,
        Special
    }

    [Serializable]
    [CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Item/ItemData")]
    public class ItemTypeData : ScriptableObject, IBaseTypeData
    {
        [SerializeField, Readonly] private int itemTypeId;
        [SerializeField] private string itemTypeName;

        public ItemType itemType;
        public CategoryItem categoryItem;
        public ItemRarity itemRarity = ItemRarity.Normal;

        public Sprite icon;
        public string description;
        public GameObject prefab;
        public bool canStack = false;
        public int maxStackSize = 0;
        public float weight = 0;
        public float price = 0;

        public int TypeId
        {
            get => itemTypeId;
            set => itemTypeId = value;
        }

        public string Name => itemTypeName;


        private void Awake()
        {
            TypeId = IdentifySystemHelper.GenerateId(this);
        }


        public string GetInfo()
        {
            return $"itemTypeId: {TypeId}, itemTypeName: {itemTypeName}, price: {price}, description: {description}";
        }
    }

    [SingletonFactory]
    public class ItemTypeComparer : IComparer<ItemTypeData>
    {
        public int Compare(ItemTypeData x, ItemTypeData y)
        {
            if (x.TypeId < y.TypeId)
                return -1;

            if (x.TypeId > y.TypeId)
                return 1;

            return 0;
        }
    }
}