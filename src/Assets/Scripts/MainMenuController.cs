using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private Button playButton;
    private Button guideButton;
    private Button settingsButton;
    private Button quitButton;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playButton = root.Q<Button>("StartButton");
        guideButton = root.Q<Button>("GuideButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        playButton.clicked += OnPlayClicked;
        guideButton.clicked += OnGuideClicked;
        settingsButton.clicked += OnSettingsClicked;
        quitButton.clicked += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("Level01");
    }

    private void OnGuideClicked()
    {
        SceneManager.LoadScene("Guide");
    }

    private void OnSettingsClicked()
    {
        SceneManager.LoadScene("Settings");
    }

    private void OnQuitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
