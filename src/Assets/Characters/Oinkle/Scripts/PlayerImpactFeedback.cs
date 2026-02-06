using UnityEngine;

namespace Characters.Player
{
    public class PlayerImpactFeedback : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip defaultObstacleHit;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        public void OnHitObstacle(Collision collision)
        {
            if (audioSource != null && defaultObstacleHit != null)
                audioSource.PlayOneShot(defaultObstacleHit);
        }
    }
}
