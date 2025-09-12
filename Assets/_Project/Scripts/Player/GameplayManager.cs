using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.InputSystem;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class GameplayManager : BehaviorSingleton<GameplayManager>
    {
        public InputUIReader inputUIReader;
        public InputGameplayReader inputGameplayReader;
        public bool isShowLogDebug = true;


        protected override void Awake()
        {
            base.Awake();
            inputGameplayReader = InputGameplayReader.Instance;
            inputUIReader = InputUIReader.Instance;
        }
        public void Start()
        {
            inputGameplayReader.EnablePlayerActions();
            inputUIReader.EnablePlayerActions();
            Debug.unityLogger.logEnabled = isShowLogDebug;
        }
    }
}