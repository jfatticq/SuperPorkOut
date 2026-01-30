using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerCaughtHandler : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private string playerTag = "Player";

    [Header("Animation")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private FarmerController farmerController;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator farmerAnimator;
    [SerializeField] private string farmerCaptureAnimation = "Captured";
    [SerializeField] private float caughtAnimLeadTime = 0.35f;

    [Header("Captured Screen")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private string restartButtonName = "RestartButton";
    [SerializeField] private string mainMenuButtonName = "MainMenuButton";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float fadeDuration = 0.35f;

    private Button _restartButton;
    private Button _mainMenuButton;
    private VisualElement _root;
    private bool _caught;

    private void Awake()
    {
        HookupUI();
    }

    private void OnEnable()
    {
        // In case UI loads after Awake in your setup
        HookupUI();
    }

    private void HookupUI()
    {
        if (uiDocument == null) return;

        VisualElement root = uiDocument.rootVisualElement;
        if (root == null) return;

        // Cache root and start hidden + non-interactive
        _root = root;
        _root.style.opacity = 0f;
        _root.pickingMode = PickingMode.Ignore;

        _restartButton = root.Q<Button>(restartButtonName);
        if (_restartButton == null) return;

        // Ensure single subscription
        _restartButton.clicked -= RestartLevel;
        _restartButton.clicked += RestartLevel;

        _mainMenuButton = root.Q<Button>(mainMenuButtonName);
        if (_mainMenuButton == null) return;

        // Ensure single subscription
        _mainMenuButton.clicked -= NavigateToMainMenu;
        _mainMenuButton.clicked += NavigateToMainMenu;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_caught) return;

        if (!other.CompareTag(playerTag)) return;

        HandleCaught();
    }

    private void HandleCaught()
    {
        _caught = true;

        // Disable your movement script(s) here if you have them.
        if (playerController != null) playerController.enabled = false;
        if (farmerController != null) farmerController.enabled = false;

        StartCoroutine(CaughtSequence());
    }

    private IEnumerator CaughtSequence()
    {
        yield return new WaitForSeconds(caughtAnimLeadTime);

        // Play farmer capture animation if configured
        if (!string.IsNullOrEmpty(farmerCaptureAnimation) && farmerAnimator != null)
        {
            farmerAnimator.Play(farmerCaptureAnimation);
        }

        if (_root != null)
        {
            StartCoroutine(FadeElement(_root, 0f, 1f, fadeDuration));
        }
    }

    private IEnumerator FadeElement(VisualElement element, float from, float to, float duration)
    {
        if (element == null)
        {
            yield break;
        }

        duration = Mathf.Max(0.001f, duration);

        element.style.opacity = from;
        element.pickingMode = PickingMode.Ignore;

        // Make children non-interactive while fading (so clicks don't pass through)
        foreach (var child in element.Children())
        {
            child.pickingMode = PickingMode.Ignore;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            element.style.opacity = a;
            // Force repaint to ensure visual update
            element.MarkDirtyRepaint();
            yield return null;
        }

        element.style.opacity = to;
        element.pickingMode = PickingMode.Position;

        // Re-enable interaction for children (buttons etc.)
        foreach (var child in element.Children())
        {
            child.pickingMode = PickingMode.Position;
        }
        element.MarkDirtyRepaint();
        // If we've faded the root fully in, start a background fade to black
        if (to >= 1f)
        {
            // Start background fade but do not modify child elements' colors
            StartCoroutine(FadeBackgroundToBlack(element, duration));
        }
    }

    private IEnumerator FadeBackgroundToBlack(VisualElement element, float duration)
    {
        if (element == null)
        {
            yield break;
        }

        duration = Mathf.Max(0.001f, duration);

        // Use the resolved background color as the start so we don't override styles unintentionally
        Color startColor = element.resolvedStyle.backgroundColor;
        Color targetColor = Color.black;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            Color c = Color.Lerp(startColor, targetColor, u);
            element.style.backgroundColor = new StyleColor(c);
            element.MarkDirtyRepaint();
            yield return null;
        }

        element.style.backgroundColor = new StyleColor(targetColor);
        element.MarkDirtyRepaint();
    }

    private void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void NavigateToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
