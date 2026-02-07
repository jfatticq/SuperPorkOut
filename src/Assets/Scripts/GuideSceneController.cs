using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GuideSceneController : MonoBehaviour
{
    private Button backButton;

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        backButton = root.Q<Button>("BackButton");

        if (backButton == null)
        {
            Debug.LogWarning("GuideSceneController: Back button not found.");
            return;
        }

        backButton.clicked += GoBack;

        // Enable UI input mode
        InputManager.Instance.SetMode(GameMode.Guide);

        // Listen for Cancel (Escape / B)
        InputManager.Instance.Actions.UI.Cancel.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.clicked -= GoBack;

        if (InputManager.Instance != null)
            InputManager.Instance.Actions.UI.Cancel.performed -= OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext _)
    {
        GoBack();
    }

    private void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
