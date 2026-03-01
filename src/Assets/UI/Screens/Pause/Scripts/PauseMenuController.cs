using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    private UIDocument pauseMenuDocument;

    [SerializeField] private PauseListener pauseListener;

    private void OnEnable()
    {
        if (pauseListener == null)
        {
            Debug.LogError($"{nameof(PauseMenuController)} requires a {nameof(PauseListener)} reference.");
            return;
        }

        pauseMenuDocument = GetComponent<UIDocument>();
        var root = pauseMenuDocument.rootVisualElement;

        if (root == null) return;

        var resumeButton = root.Q<Button>("ResumeButton");
        if (resumeButton != null)
        {
            resumeButton.clicked += pauseListener.Resume;
        }

        var restartButton = root.Q<Button>("RestartButton");
        if (restartButton != null)
        {
            restartButton.clicked += pauseListener.RestartLevel;
        }

        var mainMenuButton = root.Q<Button>("MainMenuButton");
        if (mainMenuButton != null)
        {
            mainMenuButton.clicked += pauseListener.QuitToMainMenu;
        }
    }

    private void OnDisable()
    {
        if (pauseMenuDocument == null || pauseListener == null) return;

        var root = pauseMenuDocument.rootVisualElement;

        if (root == null) return;

        var resumeButton = root.Q<Button>("ResumeButton");
        if (resumeButton != null)
        {
            resumeButton.clicked -= pauseListener.Resume;
        }

        var restartButton = root.Q<Button>("RestartButton");
        if (restartButton != null)
        {
            restartButton.clicked -= pauseListener.RestartLevel;
        }

        var mainMenuButton = root.Q<Button>("MainMenuButton");
        if (mainMenuButton != null)
        {
            mainMenuButton.clicked -= pauseListener.QuitToMainMenu;
        }
    }
}
