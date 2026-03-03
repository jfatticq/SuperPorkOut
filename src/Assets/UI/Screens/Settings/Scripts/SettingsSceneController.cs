using SuperPorkOut.Core;
using SuperPorkOut.Levels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(SceneFlow))]
public class SettingsSceneController : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private SceneFlow sceneFlow;

    private Button backButton;
    private Button resetGameStateButton; 
    private SliderInt masterVolumeSlider;
    private SliderInt musicVolumeSlider;
    private SliderInt fxVolumeSlider;

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        backButton = root.Q<Button>("BackButton");

        if (backButton == null)
        {
            Debug.LogWarning("SettingsSceneController: Could not find a Button named or labeled 'Back' in the UIDocument.");
            return;
        }

        backButton.clicked += GoBack;

        resetGameStateButton = root.Q<Button>("ResetGameStateButton");
        if (resetGameStateButton != null)
        {
            resetGameStateButton.clicked += ResetGameState;
        }

        // Enable UI input mode
        InputManager.Instance.SetMode(GameMode.Settings);

        // Listen for Cancel (Escape / B)
        InputManager.Instance.Actions.UI.Cancel.performed += OnCancel;

        // Populate Resolution dropdown if present
        var resolutionDropdown = root.Q<DropdownField>("ResolutionDropDown");
        PopulateResolutionDropDown(resolutionDropdown);

        // Populate Fullscreen Mode dropdown if present
        var fullscreenDropdown = root.Q<DropdownField>("FullscreenDropDown");
        PopulateFullScreenDropDown(fullscreenDropdown);

        var audioSettings = AudioSettingsService.Instance;
        if (audioSettings != null)
        {
            masterVolumeSlider = root.Q<SliderInt>("MasterVolumeSlider");
            musicVolumeSlider = root.Q<SliderInt>("MusicVolumeSlider");
            fxVolumeSlider = root.Q<SliderInt>("FXVolumeSlider");

            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.SetValueWithoutNotify(audioSettings.GetMasterVolumePercent());
                masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.SetValueWithoutNotify(audioSettings.GetMusicVolumePercent());
                musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            }

            if (fxVolumeSlider != null)
            {
                fxVolumeSlider.SetValueWithoutNotify(audioSettings.GetFxVolumePercent());
                fxVolumeSlider.RegisterValueChangedCallback(OnFxVolumeChanged);
            }
        }
        else
        {
            Debug.LogWarning("SettingsSceneController: AudioSettingsService.Instance is null. Volume sliders will not be wired.");
        }
    }

    private static void PopulateResolutionDropDown(DropdownField resolutionDropdown)
    {
        if (resolutionDropdown != null)
        {
            var resolutions = Screen.resolutions;
            if (resolutions == null || resolutions.Length == 0)
            {
                Debug.LogWarning("SettingsSceneController: No available screen resolutions found.");
            }
            else
            {
                // Build readable choices (width x height @ refreshHz) and remove duplicates
                var choices = resolutions
                    .Select(r =>
                    {
                        // refreshRate is obsolete; use refreshRateRatio to compute an integer refresh rate
                        var ratio = r.refreshRateRatio;
                        int refreshHz;
                        if (ratio.denominator != 0)
                            refreshHz = Mathf.RoundToInt((float)ratio.numerator / ratio.denominator);
                        else
                            refreshHz = (int)ratio.numerator;
                        return $"{r.width} x {r.height} @ {refreshHz}Hz";
                    })
                    .Distinct()
                    .ToList();

                resolutionDropdown.choices = choices;

                // Try to set the current resolution as the selected value
                var current = Screen.currentResolution;
                var currentRatio = current.refreshRateRatio;
                int currentRefreshHz;
                if (currentRatio.denominator != 0)
                    currentRefreshHz = Mathf.RoundToInt((float)currentRatio.numerator / currentRatio.denominator);
                else
                    currentRefreshHz = (int)currentRatio.numerator;
                var currentString = $"{current.width} x {current.height} @ {currentRefreshHz}Hz";
                if (!choices.Contains(currentString))
                {
                    // Fallback: try to match by width x height only
                    var shortCurrent = $"{current.width} x {current.height}";
                    var match = choices.FirstOrDefault(c => c.StartsWith(shortCurrent));
                    currentString = match ?? choices.First();
                }

                resolutionDropdown.value = currentString;

                // When the user selects a new resolution, apply it (keeps current fullscreen mode)
                resolutionDropdown.RegisterValueChangedCallback(evt =>
                {
                    var parts = evt.newValue.Split('@');
                    if (parts.Length >= 1)
                    {
                        var dims = parts[0].Trim();
                        var wh = dims.Split('x');
                        if (wh.Length >= 2 && int.TryParse(wh[0].Trim(), out int w) && int.TryParse(wh[1].Trim(), out int h))
                        {
                            Screen.SetResolution(w, h, Screen.fullScreenMode);
                        }
                    }
                });
            }
        }
    }

    private static void PopulateFullScreenDropDown(DropdownField fullscreenDropdown)
    {
        if (fullscreenDropdown != null)
        {
            // Friendly labels mapped to FullScreenMode values
            var modeMap = new Dictionary<string, FullScreenMode>
            {
                { "Windowed", FullScreenMode.Windowed },
                { "Fullscreen (Windowed)", FullScreenMode.FullScreenWindow },
                { "Exclusive Fullscreen", FullScreenMode.ExclusiveFullScreen },
                { "Maximized Window", FullScreenMode.MaximizedWindow }
            };

            var modeChoices = modeMap.Keys.ToList();
            fullscreenDropdown.choices = modeChoices;

            // Set current value
            var currentModeLabel = modeMap.FirstOrDefault(kv => kv.Value == Screen.fullScreenMode).Key ?? modeChoices.First();
            fullscreenDropdown.value = currentModeLabel;

            // Apply when changed
            fullscreenDropdown.RegisterValueChangedCallback(evt =>
            {
                if (modeMap.TryGetValue(evt.newValue, out var mode))
                {
                    Screen.fullScreenMode = mode;
                }
            });
        }
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.clicked -= GoBack;

        if (resetGameStateButton != null)
            resetGameStateButton.clicked -= ResetGameState;

        if (InputManager.Instance != null)
            InputManager.Instance.Actions.UI.Cancel.performed -= OnCancel;

        if (masterVolumeSlider != null)
            masterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);

        if (fxVolumeSlider != null)
            fxVolumeSlider.UnregisterValueChangedCallback(OnFxVolumeChanged);
    }

    private void OnCancel(InputAction.CallbackContext _)
    {
        GoBack();
    }

    private void GoBack()
    {
        if (sceneFlow == null)
        {
            Debug.LogError("[SettingsSceneController] SceneFlow not assigned.");
            return;
        }
        sceneFlow.GoToMainMenu();
    }

    private static void ResetGameState()
    {
        GameState.IsTutorialCompleted = false;
    }

    private void OnMasterVolumeChanged(ChangeEvent<int> evt)
    {
        var audioSvc = AudioSettingsService.Instance;
        if (audioSvc != null)
            audioSvc.SetMasterVolumePercent(evt.newValue);
    }

    private void OnMusicVolumeChanged(ChangeEvent<int> evt)
    {
        var audioSvc = AudioSettingsService.Instance;
        if (audioSvc != null)
            audioSvc.SetMusicVolumePercent(evt.newValue);
    }

    private void OnFxVolumeChanged(ChangeEvent<int> evt)
    {
        var audioSvc = AudioSettingsService.Instance;
        if (audioSvc != null)
            audioSvc.SetFxVolumePercent(evt.newValue);
    }
}
