using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseListener : MonoBehaviour
{
    [SerializeField] private UIDocument pauseMenuDocument;

    [SerializeField] private string MainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("LevelPauseManager: InputManager.Instance is null. Ensure GameRoot is loaded.");
            return;
        }

        // We’re in a gameplay level, so set the mode
        InputManager.Instance.SetMode(GameMode.Playing);

        InputManager.Instance.PausePressed += TogglePause;

        // Ensure pause UI starts hidden
        SetPauseUIVisible(false);
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.PausePressed -= TogglePause;

        // Safety: if the scene unloads while paused, restore time
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    private void TogglePause()
    {
        // Double safety gate
        if (InputManager.Instance.Mode != GameMode.Playing && InputManager.Instance.Mode != GameMode.Paused)
            return;

        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        SetPauseUIVisible(isPaused);

        InputManager.Instance.SetMode(isPaused ? GameMode.Paused : GameMode.Playing);

        if (isPaused)
            FocusFirstPauseButton();
    }

    private void SetPauseUIVisible(bool visible)
    {
        if (pauseMenuDocument != null)
            pauseMenuDocument.gameObject.SetActive(visible);
    }

    private void FocusFirstPauseButton()
    {
        // Optional, but makes controller/keyboard feel right in UI Toolkit
        if (pauseMenuDocument == null) return;

        var root = pauseMenuDocument.rootVisualElement;
        var firstButton = root.Q<Button>(); // grabs first Button it finds
        firstButton?.Focus();
    }

    /// <summary>
    /// Resumes gameplay after a pause, restoring normal time progression and input handling.
    /// </summary>
    /// <remarks>Call this method to exit a paused state and allow the game to continue. If the game is not
    /// currently paused, this method has no effect. After resuming, input mode is set to gameplay and any pause-related
    /// UI is hidden.</remarks>
    public void Resume()
    {
        if (!isPaused) return;

        TogglePause();
    }

    public void RestartLevel()
    {
        // Always restore time before reloading
        Time.timeScale = 1f;

        // Reset input mode so gameplay controls are active on load
        InputManager.Instance.SetMode(GameMode.Playing);

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
