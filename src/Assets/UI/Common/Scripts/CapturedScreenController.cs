using System.Collections;
using SuperPorkOut.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SuperPorkOut.Levels
{
    [RequireComponent(typeof(UIDocument))]
    public class CapturedScreenController : MonoBehaviour
    {
        [Header("UI Elements (UXML names)")]
        [SerializeField] private string restartButtonName = "RestartButton";
        [SerializeField] private string mainMenuButtonName = "MainMenuButton";
        [SerializeField] private string fadeOverlayName = "FadeOverlay";

        [Header("Fade")]
        [SerializeField, Min(0f)] private float fadeToBlackSeconds = 1.0f;

        [Header("Actions")]
        [SerializeField] private SceneFlow sceneFlow;

        private UIDocument doc;
        private VisualElement root;
        private VisualElement fadeOverlay;
        private Button restartButton;
        private Button mainMenuButton;

        private Coroutine fadeRoutine;

        private void Awake()
        {
            doc = GetComponent<UIDocument>();
            doc.enabled = false; // start hidden
        }

        private void OnEnable()
        {
            // When enabled, UI Toolkit root is available.
            BindUI();
        }

        private void OnDisable()
        {
            UnbindUI();
        }

        public void Show()
        {
            doc.enabled = true;
            BindUI();

            if (fadeOverlay != null)
            {
                fadeOverlay.style.display = DisplayStyle.Flex;

                // Ensure the element itself is fully "on"; we'll animate color alpha instead.
                fadeOverlay.style.opacity = 1f;

                // Start transparent black.
                fadeOverlay.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
            }

            RefreshStatsPanel();

            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeToBlack());
        }

        public void Hide()
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = null;

            doc.enabled = false;
        }

        private void BindUI()
        {
            if (doc == null) return;

            root = doc.rootVisualElement;
            if (root == null) return;

            fadeOverlay = root.Q<VisualElement>(fadeOverlayName);
            restartButton = root.Q<Button>(restartButtonName);
            mainMenuButton = root.Q<Button>(mainMenuButtonName);

            if (restartButton != null) restartButton.clicked += OnRestartClicked;
            if (mainMenuButton != null) mainMenuButton.clicked += OnMainMenuClicked;
        }

        private void UnbindUI()
        {
            if (restartButton != null) restartButton.clicked -= OnRestartClicked;
            if (mainMenuButton != null) mainMenuButton.clicked -= OnMainMenuClicked;
        }

        private IEnumerator FadeToBlack()
        {
            if (fadeOverlay == null)
                yield break;

            float time = 0f;
            float duration = Mathf.Max(0.0001f, fadeToBlackSeconds);

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(time / duration);

                // Fade ONLY the overlay background
                fadeOverlay.style.backgroundColor = new Color(0f, 0f, 0f, a);

                yield return null;
            }

            fadeOverlay.style.backgroundColor = new Color(0f, 0f, 0f, 1f);
        }

        private void RefreshStatsPanel()
        {
            if (root == null) return;

            var panel = root.Q<RunStatsPanel>();
            if (panel != null)
                panel.Refresh(SceneManager.GetActiveScene().name);
        }

        private void OnRestartClicked()
        {
            if (sceneFlow != null) sceneFlow.RestartCurrentScene();
        }

        private void OnMainMenuClicked()
        {
            if (sceneFlow != null) sceneFlow.GoToMainMenu();
        }
    }
}
