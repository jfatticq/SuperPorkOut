using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Speed Boost")]
    public float boostMultiplier = 1.5f;
    public float boostDuration = 3f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 20f;

    private bool used = false;
    private float currentStamina;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.TryGetComponent(out PlayerController controller))
        {
            used = true;
            StartCoroutine(Boost(controller));
        }
    }

    private IEnumerator Boost(PlayerController controller)
    {
        // Use the PlayerController's multiplier API instead of a non-existent MoveSpeed property.
        // Clamp the multiplier to the expected 0..1 range used by PlayerController.
        controller.SetForwardMultiplier(Mathf.Clamp01(boostMultiplier));

        float timer = 0f;

        while (timer < boostDuration && currentStamina > 0f)
        {
            currentStamina -= staminaDrainPerSecond * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        controller.ResetForwardMultiplier();

        Destroy(gameObject);
    }
}
