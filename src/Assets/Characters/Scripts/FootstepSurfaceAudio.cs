using SuperPorkOut.Gameplay.Hazards;
using System.Collections.Generic;
using UnityEngine;

namespace SuperPorkOut.Characters
{
    public enum FootstepActor
    {
        Oinkle = 0,
        Farmer = 1
    }

    [RequireComponent(typeof(AudioSource))]
    public class FootstepSurfaceAudio : MonoBehaviour
    {
        [Header("Actor")]
        [SerializeField] private FootstepActor actor = FootstepActor.Oinkle;

        [Header("Playback")]
        [SerializeField] private AudioSource footstepsSource;
        [SerializeField] private AudioClip defaultFootstepLoopClip;

        private readonly HashSet<SurfaceZone> activeZones = new();

        private void Awake()
        {
            if (footstepsSource == null)
                footstepsSource = GetComponent<AudioSource>();

            if (defaultFootstepLoopClip == null && footstepsSource != null)
                defaultFootstepLoopClip = footstepsSource.clip;

            ApplyClip(defaultFootstepLoopClip);
        }

        /// <summary>Call when you enter a surface zone.</summary>
        public void EnterZone(SurfaceZone zone)
        {
            if (zone == null) return;

            activeZones.Add(zone);
            Refresh();
        }

        /// <summary>Call when you exit a surface zone.</summary>
        public void ExitZone(SurfaceZone zone)
        {
            if (zone == null) return;
            activeZones.Remove(zone);
            Refresh();
        }

        private void Refresh()
        {
            AudioClip desiredClip = defaultFootstepLoopClip;
            foreach (var zone in activeZones)
            {
                if (zone == null) continue;

                var zoneClip = zone.GetFootstepLoopClip(actor);
                if (zoneClip != null)
                {
                    desiredClip = zoneClip;
                    break;
                }
            }

            ApplyClip(desiredClip);
        }

        private void ApplyClip(AudioClip clip)
        {
            if (footstepsSource == null) return;
            if (footstepsSource.clip == clip) return;

            bool wasPlaying = footstepsSource.isPlaying;

            footstepsSource.clip = clip;

            // If your footsteps are a loop, keep it seamless-ish:
            if (wasPlaying)
                footstepsSource.Play();
        }
    }
}
