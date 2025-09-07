// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.InputSystem;
// using UnityEngine.Serialization;
//
// public class InputEventManager : MonoBehaviour
// {
//     private InputSystem_Actions inputSystemActions;
//     
//     [FormerlySerializedAs("playerMovementAction")] [SerializeField]
//     private UnityEvent<InputAction.CallbackContext> MovePlayer;
//     [FormerlySerializedAs("playerStopMovementAction")] [SerializeField]
//     private UnityEvent<InputAction.CallbackContext> StopMovePlayer;
//
//     void Awake()
//     {
//         inputSystemActions = new InputSystem_Actions();
//     }
//     
//     void OnEnable() 
//     {
//         inputSystemActions.Enable();
//         inputSystemActions.Player.Map.performed += OnMapPerformed;
//         inputSystemActions.Player.Move.performed += MovePlayer.Invoke;
//         inputSystemActions.Player.Move.canceled += StopMovePlayer.Invoke;
//     }
//
//     private void OnMapPerformed(InputAction.CallbackContext context)
//     {
//         Debug.Log("Map performed");
//     }
//     
//     void OnDisable()
//     {
//         inputSystemActions.Disable();
//         inputSystemActions.Player.Map.performed -= OnMapPerformed;
//         inputSystemActions.Player.Move.performed -= MovePlayer.Invoke;
//         inputSystemActions.Player.Move.canceled -= StopMovePlayer.Invoke;
//     }
// }