using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOTG.Mechanics.Config;
using SOTG.Mechanics.Egg;

namespace SOTG.Mechanics.Intruder
{
    /// <summary>
    /// Spawns intruders at random positions on the NavMesh.
    /// </summary>
    public class IntruderSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _intruderPrefab;

        [Header("Settings")]
        [SerializeField] private int _intruderCount = 5;
        [SerializeField] private float _spawnRadius = 20f;
        [SerializeField] private Vector2 _spawnAreaCenter = Vector2.zero;

        [Header("References")]
        [SerializeField] private EggSpawner _eggSpawner;

        private List<Intruder> _spawnedIntruders = new List<Intruder>();

        public IReadOnlyList<Intruder> SpawnedIntruders => _spawnedIntruders;

        private void Start()
        {
            // Load from config if available
            var config = GameConfig.Instance;
            if (config != null)
            {
                _intruderCount = config.IntruderCount;
            }

            // Find egg spawner if not assigned
            if (_eggSpawner == null)
            {
                _eggSpawner = FindFirstObjectByType<EggSpawner>();
            }

            SpawnIntruders();
        }

        private void SpawnIntruders()
        {
            if (_intruderPrefab == null)
            {
                Debug.LogWarning("IntruderSpawner: No intruder prefab assigned!");
                return;
            }

            for (int i = 0; i < _intruderCount; i++)
            {
                SpawnSingleIntruder();
            }
        }

        private void SpawnSingleIntruder()
        {
            Vector3 spawnPosition = GetRandomNavMeshPosition();
            if (spawnPosition == Vector3.zero) return;

            GameObject intruderObj = Instantiate(_intruderPrefab, spawnPosition, Quaternion.identity);
            intruderObj.transform.parent = transform;

            Intruder intruder = intruderObj.GetComponent<Intruder>();
            if (intruder != null)
            {
                _spawnedIntruders.Add(intruder);
            }
        }

        private Vector3 GetRandomNavMeshPosition()
        {
            Vector2 randomPoint = _spawnAreaCenter + Random.insideUnitCircle * _spawnRadius;
            Vector3 searchPosition = new Vector3(randomPoint.x, 0, randomPoint.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(searchPosition, out hit, _spawnRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return Vector3.zero;
        }
    }
}