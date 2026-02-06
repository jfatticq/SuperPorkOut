using Assets.Characters.Player.Scripts;
using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(PlayerFacade))]
    public class PlayerTriggerRouter : MonoBehaviour
    {
        private PlayerFacade player;

        private void Awake()
        {
            player = GetComponent<PlayerFacade>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!TryGetTriggerInteractable(other, out var target)) return;

            target.OnPlayerEnter(player);
            player.InteractionBus.RaiseTriggerEntered(target);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!TryGetTriggerInteractable(other, out var target)) return;

            target.OnPlayerExit(player);
            player.InteractionBus.RaiseTriggerExited(target);
        }

        private static bool TryGetTriggerInteractable(Collider other, out IPlayerTriggerInteractable target)
        {
            // Prefer component on collider, fallback to parents.
            if (other.TryGetComponent<IPlayerTriggerInteractable>(out target)) return true;
            target = other.GetComponentInParent<IPlayerTriggerInteractable>();
            return target != null;
        }
    }
}
