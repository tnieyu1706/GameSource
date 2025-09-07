using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Interact;
using _Project.Scripts.Storage;
using _Project.Scripts.UI;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Item
{
    public class ItemUserBehavior : MonoBehaviour, IItemUser, IInputGameplayHandler
    {
        [SerializeField] private StorageSlot slotSelected;
        [SerializeField] private List<StorageSO> playerStorageSos;

        private IEnumerable<IStorage> playerStorage;

        public StorageSlot SlotSelected
        {
            get => slotSelected;
            set => slotSelected = value;
        }

        public Action<StorageSlot, int> OnUsedItem { get; set; }

        public IEnumerable<IStorage> PlayerStorages
        {
            get
            {
                if (playerStorage == null)
                {
                    playerStorage = playerStorageSos.Select(s => s.entity);
                }
                return playerStorage;
            }
        }
        public InputGameplayReader InputGamePlay { get; set; }

        private void Awake()
        {
            InputGamePlay = InputGameplayReader.Instance;
        }

        private void OnEnable()
        {
            OnUsedItem += AfterUseItem;
            InputGamePlay.DropItem += DropCurrentSelectedItem;
        }

        private void OnDisable()
        {
            OnUsedItem -= AfterUseItem;
            InputGamePlay.DropItem -= DropCurrentSelectedItem;
        }

        [UIShowLog]
        private void DropCurrentSelectedItem()
        {
            if (slotSelected == null || slotSelected.ItemData == null || slotSelected.Quantity < 1)
            {
                UILog.Instance.WriteLog("No item selected");
                return;
            }
            UILog.Instance.WriteLog($"Drop item - {slotSelected.ItemData.Name}");
            
            ItemDisplayManager.Instance.GenerateItemDisplay(SlotSelected.ItemData, transform.position);
            this.OnUsedItemQuick(slotSelected);
            
        }

        [UIShowLog]
        private void AfterUseItem(StorageSlot slot, int quantity)
        {
            slot.SetValue(slot.ItemData, slot.Quantity - quantity);
        }

        public void UseItem()
        {
            OnUsedItem?.Invoke(slotSelected, 1);
            throw new NotImplementedException();
        }

        public void Visit(IItemInteractable interactable)
        {
            interactable.Accept(this);
        }

        void OnTriggerEnter(Collider target)
        {
            IInteractable interactable = target.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("enter trigger interactable");
                interactable.Accept(this);
            }
        }

        void OnTriggerExit(Collider target)
        {
            IUnsubscribeInteractable interactable = target.gameObject.GetComponent<IUnsubscribeInteractable>();
            if (interactable != null)
            {
                Debug.Log("exit trigger interactable");
                interactable.Unsubscribe(this);
            }
        }
    }
}