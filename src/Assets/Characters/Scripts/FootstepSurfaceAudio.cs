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

        [Header("Speed-Based Pitch")]
        [Tooltip("Optional rigidbody to read planar velocity from. If not set, this script will estimate speed from transform movement.")]
        [SerializeField] private Rigidbody movementRigidbody;

        [Tooltip("Planar speed at or below this value uses the minimum pitch.")]
        [SerializeField, Min(0f)] private float minSpeedForPitch = 0f;

        [Tooltip("Planar speed at or above this value uses the maximum pitch.")]
        [SerializeField, Min(0.01f)] private float maxSpeedForPitch = 20f;

        [Tooltip("Playback pitch when moving slowly.")]
        [SerializeField, Min(0.1f)] private float minPitch = 0.75f;

        [Tooltip("Playback pitch when moving quickly.")]
        [SerializeField, Min(0.1f)] private float maxPitch = 1.35f;

        [Tooltip("How quickly pitch reacts to speed changes.")]
        [SerializeField, Min(0f)] private float pitchLerpSpeed = 10f;

        private readonly HashSet<SurfaceZone> activeZones = new();
        private Vector3 previousPosition;
        private float currentPitch = 1f;

        private void Awake()
        {
            if (footstepsSource == null)
                footstepsSource = GetComponent<AudioSource>();

            if (movementRigidbody == null)
                movementRigidbody = GetComponent<Rigidbody>();

            if (defaultFootstepLoopClip == null && footstepsSource != null)
                defaultFootstepLoopClip = footstepsSource.clip;

            previousPosition = transform.position;
            currentPitch = Mathf.Clamp(footstepsSource != null ? footstepsSource.pitch : 1f, minPitch, maxPitch);

            ApplyClip(defaultFootstepLoopClip);
            ApplyPitch(currentPitch);
        }

        private void Update()
        {
            float planarSpeed = GetPlanarSpeed();
            float speedRatio = maxSpeedForPitch <= minSpeedForPitch
                ? 1f
                : Mathf.InverseLerp(minSpeedForPitch, maxSpeedForPitch, planarSpeed);

            float targetPitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
            float lerpT = 1f - Mathf.Exp(-pitchLerpSpeed * Time.deltaTime);
            currentPitch = Mathf.Lerp(currentPitch, targetPitch, lerpT);

            ApplyPitch(currentPitch);
            previousPosition = transform.position;
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

        private float GetPlanarSpeed()
        {
            if (movementRigidbody != null)
            {
                Vector3 planarVelocity = movementRigidbody.linearVelocity;
                planarVelocity.y = 0f;
                return planarVelocity.magnitude;
            }

            float dt = Mathf.Max(Time.deltaTime, 0.0001f);
            Vector3 displacement = transform.position - previousPosition;
            displacement.y = 0f;
            return displacement.magnitude / dt;
        }

        private void ApplyPitch(float pitch)
        {
            if (footstepsSource == null) return;
            footstepsSource.pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }
}
