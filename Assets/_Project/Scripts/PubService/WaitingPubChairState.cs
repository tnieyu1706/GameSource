using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.AsyncTask;
using _Project.Scripts.General.ReusePatterns;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Item;
using _Project.Scripts.Storage;
using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.PubService
{
    [Serializable]
    public class WaitingPubChairState : PubChairState, IInputGameplayHandler, IStateUIInteraction
    {
        [SerializeField] private float waitingTime;
        [SerializeField] private float waitingTimeLimit = 30f;
        public float eatingTime = 10f;

        private bool isWaiting = true;

        public InputGameplayReader InputGamePlay { get; }

        public Action OnUIInitialized { get; }
        public Action<float> OnUIUpdated { get; }
        public Action OnUIClosed { get; }

        public WaitingPubChairState(
            IPubChairContext context,
            Action onUIInitialized,
            Action<float> onUIUpdated,
            Action onUIClosed
        ) : base(context)
        {
            InputGamePlay = InputGameplayReader.Instance;
            this.OnUIInitialized = onUIInitialized;
            this.OnUIUpdated = onUIUpdated;
            this.OnUIClosed = onUIClosed;
        }

        public override void Entry()
        {
            //setup config for waiting
            waitingTime = waitingTimeLimit;
            isWaiting = true;

            OnUIInitialized?.Invoke();
            SubscribeInteraction();
        }

        public override void Exit()
        {
            UnsubscribeInteraction();
            OnUIClosed?.Invoke();
        }

        public override void Update()
        {
            Debug.Log("Pub Table state is now : Waiting");

            if (isWaiting)
            {
                waitingTime -= Time.deltaTime;

                OnUIUpdated?.Invoke(waitingTime / waitingTimeLimit);

                if (waitingTime <= 0)
                {
                    // transition -> Idle
                    Context.CurrentState = Context.PubChairStateHolder.idleState;
                    this.Exit();
                    Context.CurrentState.Entry();
                }
            }
        }

        public override void SubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact += SubmitItem;
            }
        }

        public override void UnsubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact -= SubmitItem;
            }
        }

        [UIShowLog]
        void SubmitItem()
        {
            if (Context.CurrentItemUser == null || Context.CurrentItemUser.SlotSelected == null ||
                Context.CurrentItemUser.SlotSelected.ItemData == null)
            {
                Debug.Log("slotSelected or item is null");
                return;
            }

            if (Context.OrderItems.IsStorageSlotsEmpty())
            {
                Debug.Log("All order items have completed");
                return;
            }

            StorageSlot selectedSlot = Context.CurrentItemUser.SlotSelected;

            bool isSelectedCorrect = false;
            bool isCompletedServe = true;

            foreach (var slot in Context.OrderItems.StorageSlots)
            {
                if (
                    selectedSlot.ItemData != null
                    && slot.ItemData != null
                    && selectedSlot.ItemData.TypeId == slot.ItemData.TypeId
                )
                {
                    //execute when submit item for people.
                    int consumedQuantity = slot.Quantity;
                    if (!Context.CurrentItemUser
                            .CheckForCanOnUsedItem(selectedSlot, consumedQuantity))
                    {
                        consumedQuantity = selectedSlot.Quantity;
                    }

                    slot.SetValue(slot.ItemData, slot.Quantity - consumedQuantity);

                    Context.CurrentItemUser.OnUsedItem(selectedSlot, consumedQuantity);

                    isSelectedCorrect = true;
                    Debug.Log("Submit correct item");
                }

                if (slot.ItemData != null)
                {
                    isCompletedServe = false;
                }
            }

            if (!isSelectedCorrect)
            {
                Debug.Log("No order items is similar with selected item");
                return;
            }

            //check for case completed all. transition -> clear. after time eating.
            if (isCompletedServe)
            {
                Debug.Log("Transition to clear State");
                //record rating and transition -> cleear.

                Context.Rating.waitingTaskRating =
                    MoneyTaskRating.ConvertProgressToRating(waitingTime / waitingTimeLimit);

                //set waiting false.
                isWaiting = false;
                //close UI.
                OnUIClosed?.Invoke();

                //callback
                AsyncTaskSystem.Instance.RunAsync(
                    Task.Delay(TimeSpan.FromSeconds(eatingTime)),
                    TransitionToClearState()
                );
            }
        }

        IEnumerator TransitionToClearState()
        {
            Context.CurrentState = Context.PubChairStateHolder.clearState;
            this.Exit();
            Context.CurrentState.Entry();
            yield return null;
        }
    }
}