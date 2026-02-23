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
        [SerializeField] private FoodType foodType = FoodType.Other;

        [SerializeField, Min(0f)] private float staminaAmount = 15f;

        [SerializeField] private AudioClip pickupSfx;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        public void OnPlayerEnter(PlayerFacade player)
        {
            player.Stamina.Add(staminaAmount);

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
