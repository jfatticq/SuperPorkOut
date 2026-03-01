using SuperPorkOut.Characters.Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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
            if (isCollected) return;

            isCollected = true;

            PickedUp?.Invoke(
                new PickupEventData(foodType, staminaAmount, transform.position)
            );

            PlayPickupSound();

            // Disable visuals/collider so it looks "gone"
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            Destroy(gameObject);
        }

        public void OnPlayerExit(PlayerFacade player) { }

        private void PlayPickupSound()
        {
            if (sfxSource == null || sfxSource.clip == null)
                return;

            // AudioSource is on a child object — detach it so it survives
            // the PowerUp being destroyed, preserving all mixer/spatial settings.
            if (sfxSource.gameObject != gameObject)
            {
                sfxSource.transform.SetParent(null);
                sfxSource.Play();
                Destroy(sfxSource.gameObject, sfxSource.clip.length);
                return;
            }

            // AudioSource is on this same object — clone essential settings
            // into a temporary source so the sound still routes through the mixer.
            var temp = new GameObject("PickupSFX_Temp");
            temp.transform.position = transform.position;
            var src = temp.AddComponent<AudioSource>();
            src.clip = sfxSource.clip;
            src.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            src.volume = sfxSource.volume;
            src.pitch = sfxSource.pitch;
            src.spatialBlend = sfxSource.spatialBlend;
            src.Play();
            Destroy(temp, sfxSource.clip.length);
        }
    }
}
