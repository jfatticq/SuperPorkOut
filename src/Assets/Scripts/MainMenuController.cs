using System.Collections;
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

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playButton = root.Q<Button>("StartButton");
        if (playButton == null)
        {
            Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'StartButton' in the UIDocument.");
            return;
        }

        guideButton = root.Q<Button>("GuideButton");
        if (guideButton == null)
        {
            Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'GuideButton' in the UIDocument.");
            return;
        }

        settingsButton = root.Q<Button>("SettingsButton");
        if (settingsButton == null)
        {
            Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'SettingsButton' in the UIDocument.");
            return;
        }

        quitButton = root.Q<Button>("QuitButton");
        if (quitButton == null)
        {
            Debug.LogWarning("MainMenuController: Could not find a Button named or labeled 'QuitButton' in the UIDocument.");
            return;
        }

        playButton.clicked += OnPlayClicked;
        guideButton.clicked += OnGuideClicked;
        settingsButton.clicked += OnSettingsClicked;
        quitButton.clicked += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        StartCoroutine(StartLevel());
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

    IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Level01");
    }
}
