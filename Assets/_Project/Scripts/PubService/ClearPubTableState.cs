using System;
using _Project.Scripts.InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.PubService
{
    [Serializable]
    public class ClearPubTableState : PubTableState, IInputGameplayHandler
    {
        public InputGameplayReader InputGamePlay { get; }

        [SerializeField] private Image clearIcon;
        public ClearPubTableState(IPubTableContext context, Image clearIcon) : base(context)
        {
            InputGamePlay = InputGameplayReader.Instance;
            this.clearIcon = clearIcon;
        }

        public override void Entry()
        {
            clearIcon.gameObject.SetActive(true);
            Debug.Log("Clear State Entry.");
            
            SubscribeInteraction();
        }

        public override void Exit()
        {
            UnsubscribeInteraction();
            clearIcon.gameObject.SetActive(false);
        }

        public override void Update()
        {
            Debug.Log("Pub Table state is now : Clear");
        }

        public override void SubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact += CleanupTable;
            }
        }

        public override void UnsubscribeInteraction()
        {
            if (Context.CurrentItemUser != null)
            {
                InputGamePlay.Interact -= CleanupTable;
            }
        }

        void CleanupTable()
        {
            //get money & clean table & transition -> Idle
            float totalMoney = Context.TotalOrderItemsPrice * Context.Rating.GetMoneyRating();
            
            Debug.Log("Total Money can get : " + totalMoney);
            
            //clean table (animation)...
            
            //transition -> Idle
            Context.CurrentState = Context.PubTableStateHolder.idleState;
            this.Exit();
            Context.CurrentState.Entry();
            
        }
    }
}