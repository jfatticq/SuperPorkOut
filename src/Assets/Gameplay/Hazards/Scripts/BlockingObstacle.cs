using SuperPorkOut.Characters.Player;
using UnityEngine;

namespace SuperPorkOut.Gameplay.Hazards
{
    [RequireComponent(typeof(Collider))]
    public class BlockingObstacle : MonoBehaviour, IPlayerCollisionInteractable
    {
        [Tooltip("The sound effect to play when the player collides with this obstacle.")]
        [SerializeField] private AudioSource sfxSource;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = false;
        }

        public void OnPlayerCollision(PlayerFacade player, CollisionInfo hit)
        {
            if (sfxSource != null)
                sfxSource.PlayOneShot(sfxSource.clip);
        }
    }
}
