using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    /// <summary>
    /// computes desired planar velocity based on base speeds, stamina, and modifiers.
    /// </summary>
    [RequireComponent(typeof(Stamina))]
    [RequireComponent(typeof(SpeedModifiers))]
    public class SpeedModel : MonoBehaviour
    {
        [Header("Base Speeds")]
        [SerializeField, Min(0f)] private float baseForwardSpeed = 8f;
        [SerializeField, Min(0f)] private float baseStrafeSpeed = 6f;

        [Header("Stamina -> Speed Multiplier")]
        [Tooltip("Input: stamina 0..1, Output: speed multiplier (e.g., 0.5..1.2).")]
        [SerializeField] private AnimationCurve staminaToSpeed = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);

        private Stamina stamina;
        private SpeedModifiers modifiers;

        public float ForwardSpeed { get; private set; }
        public float StrafeSpeed { get; private set; }

        private void Awake()
        {
            stamina = GetComponent<Stamina>();
            modifiers = GetComponent<SpeedModifiers>();
        }

        private void Update()
        {
            // 1. Get normalized stamina (0..1).
            //    0 = fully exhausted
            //    1 = fully rested
            float staminaNormalized = stamina.Normalized();

            // 2. Convert stamina into a speed multiplier via curve.
            //    This allows non-linear relationships:
            //      - Linear drop-off
            //      - Harsh slowdown at low stamina
            //      - Speed boost at high stamina
            float staminaMul = staminaToSpeed.Evaluate(staminaNormalized);

            // 3. Get any additional axis-specific modifiers
            //    (mud slowing strafe, powerup boosting forward, etc.)
            (float fwdMul, float strafeMul) = modifiers.GetAxisFactors();

            // 4. Final speeds:
            //    BOTH forward and strafe speeds are scaled by staminaMul.
            //    Then each axis is independently scaled by its modifier.
            ForwardSpeed = baseForwardSpeed * staminaMul * fwdMul;
            StrafeSpeed = baseStrafeSpeed * staminaMul * strafeMul;
        }

        /// <summary>
        /// Computes world-space planar velocity.
        ///
        /// Stamina affects:
        ///  - How fast the player auto-moves forward.
        ///  - How responsive horizontal strafing feels.
        ///
        /// Low stamina = slower forward pressure AND slower lane changes.
        /// </summary>
        public Vector3 GetPlanarVelocity(Vector3 basisForward, Vector3 basisRight, float strafeInput)
        {
            Vector3 forward = basisForward.normalized;
            Vector3 right = basisRight.normalized;

            float clampedStrafe = Mathf.Clamp(strafeInput, -1f, 1f);

            return forward * ForwardSpeed +
                   right * (clampedStrafe * StrafeSpeed);
        }
    }
}
