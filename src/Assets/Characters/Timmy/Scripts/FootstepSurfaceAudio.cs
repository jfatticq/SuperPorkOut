using UnityEngine;

[RequireComponent(typeof(FootstepSurfaceAudio))]
public class SurfaceZoneAudioTrigger : MonoBehaviour
{
    private FootstepSurfaceAudio audioController;

    private void Awake()
    {
        audioController = GetComponent<FootstepSurfaceAudio>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SurfaceZone zone = other.GetComponent<SurfaceZone>();
        if (zone == null)
            zone = other.GetComponentInParent<SurfaceZone>();

        if (zone == null)
            return;

        audioController.EnterZone(zone);
    }

    private void OnTriggerExit(Collider other)
    {
        SurfaceZone zone = other.GetComponent<SurfaceZone>();
        if (zone == null)
            zone = other.GetComponentInParent<SurfaceZone>();

        if (zone == null)
            return;

        audioController.ExitZone(zone);
    }
}
