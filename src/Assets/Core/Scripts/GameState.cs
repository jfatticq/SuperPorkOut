using UnityEngine;

namespace SuperPorkOut.Core
{
    public static class GameState
    {
        private const string TutorialCompletedKey = "TutorialCompleted";

        public static bool IsTutorialCompleted
        {
            get => PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;
            set
            {
                PlayerPrefs.SetInt(TutorialCompletedKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }
}
