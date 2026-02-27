using SuperPorkOut.Gameplay.Hazards;
using UnityEngine;

namespace SuperPorkOut.Characters
{
    [RequireComponent(typeof(FootstepSurfaceAudio))]
    public class SurfaceZoneAudioTrigger : MonoBehaviour
    {
        private FootstepSurfaceAudio footstepSurfaceAudio;

        private void Awake()
        {
            footstepSurfaceAudio = GetComponent<FootstepSurfaceAudio>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (TryGetSurfaceZone(other, out var zone))
                footstepSurfaceAudio.EnterZone(zone);
        }

        private void OnTriggerExit(Collider other)
        {
            if (TryGetSurfaceZone(other, out var zone))
                footstepSurfaceAudio.ExitZone(zone);
        }

        private static bool TryGetSurfaceZone(Collider other, out SurfaceZone zone)
        {
            zone = other.GetComponent<SurfaceZone>();
            if (zone != null) return true;

            zone = other.GetComponentInParent<SurfaceZone>();
            return zone != null;
        }
    }
}
