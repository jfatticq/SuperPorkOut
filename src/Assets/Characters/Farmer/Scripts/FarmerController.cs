using UnityEngine;

namespace SuperPorkOut.Characters.Farmer
{
    /// Farmer continuously moves toward the player at its configured chase speed.
    public class FarmerController : MonoBehaviour
    {
        [Header("Chase Motion")]
        [SerializeField, Min(0f)] private float farmerForwardSpeed = 3f;

        [Tooltip("Multiplier over elapsed game time (x-axis in seconds) for chase speed.\nExample: 0s=1, 60s=1.5, 120s=2")]
        [SerializeField] private AnimationCurve forwardSpeedMultiplierOverTime = new(
            new Keyframe(0f, 1f),
            new Keyframe(60f, 1.5f),
            new Keyframe(120f, 2f)
        );

        [Header("Target")]
        [Tooltip("Tag used to find the pig/player transform to chase.")]
        [SerializeField] private string playerTag = "Player";

        private Transform playerTransform;

        private float elapsedGameTime;

        private void OnEnable()
        {
            elapsedGameTime = 0f;
            CachePlayerTransform();
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                CachePlayerTransform();
            
                if (playerTransform == null) return;
            }

            elapsedGameTime += Time.deltaTime;

            float currentChaseSpeed = farmerForwardSpeed * EvaluateForwardSpeedMultiplier(elapsedGameTime);
            float step = currentChaseSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);

            Vector3 lookDirection = playerTransform.position - transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        private float EvaluateForwardSpeedMultiplier(float time)
        {
            if (forwardSpeedMultiplierOverTime == null || forwardSpeedMultiplierOverTime.length == 0)
            {
                return 1f;
            }

            Keyframe[] keys = forwardSpeedMultiplierOverTime.keys;
            float clampedTime = Mathf.Clamp(time, keys[0].time, keys[keys.Length - 1].time);
            return Mathf.Max(0f, forwardSpeedMultiplierOverTime.Evaluate(clampedTime));
        }

        private void CachePlayerTransform()
        {
            if (string.IsNullOrWhiteSpace(playerTag)) return;

            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            playerTransform = playerObject != null ? playerObject.transform : null;
        }
    }
}
