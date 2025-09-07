using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Interact;
using _Project.Scripts.Storage;

namespace _Project.Scripts.Item
{
    public interface IItemUser : IInteractor<IItemInteractable>
    {
        StorageSlot SlotSelected { get; set; }
        
        /// <summary>
        /// When use OnUsedItem need to carry on about selected item is greater than
        /// Or use OnUsedItemWithChecking instead
        /// Or check before use it through CheckForCanOnUsedItem.
        /// </summary>
        Action<StorageSlot, int> OnUsedItem { get; set; }
        IEnumerable<IStorage> PlayerStorages { get; }
        
        void UseItem();
    }

    public static class InterfaceItemUserExtensions
    {
        public static void OnUsedItemWithChecking(this IItemUser itemUser, StorageSlot slot, int quantity)
        {
            if (slot.Quantity >- quantity)
            {
                itemUser.OnUsedItem?.Invoke(slot, quantity);
            }
            
        }

        public static bool CheckForCanOnUsedItem(this IItemUser itemUser, StorageSlot slot, int quantity)
        {
            return slot.Quantity >= quantity;
        }

        public static void OnUsedItemQuick(this IItemUser itemUser, StorageSlot slot)
        {
            itemUser.OnUsedItem?.Invoke(slot, 1);
        }
    }
}