using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rigidbody-driven runner controller.
/// - Constant forward motion
/// - Strafe left/right from input
/// - Slides along blocking obstacles:
///   * head-on hit -> forward component removed (halts forward)
///   * side hit -> sideways-into-wall component removed (keeps forward)
/// - Listens to obstacle impacts + slow zones via events.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Stamina))]
public class PlayerController : MonoBehaviour
{
    [Header("Base Speeds")]
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float strafeSpeed = 6f;
    [SerializeField] private Transform movementBasis;

    [Header("Stamina Tunables")]
    [SerializeField]
    private bool staminaAffectsStrafe = true;

    [Header("World X Clamp")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;

    [Header("Physics")]
    [SerializeField] private float extraDownForce = 10f;

    [Header("Optional Animations")]
    [Tooltip("Optional Animator on the pig (stumble, etc.)")]
    [SerializeField] private Animator pigAnimator;

    [Tooltip("Animator on the camera (child of pig). If empty, will try to find one.")]
    [SerializeField] private Animator cameraAnimator;

    [Tooltip("Pig stumble state name (optional).")]
    [SerializeField] private string pigStumbleStateName = "PigStumble";

    [Tooltip("Camera collision animation state name (optional).")]
    [SerializeField] private string cameraCollisionStateName = "CollisionCam";

    private Rigidbody rb;

    private Stamina stamina;

    private float strafeInput;

    // Multipliers (1 = normal speed)
    private float obstacleForwardMultiplier = 1f;
    private float obstacleStrafeMultiplier = 1f;

    // Slow zones stack: choose the lowest multiplier currently active (strongest slow wins)
    private readonly Dictionary<int, (float fwdMul, float strafeMul)> activeZoneMultipliers = new();

    // Cache contacts to avoid per-frame allocations
    private readonly List<ContactPoint> contacts = new(8);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stamina = GetComponent<Stamina>();
    }

    private void OnEnable()
    {
        // Auto-resolve animators if not assigned
        if (pigAnimator == null)
        {
            pigAnimator = GetComponentInChildren<Animator>();
        }

        if (cameraAnimator == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraAnimator = cam.GetComponent<Animator>();
            }
        }
    }

    private void Update()
    {
        // InputManager owns Actions + map enabling/disabling.
        // We only read if available.
        InputManager im = InputManager.Instance;
        if (im == null || im.Actions == null)
        {
            strafeInput = 0f;
            return;
        }

        strafeInput = Mathf.Clamp(im.Actions.Gameplay.Move.ReadValue<Vector2>().x, -1f, 1f);
    }

    private void FixedUpdate()
    {
        // --- compute multipliers (stamina/slow zones/obstacles) exactly like you already do ---
        float zoneFwdMul = 1f;
        float zoneStrafeMul = 1f;

        if (activeZoneMultipliers.Count > 0)
        {
            foreach (var kvp in activeZoneMultipliers)
            {
                zoneFwdMul = Mathf.Min(zoneFwdMul, kvp.Value.fwdMul);
                zoneStrafeMul = Mathf.Min(zoneStrafeMul, kvp.Value.strafeMul);
            }
        }

        float staminaMul = 1f;
        if (stamina != null) staminaMul = stamina.SpeedMultiplier;

        float finalForwardSpeed = forwardSpeed * staminaMul * obstacleForwardMultiplier * zoneFwdMul;

        float strafeStaminaMul = staminaAffectsStrafe ? staminaMul : 1f;
        float finalStrafeSpeed = strafeSpeed * strafeStaminaMul * obstacleStrafeMultiplier * zoneStrafeMul;

        Vector3 forwardDir = movementBasis != null ? movementBasis.forward : Vector3.forward;
        Vector3 strafeDir = movementBasis != null ? movementBasis.right : Vector3.right;

        // --- desired planar velocity ---
        Vector3 planarVel =
            forwardDir * finalForwardSpeed +
            strafeDir * (strafeInput * finalStrafeSpeed);

        // Preserve Y velocity (gravity, etc.)
        float yVel = rb.linearVelocity.y;

        // Move to next position (this is the important part)
        Vector3 nextPos = rb.position + planarVel * Time.fixedDeltaTime;
        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);

        rb.MovePosition(nextPos);

        // Keep Y velocity
        rb.linearVelocity = new Vector3(0f, yVel, 0f) + new Vector3(0f, 0f, 0f);

        // Optional: extra down force
        if (extraDownForce > 0f)
        {
            rb.AddForce(Vector3.down * extraDownForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Core "slide along wall" behavior.
    /// Removes any component of velocity pointing into obstacle contact normals.
    /// This produces:
    /// - Head-on: kills forward component
    /// - Side: kills sideways-into-wall, keeps forward
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount <= 0) return;

        Vector3 v = rb.linearVelocity;

        contacts.Clear();
        collision.GetContacts(contacts);

        for (int i = 0; i < contacts.Count; i++)
        {
            Vector3 n = contacts[i].normal;       // points out of obstacle
            float into = Vector3.Dot(v, n);       // negative means we are moving into the obstacle

            if (into < 0f)
            {
                v -= into * n; // subtract the "into-wall" component
            }
        }

        rb.linearVelocity = v;
    }

    #region Events from other scripts

    /// <summary>
    /// Called by CollisionDetect (blocking obstacles) when impact happens.
    /// CollisionDetect is responsible for audio; Player decides gameplay response + animations.
    /// </summary>
    public void OnBlockingObstacleImpact(CollisionDetect.BlockingImpact impact)
    {
        // Optional: play pig stumble
        if (pigAnimator != null && string.IsNullOrEmpty(pigStumbleStateName) == false)
        {
            pigAnimator.Play(pigStumbleStateName);
        }

        // Optional: play camera collision animation
        if (cameraAnimator != null && string.IsNullOrEmpty(cameraCollisionStateName) == false)
        {
            cameraAnimator.Play(cameraCollisionStateName);
        }

        // Optional: if you want certain obstacles to reduce speed while touching, you can
        // set these multipliers from impact data. Default is "no multiplier changes".
        if (impact.applySpeedMultipliers)
        {
            obstacleForwardMultiplier = Mathf.Clamp01(impact.forwardMultiplier);
            obstacleStrafeMultiplier = Mathf.Clamp01(impact.strafeMultiplier);
        }
    }

    /// <summary>
    /// Called by CollisionDetect when leaving a blocking obstacle (if it was applying multipliers).
    /// </summary>
    public void OnBlockingObstacleExit()
    {
        obstacleForwardMultiplier = 1f;
        obstacleStrafeMultiplier = 1f;
    }

    /// <summary>
    /// Called by SlowZoneTrigger when entering mud/water.
    /// </summary>
    public void OnEnterSlowZone(int zoneId, float forwardMultiplier, float strafeMultiplier)
    {
        activeZoneMultipliers[zoneId] = (Mathf.Clamp01(forwardMultiplier), Mathf.Clamp01(strafeMultiplier));
    }

    /// <summary>
    /// Called by SlowZoneTrigger when exiting mud/water.
    /// </summary>
    public void OnExitSlowZone(int zoneId)
    {
        activeZoneMultipliers.Remove(zoneId);
    }

    #endregion
}
