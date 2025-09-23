using _Project.Scripts.InputSystem;
using _Project.Scripts.Interact;
using _Project.Scripts.Item;
using _Project.Scripts.UI;
using UnityEngine;

namespace _Project.Scripts.PubService
{
    public class PubServiceBehavior : MonoBehaviour, IItemInteractable, IUnsubscribeInteractable<IInteractor>, IInputGameplayHandler
    {
        [SerializeField] private GameObject pubServiceView;
        public InputGameplayReader InputGamePlay { get; set; }
        
        #region UIInteract
        
        private const string ButtonTypeInteract = "E";
        private const string ContentInteract = "Open/Close Pub";
        
        #endregion
        
        private void Awake()
        {
            InputGamePlay = InputGameplayReader.Instance;
        }

        public void Accept(IItemUser interactor)
        {
            InputGamePlay.Interact += OpenClosePubServiceView;
            
            UIManager.Instance.TurnOnInteractInfo(ButtonTypeInteract, ContentInteract);
        }

        public void Unsubscribe(IInteractor interactor)
        {
            InputGamePlay.Interact -= OpenClosePubServiceView;
            
            UIManager.Instance.TurnOffInteractInfo();
        }

        void OpenClosePubServiceView()
        {
            pubServiceView.SetActive(!pubServiceView.activeSelf);
        }
        
    }
}