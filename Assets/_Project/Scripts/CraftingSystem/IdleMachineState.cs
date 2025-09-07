using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.General.ReusePatterns;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Item;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.CraftingSystem
{
    [Serializable]
    public class IdleMachineState : MachineState, IInputGameplayHandler, IStateUIInteraction
    {
        public InputGameplayReader InputGamePlay { get; set; }

        public Stack<ItemTypeData> ItemStack = new Stack<ItemTypeData>();

        public Action<Stack<ItemTypeData>> OnUpdateUI;

        public Action OnUIInitialized { get; }
        public Action OnUIClosed { get; }

        public IdleMachineState(
            IMachineContext context,
            Action onUIInitialized,
            Action<Stack<ItemTypeData>> onUpdateUI,
            Action onUIClosed
        ) : base(context)
        {
            InputGamePlay = InputGameplayReader.Instance;
            this.OnUIInitialized = onUIInitialized;
            this.OnUpdateUI = onUpdateUI;
            this.OnUIClosed = onUIClosed;
        }

        public override void Entry()
        {
            SubscribeInteraction();

            OnUIInitialized?.Invoke();
        }

        public override void Exit()
        {
            UnsubscribeInteraction();
            ItemStack.Clear();

            OnUIClosed?.Invoke();
        }

        public override void Update()
        {
            Debug.Log("current state is : Idle");
        }

        public override void SubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact += PushItemToMachine;
                InputGamePlay.PopMachineItem += PopItemFromMachine;
                InputGamePlay.RunMachine += ActivateMachine;
            }
        }

        public override void UnsubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact -= PushItemToMachine;
                InputGamePlay.PopMachineItem -= PopItemFromMachine;
                InputGamePlay.RunMachine -= ActivateMachine;
            }
        }

        #region Events

        [UIShowLog]
        private void PushItemToMachine()
        {
            if (Context.CurrentItemUser == null || Context.CurrentItemUser.SlotSelected == null ||
                Context.CurrentItemUser.SlotSelected.ItemData == null)
            {
                Debug.Log("slotSelected or item is null");
                return;
            }

            // max stack
            if (ItemStack.Count == Context.SlotComponent)
            {
                Debug.Log($"Max stack is only = {Context.SlotComponent}");
                return;
            }

            //data
            ItemStack.Push(Context.CurrentItemUser.SlotSelected.ItemData);


            Context.CurrentItemUser.OnUsedItemQuick(Context.CurrentItemUser.SlotSelected);

            // ui
            Debug.Log("stack count : " + ItemStack.Count);
            OnUpdateUI?.Invoke(ItemStack);
        }

        [UIShowLog]
        private void PopItemFromMachine()
        {
            if (ItemStack == null || ItemStack.Count == 0)
            {
                Debug.Log("stack is empty");
                return;
            }

            ItemTypeData item = ItemStack.Pop();
            StorageSystem.PutItemToStorages(
                item,
                Context.CurrentItemUser.PlayerStorages,
                (slot) =>
                {
                    Debug.Log($"Have just add {item.Name} into player storages");
                    OnUpdateUI?.Invoke(ItemStack);
                },
                () => { Debug.Log("some thing wrong when putt item to play storages!"); }
            );
        }

        [UIShowLog]
        private void ActivateMachine()
        {
            // check can run ?
            if (ItemStack.Count == 0)
            {
                Debug.Log("stack is empty");
                return;
            }

            // chuyen doi trang thai sang working.
            Context.CurrentState = Context.StateHolder.WorkingState;
            if (Context.CurrentState is WorkingMachineState workingState)
            {
                //exec recipe -> get item
                var correspondingRecipes =
                    RecipeManager.Instance
                        .GetRecipesWithMachineAndProgressingType(
                            Context.MachineType,
                            Context.ProgressingType
                        );

                Debug.Log($"correspondingRecipes count : {correspondingRecipes.Count}");
                var recipe = RecipeManager.Instance
                    .GetRecipeWithIngredientsBySpecifyRecipes(
                        correspondingRecipes,
                        ItemStack.ToList()
                    );

                //exec if not find recipe => trash.
                var resultItem = recipe ? recipe.outputComponent : RecipeManager.Instance.trashItem;
                var duration = recipe ? recipe.executeSecondTime : 2f;

                Debug.Log("itemResult :" + resultItem.Name);

                workingState.SetupWorking(resultItem, duration);
            }

            this.Exit();
            Context.CurrentState.Entry();
        }

        #endregion
    }
}