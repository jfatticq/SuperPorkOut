using System;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;

    [Header("Drain")]
    [Tooltip("Stamina lost per second while running.")]
    [SerializeField] private float drainPerSecond = 8f;

    [Header("Speed Mapping")]
    [Tooltip("When stamina is 0%, speed multiplier is minSpeedMultiplier.")]
    [Range(0f, 1f)]
    [SerializeField] private float minSpeedMultiplier = 0.5f;

    [Tooltip("When stamina is 100%, speed multiplier is 1.0 (full speed).")]
    [Range(0f, 1f)]
    [SerializeField] private float maxSpeedMultiplier = 1.0f;

    [Header("Options")]
    [Tooltip("If false, stamina doesn't drain (useful for menus/testing).")]
    [SerializeField] private bool drains = true;

    public event Action<float, float> StaminaChanged; // (current, max)

    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;

    /// <summary>
    /// 0..1
    /// </summary>
    public float Normalized => maxStamina > 0f ? Mathf.Clamp01(currentStamina / maxStamina) : 0f;

    /// <summary>
    /// Multiplier to apply to movement speed, based on stamina.
    /// </summary>
    public float SpeedMultiplier => Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, Normalized);

    private void Awake()
    {
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private void Update()
    {
        if (!drains) return;

        float before = currentStamina;

        currentStamina -= drainPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (!Mathf.Approximately(before, currentStamina))
        {
            StaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    public void AddStamina(float amount)
    {
        if (amount <= 0f) return;

        float before = currentStamina;

        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);

        if (!Mathf.Approximately(before, currentStamina))
        {
            StaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    public bool IsEmpty => currentStamina <= 0f;

    public void SetDrains(bool shouldDrain)
    {
        drains = shouldDrain;
    }
}
