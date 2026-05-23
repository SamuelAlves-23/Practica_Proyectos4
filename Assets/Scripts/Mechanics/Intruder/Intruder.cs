using UnityEngine;
using UnityEngine.AI;
using SOTG.Mechanics.Egg;

namespace SOTG.Mechanics.Intruder
{
    /// <summary>
    /// AI intruder that seeks eggs, kidnaps them, and escapes to caves.
    /// States: Searching -> CarryingEgg -> Escaping
    /// </summary>
    public class Intruder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _kidnapRadius = 1f;
        [SerializeField] private float _detectionRadius = 30f;
        [SerializeField] private float _exitDistanceThreshold = 1.5f;
        [SerializeField] private float _animatorDampTime = 0.1f;

        private NavMeshAgent _agent;
        private Animator _animator;
        private EggEntity _targetEgg;
        private bool _hasKidnapped = false;
        private bool _isEscaping = false;
        private bool _hasEscaped = false;

        // Track which egg we're targeting (for claim/release)
        private EggEntity _targetedEgg;

        // Track the egg we actually kidnapped (for recovery if we die)
        private EggEntity _kidnappedEgg;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _speed;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (_hasEscaped) return;

            // Drive animator parameters from NavMeshAgent velocity
            UpdateAnimation();

            // After kidnapping, stop looking for eggs and start escaping
            if (_hasKidnapped || _isEscaping)
            {
                // Only check cave arrival when we're actually escaping,
                // not right after kidnap (agent is still at egg position, distance ≈ 0)
                if (_isEscaping && !_agent.pathPending && _agent.remainingDistance <= _exitDistanceThreshold)
                {
                    Escape();
                }
                return;
            }

            // Check if we still have a valid target egg
            if (_targetEgg != null && !_targetEgg.IsKidnapped)
            {
                // State: Moving to target egg
                _agent.SetDestination(_targetEgg.transform.position);

                float distance = Vector3.Distance(transform.position, _targetEgg.transform.position);
                if (distance <= _kidnapRadius)
                {
                    KidnapEgg();
                }
            }
            else
            {
                // State: Looking for eggs or escaping
                FindNewTarget();
            }
        }

        private void UpdateAnimation()
        {
            if (_animator == null) return;

            // Convert world velocity to local space relative to agent's facing direction
            Vector3 velocity = _agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            // Use forward speed (local Z) as the 1D blend parameter for XSpeed
            float forwardSpeed = Mathf.Clamp01(Mathf.Abs(localVelocity.z) / _speed);

            _animator.SetFloat("XSpeed", forwardSpeed, _animatorDampTime, Time.deltaTime);
            _animator.SetFloat("YSpeed", 0f);
            _animator.SetBool("Walking", _agent.velocity.magnitude > 0.1f);
        }

        private void FindNewTarget()
        {
            // Release our previous target so other intruders can claim it
            if (_targetedEgg != null)
            {
                _targetedEgg.SetTargeted(false);
                _targetedEgg = null;
            }

            // Find an available egg via the static EggEntity registry
            EggEntity availableEgg = EggEntity.GetAvailableEgg(transform.position);

            if (availableEgg != null)
            {
                // Claim this egg so other intruders don't target it
                availableEgg.SetTargeted(true);
                _targetedEgg = availableEgg;
                _targetEgg = availableEgg;
                _agent.SetDestination(_targetEgg.transform.position);
            }
            else
            {
                // No eggs available - escape to nearest cave
                GoToNearestCave();
            }
        }

        private void KidnapEgg()
        {
            if (_targetEgg == null || _hasKidnapped) return;

            // Release the egg claim before kidnapping
            if (_targetedEgg != null)
            {
                _targetedEgg.SetTargeted(false);
                _targetedEgg = null;
            }

            _hasKidnapped = true;

            // Store reference for recovery if we die before escaping
            _kidnappedEgg = _targetEgg;
            _kidnappedEgg.Kidnap();
            _targetEgg = null;

            // After kidnapping, escape to nearest cave
            Invoke(nameof(GoToNearestCave), 0.3f);
        }

        private void GoToNearestCave()
        {
            Transform nearestCave = Cave.GetNearestCave(transform.position);

            if (nearestCave == null)
            {
                Debug.LogWarning($"Intruder {gameObject.name}: No caves found in scene!");
                return;
            }

            _isEscaping = true;
            _agent.SetDestination(nearestCave.position);
        }

        private void Escape()
        {
            _hasEscaped = true;

            // Intruder escaped with the egg — it's lost forever
            if (_kidnappedEgg != null)
            {
                Destroy(_kidnappedEgg.gameObject);
                _kidnappedEgg = null;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Die when touching player
            if (other.CompareTag("Player"))
            {
                Die();
            }
        }

        /// <summary>
        /// Called by HitBox when the player attacks this intruder.
        /// </summary>
        public void Kill()
        {
            Die();
        }

        private void Die()
        {
            // Release our target if we die
            if (_targetedEgg != null)
            {
                _targetedEgg.SetTargeted(false);
            }

            // If we were carrying a kidnapped egg, recover it
            if (_kidnappedEgg != null)
            {
                _kidnappedEgg.Recover();
                _kidnappedEgg = null;
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            if (_targetEgg != null && !_hasKidnapped)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _targetEgg.transform.position);
            }
        }
    }
}