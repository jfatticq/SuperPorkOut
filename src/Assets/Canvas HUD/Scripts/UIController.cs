using TMPro;
using UnityEngine;
using SuperPorkOut.Core;
using SuperPorkOut.Gameplay.Pickups;

public class UIController : MonoBehaviour
{
    [SerializeField] private RunStatsRecorder runstatsrecorder;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI veggieText;

    void Update()
    {
        if (runstatsrecorder == null) return;

        distanceText.text = $" {runstatsrecorder.DistanceTraveled:0.0}";
        veggieText.text = $" {runstatsrecorder.PickupCounts[FoodType.Carrot] + runstatsrecorder.PickupCounts[FoodType.Tomato] + runstatsrecorder.PickupCounts[FoodType.Cabbage] + runstatsrecorder.PickupCounts[FoodType.Other]}";
    }
}
