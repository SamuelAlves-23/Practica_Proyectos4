using UnityEngine;
using SOTG.Mechanics.Egg;
using UnityEngine.SceneManagement;

using IntruderClass = SOTG.Mechanics.Intruder.Intruder;

namespace SOTG.Mechanics.Game
{

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
            _initialEggCount = EggEntity.AllEggs.Count;
            _currentEggCount = _initialEggCount;

            EggEntity.OnAnyEggKidnapped += HandleEggKidnapped;
            EggEntity.OnAnyEggRecovered += HandleEggRecovered;

            _initialIntruderCount = IntruderClass.AllIntruders.Count;
            _currentIntruderCount = _initialIntruderCount;

            _intrudersKilled = 0;
            IntruderClass.OnAnyIntruderKilled += HandleIntruderKilled;

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
            PlayerPrefs.SetInt("SOTG_LastResult_Won", PlayerWon ? 1 : 0);
            PlayerPrefs.SetInt("SOTG_LastResult_EggsRemaining", _currentEggCount);
            PlayerPrefs.SetInt("SOTG_LastResult_InitialEggs", _initialEggCount);
            PlayerPrefs.SetInt("SOTG_LastResult_IntrudersKilled", _intrudersKilled);
            PlayerPrefs.Save();

            SceneManager.LoadScene(_gameOverSceneName);
        }

        public void RestartGame()
        {
            _isGameOver = false;
            _currentEggCount = _initialEggCount;
            OnEggsChanged?.Invoke(_currentEggCount);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
