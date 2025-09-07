using System.Collections.Generic;
using _Project.Scripts.Utils;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    public interface IStorage
    {
        string StorageName { get; }
        int SlotQuantity { get; set; }
        List<StorageSlot> StorageSlots { get; }
    }

    public static class InterfaceStorageExtensions
    {
        public static void ResetStorageSlots(this IStorage storage)
        {
            foreach (StorageSlot slot in storage.StorageSlots)
            {
                slot.SetToDefault();
            }
        }

        public static bool IsStorageSlotsEmpty(this IStorage storage)
        {
            foreach (var storageSlot in storage.StorageSlots)
            {
                if (!storageSlot.IsDefault())
                    return false;
            }

            return true;
        }
    }
}