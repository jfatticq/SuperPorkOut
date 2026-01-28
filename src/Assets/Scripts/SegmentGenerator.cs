using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segments;

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
        if (randomizeSegments)
            segmentNumber = Random.Range(0, segments.Length - 1);

        Instantiate(segments[segmentNumber], new Vector3(0, 0, segmentLength), Quaternion.identity);
        segmentLength += 50;
        yield return new WaitForSeconds(3);
        creatingSegment = false;

        if (!randomizeSegments)
        {
            segmentNumber++;
            if (segmentNumber >= segments.Length)
                segmentNumber = 0;
        }
    }
}
