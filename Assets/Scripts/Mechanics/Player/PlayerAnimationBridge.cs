using UnityEngine;
using SOTG.Mechanics.Player;

namespace SOTG.Mechanics.Player
{
    /// <summary>
    /// Bridges player events to Animator.
    /// </summary>
    public class PlayerAnimationBridge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerAttack _attack;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (_movement != null)
            {
                _movement.OnMovementChanged += HandleMovement;
            }

            if (_attack != null)
            {
                _attack.OnAttackPerformed += HandleAttack;
            }
        }

        private void OnDestroy()
        {
            if (_movement != null)
            {
                _movement.OnMovementChanged -= HandleMovement;
            }

            if (_attack != null)
            {
                _attack.OnAttackPerformed -= HandleAttack;
            }
        }

        private void HandleMovement(Vector2 input)
        {
            if (_animator == null) return;

            _animator.SetFloat("XSpeed", input.x);
            _animator.SetFloat("YSpeed", input.y);
        }

        private void HandleAttack()
        {
            if (_animator == null) return;

            _animator.SetTrigger("Attack");
        }
    }
}