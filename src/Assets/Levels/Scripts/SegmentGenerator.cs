using System.Collections.Generic;
using UnityEngine;

namespace SuperPorkOut.Levels
{
    public class SegmentGenerator : MonoBehaviour
    {
        public GameObject[] nextSegments;

        [Tooltip("Z position where the next generated segment will be placed")]
        [SerializeField] int nextSegmentZPosition = 50;

        [Tooltip("Length of each segment along the Z axis")]
        [SerializeField] int segmentSize = 50;

        [Tooltip("If true, segments will be chosen randomly from the nextSegments array")]
        [SerializeField] bool randomizeSegments = false;

        [Tooltip("Number of segments to keep generated ahead of the player. Set to 0 to disable generation.")]
        [SerializeField] int maxSegments = 10;

        [Tooltip("Player transform used to determine which segments should exist")]
        [SerializeField] Transform playerTransform;

        [Tooltip("How many segments behind the player should be kept alive")]
        [SerializeField] int segmentsBehindPlayerToKeep = 1;

        private readonly Queue<GameObject> generatedSegmentsQueue = new Queue<GameObject>();

        private int segmentNumber;

        // <summary>Resolves dependencies before generation begins.</summary>
        void Awake()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTransform = player.transform;
            }
        }

        // <summary>Keeps segments in sync with the player's position by cleaning old segments and generating ahead.</summary>
        void Update()
        {
            if (nextSegments == null || nextSegments.Length == 0 || playerTransform == null || maxSegments <= 0)
                return;

            CleanupOldSegments();
            GenerateSegmentsAheadOfPlayer();
        }

        // <summary>Generates segments until the configured ahead-of-player budget is satisfied.</summary>
        private void GenerateSegmentsAheadOfPlayer()
        {
            int playerSegmentIndex = GetSegmentIndex(playerTransform.position.z);
            int farthestAllowedSegmentIndex = playerSegmentIndex + maxSegments;

            while (GetSegmentIndex(nextSegmentZPosition) <= farthestAllowedSegmentIndex)
            {
                if (randomizeSegments)
                    segmentNumber = Random.Range(0, nextSegments.Length);

                GameObject newSegment = Instantiate(nextSegments[segmentNumber], new Vector3(0, 0, nextSegmentZPosition), Quaternion.identity);
                generatedSegmentsQueue.Enqueue(newSegment);

                nextSegmentZPosition += segmentSize;

                if (!randomizeSegments)
                {
                    segmentNumber++;
                    if (segmentNumber >= nextSegments.Length)
                        segmentNumber = 0;
                }
            }
        }

        // <summary>Removes generated segments that are farther behind the player than the configured trailing buffer.</summary>
        private void CleanupOldSegments()
        {
            int playerSegmentIndex = GetSegmentIndex(playerTransform.position.z);
            int oldestAllowedSegmentIndex = playerSegmentIndex - segmentsBehindPlayerToKeep;

            while (generatedSegmentsQueue.Count > 0)
            {
                GameObject oldest = generatedSegmentsQueue.Peek();
                if (oldest == null)
                {
                    generatedSegmentsQueue.Dequeue();
                    continue;
                }

                int segmentIndex = GetSegmentIndex(oldest.transform.position.z);
                if (segmentIndex >= oldestAllowedSegmentIndex)
                    break;

                Destroy(oldest);
                generatedSegmentsQueue.Dequeue();
            }
        }

        // <summary>Converts a world-space Z position into a zero-based segment index.</summary>
        // <param name="zPosition">The world-space Z position to map to a segment index.</param>
        // <returns>The segment index containing the provided Z position.</returns>
        private int GetSegmentIndex(float zPosition)
        {
            if (segmentSize <= 0)
                return 0;

            return Mathf.FloorToInt(zPosition / segmentSize);
        }
    }
}
