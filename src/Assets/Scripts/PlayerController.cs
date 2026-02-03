using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 2f;
    public float horizontalSpeed = 3f;

    private float forwardMultiplier = 1f;
    private float horizontalMultiplier = 1f;

    public void SetForwardMultiplier(float multiplier)
    {
        forwardMultiplier = Mathf.Clamp01(multiplier);
    }

    public void ResetForwardMultiplier()
    {
        forwardMultiplier = 1f;
    }

    public void SetHorizontalMultiplier(float multiplier)
    {
        horizontalMultiplier = Mathf.Clamp01(multiplier);
    }

    public void ResetHorizontalMultiplier()
    {
        horizontalMultiplier = 1f;
    }

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
        Vector3 forward = playerSpeed * forwardMultiplier * Time.deltaTime * Vector3.forward;

        // horizontal input comes from the x component of the MoveLeftRight Vector2 action
        float horizontalInput = actions.Gameplay.Move.ReadValue<Vector2>().x;

        Vector3 horizontal = horizontalInput * horizontalSpeed * horizontalMultiplier * Time.deltaTime * Vector3.right;

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
