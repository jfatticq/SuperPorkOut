using SuperPorkOut.Levels;
using UnityEngine;

namespace SuperPorkOut.Characters.Farmer
{
    [RequireComponent(typeof(Collider))]
    public class FarmerCatchDetector : MonoBehaviour
    {
        [SerializeField] private GameStateBus gameStateBus;

        [SerializeField] private string pigTag = "Player";

        [SerializeField] private string reason = "FarmerOverlap";

        private void Reset()
        {
            var c = GetComponent<Collider>();
            c.isTrigger = true;
        }

        private void Awake()
        {
            if (gameStateBus == null)
                gameStateBus = FindFirstObjectByType<GameStateBus>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (gameStateBus == null || gameStateBus.IsCaptured) return;

            if (!other.CompareTag(pigTag)) return;

            Vector3 point = other.ClosestPoint(transform.position);
            gameStateBus.RaiseCaptured(new CapturedEvent(other.gameObject, gameObject, point, reason));
        }
    }
}
