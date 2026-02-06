using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Stamina))]
    [RequireComponent(typeof(SpeedModifiers))]
    [RequireComponent(typeof(SpeedModel))]
    [RequireComponent(typeof(PlayerInteractionBus))]
    [RequireComponent(typeof(PlayerTriggerRouter))]
    [RequireComponent(typeof(PlayerCollisionRouter))]
    public class PlayerFacade : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }

        public Stamina Stamina { get; private set; }

        public SpeedModifiers SpeedModifiers { get; private set; }

        public SpeedModel SpeedModel { get; private set; }

        public PlayerInteractionBus InteractionBus { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Stamina = GetComponent<Stamina>();
            SpeedModifiers = GetComponent<SpeedModifiers>();
            SpeedModel = GetComponent<SpeedModel>();
            InteractionBus = GetComponent<PlayerInteractionBus>();
        }
    }
}
