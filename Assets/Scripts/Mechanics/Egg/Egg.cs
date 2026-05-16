using UnityEngine;

namespace SOTG.Mechanics.Egg
{
    /// <summary>
    /// Represents an egg that can be kidnapped by intruders.
    /// </summary>
    public class EggEntity : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private bool _isKidnapped = false;

        public bool IsKidnapped => _isKidnapped;

        /// <summary>
        /// Called when an intruder grabs this egg.
        /// </summary>
        public void Kidnap()
        {
            if (_isKidnapped) return;

            _isKidnapped = true;
            OnKidnapped?.Invoke(this);
            Destroy(gameObject);
        }

        public System.Action<EggEntity> OnKidnapped;

        /// <summary>
        /// Called when player rescues this egg (if carried).
        /// </summary>
        public void Rescue()
        {
            _isKidnapped = false;
            OnRescued?.Invoke(this);
        }

        public System.Action<EggEntity> OnRescued;
    }
}