using System;
using UnityEngine;

namespace SuperPorkOut.Gameplay
{
    public class GameStateBus : MonoBehaviour
    {
        public event Action<CapturedEvent> Captured;

        public bool IsCaptured { get; private set; }

        public void RaiseCaptured(CapturedEvent evt)
        {
            if (IsCaptured) return; // fire once
            IsCaptured = true;
            Captured?.Invoke(evt);
        }

        public void ResetFlags()
        {
            IsCaptured = false;
        }
    }

    public readonly struct CapturedEvent
    {
        public readonly GameObject pig;
        public readonly GameObject farmer;
        public readonly Vector3 point;
        public readonly string reason;

        public CapturedEvent(GameObject pig, GameObject farmer, Vector3 point, string reason)
        {
            this.pig = pig;
            this.farmer = farmer;
            this.point = point;
            this.reason = reason;
        }
    }
}
