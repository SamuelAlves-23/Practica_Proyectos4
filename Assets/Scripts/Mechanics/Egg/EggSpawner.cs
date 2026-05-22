using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOTG.Mechanics.Config;

namespace SOTG.Mechanics.Egg
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

        private List<EggEntity> _spawnedEggs = new List<EggEntity>();

        public IReadOnlyList<EggEntity> SpawnedEggs => _spawnedEggs;

        public System.Action<int> OnEggCountChanged;

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

            EggEntity egg = eggObj.GetComponent<EggEntity>();
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

        private void HandleEggKidnapped(EggEntity egg)
        {
            if (_spawnedEggs.Contains(egg))
            {
                egg.OnKidnapped -= HandleEggKidnapped;
                _spawnedEggs.Remove(egg);
                OnEggCountChanged?.Invoke(_spawnedEggs.Count);
            }
        }

        /// <summary>
        /// Returns the closest egg to a given position.
        /// </summary>
        public EggEntity GetClosestEgg(Vector3 position)
        {
            EggEntity closest = null;
            float closestDistance = float.MaxValue;

            foreach (EggEntity egg in _spawnedEggs)
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

        /// <summary>
        /// Returns an egg that is not kidnapped AND not targeted by another intruder.
        /// </summary>
        public EggEntity GetAvailableEgg(Vector3 position)
        {
            EggEntity bestCandidate = null;
            float closestDistance = float.MaxValue;

            foreach (EggEntity egg in _spawnedEggs)
            {
                if (egg == null || egg.IsKidnapped || egg.IsTargeted) continue;

                float distance = Vector3.Distance(position, egg.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCandidate = egg;
                }
            }

            return bestCandidate;
        }

        /// <summary>
        /// Marks an egg as targeted by an intruder.
        /// </summary>
        public void ClaimEgg(EggEntity egg)
        {
            if (egg != null)
            {
                egg.SetTargeted(true);
            }
        }

        /// <summary>
        /// Releases an egg so other intruders can target it.
        /// </summary>
        public void ReleaseEgg(EggEntity egg)
        {
            if (egg != null)
            {
                egg.SetTargeted(false);
            }
        }
    }
}