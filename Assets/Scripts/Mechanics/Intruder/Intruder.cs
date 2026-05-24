using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOTG.Mechanics.Egg;

namespace SOTG.Mechanics.Intruder
{
    public class Intruder : MonoBehaviour
    {
    public static List<Intruder> AllIntruders { get; private set; } = new List<Intruder>();

    public static System.Action<Intruder> OnAnyIntruderKilled;

    [Header("Settings")]
    [SerializeField] private float _speed = 7.5f;
        [SerializeField] private float _kidnapRadius = 1f;
        [SerializeField] private float _detectionRadius = 30f;
        [SerializeField] private float _exitDistanceThreshold = 1.5f;
        [SerializeField] private float _animatorDampTime = 0.1f;

        private NavMeshAgent _agent;
        private Animator _animator;
        private EggEntity _targetEgg;
        private bool _hasKidnapped = false;
        public bool HasKidnapped => _hasKidnapped;
        private bool _isEscaping = false;
        private bool _hasEscaped = false;

        private EggEntity _targetedEgg;

        private EggEntity _kidnappedEgg;

    private void OnEnable()
    {
        if (!AllIntruders.Contains(this))
        {
            AllIntruders.Add(this);
        }
    }

    private void OnDisable()
    {
        AllIntruders.Remove(this);
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _animator = GetComponentInChildren<Animator>();
    }

        private void Update()
        {
            if (_hasEscaped) return;

            UpdateAnimation();

            if (_hasKidnapped || _isEscaping)
            {
                if (_isEscaping && !_agent.pathPending && _agent.remainingDistance <= _exitDistanceThreshold)
                {
                    Escape();
                }
                return;
            }

            if (_targetEgg != null && !_targetEgg.IsKidnapped)
            {
                _agent.SetDestination(_targetEgg.transform.position);

                float distance = Vector3.Distance(transform.position, _targetEgg.transform.position);
                if (distance <= _kidnapRadius)
                {
                    KidnapEgg();
                }
            }
            else
            {
                FindNewTarget();
            }
        }

        private void UpdateAnimation()
        {
            if (_animator == null) return;

            Vector3 velocity = _agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float forwardSpeed = Mathf.Clamp01(Mathf.Abs(localVelocity.z) / _speed);

            _animator.SetFloat("XSpeed", forwardSpeed, _animatorDampTime, Time.deltaTime);
            _animator.SetFloat("YSpeed", 0f);
            _animator.SetBool("Walking", _agent.velocity.magnitude > 0.1f);
        }

        private void FindNewTarget()
        {
            if (_targetedEgg != null)
            {
                _targetedEgg.SetTargeted(false);
                _targetedEgg = null;
            }

            EggEntity availableEgg = EggEntity.GetAvailableEgg(transform.position);

            if (availableEgg != null)
            {
                availableEgg.SetTargeted(true);
                _targetedEgg = availableEgg;
                _targetEgg = availableEgg;
                _agent.SetDestination(_targetEgg.transform.position);
            }
            else
            {
                GoToNearestCave();
            }
        }

        private void KidnapEgg()
        {
            if (_targetEgg == null || _hasKidnapped) return;

            if (_targetedEgg != null)
            {
                _targetedEgg.SetTargeted(false);
                _targetedEgg = null;
            }

            _hasKidnapped = true;

            _kidnappedEgg = _targetEgg;
            _kidnappedEgg.Kidnap();
            _targetEgg = null;

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

            if (_kidnappedEgg != null)
            {
                Destroy(_kidnappedEgg.gameObject);
                _kidnappedEgg = null;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Die();
            }
        }

        public void Kill()
        {
            Die();
        }

        private void Die()
        {
            if (_targetedEgg != null)
            {
                _targetedEgg.SetTargeted(false);
            }

            if (_kidnappedEgg != null)
            {
                _kidnappedEgg.Recover();
                _kidnappedEgg = null;
            }

            OnAnyIntruderKilled?.Invoke(this);

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