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
        [SerializeField, Min(0f)] private float maxStamina = 100f;
        [SerializeField, Min(0f)] private float currentStamina = 100f;

        [Header("Drain")]
        [SerializeField, Min(0f)] private float drainRatePerSecond = 10f;
        internal float CurrentStamina;

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
    }
}
