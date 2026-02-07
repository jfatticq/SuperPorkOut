using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSurfaceAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource footstepsSource;

    [Header("Default")]
    [SerializeField] private AudioClip defaultFootstepsClip;

    [Header("Zone Selection")]
    [Tooltip("Higher wins if multiple surface zones overlap.")]
    [SerializeField] private bool useHighestPriority = true;

    private readonly Dictionary<SurfaceZone, int> activeZones = new();

    private void Awake()
    {
        if (footstepsSource == null)
            footstepsSource = GetComponent<AudioSource>();

        // Initialize to default
        ApplyClip(defaultFootstepsClip);
    }

    /// <summary>Call when you enter a surface zone.</summary>
    public void EnterZone(SurfaceZone zone)
    {
        if (zone == null) return;
        int priority = GetZonePriority(zone);
        activeZones[zone] = priority;
        Refresh();
    }

    /// <summary>Call when you exit a surface zone.</summary>
    public void ExitZone(SurfaceZone zone)
    {
        if (zone == null) return;
        activeZones.Remove(zone);
        Refresh();
    }

    private int GetZonePriority(SurfaceZone zone)
    {
        // Optional: add a ZonePriority component if you want.
        // For now: if not present, priority = 0.
        var prio = zone.GetComponent<ZonePriority>();
        return prio != null ? prio.priority : 0;
    }

    private void Refresh()
    {
        AudioClip desired = defaultFootstepsClip;

        if (activeZones.Count > 0)
        {
            SurfaceZone bestZone = null;
            int bestPriority = int.MinValue;

            foreach (var kvp in activeZones)
            {
                var zone = kvp.Key;
                var prio = kvp.Value;

                if (bestZone == null)
                {
                    bestZone = zone;
                    bestPriority = prio;
                    continue;
                }

                if (useHighestPriority)
                {
                    if (prio > bestPriority)
                    {
                        bestZone = zone;
                        bestPriority = prio;
                    }
                }
                else
                {
                    // lowest wins variant if you ever want it
                    if (prio < bestPriority)
                    {
                        bestZone = zone;
                        bestPriority = prio;
                    }
                }
            }

            if (bestZone != null && bestZone.FootstepProfile != null)
                desired = bestZone.FootstepProfile.footstepLoopClip;
        }

        ApplyClip(desired);
    }

    private void ApplyClip(AudioClip clip)
    {
        if (footstepsSource == null) return;

        if (footstepsSource.clip == clip) return; // no change

        bool wasPlaying = footstepsSource.isPlaying;

        footstepsSource.clip = clip;

        // If your footsteps are a loop, keep it seamless-ish:
        if (wasPlaying)
            footstepsSource.Play();
    }
}
