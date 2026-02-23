using UnityEngine;

namespace SuperPorkOut.Levels
{
    /// <summary>
    /// Darkens ambient sky lighting based on farmer-pig proximity.
    /// </summary>
    public class ProximitySkyDarkener : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform farmer;
        [SerializeField] private Transform pig;

        [Header("Proximity")]
        [SerializeField, Min(0.01f)] private float proximityDistance = 10f;
        [Tooltip("Maps normalized proximity (0..1) to darkness intensity (0..1).")]
        [SerializeField] private AnimationCurve proximityToDarknessCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Darkening")]
        [SerializeField, Range(0f, 1f)] private float darkeningAmount = 0.5f;

        private float baseAmbientIntensity;

        private void Awake()
        {
            baseAmbientIntensity = RenderSettings.ambientIntensity;
        }

        private void OnEnable()
        {
            Apply(0f);
        }

        private void Update()
        {
            Apply(GetDarknessAmount01());
        }

        private void OnDisable()
        {
            RenderSettings.ambientIntensity = baseAmbientIntensity;
        }

        private void OnDestroy()
        {
            RenderSettings.ambientIntensity = baseAmbientIntensity;
        }

        private float GetDarknessAmount01()
        {
            if (farmer == null || pig == null)
                return 0f;

            float distance = Vector3.Distance(farmer.position, pig.position);
            float proximity01 = 1f - Mathf.Clamp01(distance / proximityDistance);
            float darknessAmount = proximityToDarknessCurve != null
                ? proximityToDarknessCurve.Evaluate(proximity01)
                : proximity01;

            return Mathf.Clamp01(darknessAmount);
        }

        private void Apply(float darknessAmount01)
        {
            float ambientMultiplier = 1f - (darkeningAmount * darknessAmount01);
            RenderSettings.ambientIntensity = baseAmbientIntensity * ambientMultiplier;
        }
    }
}
