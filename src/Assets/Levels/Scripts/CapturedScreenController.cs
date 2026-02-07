using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace SuperPorkOut.Gameplay
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

            // Start fully transparent, then fade to black.
            if (fadeOverlay != null)
            {
                fadeOverlay.style.opacity = 0f;
                fadeOverlay.style.display = DisplayStyle.Flex;
            }

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

            float t = 0f;
            float dur = Mathf.Max(0.0001f, fadeToBlackSeconds);

            while (t < dur)
            {
                t += Time.unscaledDeltaTime; // UI should fade even if timeScale is 0
                float a = Mathf.Clamp01(t / dur);
                fadeOverlay.style.opacity = a;
                yield return null;
            }

            fadeOverlay.style.opacity = 1f;
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
