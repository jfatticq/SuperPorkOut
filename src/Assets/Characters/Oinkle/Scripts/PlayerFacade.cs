using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    /// <summary>
    /// Provides a unified interface for accessing and managing player-related components, including physics, stamina,
    /// speed, and interaction systems.
    /// </summary>
    /// <remarks>This class requires several components to be attached to the same GameObject, including
    /// Rigidbody, Stamina, SpeedModifiers, SpeedModel, PlayerInteractionBus, PlayerTriggerRouter, and
    /// PlayerCollisionRouter. All required components are initialized in the Awake method and exposed via read-only
    /// properties, allowing other scripts to interact with player systems in a consistent manner.</remarks>
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
