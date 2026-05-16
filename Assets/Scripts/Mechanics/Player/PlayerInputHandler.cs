using UnityEngine;
using UnityEngine.InputSystem;

namespace SOTG.Mechanics.Player
{

    public class PlayerInputHandler : MonoBehaviour, Player_InputAction.IPlayerActions
    {
        [Header("Input")]
        [SerializeField] private Player_InputAction _inputAction;

        // Input state
        private Vector2 _moveInput;
        private bool _isAttackPressed;

        // Events for other components
        public System.Action<Vector2> OnMoveInput;
        public System.Action OnAttackStarted;

        // Properties
        public Vector2 MoveInput => _moveInput;
        public bool IsAttacking => _isAttackPressed;

        private void Awake()
        {
            if (_inputAction == null)
            {
                _inputAction = new Player_InputAction();
            }
        }

        private void OnEnable()
        {
            _inputAction.Player.AddCallbacks(this);
            _inputAction.Enable();
        }

        private void OnDisable()
        {
            _inputAction.Disable();
            _inputAction.Player.RemoveCallbacks(this);
        }

        #region Input System Callbacks

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            OnMoveInput?.Invoke(_moveInput);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Not used in this game - reserved for future
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isAttackPressed = true;
                OnAttackStarted?.Invoke();
            }
            else if (context.canceled)
            {
                _isAttackPressed = false;
            }
        }

        #endregion
    }
}