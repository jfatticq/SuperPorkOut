using UnityEngine;

/// <summary>
/// Put this on mud/water zones.
/// Requirements:
/// - Zone collider MUST be trigger.
/// Behavior:
/// - On enter: tells PlayerController to apply slow multipliers
/// - On exit: tells PlayerController to remove them
/// </summary>
[RequireComponent(typeof(Collider))]
public class SlowZoneTrigger : MonoBehaviour
{
    [Header("Speed Multipliers While Inside (0..1)")]
    [Range(0f, 1f)]
    [SerializeField] private float forwardMultiplier = 0.6f;

    [Range(0f, 1f)]
    [SerializeField] private float strafeMultiplier = 0.7f;

    // Unique ID per instance so the player can track overlapping zones cleanly
    private int zoneId;

    private void Awake()
    {
        zoneId = GetInstanceID();

        Collider c = GetComponent<Collider>();
        if (c.isTrigger == false)
        {
            Debug.LogWarning("SlowZoneTrigger requires the collider to be marked as Trigger.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        player.OnEnterSlowZone(zoneId, forwardMultiplier, strafeMultiplier);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        player.OnExitSlowZone(zoneId);
    }
}
