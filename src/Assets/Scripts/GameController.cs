using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int coinCount = 0;

    [SerializeField] GameObject coinDisplay;

    void Update()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "Coins: " + coinCount;
    }
}
