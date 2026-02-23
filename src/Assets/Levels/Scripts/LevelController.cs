using UnityEngine;

namespace SuperPorkOut.Levels
{
    public class LevelController : MonoBehaviour
    {
        // Singleton for easy access from other gameplay scripts
        public static LevelController Instance { get; private set; }

        [Header("Targets")]
        [SerializeField] private Transform farmer;
        [SerializeField] private Transform pig;

        [Header("Sky Darkening")]
        [Tooltip("Distance at or below which effects are at full strength.")]
        [SerializeField, Min(0.01f)] private float proximityDistance = 10f;
        [Tooltip("How much to darken ambient intensity at full proximity. 0 = no change, 1 = completely dark ambient.")]
        [SerializeField, Range(0f, 1f)] private float darkeningAmount = 0.5f;

        [Header("Audio Sources")]
        [Tooltip("Farmer footsteps source (gets louder as farmer approaches pig).")]
        [SerializeField] private AudioSource farmerFootsteps;
        [Tooltip("Pig footsteps source (gets quieter as farmer approaches pig).")]
        [SerializeField] private AudioSource pigFootsteps;
        [Tooltip("Background music source on LevelController (gets quieter as farmer approaches pig).")]
        [SerializeField] private AudioSource backgroundMusic;

        [Header("Audio Mix")]
        [Tooltip("Maximum farmer footsteps boost at full proximity.")]
        [SerializeField, Range(0f, 2f)] private float farmerLoudnessBoost = 0.5f;
        [Tooltip("Maximum pig footsteps reduction at full proximity.")]
        [SerializeField, Range(0f, 1f)] private float pigQuietAmount = 0.6f;
        [Tooltip("Maximum background music reduction at full proximity.")]
        [SerializeField, Range(0f, 1f)] private float musicQuietAmount = 0.6f;

        private float baseAmbientIntensity;
        private float baseFarmerVolume;
        private float basePigVolume;
        private float baseMusicVolume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple LevelController instances detected - destroying duplicate.", this);
                Destroy(this);
                return;
            }

            Instance = this;

            if (backgroundMusic == null)
                backgroundMusic = GetComponent<AudioSource>();

            baseAmbientIntensity = RenderSettings.ambientIntensity;
            baseFarmerVolume = farmerFootsteps != null ? farmerFootsteps.volume : 0f;
            basePigVolume = pigFootsteps != null ? pigFootsteps.volume : 0f;
            baseMusicVolume = backgroundMusic != null ? backgroundMusic.volume : 0f;
        }

        private void OnDisable()
        {
            RestoreDefaults();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            RestoreDefaults();
        }

        private void Update()
        {
            if (farmer == null || pig == null)
            {
                ApplyEffects(0f);
                return;
            }

            float distance = Vector3.Distance(farmer.position, pig.position);
            float proximity01 = 1f - Mathf.Clamp01(distance / proximityDistance);
            ApplyEffects(proximity01);
        }

        private void ApplyEffects(float proximity01)
        {
            float ambientMultiplier = 1f - (darkeningAmount * proximity01);
            RenderSettings.ambientIntensity = baseAmbientIntensity * ambientMultiplier;

            if (farmerFootsteps != null)
            {
                float farmerMultiplier = 1f + (farmerLoudnessBoost * proximity01);
                farmerFootsteps.volume = baseFarmerVolume * farmerMultiplier;
            }

            if (pigFootsteps != null)
            {
                float pigMultiplier = 1f - (pigQuietAmount * proximity01);
                pigFootsteps.volume = basePigVolume * Mathf.Max(0f, pigMultiplier);
            }

            if (backgroundMusic != null)
            {
                float musicMultiplier = 1f - (musicQuietAmount * proximity01);
                backgroundMusic.volume = baseMusicVolume * Mathf.Max(0f, musicMultiplier);
            }
        }

        private void RestoreDefaults()
        {
            RenderSettings.ambientIntensity = baseAmbientIntensity;

            if (farmerFootsteps != null)
                farmerFootsteps.volume = baseFarmerVolume;

            if (pigFootsteps != null)
                pigFootsteps.volume = basePigVolume;

            if (backgroundMusic != null)
                backgroundMusic.volume = baseMusicVolume;
        }
    }
}
