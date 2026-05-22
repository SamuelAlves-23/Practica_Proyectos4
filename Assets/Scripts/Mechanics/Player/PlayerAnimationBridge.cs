using UnityEngine;

namespace SOTG.Mechanics.Player
{
    /// <summary>
    /// Bridges player events to Animator.
    /// </summary>
    public class PlayerAnimationBridge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement _movement;

        private Animator _animator;

        private void Awake()
        {
            // Get Animator from this object or children
            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            if (_movement != null && _animator != null)
            {
                _movement.OnMovementChanged += HandleMovement;
            }
        }

        private void OnDestroy()
        {
            if (_movement != null)
            {
                _movement.OnMovementChanged -= HandleMovement;
            }
        }

        private void HandleMovement(Vector2 input)
        {
            if (_animator == null) return;

            _animator.SetFloat("XSpeed", input.x);
            _animator.SetFloat("YSpeed", input.y);
        }
    }
}