using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.PubService
{
    public class PubServiceSystem : BehaviorSingleton<PubServiceSystem>
    {
        public List<PubTableContext> pubTableContexts = new();

        //Use persistence instead.
        [Required] [InlineEditor(InlineEditorObjectFieldModes.Boxed)] [NeedToSaveLoad]
        public StorageSO ServedItems;

        [UIShowLog]
        [CanBeNull]
        public StorageSlot GetRandomServedItems()
        {
            if (ServedItems.entity.IsStorageSlotsEmpty())
            {
                Debug.Log("No served items is select.");
                return null;
            }

            IEnumerable<StorageSlot> availableServedItems =
                ServedItems.entity.StorageSlots.Where(slot => !slot.IsDefault());

            int servedRandom = Random.Range(0, availableServedItems.Count());

            StorageSlot slotInternal = availableServedItems.ElementAt(servedRandom);
            int consumed = Random.Range(1, slotInternal.Quantity+1);

            Debug.Log($"##Item: {slotInternal.ItemData.Name} - quantity: {consumed}");

            var slotResult = new StorageSlot();
            slotResult.SetValue(slotInternal.ItemData, consumed);

            slotInternal.SetValue(slotResult.ItemData, slotInternal.Quantity - consumed);
            return slotResult;
        }

        [UIShowLog]
        public void ServiceTable(PubTableContext pubTableContext, List<StorageSlot> items)
        {
            //check max

            if (items.Count > GlobalConfigs.MaxTableSlot)
            {
                Debug.LogWarning("Max table slot is " + GlobalConfigs.MaxTableSlot);
                return;
            }

            float totalPrice = 0;
            for (int i = 0; i < items.Count; i++)
            {
                StorageSlot slot = pubTableContext.OrderItems.StorageSlots[i];
                StorageSlot servedSlot = items[i];

                slot.SetValue(servedSlot.ItemData, servedSlot.Quantity);
                totalPrice += servedSlot.ItemData.price * servedSlot.Quantity;
            }

            pubTableContext.TotalOrderItemsPrice = totalPrice;

            pubTableContext.ChangeState(pubTableContext.PubTableStateHolder.orderState);


            Debug.Log("Service Table");
        }

        [FoldoutGroup("Test")] public int numberTable = 0;

        [FoldoutGroup("Test")]
        [Button("TableActivate")]
        [UIShowLog]
        private void TestTableActivate()
        {
            //lieu co con ko ?.
            if (ServedItems.entity.IsStorageSlotsEmpty())
            {
                Debug.Log("ServedItems storage don't have any items.");
                return;
            }
            
            PubTableContext table = pubTableContexts[numberTable];
            //check Idle ko thi moi set dc nua.

            if (table.CurrentState is not IdlePubTableState)
            {
                Debug.Log("current working - not idle!");
                return;
            }
            
            //need to random more slot.
            // int itemRandomNumber = Random.Range(1, GlobalConfigs.MaxTableSlot+1);
            int itemRandomNumber = 2;
            List<StorageSlot> servedItems = new();
            for (int i = 0; i < itemRandomNumber; i++)
            {
                StorageSlot slot = GetRandomServedItems();
                
                if (slot == null)
                    break;
                
                servedItems.Add(slot);
            }

            ServiceTable(table, servedItems);
        }
    }
}