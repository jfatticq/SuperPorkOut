using System;
using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    public class PlayerInteractionBus : MonoBehaviour
    {
        public event Action<IPlayerTriggerInteractable> TriggerEntered;

        public event Action<IPlayerTriggerInteractable> TriggerExited;

        public event Action<IPlayerCollisionInteractable, CollisionInfo> Collided;

        internal void RaiseTriggerEntered(IPlayerTriggerInteractable target) => TriggerEntered?.Invoke(target);

        internal void RaiseTriggerExited(IPlayerTriggerInteractable target) => TriggerExited?.Invoke(target);

        internal void RaiseCollided(IPlayerCollisionInteractable target, CollisionInfo info) => Collided?.Invoke(target, info);
    }
}
