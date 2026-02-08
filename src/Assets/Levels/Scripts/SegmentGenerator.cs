using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperPorkOut.Levels
{
    public class SegmentGenerator : MonoBehaviour
    {
        public GameObject[] nextSegments;

        [Tooltip("Length of each segment along the Z axis")]
        [SerializeField] int segmentLength = 50;

        [Tooltip("If true, segments will be chosen randomly from the nextSegments array")]
        [SerializeField] bool randomizeSegments = false;

        [Tooltip("Maximum number of segments to keep in the scene at once. Set to 0 for unlimited.")]
        [SerializeField] int maxSegments = 10;

        [Tooltip("Time in seconds between segment generation attempts")]
        [SerializeField] int segmentGenerationInterval = 4;

        // store generated segments so we can remove the oldest when we hit maxSegments
        private Queue<GameObject> generatedSegmentsQueue;

        private bool creatingSegment = false;

        private int segmentNumber;

        void Awake()
        {
            if (maxSegments > 0)
                generatedSegmentsQueue = new Queue<GameObject>(maxSegments);
        }

        void Update()
        {
            if (!creatingSegment)
            {
                creatingSegment = true;
                StartCoroutine(Generate());
            }
        }

        IEnumerator Generate()
        {
            if (nextSegments == null || nextSegments.Length == 0)
            {
                // no segments configured - wait then allow generator to try again later
                yield return new WaitForSeconds(segmentGenerationInterval);
                creatingSegment = false;
                yield break;
            }

            if (randomizeSegments)
                segmentNumber = Random.Range(0, nextSegments.Length - 1);

            GameObject newSegment = Instantiate(nextSegments[segmentNumber], new Vector3(0, 0, segmentLength), Quaternion.identity);

            // track generated segments and remove oldest when we hit the limit
            if (generatedSegmentsQueue != null && maxSegments > 0)
            {
                generatedSegmentsQueue.Enqueue(newSegment);
                if (generatedSegmentsQueue.Count > maxSegments)
                {
                    GameObject old = generatedSegmentsQueue.Dequeue();
                    if (old != null)
                        Destroy(old);
                }
            }
            segmentLength += 50;
            yield return new WaitForSeconds(4);
            creatingSegment = false;

            if (!randomizeSegments)
            {
                segmentNumber++;
                if (segmentNumber >= nextSegments.Length)
                    segmentNumber = 0;
            }
        }
    }
}
