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
        [SerializeField] private AudioSource sfxSource;

        private bool isCollected;

        public void OnPlayerEnter(PlayerFacade player)
        {
            if (isCollected)
                return;

            isCollected = true;

            sfxSource.Play();
            // Disable visuals/collider so it looks "gone"
            //GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, sfxSource.clip.length);
        }

        public void OnPlayerExit(PlayerFacade player) { }
    }
}
