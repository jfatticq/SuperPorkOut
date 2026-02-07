using SuperPorkOut.Characters.Player;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUp : MonoBehaviour, IPlayerTriggerInteractable
{
    [SerializeField, Min(0f)] private float staminaAmount = 15f;

    [SerializeField] private AudioClip pickupSfx;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void OnPlayerEnter(PlayerFacade player)
    {
        player.Stamina.Add(staminaAmount);

        if (pickupSfx != null)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position);

        Destroy(gameObject);
    }

    public void OnPlayerExit(PlayerFacade player) { }
}
