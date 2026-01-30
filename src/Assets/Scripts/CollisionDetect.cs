using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Optional: assign the player's PlayerController here. If left empty the script will try to find the player automatically.")]
    [SerializeField] PlayerController playerOverride;

    [Tooltip("Reduction applied to the player's forward speed upon collision.")]
    [Range(0f, 1f)]
    [SerializeField] float forwardReduction = 1f; // 1.0 == 100% reduction (default: affect forward speed)

    [Tooltip("Reduction applied to the player's horizontal speed upon collision.")]
    [Range(0f, 1f)]
    [SerializeField] float horizontalReduction = 0f; // default: don't affect horizontal speed

    [Header("Sound Effects")]
    [Tooltip("Optional: assign an AudioSource here to play a sound effect on collision.")]
    [SerializeField] AudioSource collisionFX;

    [Header("Camera Animation")]
    [Tooltip("Optional: assign the camera's Animator here. If left empty the script will try to find the main camera (child of player or Camera.main) and use its Animator.")]
    [SerializeField] Animator cameraAnimatorOverride;

    [Tooltip("Name of the camera collision animation to play on impact.")]
    [SerializeField] string cameraCollisionAnimationName = "CollisionCam";

    private PlayerController playerController;
    private Animator cameraAnimator;

    private void Awake()
    {
        if (playerOverride != null)
        {
            playerController = playerOverride;
            return;
        }

        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("CollisionDetect: No PlayerController found in scene. Assign a PlayerController in the inspector or ensure one exists at runtime.", this);
        }

        // Resolve camera animator: use override if provided, otherwise try to find the main camera's Animator
        if (cameraAnimatorOverride != null)
        {
            cameraAnimator = cameraAnimatorOverride;
        }
        else
        {
            // Try to find camera as a child of the player
            if (playerController != null)
            {
                Camera childCam = playerController.GetComponentInChildren<Camera>();
                if (childCam != null)
                {
                    cameraAnimator = childCam.GetComponent<Animator>();
                }
            }

            // Fallback to Camera.main if still not found
            if (cameraAnimator == null && Camera.main != null)
            {
                cameraAnimator = Camera.main.GetComponent<Animator>();
            }

            if (cameraAnimator == null)
            {
                Debug.LogWarning("CollisionDetect: No camera Animator found. Assign one in the inspector if you want to play the 'CollisionCam' animation.", this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collisionFX != null) collisionFX.Play();

        if (playerController == null) return;

        if (cameraAnimator != null)
        {
            cameraAnimator.Play(cameraCollisionAnimationName);
        }

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
