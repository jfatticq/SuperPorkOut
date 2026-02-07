using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private SurfaceFootstepProfile footstepProfile;

    public SurfaceFootstepProfile FootstepProfile => footstepProfile;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
