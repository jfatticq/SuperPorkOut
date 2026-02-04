using UnityEngine;

/// <summary>
/// Put this on blocking obstacles (trees/rocks).
/// Requirements:
/// - Obstacle collider must be NOT trigger (so it blocks).
/// - Player must have a Rigidbody.
/// Behavior:
/// - Plays optional audio on collision enter
/// - Calls PlayerController to handle gameplay response (animations, speed logic, etc.)
/// </summary>
public class CollisionDetect : MonoBehaviour
{
    [System.Serializable]
    public struct BlockingImpact
    {
        public Collider obstacleCollider;
        public Vector3 contactPoint;
        public Vector3 contactNormal;
        public Vector3 playerVelocityAtImpact;

        [Header("Optional Multipliers")]
        public bool applySpeedMultipliers;
        [Range(0f, 1f)] public float forwardMultiplier;
        [Range(0f, 1f)] public float strafeMultiplier;
    }

    [Header("Optional: Speed Multipliers While Touching")]
    [SerializeField] private bool applySpeedMultipliers = false;

    [Range(0f, 1f)]
    [SerializeField] private float forwardMultiplier = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float strafeMultiplier = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource collisionFX;

    [Tooltip("If true, will only fire once per continuous collision until the player exits.")]
    [SerializeField] private bool fireOncePerTouch = true;

    private bool hasFiredThisTouch;

    private void Awake()
    {
        if (collisionFX == null)
        {
            Debug.LogWarning("Assign collisionFX in inspector");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHandleCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (fireOncePerTouch) return;
        TryHandleCollision(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerController player = collision.collider.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.OnBlockingObstacleExit();
        }

        hasFiredThisTouch = false;
    }

    /// <summary>
    /// Processes a physics <see cref="Collision"/> that involves this obstacle and,
    /// if the colliding object belongs to the player, forwards a populated
    /// <see cref="BlockingImpact"/> to the player's <c>OnBlockingObstacleImpact</c> handler.
    /// 
    /// Behavior:
    /// - Locates a <see cref="PlayerController"/> on the incoming collider's parent chain.
    /// - If <see cref="fireOncePerTouch"/> is enabled, ignores repeated collisions for the same contact
    ///   until <see cref="OnCollisionExit"/> resets <see cref="hasFiredThisTouch"/>.
    /// - Plays the optional <see cref="collisionFX"/> audio if assigned.
    /// - Uses the first contact point from the collision to build a <see cref="BlockingImpact"/>,
    ///   including the obstacle collider, contact point/normal, and the player's velocity at impact.
    /// - Copies the obstacle's configured speed multiplier settings into the impact struct.
    /// - Calls <c>player.OnBlockingObstacleImpact(impact)</c> and marks the touch as fired.
    /// </summary>
    /// <param name="collision">The <see cref="Collision"/> data supplied by Unity's physics callbacks.</param>
    /// <remarks>
    /// This method returns immediately when no <see cref="PlayerController"/> is found on the colliding object.
    /// The player's velocity at impact is taken from <c>collision.rigidbody.linearVelocity</c> if a rigidbody is present;
    /// otherwise it defaults to <c>Vector3.zero</c>.
    /// </remarks>
    private void TryHandleCollision(Collision collision)
    {
        PlayerController player = collision.collider.GetComponentInParent<PlayerController>();
        if (player == null) return;

        if (fireOncePerTouch && hasFiredThisTouch) return;

        if (collisionFX != null)
        {
            collisionFX.Play();
        }

        ContactPoint cp = collision.GetContact(0);

        // Compute the player's velocity at the moment of impact.
        // "collision.rigidbody" refers to the player's Rigidbody. It can be null when the other
        // collider has no Rigidbody (e.g. a static collider). In that case we default to
        // Vector3.zero so downstream logic receives a safe, well-defined value.
        Vector3 impactVelocity = Vector3.zero;
        if (collision.rigidbody != null)
        {
            impactVelocity = collision.rigidbody.linearVelocity;
        }

        var impact = new BlockingImpact
        {
            obstacleCollider = cp.otherCollider,
            contactPoint = cp.point,
            contactNormal = cp.normal,
            playerVelocityAtImpact = impactVelocity,

            applySpeedMultipliers = applySpeedMultipliers,
            forwardMultiplier = forwardMultiplier,
            strafeMultiplier = strafeMultiplier
        };

        player.OnBlockingObstacleImpact(impact);

        hasFiredThisTouch = true;
    }
}
