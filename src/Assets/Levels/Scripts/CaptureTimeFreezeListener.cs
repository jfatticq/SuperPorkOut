using UnityEngine;

namespace SuperPorkOut.Levels
{
    public class CaptureTimeFreezeListener : MonoBehaviour
    {
        [SerializeField] private GameStateBus gameStateBus;
        [SerializeField] private bool setTimeScaleZeroOnCapture = true;

        private void Awake()
        {
            if (gameStateBus == null)
                gameStateBus = FindFirstObjectByType<GameStateBus>();
        }

        private void OnEnable()
        {
            if (gameStateBus != null)
                gameStateBus.Captured += OnCaptured;
        }

        private void OnDisable()
        {
            if (gameStateBus != null)
                gameStateBus.Captured -= OnCaptured;
        }

        private void OnCaptured(CapturedEvent evt)
        {
            if (setTimeScaleZeroOnCapture)
                Time.timeScale = 0f;
        }
    }
}
