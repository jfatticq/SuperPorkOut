using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    [Header("Segment Prefabs (in order)")]
    public GameObject[] nextSegments;

    [Header("References")]
    [Tooltip("Reference to the pig transform to track its position")]
    [SerializeField] private Transform pig;

    [Tooltip("Length of each segment along the Z axis")]
    [SerializeField] int segmentLength = 60;

    [Tooltip("If true, segments will be chosen randomly from the nextSegments array")]
    [SerializeField] private bool randomizeSegments = false;

    [Tooltip("Maximum number of segments to keep alive at once. Set to 0 for unlimited.")]
    [SerializeField] private int maxSegments = 10;

    [Tooltip("How far behind the pig (in world units) a segment must be before it gets destroyed.")]
    [SerializeField] private float destroyDistanceBehind = 100f;

    [Tooltip("How far ahead of the pig we try to keep segments spawned (in segments). Useful if maxSegments == 0.")]
    [SerializeField] private int targetSegmentsAhead = 10;

    [Tooltip("Time in seconds between generator ticks (spawn/despawn checks).")]
    [SerializeField] private float tickInterval = 0.25f;

    private readonly List<GameObject> activeSegments = new List<GameObject>();

    private float nextSpawnZ = 0f;
    private int segmentIndex = 0;

    private void Awake()
    {
        if (pig == null)
        {
            Debug.LogError($"{nameof(SegmentGenerator)}: Pig reference not set. Assign the pig Transform in the inspector.");
        }
    }

    private void Start()
    {
        // Optional: start spawning from this generator's Z position.
        nextSpawnZ = transform.position.z;

        StartCoroutine(TickLoop());
    }

    private IEnumerator TickLoop()
    {
        while (true)
        {
            if (pig != null)
            {
                DespawnBehindPig();
                SpawnAhead();
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }

    private void SpawnAhead()
    {
        if (nextSegments == null || nextSegments.Length == 0)
            return;

        // Determine how many segments we're allowed to keep.
        int desiredMax =
            (maxSegments > 0)
                ? maxSegments
                : Mathf.Max(1, targetSegmentsAhead); // if unlimited, at least keep some ahead

        // Spawn until we reach the cap.
        while (activeSegments.Count < desiredMax)
        {
            int pick = PickNextSegmentIndex();

            Vector3 spawnPos = new Vector3(0f, 0f, nextSpawnZ);
            GameObject seg = Instantiate(nextSegments[pick], spawnPos, Quaternion.identity);
            activeSegments.Add(seg);

            nextSpawnZ += segmentLength;
        }
    }

    private void DespawnBehindPig()
    {
        if (activeSegments.Count == 0)
            return;

        // Since we spawn in order along Z, the oldest segment is always index 0.
        // We'll remove as many as are far enough behind.
        while (activeSegments.Count > 0)
        {
            GameObject oldest = activeSegments[0];
            if (oldest == null)
            {
                activeSegments.RemoveAt(0);
                continue;
            }

            float segmentStartZ = oldest.transform.position.z;
            float segmentEndZ = segmentStartZ + segmentLength;

            // If the pig is far beyond this segment's end, destroy it.
            if (pig.position.z - segmentEndZ > destroyDistanceBehind)
            {
                activeSegments.RemoveAt(0);
                Destroy(oldest);
            }
            else
            {
                // Oldest isn't eligible yet, so none after it will be either.
                break;
            }
        }
    }

    private int PickNextSegmentIndex()
    {
        if (randomizeSegments)
        {
            // IMPORTANT: for int overload, upper bound is exclusive
            return Random.Range(0, nextSegments.Length);
        }

        int idx = segmentIndex;
        segmentIndex++;
        if (segmentIndex >= nextSegments.Length)
            segmentIndex = 0;

        return idx;
    }
}
