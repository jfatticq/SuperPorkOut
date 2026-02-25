using SuperPorkOut.Characters.Player;
using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    public float amount = 10f;
    public GameObject popupPrefab;

    private void OnTriggerEnter(Collider other)
    {
        var stamina = other.GetComponent<Stamina>();
        if (stamina == null) return;

        stamina.Add(amount);

        // Spawn popup
        if (popupPrefab != null)
        {
            var popup = Instantiate(popupPrefab, transform.position, Quaternion.identity);
            popup.GetComponent<PopupText>().SetText("+" + amount + " Stamina");
        }

        Destroy(gameObject);
    }
}