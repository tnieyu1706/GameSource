using System;
using System.Collections.Generic;
using _Project.Scripts.General.Patterns.UnityTechnique;
using _Project.Scripts.Item;
using _Project.Scripts.Storage;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
using UnityEngine;

namespace _Project.Scripts.Storage
{
    [Serializable]
    public class StorageEntity : IStorage
    {
        [SerializeField] private string storageName;
        [SerializeField] private int slotQuantity;
        [SerializeField] private List<StorageSlot> storageSlots = new List<StorageSlot>();

        public string StorageName => storageName;

        public int SlotQuantity
        {
            get => slotQuantity;
            set
            {
                if (slotQuantity < 0)
                {
                    Debug.LogWarning("Storage quantity cannot be negative");
                    return;
                }
                slotQuantity = value;
                
                storageSlots.Resize(slotQuantity);
            }
        }
        public List<StorageSlot> StorageSlots => storageSlots;

        public StorageEntity()
        {
            
        }

        public StorageEntity(string storageName, int slotQuantity)
        {
            this.storageName = storageName;
            this.slotQuantity = slotQuantity;
            
            StorageSlotsInitialization(slotQuantity);
        }

        private void StorageSlotsInitialization(int number)
        {
            for (int i = 0; i < number; i++)
            {
                storageSlots.Add(new StorageSlot());
            }
        }
    }
    
    [CreateAssetMenu(fileName = "StorageSO", menuName = "Scriptable Objects/StorageSO")]
    public class StorageSO : CacheEntityScriptable<StorageEntity>
    {
        [Button]
        void ResizeStorage()
        {
            if (entity == null)
            {
                Debug.Log("Storage is empty");
                return;
            }

            if (entity.SlotQuantity < 0)
            {
                Debug.LogWarning("Storage size cannot be less than 0");
                return;
            }

            if (entity.SlotQuantity != entity.StorageSlots.Count)
            {
                entity.StorageSlots.Resize(entity.SlotQuantity);
            }
        }
    }

    public static class StorageSystem
    {
        public static StorageSlot GetStorageSimilarItemType(ItemTypeData item, IStorage storage)
        {
            StorageSlot storageSlotEmpty = null;

            foreach (var slot in storage.StorageSlots)
            {
                if (slot.ItemData != null && slot.ItemData.TypeId == item.TypeId)
                {
                    return slot;
                }
                
                if (storageSlotEmpty == null && slot.ItemData == null)
                {
                    storageSlotEmpty = slot;
                }
            }

            return storageSlotEmpty ?? null;
        }

        public static bool PutItemToStorage(
            ItemTypeData item,
            IStorage storage,
            Action<StorageSlot> onSuccess = null,
            Action onFailure = null
        )
        {
            StorageSlot storageSlot = GetStorageSimilarItemType(item, storage);

            if (storageSlot == null)
            {
                onFailure?.Invoke();
                return false;
            }

            storageSlot.SetValue(item, storageSlot.Quantity + 1);
            onSuccess?.Invoke(storageSlot);

            return true;
        }

        public static bool PutItemToStorages(
            ItemTypeData item,
            IEnumerable<IStorage> storages,
            Action<StorageSlot> onSuccess = null,
            Action onFailure = null
        )
        {
            StorageSlot storageSlot = null;
            foreach (var storage in storages)
            {
                storageSlot = GetStorageSimilarItemType(item, storage);
                
                if (storageSlot != null && storageSlot.ItemData != null)
                    break;
            }

            if (storageSlot == null)
            {
                onFailure?.Invoke();
                return false;
            }

            storageSlot.SetValue(item, storageSlot.Quantity + 1);
            onSuccess?.Invoke(storageSlot);

            return true;
        }
    }
}