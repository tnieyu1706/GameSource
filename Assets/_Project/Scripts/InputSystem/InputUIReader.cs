using _Project.Scripts.General.Patterns.Singleton;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Project.Scripts.InputSystem
{
    [CreateAssetMenu(fileName = "InputUIReader", menuName = "Scriptable Objects/Inputs/InputUIReader")]
    public class InputUIReader : ScriptableObjectSingleton<InputUIReader>, IInputReader, InputSystems.IUIActions
    {
        public InputSystems inputSystemUI;
        public UnityAction[] numberHandlers = new UnityAction[10];
        
        public event UnityAction OpenMap;
        public event UnityAction OpenInventory;
        
        public void EnablePlayerActions()
        {
            if (inputSystemUI == null)
            {
                inputSystemUI = new InputSystems();
                inputSystemUI.UI.SetCallbacks(this);
            }
    
            inputSystemUI.Enable();
        }
        
        public void OnInventory(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OpenInventory?.Invoke();
        }
        
        public void OnMap(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OpenMap?.Invoke();
        }
    
        private void CallUIEvent(InputAction.CallbackContext context, int number)
        {
            if (context.phase == InputActionPhase.Performed) 
                numberHandlers[number]?.Invoke();
        }
    
        public void OnNumber1(InputAction.CallbackContext context)
        {
            CallUIEvent(context,0);
        }
    
        public void OnNumber2(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 1);
        }
    
        public void OnNumber3(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 2);
        }
    
        public void OnNumber4(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 3);
        }
    
        public void OnNumber5(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 4);
        }
    
        public void OnNumber6(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 5);
        }
    
        public void OnNumber7(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 6);
        }
    
        public void OnNumber8(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 7);
        }
    
        public void OnNumber9(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 8);
        }
    
        public void OnNumber0(InputAction.CallbackContext context)
        {
            CallUIEvent(context, 9);
        }

        
    }
}

