using SuperPorkOut.Gameplay.Pickups;
using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    /// <summary>
    /// Simple stamina resource.
    /// - Drains over time
    /// - Pickups add stamina
    /// </summary>
    public class Stamina : MonoBehaviour
    {
        [Header("Capacity")]
        [Tooltip("Maximum stamina. Current stamina will be clamped to this value.")]
        [SerializeField, Min(0f)] private float maxStamina = 100f;

        [Tooltip("Current stamina. Will be clamped to the max stamina value.")]
        [SerializeField, Min(0f)] private float currentStamina = 100f;

        [Header("Drain")]
        [Tooltip("How much stamina is drained per second. Set to 0 to disable draining.")]
        [SerializeField, Min(0f)] private float drainRatePerSecond = 10f;

        public float Max => maxStamina;
        public float Current => currentStamina;

        public float MaxStamina { get; internal set; }

        /// <summary>0..1 percent full.</summary>
        public float Normalized()
        {
            if (maxStamina <= 0f) return 0f;
            return Mathf.Clamp01(currentStamina / maxStamina);
        }

        private void OnValidate()
        {
            maxStamina = Mathf.Max(0f, maxStamina);
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            drainRatePerSecond = Mathf.Max(0f, drainRatePerSecond);
        }

        private void Update()
        {
            if (drainRatePerSecond <= 0f || maxStamina <= 0f) return;
            currentStamina = Mathf.Max(0f, currentStamina - drainRatePerSecond * Time.deltaTime);
        }

        public void Add(float amount)
        {
            if (amount <= 0f) return;
            currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        }

        public void Set(float value)
        {
            currentStamina = Mathf.Clamp(value, 0f, maxStamina);
        }

        private void OnEnable()
        {
            PowerUp.PickedUp += OnPowerUpPickedUp;
        }

        private void OnDisable()
        {
            PowerUp.PickedUp -= OnPowerUpPickedUp;
        }

        private void OnPowerUpPickedUp(PickupEventData data)
        {
            if (data.StaminaAmount <= 0f) return;
            currentStamina = Mathf.Min(maxStamina, currentStamina + data.StaminaAmount);
        }
    }
}
