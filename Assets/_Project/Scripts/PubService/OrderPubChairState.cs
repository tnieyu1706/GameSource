using System;
using _Project.Scripts.General.ReusePatterns;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.PubService
{
    [Serializable]
    public class OrderPubChairState : PubChairState, IInputGameplayHandler, IStateUIInteraction
    {
        [SerializeField] private float orderTime;
        [SerializeField] private float orderTimeLimit = 10f;
        public InputGameplayReader InputGamePlay { get; }

        public Action OnUIInitialized { get; }
        public Action<float> OnUIUpdated { get; }
        public Action OnUIClosed { get; }

        public OrderPubChairState(IPubChairContext context, Action onUIInitialized, Action<float> onUIUpdated,
            Action onUIClosed) : base(context)
        {
            InputGamePlay = InputGameplayReader.Instance;
            OnUIInitialized = onUIInitialized;
            OnUIUpdated = onUIUpdated;
            OnUIClosed = onUIClosed;
        }

        public override void Entry()
        {
            //setup item for client.
            orderTime = orderTimeLimit;
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
            Debug.Log("Pub Table state is now : Order");

            orderTime -= Time.deltaTime;

            OnUIUpdated?.Invoke(orderTime / orderTimeLimit);

            if (orderTime <= 0)
            {
                //transition state -> Idle
                Context.CurrentState = Context.PubChairStateHolder.idleState;
                this.Exit();
                Context.CurrentState.Entry();
            }
        }

        public override void SubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact += AcceptOrder;
            }
        }

        public override void UnsubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact -= AcceptOrder;
            }
        }

        [UIShowLog]
        void AcceptOrder()
        {
            //transition state -> Waiting & ghi nhan rating Order
            Context.Rating.orderTaskRating = MoneyTaskRating.ConvertProgressToRating(orderTime / orderTimeLimit);

            Context.CurrentState = Context.PubChairStateHolder.waitingState;
            this.Exit();
            Context.CurrentState.Entry();
        }
    }
}