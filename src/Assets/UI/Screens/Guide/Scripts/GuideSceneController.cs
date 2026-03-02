using SuperPorkOut.Levels;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(SceneFlow))]
public class GuideSceneController : MonoBehaviour
{
    [SerializeField] private string TutorialSceneName = "Level01";
    
    private SceneFlow sceneFlow;

    private Button backButton;

    private Button tutorialButton;

    private void OnEnable()
    {
        sceneFlow = GetComponent<SceneFlow>();

        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        backButton = root.Q<Button>("BackButton");

        if (backButton == null)
        {
            Debug.LogWarning("GuideSceneController: Back button not found.");
            return;
        }

        backButton.clicked += GoBack;

        tutorialButton = root.Q<Button>("btnTutorial");

        if (tutorialButton == null)
        {
            Debug.LogWarning("GuideSceneController: Tutorial button not found.");
            return;
        }

        tutorialButton.clicked += GoToTutorial;

        // Enable UI input mode
        InputManager.Instance.SetMode(GameMode.Guide);

        // Listen for Cancel (Escape / B)
        InputManager.Instance.Actions.UI.Cancel.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.clicked -= GoBack;

        if (tutorialButton != null)
            tutorialButton.clicked -= GoToTutorial;

        if (InputManager.Instance != null)
            InputManager.Instance.Actions.UI.Cancel.performed -= OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext _)
    {
        GoBack();
    }

    private void GoBack()
    {
        if (sceneFlow == null)
        {
            Debug.LogError("[GuideSceneController] SceneFlow not assigned.");
            return;
        }
        sceneFlow.GoToMainMenu();
    }

    private void GoToTutorial()
    {
        SceneManager.LoadScene(TutorialSceneName);
    }
}
