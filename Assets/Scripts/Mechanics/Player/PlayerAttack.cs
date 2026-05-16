using UnityEngine;
using SOTG.Mechanics.Intruder;

namespace SOTG.Mechanics.Player
{
    /// <summary>
    /// Handles player melee attack to eliminate intruders.
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackCooldown = 0.5f;

        [Header("References")]
        [SerializeField] private PlayerInputHandler _inputHandler;

        private float _lastAttackTime;
        private bool _canAttack = true;

        private void Start()
        {
            if (_inputHandler != null)
            {
                _inputHandler.OnAttackStarted += PerformAttack;
            }
        }

        private void OnDestroy()
        {
            if (_inputHandler != null)
            {
                _inputHandler.OnAttackStarted -= PerformAttack;
            }
        }

        private void PerformAttack()
        {
            if (!_canAttack) return;

            if (Time.time - _lastAttackTime < _attackCooldown) return;

            _lastAttackTime = Time.time;
            _canAttack = false;

            // Cooldown reset
            Invoke(nameof(ResetCooldown), _attackCooldown);

            // Find and destroy intruders in range
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange);
            foreach (var collider in hitColliders)
            {
                SOTG.Mechanics.Intruder.Intruder intruder = collider.GetComponent<SOTG.Mechanics.Intruder.Intruder>();
                if (intruder != null)
                {
                    Destroy(intruder.gameObject);
                }
            }

            // Visual feedback (optional: trigger animation)
            OnAttackPerformed?.Invoke();
        }

        private void ResetCooldown()
        {
            _canAttack = true;
        }

        public System.Action OnAttackPerformed;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}