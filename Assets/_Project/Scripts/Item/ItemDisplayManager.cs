using System.Collections.Generic;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Storage;
using _Project.Scripts.UI;
using _Project.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace _Project.Scripts.Item
{
    public class ItemDisplayManager : BehaviorSingleton<ItemDisplayManager>
    {
        [SerializeField] private GameObject itemDisplayPrefab;
        private ObjectPool<GameObject> itemDisplayPool;
        public bool collectionCheck = true;
        public int defaultCapacity = 10;
        public int maxCapacity = 20;

        void Start()
        {
            itemDisplayPool = new ObjectPool<GameObject>(
                CreateItemDisplay,
                GetItemDisplay,
                ReleaseItemDisplay,
                DestroyItemDisplay,
                collectionCheck,
                defaultCapacity,
                maxCapacity
            );
        }

        private GameObject CreateItemDisplay()
        {
            return Instantiate(itemDisplayPrefab);
        }

        private void GetItemDisplay(GameObject itemDisplay)
        {
            itemDisplay.SetActive(true);
        }

        private void ReleaseItemDisplay(GameObject itemDisplay)
        {
            itemDisplay.SetActive(false);
        }

        private void DestroyItemDisplay(GameObject itemDisplay)
        {
            Destroy(itemDisplay);
        }

        public void GenerateItemDisplay(ItemTypeData itemTypeData, Vector3 position, Transform container = null)
        {
            GameObject obj = itemDisplayPool.Get();
            obj.transform.position = position;
            obj.GetComponent<ItemDisplayBehavior>().ItemTypeData = itemTypeData;
        }

        [UIShowLog]
        public void PickupItemToInventory(
            ItemDisplayBehavior itemDisplayBehavior,
            IEnumerable<IStorage> storageDatas
        )
        {
            StorageSystem.PutItemToStorages(
                itemDisplayBehavior.ItemTypeData,
                storageDatas,
                (slot) =>
                {
                    Debug.Log("Storage slot quantity :" + slot.Quantity);
                    UILog.Instance.WriteLog($"Picked up Item - {itemDisplayBehavior.ItemTypeData.Name}");

                    itemDisplayPool.Release(itemDisplayBehavior.gameObject);
                },
                () => { UILog.Instance.WriteLog($"Can't pick up Item"); }
            );

        }
    }
}