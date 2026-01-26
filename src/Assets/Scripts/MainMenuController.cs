using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private Button playButton;
    private Button quitButton;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playButton = root.Q<Button>("StartButton");
        quitButton = root.Q<Button>("QuitButton");

        playButton.clicked += OnPlayClicked;
        quitButton.clicked += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("Level01");
    }

    private void OnQuitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
