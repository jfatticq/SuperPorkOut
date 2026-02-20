using UnityEngine;

public class CollectibleRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1;

    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0, Space.World);
    }
}
