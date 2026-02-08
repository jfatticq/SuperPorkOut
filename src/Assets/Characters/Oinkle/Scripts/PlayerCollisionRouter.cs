using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    [RequireComponent(typeof(PlayerFacade))]
    public class PlayerCollisionRouter : MonoBehaviour
    {
        private PlayerFacade player;

        private void Awake()
        {
            player = GetComponent<PlayerFacade>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!TryGetCollisionInteractable(collision.collider, out var target)) return;

            var info = CollisionInfo.FromCollision(collision);
            target.OnPlayerCollision(player, info);
            player.InteractionBus.RaiseCollided(target, info);
        }

        private static bool TryGetCollisionInteractable(Collider other, out IPlayerCollisionInteractable target)
        {
            if (other.TryGetComponent<IPlayerCollisionInteractable>(out target)) return true;
            target = other.GetComponentInParent<IPlayerCollisionInteractable>();
            return target != null;
        }
    }
}
