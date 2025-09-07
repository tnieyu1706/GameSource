using System;
using _Project.Scripts.Storage;
using UnityEngine;

namespace _Project.Scripts.PubService
{
    [Serializable]
    public class IdlePubTableState : PubTableState
    {
        public IdlePubTableState(IPubTableContext context) : base(context)
        {
        }


        public override void Entry()
        {
            //Reset all value for context.
            Context.ClientName = String.Empty;
            Context.Rating.Reset();
            Context.OrderItems.ResetStorageSlots();
        }

        public override void Exit()
        {
            UnsubscribeInteraction();
        }

        public override void Update()
        {
            Debug.Log("Pub Table state is now : Idle");
        }

        public override void SubscribeInteraction()
        {
        }

        public override void UnsubscribeInteraction()
        {
        }
    }
}