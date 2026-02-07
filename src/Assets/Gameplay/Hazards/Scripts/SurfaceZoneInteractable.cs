using SuperPorkOut.Characters.Player;
using UnityEngine;

namespace SuperPorkOut.Gameplay.Hazards
{
    [RequireComponent(typeof(SurfaceZone))]
    public class SurfaceZoneInteractable : MonoBehaviour, IPlayerTriggerInteractable
    {
        private SurfaceZone zone;

        private void Awake()
        {
            zone = GetComponent<SurfaceZone>();
        }

        public void OnPlayerEnter(PlayerFacade player)
        {
            if (player.TryGetComponent<FootstepSurfaceAudio>(out var audio))
                audio.EnterZone(zone);
        }

        public void OnPlayerExit(PlayerFacade player)
        {
            if (player.TryGetComponent<FootstepSurfaceAudio>(out var audio))
                audio.ExitZone(zone);
        }
    }
}
