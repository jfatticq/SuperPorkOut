using UnityEngine;

namespace SuperPorkOut.Levels
{
    [RequireComponent(typeof(Collider))]
    public class EndLevelTrigger : MonoBehaviour
    {
        [SerializeField] private GameStateBus gameStateBus;
        [SerializeField] private string pigTag = "Player";
        [SerializeField] private GameObject farmer; // assign in inspector if you want; optional
        [SerializeField] private string reason = "EndLevelTrigger";

        [Header("Optional Audio")]
        [SerializeField] private AudioClip endLevelSfx;
        [SerializeField] private float endLevelSfxVolume = 1f;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Awake()
        {
            if (gameStateBus == null)
                gameStateBus = FindFirstObjectByType<GameStateBus>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (gameStateBus == null || gameStateBus.IsGameOver) return;
            if (!other.CompareTag(pigTag)) return;

            if (endLevelSfx != null)
                AudioSource.PlayClipAtPoint(endLevelSfx, transform.position, Mathf.Clamp01(endLevelSfxVolume));

            // Farmer can be resolved later by coordinator if you prefer.
            GameObject farmerObj = farmer != null ? farmer : FindFarmerFallback();

            Vector3 point = other.ClosestPoint(transform.position);
            gameStateBus.RaiseLevelEnded(new LevelEndedEvent(other.gameObject, farmerObj, point, reason));
        }

        private GameObject FindFarmerFallback()
        {
            // If you tag farmer as "Farmer", use that instead.
            var farmerGo = GameObject.FindWithTag("Farmer");
            return farmerGo;
        }
    }
}
