using System;
using _Project.Scripts.InputSystem;
using _Project.Scripts.UI;
using UnityEngine;

namespace _Project.Scripts.Item
{
    [RequireComponent(typeof(ItemDisplayBehavior))]
    public class ItemDisplayInputHandler : MonoBehaviour, IInputGameplayHandler, IItemInteractable,
        IUnsubscribeInteractable<IItemUser>
    {
        public InputGameplayReader InputGamePlay { get; set; }
        IItemUser itemUser;
        ItemDisplayBehavior itemDisplayBehavior;
        
        #region UIInteract
        
        private const string ButtonTypeInteract = "T";
        private const string ContentInteract = "Pick up";
        
        #endregion

        void Awake()
        {
            InputGamePlay = InputGameplayReader.Instance;
            itemDisplayBehavior = GetComponent<ItemDisplayBehavior>();

            if (itemDisplayBehavior == null)
            {
                Debug.LogError("ItemDisplayBehavior is missing.");
            }
        }

        public void Accept(IItemUser interactor)
        {
            itemUser = interactor;
            if (itemUser != null)
                InputGamePlay.PickupItem += PickUpItem;
            UIManager.Instance.TurnOnInteractInfo(ButtonTypeInteract, ContentInteract);
        }

        public void Unsubscribe(IItemUser interactor)
        {
            if (itemUser != null)
                InputGamePlay.PickupItem -= PickUpItem;
            itemUser = null;
            
            UIManager.Instance.TurnOffInteractInfo();
        }

        void PickUpItem()
        {
            Debug.Log("Picked up item");
            ItemDisplayManager.Instance.PickupItemToInventory(
                itemDisplayBehavior,
                itemUser.PlayerStorages
            );

            InputGamePlay.PickupItem -= PickUpItem;
            UIManager.Instance.TurnOffInteractInfo();
        }
    }
}