using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 2f;

    public float horizontalSpeed = 3f;

    public Vector3 Position => transform.position;

    // left/right movement limits (world X coordinates)
    public float minX = -8f;
    public float maxX = 8f;

    private InputSystem_Actions actions;

    private void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Enable();
    }

    private void Update()
    {
        // constant forward motion
        Vector3 forward = playerSpeed * Time.deltaTime * Vector3.forward;

        // horizontal input comes from the x component of the MoveLeftRight Vector2 action
        float horizontalInput = actions.Player.Move.ReadValue<Vector2>().x;

        Vector3 horizontal = horizontalInput * horizontalSpeed * Time.deltaTime * Vector3.right;

        // compute new position then clamp X to stay within boundaries
        Vector3 newPos = transform.position + forward + horizontal;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        transform.position = newPos;
    }

    private void OnDestroy()
    {
        actions.Disable();
    }
}
