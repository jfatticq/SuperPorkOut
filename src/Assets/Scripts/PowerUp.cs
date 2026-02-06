using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUp : MonoBehaviour
{
    [Header("Speed Boost")]
    public float boostMultiplier = 1.5f;
}
