using SuperPorkOut.Characters.Player;
using System;
using UnityEngine;

namespace SuperPorkOut.Gameplay.Pickups
{
    public enum FoodType
    {
        Carrot,
        Cabbage,
        Tomato,
        Other
    }

    [RequireComponent(typeof(Collider))]
    public class PowerUp : MonoBehaviour, IPlayerTriggerInteractable
    {
        public static event Action<PickupEventData> PickedUp;

        [Header("PowerUp Settings")]
        [Tooltip("Type of food this power-up represents. Used for counting/scoring.")]
        [SerializeField] private FoodType foodType = FoodType.Other;

        [Tooltip("Amount of stamina this power-up restores when picked up.")]
        [SerializeField, Min(0f)] private float staminaAmount = 15f;

        [Tooltip("Sound effect to play when the power-up is picked up.")]
        [SerializeField] private AudioClip pickupSfx;

        public void OnPlayerEnter(PlayerFacade player)
        {
            if (pickupSfx != null)
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position);

            PickedUp?.Invoke(
                new PickupEventData(foodType, staminaAmount, transform.position)
            );
            Destroy(gameObject);
        }

        public void OnPlayerExit(PlayerFacade player) { }
    }
}
