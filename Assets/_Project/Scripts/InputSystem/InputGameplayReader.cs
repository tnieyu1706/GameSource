using _Project.Scripts.General.Patterns.Singleton;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.InputSystem
{
    public interface IInputReader
    {
        void EnablePlayerActions();
    }

    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/Inputs/InputReader")]
    public class InputGameplayReader : ScriptableObjectSingleton<InputGameplayReader>, InputSystems.IPlayerActions,
        IInputReader
    {
        public InputSystems inputGameplaySystem;
        public event UnityAction<Vector2> Move;
        public event UnityAction<bool> Jump;
        public event UnityAction Interact;
        public event UnityAction PopMachineItem;
        public event UnityAction RunMachine;
        public event UnityAction PickupItem;
        public event UnityAction DropItem;

        public event UnityAction UseItem;
        public event UnityAction Attack;


        public Vector2 MoveDirection => inputGameplaySystem.Player.Move.ReadValue<Vector2>();
        public bool IsJumpPressed => inputGameplaySystem.Player.Jump.IsPressed();

        public void EnablePlayerActions()
        {
            if (inputGameplaySystem == null)
            {
                inputGameplaySystem = new InputSystems();
                inputGameplaySystem.Player.SetCallbacks(this);
            }

            inputGameplaySystem.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump?.Invoke(false);
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                Interact?.Invoke();
        }

        public void OnPopMachineItem(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                PopMachineItem?.Invoke();
        }

        public void OnPickUp(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                PickupItem?.Invoke();
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                DropItem?.Invoke();
        }

        public void OnRunMachine(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                RunMachine?.Invoke();
        }

        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                UseItem?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                Attack?.Invoke();
        }
    }
}