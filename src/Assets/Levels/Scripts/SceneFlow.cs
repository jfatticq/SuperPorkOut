using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperPorkOut.Gameplay
{
    public class SceneFlow : MonoBehaviour
    {
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        public void RestartCurrentScene()
        {
            // Reset timeScale in case you paused/froze things
            Time.timeScale = 1f;
            var active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.name);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
