using UnityEngine;
using SOTG.Mechanics.Egg;

namespace SOTG.Mechanics.Game
{
    /// <summary>
    /// Manages game state, tracks eggs, win/lose conditions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EggSpawner _eggSpawner;

        private int _initialEggCount;
        private int _currentEggCount;
        private bool _isGameOver;

        public int EggsRemaining => _currentEggCount;
        public int InitialEggCount => _initialEggCount;
        public bool IsGameOver => _isGameOver;

        public System.Action<int> OnEggsChanged;
        public System.Action OnGameWon;
        public System.Action OnGameLost;

        private void Start()
        {
            if (_eggSpawner != null)
            {
                _initialEggCount = _eggSpawner.SpawnedEggs.Count;
                _currentEggCount = _initialEggCount;

                // Subscribe to egg count changes
                _eggSpawner.OnEggCountChanged += HandleEggCountChanged;
            }
        }

        private void OnDestroy()
        {
            if (_eggSpawner != null)
            {
                _eggSpawner.OnEggCountChanged -= HandleEggCountChanged;
            }
        }

        private void HandleEggCountChanged(int remaining)
        {
            _currentEggCount = remaining;
            OnEggsChanged?.Invoke(_currentEggCount);
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            if (_isGameOver) return;

            if (_currentEggCount <= 0)
            {
                _isGameOver = true;
                OnGameLost?.Invoke();
            }
        }

        /// <summary>
        /// Called when all intruders are eliminated (if implemented).
        /// </summary>
        public void OnAllIntrudersDefeated()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            OnGameWon?.Invoke();
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
        public void RestartGame()
        {
            _isGameOver = false;
            _currentEggCount = _initialEggCount;
            OnEggsChanged?.Invoke(_currentEggCount);
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}