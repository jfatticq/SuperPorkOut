using UnityEngine;
using StarterAssets;
using System.Collections;

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

        if (other.TryGetComponent(out ThirdPersonController controller))
        {
            used = true;
            StartCoroutine(Boost(controller));
        }
    }

    private IEnumerator Boost(ThirdPersonController controller)
    {
        float originalMoveSpeed = controller.MoveSpeed;
        float originalSprintSpeed = controller.SprintSpeed;

        controller.MoveSpeed *= boostMultiplier;
        controller.SprintSpeed *= boostMultiplier;

        float timer = 0f;

        while (timer < boostDuration && currentStamina > 0f)
        {
            currentStamina -= staminaDrainPerSecond * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        controller.MoveSpeed = originalMoveSpeed;
        controller.SprintSpeed = originalSprintSpeed;

        Destroy(gameObject);
    }
}

