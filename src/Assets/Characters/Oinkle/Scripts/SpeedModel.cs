using Characters.Player;
using UnityEngine;

[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(SpeedModifiers))]
public class SpeedModel : MonoBehaviour
{
    [Header("Base Speeds")]
    [SerializeField, Min(0f)] private float baseForwardSpeed = 8f;
    [SerializeField, Min(0f)] private float baseStrafeSpeed = 6f;

    [Header("Stamina ? Speed Multiplier")]
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
        float staminaMul = staminaToSpeed.Evaluate(stamina.Normalized());
        (float fwdMul, float strafeMul) = modifiers.GetAxisFactors();

        ForwardSpeed = baseForwardSpeed * staminaMul * fwdMul;
        StrafeSpeed = baseStrafeSpeed * staminaMul * strafeMul;
    }

    public Vector3 GetPlanarVelocity(Vector3 basisForward, Vector3 basisRight, float strafeInput)
    {
        Vector3 forward = basisForward.normalized;
        Vector3 right = basisRight.normalized;

        return forward * ForwardSpeed + right * (Mathf.Clamp(strafeInput, -1f, 1f) * StrafeSpeed);
    }
}
