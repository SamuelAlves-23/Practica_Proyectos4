using UnityEngine;

namespace ProtectTheForest.Mechanics.Config
{

    [CreateAssetMenu(fileName = "GameConfig", menuName = "ProtectTheForest/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Settings")]
        [Tooltip("Number of eggs spawn in the forest")]
        [SerializeField] private int _eggCount = 10;
        [Tooltip("Number of intruders spawn")]
        [SerializeField] private int _intruderCount = 5;

        // Properties
        public int EggCount => _eggCount;
        public int IntruderCount => _intruderCount;

        // Singleton para acceso rápido
        private static GameConfig _instance;
        public static GameConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameConfig>("GameConfig");
                }
                return _instance;
            }
        }
    }
}