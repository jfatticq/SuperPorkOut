using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] nextSegments;

    [SerializeField] int segmentLength = 50;

    [SerializeField] bool randomizeSegments = false;

    [SerializeField] int maxSegments = 10;

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
            yield return new WaitForSeconds(3);
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
