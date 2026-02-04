using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUp : MonoBehaviour
{
    [Header("Stamina Gain")]
    [SerializeField] private float staminaAmount = 15f;

    [Header("Optional")]
    [SerializeField] private AudioSource pickupSfx;
    [SerializeField] private bool destroyOnPickup = true;

    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        if (c.isTrigger == false)
        {
            Debug.LogWarning("PowerUp expects the collider to be marked as Trigger.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Pig might be on child collider, so search parent
        Stamina stamina = other.GetComponentInParent<Stamina>();
        if (stamina == null) return;

        if (pickupSfx != null)
        {
            pickupSfx.Play();
        }

        stamina.AddStamina(staminaAmount);

        if (destroyOnPickup)
        {
            // If you play SFX from this object, consider:
            // - audio source on pig instead, OR
            // - detach audio source before destroying
            Destroy(gameObject);
        }
    }
}
