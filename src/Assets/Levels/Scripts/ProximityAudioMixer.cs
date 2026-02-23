using UnityEngine;

namespace SuperPorkOut.Levels
{
    /// <summary>
    /// Mixes footsteps and music levels based on farmer-pig proximity.
    /// </summary>
    public class ProximityAudioMixer : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform farmer;
        [SerializeField] private Transform pig;

        [Header("Proximity")]
        [SerializeField, Min(0.01f)] private float proximityDistance = 10f;
        [Tooltip("Maps normalized proximity (0..1) to effect intensity (0..1).")]
        [SerializeField] private AnimationCurve proximityToEffectCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Audio Sources")]
        [SerializeField] private AudioSource farmerFootsteps;
        [SerializeField] private AudioSource pigFootsteps;
        [SerializeField] private AudioSource backgroundMusic;

        [Header("Mix Amounts")]
        [SerializeField, Range(0f, 2f)] private float farmerLoudnessBoost = 0.5f;
        [SerializeField, Range(0f, 1f)] private float pigQuietAmount = 0.6f;
        [SerializeField, Range(0f, 1f)] private float musicQuietAmount = 0.6f;

        private float baseFarmerVolume;
        private float basePigVolume;
        private float baseMusicVolume;

        private void Awake()
        {
            if (backgroundMusic == null)
                backgroundMusic = GetComponent<AudioSource>();

            baseFarmerVolume = farmerFootsteps != null ? farmerFootsteps.volume : 0f;
            basePigVolume = pigFootsteps != null ? pigFootsteps.volume : 0f;
            baseMusicVolume = backgroundMusic != null ? backgroundMusic.volume : 0f;
        }

        private void OnEnable()
        {
            Apply(0f);
        }

        private void Update()
        {
            Apply(GetEffectAmount01());
        }

        private void OnDisable()
        {
            RestoreDefaults();
        }

        private void OnDestroy()
        {
            RestoreDefaults();
        }

        private float GetEffectAmount01()
        {
            if (farmer == null || pig == null)
                return 0f;

            float distance = Vector3.Distance(farmer.position, pig.position);
            float proximity01 = 1f - Mathf.Clamp01(distance / proximityDistance);
            float effectAmount = proximityToEffectCurve != null
                ? proximityToEffectCurve.Evaluate(proximity01)
                : proximity01;

            return Mathf.Clamp01(effectAmount);
        }

        private void Apply(float effectAmount01)
        {
            if (farmerFootsteps != null)
            {
                float farmerMultiplier = 1f + (farmerLoudnessBoost * effectAmount01);
                farmerFootsteps.volume = baseFarmerVolume * farmerMultiplier;
            }

            if (pigFootsteps != null)
            {
                float pigMultiplier = 1f - (pigQuietAmount * effectAmount01);
                pigFootsteps.volume = basePigVolume * Mathf.Max(0f, pigMultiplier);
            }

            if (backgroundMusic != null)
            {
                float musicMultiplier = 1f - (musicQuietAmount * effectAmount01);
                backgroundMusic.volume = baseMusicVolume * Mathf.Max(0f, musicMultiplier);
            }
        }

        private void RestoreDefaults()
        {
            if (farmerFootsteps != null)
                farmerFootsteps.volume = baseFarmerVolume;

            if (pigFootsteps != null)
                pigFootsteps.volume = basePigVolume;

            if (backgroundMusic != null)
                backgroundMusic.volume = baseMusicVolume;
        }
    }
}
