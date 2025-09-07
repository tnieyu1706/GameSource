using System;
using _Project.Scripts.Item;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.CraftingSystem
{
    [Serializable]
    public class WorkingMachineState : MachineState
    {
        public float progress = 0f;
        private float targetProgress = 1f;
        public float progressScale = 1f;
        public float duration;
        [SerializeField] private Image progressingBar;
        private ItemTypeData itemResult;

        public WorkingMachineState(IMachineContext context, Image progressingBar, float progressScale = 1f) : base(context)
        {
            this.progressingBar = progressingBar;
            this.progressScale = progressScale;
        }

        public void SetupWorking(ItemTypeData itemResult, float duration)
        {
            this.itemResult = itemResult;
            this.duration = duration;
        }

        public override void Entry()
        {
            //execute item -> recipe
            //ui
            progress = 0f;
            progressingBar.gameObject.SetActive(true);
            progressingBar.fillAmount = 0f;
        }

        public override void Exit()
        {
            progressingBar.gameObject.SetActive(false);
        }

        public override void Update()
        {
            Debug.Log("current state is : Working");
            //data & ui

            float updateSmooth = Time.deltaTime / duration * progressScale;
            progress += updateSmooth;
            progressingBar.fillAmount = progress;


            //excute auto change
            if (progress >= 1f)
            {
                Context.CurrentState = Context.StateHolder.CompletedState;

                if (Context.CurrentState is CompletedMachineState completedState)
                {
                    completedState.SetupCompleted(itemResult);
                }

                this.Exit();
                Context.CurrentState.Entry();
            }
        }

        public override void SubscribeInteraction()
        {
        }

        public override void UnsubscribeInteraction()
        {
        }
    }
}