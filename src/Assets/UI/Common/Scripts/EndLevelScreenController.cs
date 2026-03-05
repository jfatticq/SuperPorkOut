using SuperPorkOut.Core;
using SuperPorkOut.Levels;
using SuperPorkOut.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SuperPorkOut.Gameplay
{
    [RequireComponent(typeof(UIDocument))]
    public class EndLevelScreenController : MonoBehaviour
    {
        [Header("UXML names")]
        [SerializeField] private string restartButtonName = "RestartButton";
        [SerializeField] private string mainMenuButtonName = "MainMenuButton";

        [Header("Actions")]
        [SerializeField] private SceneFlow sceneFlow;
        [SerializeField] private RunStatsRecorder runStatsRecorder;

        private UIDocument doc;
        private Button restartButton;
        private Button mainMenuButton;
        private bool bound;

        private void Awake()
        {
            doc = GetComponent<UIDocument>();
            doc.enabled = false;
        }

        public void Show()
        {
            doc.enabled = true;
            Bind();
            SaveAndRefreshStats();
        }

        public void Hide()
        {
            Unbind();
            doc.enabled = false;
        }

        private void Bind()
        {
            if (bound) return;
            var root = doc.rootVisualElement;
            if (root == null) return;

            restartButton = root.Q<Button>(restartButtonName);
            mainMenuButton = root.Q<Button>(mainMenuButtonName);

            if (restartButton != null) restartButton.clicked += OnRestart;
            else Debug.LogWarning($"[EndLevelScreenController] Missing '{restartButtonName}' button.");

            if (mainMenuButton != null) mainMenuButton.clicked += OnMainMenu;
            else Debug.LogWarning($"[EndLevelScreenController] Missing '{mainMenuButtonName}' button.");

            bound = true;
        }

        private void Unbind()
        {
            if (!bound) return;

            if (restartButton != null) restartButton.clicked -= OnRestart;
            if (mainMenuButton != null) mainMenuButton.clicked -= OnMainMenu;

            bound = false;
        }

        private void OnRestart()
        {
            if (sceneFlow == null)
            {
                Debug.LogError("[EndLevelScreenController] SceneFlow not assigned.");
                return;
            }
            sceneFlow.RestartCurrentScene();
        }

        private void OnMainMenu()
        {
            if (sceneFlow == null)
            {
                Debug.LogError("[EndLevelScreenController] SceneFlow not assigned.");
                return;
            }
            sceneFlow.GoToMainMenu();
        }

        private void SaveAndRefreshStats()
        {
            var sceneName = SceneManager.GetActiveScene().name;

            if (runStatsRecorder != null)
            {
                var entry = runStatsRecorder.BuildEntry();
                RunStatsStore.Save(sceneName, entry);
            }

            var root = doc.rootVisualElement;
            if (root == null) return;

            var panel = root.Q<RunStatsPanel>("RunStatsPanel");
            panel?.Refresh(SceneManager.GetActiveScene().name);
            panel.Refresh(sceneName);
        }
    }
}
