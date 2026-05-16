using UnityEngine;
using UnityEngine.AI;

namespace ProtectTheForest.Mechanics.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PlayerInputHandler _inputHandler;

        private NavMeshAgent _agent;
        private Camera _mainCamera;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _mainCamera = Camera.main;

            // Subscribe to input
            if (_inputHandler != null)
            {
                _inputHandler.OnMoveInput += HandleMoveInput;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from input
            if (_inputHandler != null)
            {
                _inputHandler.OnMoveInput -= HandleMoveInput;
            }
        }

        private void HandleMoveInput(Vector2 input)
        {
            if (input.magnitude > 0.1f)
            {
                Move(input);
            }
            else
            {
                Stop();
            }
        }

        public void Move(Vector2 input)
        {
            if (_mainCamera == null) return;

            // Convert 2D input to world direction
            Vector3 forward = _mainCamera.transform.forward;
            Vector3 right = _mainCamera.transform.right;

            // Flatten to horizontal plane
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = (forward * input.y + right * input.x).normalized;

            if (direction.magnitude > 0.1f)
            {
                // Rotate player to face movement direction with smooth interpolation
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 10f
                );

                // Move towards target direction (NavMeshAgent handles speed internally)
                Vector3 targetPosition = transform.position + direction * 10f;
                _agent.SetDestination(targetPosition);
                _agent.isStopped = false;
            }
        }

        public void Stop()
        {
            _agent.isStopped = true;
        }

        // For external control
        public NavMeshAgent Agent => _agent;

        public void RotateTo(Vector3 direction)
        {
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}