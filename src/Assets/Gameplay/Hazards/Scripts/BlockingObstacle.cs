using SuperPorkOut.Characters.Player;
using UnityEngine;

namespace SuperPorkOut.Gameplay.Hazards
{
    [RequireComponent(typeof(Collider))]
    public class BlockingObstacle : MonoBehaviour, IPlayerCollisionInteractable
    {
        [SerializeField] private AudioClip hitSfx;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = false;
        }

        public void OnPlayerCollision(PlayerFacade player, CollisionInfo hit)
        {
            if (hitSfx != null)
                AudioSource.PlayClipAtPoint(hitSfx, hit.Point);
        }
    }
}
