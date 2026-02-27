using SuperPorkOut.Characters;
using UnityEngine;

namespace SuperPorkOut.Gameplay.Hazards
{
    [RequireComponent(typeof(Collider))]
    public class SurfaceZone : MonoBehaviour
    {
        [Header("Character-specific surface loops")]
        [SerializeField] private AudioClip oinkleFootstepLoopClip;
        [SerializeField] private AudioClip farmerFootstepLoopClip;

        public AudioClip GetFootstepLoopClip(FootstepActor actor)
        {
            return actor == FootstepActor.Farmer ? farmerFootstepLoopClip : oinkleFootstepLoopClip;
        }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }
}
