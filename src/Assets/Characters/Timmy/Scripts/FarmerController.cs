using UnityEngine;

namespace SuperPorkOut.Characters.Farmer
{
    /// <summary>
    /// Farmer moves forward at its own speed (so distance to pig can change),
    /// while always staying directly behind the pig laterally (X).
    /// </summary>
    public class FarmerController : MonoBehaviour
    {
        [Header("Forward Motion")]
        [SerializeField, Min(0f)] private float farmerForwardSpeed = 3f;

        [Tooltip("Multiplier over elapsed game time (x-axis in seconds) for forward speed.\nExample: 0s=1, 60s=1.5, 120s=2")]
        [SerializeField] private AnimationCurve forwardSpeedMultiplierOverTime = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(60f, 1.5f),
            new Keyframe(120f, 2f)
        );

        [Header("Lateral Follow")]
        [Tooltip("The pig/player transform to follow.")]
        [SerializeField] private Transform playerTransform;

        public enum LateralFollowMode { Snap, Smooth }

        [SerializeField] private LateralFollowMode followMode = LateralFollowMode.Smooth;

        [Tooltip("How fast farmer matches player's X when in Smooth mode (units/sec).")]
        [SerializeField, Min(0f)] private float lateralFollowSpeed = 25f;

        [Tooltip("Optional clamp on how far farmer can shift in X per frame (prevents huge jumps). Set 0 for no clamp.")]
        [SerializeField, Min(0f)] private float maxLateralStepPerFrame = 0f;

        [SerializeField] private bool neverPassPig = true;
        [SerializeField] private float minBehindDistance = 1.5f;

        private float elapsedGameTime;

        private void OnEnable()
        {
            elapsedGameTime = 0f;
        }

        private void Update()
        {
            elapsedGameTime += Time.deltaTime;

            // Move forward constantly in world Z
            Vector3 pos = transform.position;
            float currentForwardSpeed = farmerForwardSpeed * EvaluateForwardSpeedMultiplier(elapsedGameTime);
            pos.z += currentForwardSpeed * Time.deltaTime;

            // Clamp to never pass pig
            if (neverPassPig && playerTransform != null)
            {
                float maxZ = playerTransform.position.z - minBehindDistance;
                pos.z = Mathf.Min(pos.z, maxZ);
            }

            if (playerTransform != null)
            {
                float targetX = playerTransform.position.x;

                if (followMode == LateralFollowMode.Snap)
                {
                    pos.x = targetX;
                }
                else // Smooth
                {
                    float newX = Mathf.MoveTowards(pos.x, targetX, lateralFollowSpeed * Time.deltaTime);

                    if (maxLateralStepPerFrame > 0f)
                    {
                        float delta = newX - pos.x;
                        delta = Mathf.Clamp(delta, -maxLateralStepPerFrame, maxLateralStepPerFrame);
                        newX = pos.x + delta;
                    }

                    pos.x = newX;
                }
            }

            transform.position = pos;
        }

        // Optional: expose for other scripts
        public void SetPlayer(Transform player)
        {
            playerTransform = player;
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
    }
}
