using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] AudioSource coinCollected;

    private void OnTriggerEnter(Collider other)
    {
        coinCollected.Play();

        GameController.coinCount += 1;

        this.gameObject.SetActive(false);
    }
}
