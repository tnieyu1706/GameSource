using _Project.Scripts.InputSystem;
using AwesomeAttributes;
using LazySquirrelLabs.MinMaxRangeAttribute;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementBehavior : MonoBehaviour, IInputGameplayHandler
    {
        [SerializeField, Range(1, 10)] private float moveSpeed = 5f;
        [SerializeField, MinMaxRange(0, 10)] private Vector2Int jumpForce;
        [SerializeField] private Transform playerTransform;

        public InputGameplayReader InputGamePlay { get; set; }
        private Rigidbody _rb;
        private Camera _mainCamera;
        private Vector2 _moveInput;
        [SerializeField, Readonly] private bool isGrounded = true;

        private void OnValidate()
        {
            if (playerTransform == null)
                playerTransform = GetComponent<Transform>();
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }
        }

        void Awake()
        {
            _mainCamera = Camera.main;
            InputGamePlay = InputGameplayReader.Instance;
        }

        void OnEnable()
        {
            InputGamePlay.Move += OnMove;
            InputGamePlay.Jump += OnJump;
        }

        void OnMove(Vector2 move)
        {
            _moveInput = move;
        }

        void OnJump(bool isJump)
        {
            if (isJump)
                Jump();
            else
                Land();
        }

        void Update()
        {
            Move(CalculateMovementDirection());
        }

        void OnDisable()
        {
            InputGamePlay.Move -= OnMove;
            InputGamePlay.Jump -= OnJump;
        }

        void Move(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                playerTransform.rotation = Quaternion.LookRotation(direction);
                playerTransform.position += direction * (moveSpeed * Time.deltaTime);
            }
        }

        Vector3 CalculateMovementDirection()
        {
            Vector3 cameraVertical = _mainCamera.transform.up.With(y: 0);
            Vector3 cameraHorizontal = _mainCamera.transform.right.With(y: 0);
            return cameraVertical.normalized * _moveInput.y + cameraHorizontal.normalized * _moveInput.x;
        }

        void Jump()
        {
            if (isGrounded)
            {
                _rb.AddForce(Vector3.up * Random.Range(jumpForce.x, jumpForce.y), ForceMode.Impulse);
                isGrounded = false;
            }
        }

        void Land()
        {
            print("Player is not on Landing Area");
        }

        void OnTriggerEnter(Collider other)
        {
            print("something");

            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isGrounded = true;
            }
        }

        
    }
}