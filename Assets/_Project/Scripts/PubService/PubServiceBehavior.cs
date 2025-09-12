using _Project.Scripts.InputSystem;
using _Project.Scripts.Interact;
using _Project.Scripts.Item;
using UnityEngine;

namespace _Project.Scripts.PubService
{
    public class PubServiceBehavior : MonoBehaviour, IItemInteractable, IUnsubscribeInteractable<IInteractor>, IInputGameplayHandler
    {
        [SerializeField] private GameObject pubServiceView;
        public InputGameplayReader InputGamePlay { get; set; }
        
        private void Awake()
        {
            InputGamePlay = InputGameplayReader.Instance;
        }

        public void Accept(IItemUser interactor)
        {
            InputGamePlay.Interact += OpenClosePubServiceView;
        }

        public void Unsubscribe(IInteractor interactor)
        {
            InputGamePlay.Interact -= OpenClosePubServiceView;
        }

        void OpenClosePubServiceView()
        {
            pubServiceView.SetActive(!pubServiceView.activeSelf);
        }
        
    }
}