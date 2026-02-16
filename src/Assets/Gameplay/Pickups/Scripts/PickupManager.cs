using UnityEngine;


namespace SuperPorkOut.Gameplay.Pickups
{
    public class PickupParticleManager : MonoBehaviour
    {
        public ParticleSystem pickupExplosion;

        private bool collected = false;

        private void OnTriggerEnter(Collider other)
        {
            if (collected) return;

            if (other.CompareTag("Player"))
            {
                collected = true;
                Collect();
            }
        }

        void Collect()
        {
            if (pickupExplosion != null)
            {
                pickupExplosion.transform.parent = null;

                pickupExplosion.Play();

                Destroy(pickupExplosion.gameObject, pickupExplosion.main.duration);
            }

            Destroy(gameObject);
        }
    }
}
