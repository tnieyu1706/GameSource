using System;
using _Project.Scripts.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Storage
{
    [Serializable]
    public class StorageSlot
    {
        [SerializeField] private ItemTypeData _itemData;
        [SerializeField] private int _quantity = 0;
        public Action<StorageSlot> OnSlotUpdated;

        public ItemTypeData ItemData => _itemData;
        public int Quantity => _quantity;

        public void SetValue(ItemTypeData itemData, int quantity)
        {
            this._quantity = quantity;
            this._itemData = quantity < 1 ? null : itemData;
            OnSlotUpdated?.Invoke(this);
        }

        public void SetToDefault()
        {
            _itemData = null;
            _quantity = 0;
            OnSlotUpdated?.Invoke(this);
        }

        public bool IsDefault()
        {
            return _itemData == null || _quantity == 0;
        }
    }
}