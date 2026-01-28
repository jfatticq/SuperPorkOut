using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject player;

    [SerializeField] GameObject playerAnim;

    private void OnTriggerEnter(Collider other)
    {
        player.GetComponent<PlayerController>().enabled = false;
        // player animation for running into an obstacle?
        if (playerAnim != null)
        {
            playerAnim.GetComponent<Animator>().Play("");
        }
    }
}
