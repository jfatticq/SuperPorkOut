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

        // Display current level statistics in the pause menu if a Label exists
        var statsLabel = root.Q<Label>("StatsLabel");
        if (statsLabel != null)
        {
            var lc = LevelController.Instance;
            if (lc != null)
            {
                float total = lc.TotalPlayTimeSeconds;
                int minutes = Mathf.FloorToInt(total / 60f);
                int seconds = Mathf.FloorToInt(total % 60f);
                float avg = lc.AverageDistance;
                int pickups = lc.PickupsCollected;

                statsLabel.text = string.Format("Time: {0:00}:{1:00}\nAvg Dist: {2:0.00}\nPickups: {3}",
                    minutes, seconds, avg, pickups);
            }
            else
            {
                statsLabel.text = string.Empty;
            }
        }
    }
}
