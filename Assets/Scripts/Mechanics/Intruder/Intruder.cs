using UnityEngine;
using UnityEngine.AI;
using SOTG.Mechanics.Egg;

namespace SOTG.Mechanics.Intruder
{
    /// <summary>
    /// AI intruder that seeks eggs, kidnaps them, and disappears.
    /// </summary>
    public class Intruder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed = 3f;
        [SerializeField] private float _kidnapRadius = 1f;
        [SerializeField] private float _detectionRadius = 30f;

        [Header("References")]
        [SerializeField] private EggSpawner _eggSpawner;

        private NavMeshAgent _agent;
        private EggEntity _targetEgg;
        private bool _hasKidnapped = false;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _speed;
        }

        private void Start()
        {
            if (_eggSpawner == null)
            {
                _eggSpawner = FindFirstObjectByType<EggSpawner>();
            }

            FindNewTarget();
        }

        private void Update()
        {
            if (_hasKidnapped) return;

            if (_targetEgg == null || _targetEgg.IsKidnapped)
            {
                FindNewTarget();
                return;
            }

            // Move towards target egg
            _agent.SetDestination(_targetEgg.transform.position);

            // Check if close enough to kidnap
            float distance = Vector3.Distance(transform.position, _targetEgg.transform.position);
            if (distance <= _kidnapRadius)
            {
                KidnapEgg();
            }
        }

        private void FindNewTarget()
        {
            if (_eggSpawner == null) return;

            _targetEgg = _eggSpawner.GetClosestEgg(transform.position);

            if (_targetEgg != null)
            {
                _agent.SetDestination(_targetEgg.transform.position);
            }
        }

        private void KidnapEgg()
        {
            if (_targetEgg == null || _hasKidnapped) return;

            _hasKidnapped = true;
            _targetEgg.Kidnap();

            // Disappear after kidnapping
            Invoke("Despawn", 0.5f);
        }

        private void Despawn()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualization for debug
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            if (_targetEgg != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _targetEgg.transform.position);
            }
        }
    }
}