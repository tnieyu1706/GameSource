using _Project.Scripts.InputSystem;
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
        }

        public void Unsubscribe(IItemUser interactor)
        {
            if (itemUser != null)
                InputGamePlay.PickupItem -= PickUpItem;
            itemUser = null;
        }

        void PickUpItem()
        {
            Debug.Log("Picked up item");
            ItemDisplayManager.Instance.PickupItemToInventory(
                itemDisplayBehavior,
                itemUser.PlayerStorages
            );

            InputGamePlay.PickupItem -= PickUpItem;
        }
    }
}