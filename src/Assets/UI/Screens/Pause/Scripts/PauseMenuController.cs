using SuperPorkOut.Core;
using SuperPorkOut.Gameplay.Pickups;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    private UIDocument pauseMenuDocument;

    [SerializeField] private PauseListener pauseListener;
    [SerializeField] private RunStatsRecorder runStatsRecorder;

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

        var statsLabel = root.Q<Label>("StatsLabel");
        if (statsLabel != null && runStatsRecorder != null)
        {
            statsLabel.text = FormatCurrentRunStats();
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

    private string FormatCurrentRunStats()
    {
        float dist = runStatsRecorder.DistanceTraveled;
        float time = runStatsRecorder.ElapsedSeconds;
        var pickups = runStatsRecorder.PickupCounts;

        int mins = Mathf.FloorToInt(time) / 60;
        int secs = Mathf.FloorToInt(time) % 60;

        return $"Distance: {dist:0.0}m\n"
             + $"Time: {mins:00}:{secs:00}\n"
             + $"Carrots: {pickups[FoodType.Carrot]}  "
             + $"Cabbages: {pickups[FoodType.Cabbage]}  "
             + $"Tomatoes: {pickups[FoodType.Tomato]}";
    }
}
