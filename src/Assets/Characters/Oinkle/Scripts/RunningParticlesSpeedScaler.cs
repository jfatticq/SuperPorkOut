using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    /// <summary>
    /// Scales Oinkle's running particles based on current planar movement speed.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RunningParticlesSpeedScaler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ParticleSystem runningParticles;
        [SerializeField] private Rigidbody playerRigidbody;

        [Header("Speed Mapping")]
        [Tooltip("Planar speed that should produce the maximum particle size.")]
        [SerializeField, Min(0.01f)] private float topSpeed = 10f;

        [Tooltip("Particle size when nearly stationary.")]
        [SerializeField, Min(0f)] private float minParticleSize = 0.2f;

        [Tooltip("Particle size at or above top speed.")]
        [SerializeField, Min(0f)] private float maxParticleSize = 0.8f;

        [Header("Responsiveness")]
        [Tooltip("How quickly particle size reacts to speed changes.")]
        [SerializeField, Min(0f)] private float sizeLerpSpeed = 10f;

        private float currentSize;

        private void Awake()
        {
            if (playerRigidbody == null)
                playerRigidbody = GetComponent<Rigidbody>();

            if (runningParticles == null)
            {
                Transform runningParticlesTransform = transform.Find("RunningParticles");
                if (runningParticlesTransform != null)
                    runningParticles = runningParticlesTransform.GetComponent<ParticleSystem>();
            }

            currentSize = minParticleSize;
            ApplyParticleSize(currentSize);
        }

        private void Update()
        {
            if (runningParticles == null || playerRigidbody == null)
                return;

            Vector3 planarVelocity = playerRigidbody.linearVelocity;
            planarVelocity.y = 0f;
            float speedRatio = Mathf.Clamp01(planarVelocity.magnitude / topSpeed);

            float targetSize = Mathf.Lerp(minParticleSize, maxParticleSize, speedRatio);
            float lerpT = 1f - Mathf.Exp(-sizeLerpSpeed * Time.deltaTime);
            currentSize = Mathf.Lerp(currentSize, targetSize, lerpT);

            ApplyParticleSize(currentSize);
        }

        private void ApplyParticleSize(float size)
        {
            var main = runningParticles.main;
            main.startSizeMultiplier = size;
        }
    }
}
