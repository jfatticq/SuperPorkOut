using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument pauseMenuDocument;

    [SerializeField] private PauseListener pauseListener;

    private void OnEnable()
    {
        var root = pauseMenuDocument.rootVisualElement;

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
}
