using SuperPorkOut.Core;
using SuperPorkOut.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SuperPorkOut.Screens.MainMenu
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string TutorialSceneName = "Level01";
        [SerializeField] private string EndlessSceneName = "Endless";
        [SerializeField] private string GuideSceneName = "Guide";
        [SerializeField] private string SettingsSceneName = "Settings";

        private static readonly WaitForSeconds _waitForSeconds1 = new(1);

        private Button playButton;
        private Button guideButton;
        private Button settingsButton;
        private Button quitButton;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            playButton = root.Q<Button>("StartButton");
            if (playButton == null)
            {
                Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'StartButton' in the UIDocument.");
                return;
            }

            guideButton = root.Q<Button>("GuideButton");
            if (guideButton == null)
            {
                Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'GuideButton' in the UIDocument.");
                return;
            }

            settingsButton = root.Q<Button>("SettingsButton");
            if (settingsButton == null)
            {
                Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'SettingsButton' in the UIDocument.");
                return;
            }

            quitButton = root.Q<Button>("QuitButton");
            if (quitButton == null)
            {
                Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'QuitButton' in the UIDocument.");
                return;
            }

            playButton.text = GameState.IsTutorialCompleted
                ? "Escape Endless!"
                : "Test the Fence!";

            playButton.clicked += OnPlayClicked;
            guideButton.clicked += OnGuideClicked;
            settingsButton.clicked += OnSettingsClicked;
            quitButton.clicked += OnQuitClicked;

            var statsPanel = root.Q<RunStatsPanel>("RunStatsPanel");
            statsPanel?.Refresh(EndlessSceneName);
        }

        private void OnDisable()
        {
            if (playButton != null)
                playButton.clicked -= OnPlayClicked;
            if (guideButton != null)
                guideButton.clicked -= OnGuideClicked;
            if (settingsButton != null)
                settingsButton.clicked -= OnSettingsClicked;
            if (quitButton != null)
                quitButton.clicked -= OnQuitClicked;
        }

        private void OnPlayClicked()
        {
            StartCoroutine(StartLevel());
        }

        private void OnGuideClicked()
        {
            SceneManager.LoadScene(GuideSceneName);
        }

        private void OnSettingsClicked()
        {
            SceneManager.LoadScene(SettingsSceneName);
        }

        private void OnQuitClicked()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        IEnumerator StartLevel()
        {
            yield return _waitForSeconds1;

            var nextScene = GameState.IsTutorialCompleted
                ? EndlessSceneName
                : TutorialSceneName;

            SceneManager.LoadScene(nextScene);
        }
    }
}
