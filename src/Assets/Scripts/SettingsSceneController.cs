using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;

[RequireComponent(typeof(UIDocument))]
public class SettingsSceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Try to find the UIDocument on this GameObject and hook up the Back button
        if (!TryGetComponent<UIDocument>(out var uiDoc))
        {
            Debug.LogWarning("SettingsSceneController: No UIDocument found on SceneController GameObject.");
            return;
        }

        var root = uiDoc.rootVisualElement;
        Button backButton = root.Q<Button>("Back");

        // If button wasn't found by name, try to locate by displayed text
        if (backButton == null)
        {
            var allButtons = root.Query<Button>().ToList();
            foreach (var b in allButtons)
            {
                if (string.Equals(b.text, "Back", System.StringComparison.OrdinalIgnoreCase))
                {
                    backButton = b;
                    break;
                }
            }
        }

        if (backButton == null)
        {
            Debug.LogWarning("SettingsSceneController: Could not find a Button named or labeled 'Back' in the UIDocument.");
            return;
        }

        backButton.clicked += () =>
        {
            SceneManager.LoadScene("MainMenu");
        };

        // Populate Resolution dropdown if present
        var resolutionDropdown = root.Q<DropdownField>("ResolutionDropDown");
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
}
