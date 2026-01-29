using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] AudioSource coinCollected;

    private void OnTriggerEnter(Collider other)
    {
        coinCollected.Play();

        LevelController.coinCount += 1;

        this.gameObject.SetActive(false);
    }
}
