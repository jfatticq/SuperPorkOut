using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segments;

    [SerializeField] int zPosition = 50;

    [SerializeField] bool creatingSegment = false;

    [SerializeField] int segmentNumber;

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
        segmentNumber = Random.Range(0, segments.Length - 1);
        Instantiate(segments[segmentNumber], new Vector3(0, 0, zPosition), Quaternion.identity);
        zPosition += 50;
        yield return new WaitForSeconds(3);
        creatingSegment = false;
    }
}
