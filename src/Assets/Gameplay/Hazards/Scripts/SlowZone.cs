using UnityEngine;
using Characters.Player;

[RequireComponent(typeof(Collider))]
public class SlowZone : MonoBehaviour, IPlayerTriggerInteractable
{
    [SerializeField] private SpeedAxes affects = SpeedAxes.Both;

    [SerializeField, Range(0f, 1f)] private float multiplier = 0.6f;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void OnPlayerEnter(PlayerFacade player)
    {
        player.SpeedModifiers.SetModifier(this, affects, multiplier);
    }

    public void OnPlayerExit(PlayerFacade player)
    {
        player.SpeedModifiers.RemoveModifier(this);
    }
}
