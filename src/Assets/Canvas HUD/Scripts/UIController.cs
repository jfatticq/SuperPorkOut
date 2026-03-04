using TMPro;
using UnityEngine;
using SuperPorkOut.Core;

public class UIController : MonoBehaviour
{
    [SerializeField] private RunStatsRecorder runstatsrecorder;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI veggieText;

    void Update()
    {
        if (runstatsrecorder == null) return;

        distanceText.text = $"Distance: {runstatsrecorder.Distancecount:F1}";
        veggieText.text = $"Veggies: {runstatsrecorder.Veggiecount}";
    }
}
