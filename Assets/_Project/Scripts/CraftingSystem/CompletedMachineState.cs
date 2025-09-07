using System;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Item;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.CraftingSystem
{
    [Serializable]
    public class CompletedMachineState : MachineState, IInputGameplayHandler
    {
        public InputGameplayReader InputGamePlay { get; set; }
        public ItemTypeData completedItem;
        [SerializeField] private Image iconCompletedItem;

        public CompletedMachineState(IMachineContext context, Image iconCompletedItem) : base(context)
        {
            InputGamePlay = InputGameplayReader.Instance;
            this.iconCompletedItem = iconCompletedItem;
        }

        public void SetupCompleted(ItemTypeData resultItem)
        {
            completedItem = resultItem;
            iconCompletedItem.sprite = completedItem.icon;
        }

        public override void Entry()
        {
            SubscribeInteraction();
            
            iconCompletedItem.gameObject.SetActive(true);
        }

        public override void Exit()
        {
            UnsubscribeInteraction();
            
            iconCompletedItem.gameObject.SetActive(false);
        }

        public override void Update()
        {
            Debug.Log("current state is : Completed");
        }

        public override void SubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
                InputGamePlay.Interact += GetCompletedItem;
        }

        public override void UnsubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
                InputGamePlay.Interact -= GetCompletedItem;
        }

        [UIShowLog]
        private void GetCompletedItem()
        {
            StorageSystem.PutItemToStorages(
                completedItem,
                Context.CurrentItemUser.PlayerStorages,
                (slot) =>
                {
                    Debug.Log($"Have just add {completedItem.Name} into player storages");

                    //change state to idle
                    Context.CurrentState = Context.StateHolder.IdleState;

                    this.Exit();
                    Context.CurrentState.Entry();
                },
                () => { Debug.Log("some thing wrong when get completed item to play storages!"); }
            );
        }
    }
}