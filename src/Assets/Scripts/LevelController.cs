using Characters.Player;
using SuperPorkOut.Characters.Farmer;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    // Singleton for easy access from other gameplay scripts
    public static LevelController Instance { get; private set; }

    [Header("References")]
    [SerializeField] GameObject distanceToFarmerDisplay;
    [SerializeField] PlayerController playerController;
    [SerializeField] FarmerController farmerController;

    // Tracking fields
    private float totalPlayTimeSeconds = 0f;
    private float distanceSum = 0f;
    private int distanceSamples = 0;
    private int pickupsCollected = 0;

    // Cached text component
    private TMPro.TMP_Text _textDisplay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple LevelController instances detected - destroying duplicate.", this);
            Destroy(this);
            return;
        }

        Instance = this;

        if (distanceToFarmerDisplay != null)
        {
            _textDisplay = distanceToFarmerDisplay.GetComponent<TMPro.TMP_Text>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        // track total play time while scene is loaded
        totalPlayTimeSeconds += Time.deltaTime;

        // track distance between player and farmer
        if (playerController != null && farmerController != null)
        {
            float dist = Vector3.Distance(playerController.transform.position, farmerController.transform.position);
            distanceSum += dist;
            distanceSamples++;
        }

        // update display if available
        if (_textDisplay != null)
        {
            float avgDistance = distanceSamples > 0 ? distanceSum / distanceSamples : 0f;
            _textDisplay.text = string.Format("Time: {0:00}:{1:00}\nAvg Dist: {2:0.00}\nPickups: {3}",
                Mathf.FloorToInt(totalPlayTimeSeconds / 60f),
                Mathf.FloorToInt(totalPlayTimeSeconds % 60f),
                avgDistance,
                pickupsCollected);
        }
    }

    // Public API for other scripts to report a pickup collected
    public void RegisterPickupCollected()
    {
        pickupsCollected++;
    }

    // Expose read-only properties if needed elsewhere
    public float TotalPlayTimeSeconds => totalPlayTimeSeconds;

    public float AverageDistance => distanceSamples > 0 ? distanceSum / distanceSamples : 0f;

    public int PickupsCollected => pickupsCollected;
}
