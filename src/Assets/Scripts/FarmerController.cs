using UnityEngine;

public class FarmerController : MonoBehaviour
{
    public float farmerSpeed = 3f;
    
    public PlayerController playerController;

    // Update is called once per frame
    void Update()
    {
        // constant forward motion
        Vector3 forward = farmerSpeed * Time.deltaTime * Vector3.forward;

        if (playerController != null)         {
            // match horizontal position with the player
            Vector3 playerPos = playerController.Position;
            Vector3 newPos = new Vector3(playerPos.x, transform.position.y, transform.position.z) + forward;
            transform.position = newPos;
        }
        else
        {
            // if no player controller is assigned, just move forward
            transform.position += forward;
        }
    }
}
