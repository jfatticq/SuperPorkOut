using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    private UIDocument pauseMenuDocument;

    [SerializeField] private PauseListener pauseListener;

    private void OnEnable()
    {
        // get the UIDocument from this game object
        pauseMenuDocument = GetComponent<UIDocument>();
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
