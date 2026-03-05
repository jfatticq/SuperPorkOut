using SuperPorkOut.Core;
using SuperPorkOut.Gameplay.Pickups;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private RunStatsRecorder runstatsrecorder;

    [Header("UI References")]
    [SerializeField] private TMPro.TextMeshProUGUI distanceText;
    [SerializeField] private TMPro.TextMeshProUGUI veggieText;

    void Update()
    {
        if (runstatsrecorder == null) return;

        distanceText.text = $"{runstatsrecorder.DistanceTraveled:0.0}";

        int totalPickups = 
            runstatsrecorder.PickupCounts[FoodType.Carrot] + 
            runstatsrecorder.PickupCounts[FoodType.Tomato] + 
            runstatsrecorder.PickupCounts[FoodType.Cabbage] + 
            runstatsrecorder.PickupCounts[FoodType.Other];
        veggieText.text = $"{totalPickups}";
    }
}
