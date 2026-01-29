using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject player;
    [Range(0f, 1f)]
    [SerializeField] float forwardReduction = 1f; // 1.0 == 100% reduction (default: affect forward speed)

    [Range(0f, 1f)]
    [SerializeField] float horizontalReduction = 0f; // default: don't affect horizontal speed

    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (player == null) return;
        playerController = player.GetComponent<PlayerController>();
        if (playerController == null) return;

        float forwardMultiplier = Mathf.Clamp01(1f - forwardReduction);
        float horizontalMultiplier = Mathf.Clamp01(1f - horizontalReduction);

        playerController.SetForwardMultiplier(forwardMultiplier);
        playerController.SetHorizontalMultiplier(horizontalMultiplier);
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController == null) return;
        playerController.ResetForwardMultiplier();
        playerController.ResetHorizontalMultiplier();
    }
}
