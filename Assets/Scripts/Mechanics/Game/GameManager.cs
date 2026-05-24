using UnityEngine;
using SOTG.Mechanics.Egg;
using UnityEngine.SceneManagement;

// Type alias to resolve namespace/class collision (SOTG.Mechanics.Intruder vs Intruder class)
using IntruderClass = SOTG.Mechanics.Intruder.Intruder;

namespace SOTG.Mechanics.Game
{
    /// <summary>
    /// Manages game state, tracks eggs and intruders, win/lose conditions.
    /// Eggs and intruders are placed manually in the scene — no spawners.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Over")]
        [SerializeField] private string _gameOverSceneName = "GameOverScene";

        private int _initialEggCount;
        private int _currentEggCount;
        private int _initialIntruderCount;
        private int _currentIntruderCount;
        private int _intrudersKilled;
        private bool _isGameOver;

        public int EggsRemaining => _currentEggCount;
        public int InitialEggCount => _initialEggCount;
        public int IntrudersRemaining => _currentIntruderCount;
        public int InitialIntruderCount => _initialIntruderCount;
        public bool IsGameOver => _isGameOver;
        public bool PlayerWon { get; private set; }

        public System.Action<int> OnEggsChanged;
        public System.Action<int> OnIntrudersChanged;
        public System.Action OnGameWon;
        public System.Action OnGameLost;

        private void Start()
        {
            // Count eggs placed manually in the scene
            _initialEggCount = EggEntity.AllEggs.Count;
            _currentEggCount = _initialEggCount;

            // Subscribe to global egg kidnap/recover events
            EggEntity.OnAnyEggKidnapped += HandleEggKidnapped;
            EggEntity.OnAnyEggRecovered += HandleEggRecovered;

            // Count ALL intruders in scene (hand-placed)
            _initialIntruderCount = IntruderClass.AllIntruders.Count;
            _currentIntruderCount = _initialIntruderCount;

            // Track kills separately from escapes
            _intrudersKilled = 0;
            IntruderClass.OnAnyIntruderKilled += HandleIntruderKilled;

            // Notify initial state
            OnEggsChanged?.Invoke(_currentEggCount);
            OnIntrudersChanged?.Invoke(_currentIntruderCount);
        }

        private void OnDestroy()
        {
            EggEntity.OnAnyEggKidnapped -= HandleEggKidnapped;
            EggEntity.OnAnyEggRecovered -= HandleEggRecovered;
            IntruderClass.OnAnyIntruderKilled -= HandleIntruderKilled;
        }

        private void HandleIntruderKilled(IntruderClass intruder)
        {
            _intrudersKilled++;
        }

        private void Update()
        {
            // Track intruder count in real-time (kills + escapes)
            if (!_isGameOver)
            {
                int aliveCount = IntruderClass.AllIntruders.Count;
                if (aliveCount != _currentIntruderCount)
                {
                    _currentIntruderCount = aliveCount;
                    OnIntrudersChanged?.Invoke(_currentIntruderCount);
                    CheckVictory();
                }
            }
        }

        private void HandleEggKidnapped(EggEntity egg)
        {
            _currentEggCount--;
            OnEggsChanged?.Invoke(_currentEggCount);
            CheckDefeat();
        }

        private void HandleEggRecovered(EggEntity egg)
        {
            _currentEggCount++;
            OnEggsChanged?.Invoke(_currentEggCount);
        }

        private void CheckDefeat()
        {
            if (_isGameOver) return;

            if (_currentEggCount <= 0)
            {
                _isGameOver = true;
                PlayerWon = false;
                OnGameLost?.Invoke();
                Invoke(nameof(LoadGameOverScene), 1.5f);
            }
        }

        private void CheckVictory()
        {
            if (_isGameOver) return;

            // Win when all intruders are dead AND not all eggs are lost
            if (_currentIntruderCount <= 0 && _currentEggCount > 0)
            {
                _isGameOver = true;
                PlayerWon = true;
                OnGameWon?.Invoke();
                Invoke(nameof(LoadGameOverScene), 1.5f);
            }
        }

        private void LoadGameOverScene()
        {
            // Pass results via PlayerPrefs
            PlayerPrefs.SetInt("SOTG_LastResult_Won", PlayerWon ? 1 : 0);
            PlayerPrefs.SetInt("SOTG_LastResult_EggsRemaining", _currentEggCount);
            PlayerPrefs.SetInt("SOTG_LastResult_InitialEggs", _initialEggCount);
            PlayerPrefs.SetInt("SOTG_LastResult_IntrudersKilled", _intrudersKilled);
            PlayerPrefs.Save();

            SceneManager.LoadScene(_gameOverSceneName);
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
        public void RestartGame()
        {
            _isGameOver = false;
            _currentEggCount = _initialEggCount;
            OnEggsChanged?.Invoke(_currentEggCount);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
