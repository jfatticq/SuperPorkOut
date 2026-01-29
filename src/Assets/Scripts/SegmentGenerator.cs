using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] nextSegments;

    [SerializeField] int segmentLength = 50;

    [SerializeField] bool randomizeSegments = false;

    private bool creatingSegment = false;

    private int segmentNumber;

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

        Instantiate(nextSegments[segmentNumber], new Vector3(0, 0, segmentLength), Quaternion.identity);
        segmentLength += 50;
        yield return new WaitForSeconds(3);
        creatingSegment = false;

        if (!randomizeSegments)
        {
            segmentNumber++;
            if (segmentNumber >= nextSegments.Length)
                segmentNumber = 0;
        }
    }
}
