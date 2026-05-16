using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ProtectTheForest.Mechanics.Config;

namespace ProtectTheForest.Mechanics.Egg
{
    /// <summary>
    /// Spawns eggs at random positions on the NavMesh.
    /// </summary>
    public class EggSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _eggPrefab;

        [Header("Settings")]
        [SerializeField] private int _eggCount = 10;
        [SerializeField] private float _spawnRadius = 20f;
        [SerializeField] private Vector2 _spawnAreaCenter = Vector2.zero;

        private List<Egg> _spawnedEggs = new List<Egg>();

        public IReadOnlyList<Egg> SpawnedEggs => _spawnedEggs;

        private void Start()
        {
            // Load from config if available
            var config = GameConfig.Instance;
            if (config != null)
            {
                _eggCount = config.EggCount;
            }

            SpawnEggs();
        }

        private void SpawnEggs()
        {
            if (_eggPrefab == null)
            {
                Debug.LogWarning("EggSpawner: No egg prefab assigned!");
                return;
            }

            for (int i = 0; i < _eggCount; i++)
            {
                SpawnSingleEgg();
            }
        }

        private void SpawnSingleEgg()
        {
            Vector3 spawnPosition = GetRandomNavMeshPosition();
            if (spawnPosition == Vector3.zero) return;

            GameObject eggObj = Instantiate(_eggPrefab, spawnPosition, Quaternion.identity);
            eggObj.transform.parent = transform;

            Egg egg = eggObj.GetComponent<Egg>();
            if (egg != null)
            {
                egg.OnKidnapped += HandleEggKidnapped;
                _spawnedEggs.Add(egg);
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

        private void HandleEggKidnapped(Egg egg)
        {
            if (_spawnedEggs.Contains(egg))
            {
                egg.OnKidnapped -= HandleEggKidnapped;
                _spawnedEggs.Remove(egg);
            }
        }

        /// <summary>
        /// Returns the closest egg to a given position.
        /// </summary>
        public Egg GetClosestEgg(Vector3 position)
        {
            Egg closest = null;
            float closestDistance = float.MaxValue;

            foreach (Egg egg in _spawnedEggs)
            {
                if (egg == null || egg.IsKidnapped) continue;

                float distance = Vector3.Distance(position, egg.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = egg;
                }
            }

            return closest;
        }
    }
}