using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 2f;

    public float horizontalSpeed = 3f;

    public Vector3 Position => transform.position;

    // left/right movement limits (world X coordinates)
    public float minX = -8f;
    public float maxX = 8f;

    // Assign your "MoveLeftRight" (2D Vector) action from the Input Actions asset in the Inspector.
    [SerializeField] private InputActionReference moveAction;

    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        // constant forward motion
        Vector3 forward = playerSpeed * Time.deltaTime * Vector3.forward;

        // horizontal input comes from the x component of the MoveLeftRight Vector2 action
        float horizontalInput = 0f;
        if (moveAction != null && moveAction.action != null)
        {
            Vector2 v = moveAction.action.ReadValue<Vector2>();
            horizontalInput = v.x;
        }

        Vector3 horizontal = horizontalInput * horizontalSpeed * Time.deltaTime * Vector3.right;

        // compute new position then clamp X to stay within boundaries
        Vector3 newPos = transform.position + forward + horizontal;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        transform.position = newPos;
    }
}
