using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Project.Scripts.PubService
{
    public class PubServiceSystem : BehaviorSingleton<PubServiceSystem>
    {
        public List<PubChairContext> pubTableContexts = new();

        //Use persistence instead.
        [Required] [InlineEditor(InlineEditorObjectFieldModes.Boxed)] [NeedToSaveLoad]
        public StorageSO ServedItems;

        public Vector2Int randomServedItemQuantity = new Vector2Int(1, 3);

        [FormerlySerializedAs("maxRandomServedTypeItemCount")]
        [FormerlySerializedAs("randomServedTypeItemCount")]
        [Range(1, 2)]
        public int maxRandomServedTypeItem;

        public float pubServiceSecondsLimit = 60f;
        [SerializeField] private float curPubServiceSeconds;
        public float delayEachServingSeconds;
        
        private Coroutine pubServiceCoroutine;

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
            int consumed = (int)Random.Range(randomServedItemQuantity.x, randomServedItemQuantity.y);

            Debug.Log($"##Item: {slotInternal.ItemData.Name} - quantity: {consumed}");

            var slotResult = new StorageSlot();
            slotResult.SetValue(slotInternal.ItemData, consumed);

            slotInternal.SetValue(slotResult.ItemData, slotInternal.Quantity - consumed);
            return slotResult;
        }

        [UIShowLog]
        public void ServiceTable(PubChairContext pubChairContext, List<StorageSlot> items)
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
                StorageSlot slot = pubChairContext.OrderItems.StorageSlots[i];
                StorageSlot servedSlot = items[i];

                slot.SetValue(servedSlot.ItemData, servedSlot.Quantity);
                totalPrice += servedSlot.ItemData.price * servedSlot.Quantity;
            }

            pubChairContext.TotalOrderItemsPrice = totalPrice;

            pubChairContext.ChangeState(pubChairContext.PubChairStateHolder.orderState);


            Debug.Log("Service Table");
        }

        [Button]
        public void OpenPubService()
        {
            curPubServiceSeconds = pubServiceSecondsLimit;
            pubServiceCoroutine = StartCoroutine(OnPubServiceExist());
        }

        [Button]
        public void ClosePubServiceEarly()
        {
            curPubServiceSeconds = 0;
            if (pubServiceCoroutine != null)
                StopCoroutine(pubServiceCoroutine);
        }

        public IEnumerator OnPubServiceExist()
        {
            while (!ServedItems.entity.IsStorageSlotsEmpty() && curPubServiceSeconds > 0)
            {
                yield return SingletonFactory
                    .GetInstance<WaitForSecondsServiceLocator>()
                    .GetService(delayEachServingSeconds);
                curPubServiceSeconds -= delayEachServingSeconds;

                //Give item quantity available for chair
                int itemServed = Mathf.Min(maxRandomServedTypeItem, ServedItems.entity.StorageSlots.Count);
                var chairContexts =
                    pubTableContexts
                        .Where(c => c.CurrentState is IdlePubChairState)
                        .ToList();
                PubChairContext context = chairContexts[Random.Range(0, chairContexts.Count)];

                if (context != null)
                {
                    GiveItemForTable(context, itemServed);
                }
                else
                {
                    Debug.Log($"No context.");
                }
            }
        }

        [UIShowLog]
        private void GiveItemForTable(PubChairContext chairContext, int servedItemQuantity)
        {
            //double check
            if (chairContext.CurrentState is not IdlePubChairState)
            {
                Debug.Log($"pub chair {chairContext.Name} state is not Idle!");
                return;
            }

            List<StorageSlot> servedItems = new();
            for (int i = 0; i < servedItemQuantity; i++)
            {
                StorageSlot slot = GetRandomServedItems();

                if (slot == null)
                    break;

                servedItems.Add(slot);
            }

            ServiceTable(chairContext, servedItems);
        }
    }
}