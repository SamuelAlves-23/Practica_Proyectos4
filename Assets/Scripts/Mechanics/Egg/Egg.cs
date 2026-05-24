using System.Collections.Generic;
using UnityEngine;

namespace SOTG.Mechanics.Egg
{
    public class EggEntity : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private bool _isKidnapped = false;
        [SerializeField] private bool _isTargeted = false;

        [Header("Visual")]
        [SerializeField] private GameObject _visualRoot;

        public static List<EggEntity> AllEggs { get; private set; } = new List<EggEntity>();

        public static System.Action<EggEntity> OnAnyEggKidnapped;

        public static System.Action<EggEntity> OnAnyEggRecovered;

        public bool IsKidnapped => _isKidnapped;
        public bool IsTargeted => _isTargeted;

        private Renderer[] _renderers;

        private void Awake()
        {
            if (_visualRoot != null)
            {
                _renderers = _visualRoot.GetComponentsInChildren<Renderer>(true);
            }
            else
            {
                _renderers = GetComponentsInChildren<Renderer>(true);
            }
        }

        private void OnEnable()
        {
            if (!AllEggs.Contains(this))
            {
                AllEggs.Add(this);
            }
        }

        private void OnDisable()
        {
            AllEggs.Remove(this);
        }

        private void OnDestroy()
        {
            AllEggs.Remove(this);
        }

        public void SetTargeted(bool targeted)
        {
            _isTargeted = targeted;
        }
        public static EggEntity GetAvailableEgg(Vector3 fromPosition)
        {
            EggEntity bestCandidate = null;
            float closestDistance = float.MaxValue;

            for (int i = AllEggs.Count - 1; i >= 0; i--)
            {
                EggEntity egg = AllEggs[i];
                if (egg == null)
                {
                    AllEggs.RemoveAt(i);
                    continue;
                }
                if (egg._isKidnapped || egg._isTargeted) continue;

                float distance = Vector3.Distance(fromPosition, egg.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCandidate = egg;
                }
            }

            return bestCandidate;
        }
        public void Kidnap()
        {
            if (_isKidnapped) return;

            _isKidnapped = true;
            SetVisualsActive(false);
            OnKidnapped?.Invoke(this);
            OnAnyEggKidnapped?.Invoke(this);
        }

        public void Recover()
        {
            if (!_isKidnapped) return;

            _isKidnapped = false;
            _isTargeted = false;
            SetVisualsActive(true);
            OnRecovered?.Invoke(this);
            OnAnyEggRecovered?.Invoke(this);
        }

        private void SetVisualsActive(bool active)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                {
                    _renderers[i].enabled = active;
                }
            }
        }

        public System.Action<EggEntity> OnKidnapped;
        public System.Action<EggEntity> OnRecovered;
    }
}