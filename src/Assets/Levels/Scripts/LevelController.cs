using UnityEngine;

namespace SuperPorkOut.Levels
{
    public class LevelController : MonoBehaviour
    {
        // Singleton for easy access from other gameplay scripts
        public static LevelController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple LevelController instances detected - destroying duplicate.", this);
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
