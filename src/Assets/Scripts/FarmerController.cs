using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FarmerController : MonoBehaviour
{
    public float farmerSpeed = 3f;
    
    public PlayerController playerController;
    
    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepAudioSource;
    [Tooltip("Distance at or below which the audio is at full volume.")]
    public float minAudibleDistance = 1f;

    [Tooltip("Distance at or beyond which the audio is inaudible.")]
    public float maxAudibleDistance = 20f;

    [Tooltip("Multiplier applied to the computed volume based on distance.")]
    [Range(0f, 1f)] public float volumeMultiplier = 1f;

    [Tooltip("When true the audio source will be switched to full 3D spatial blend and use linear rolloff.")]
    public bool enforce3DSpatial = true;

    private void Start()
    {
        if (footstepAudioSource == null)
        {
            footstepAudioSource = GetComponent<AudioSource>();
        }

        if (footstepAudioSource != null && enforce3DSpatial)
        {
            // make sure audio is using 3D spatial settings so distance changes are noticeable
            footstepAudioSource.spatialBlend = 1f;
            footstepAudioSource.rolloffMode = AudioRolloffMode.Linear;
            footstepAudioSource.minDistance = minAudibleDistance;
            footstepAudioSource.maxDistance = maxAudibleDistance;
        }
    }

    void Update()
    {
        // constant forward motion
        Vector3 forward = farmerSpeed * Time.deltaTime * Vector3.forward;

        if (playerController != null)         {
            // match horizontal position with the player
            Vector3 playerPos = playerController.Position;
            Vector3 newPos = new Vector3(playerPos.x, transform.position.y, transform.position.z) + forward;
            transform.position = newPos;
            // adjust footstep volume based on distance to player
            if (footstepAudioSource != null)
            {
                float dist = Vector3.Distance(transform.position, playerPos);
                float t = 1f - Mathf.InverseLerp(minAudibleDistance, maxAudibleDistance, dist);
                footstepAudioSource.volume = Mathf.Clamp01(t * volumeMultiplier);
            }
        }
        else
        {
            // if no player controller is assigned, just move forward
            transform.position += forward;
        }
    }
}
