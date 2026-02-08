using System;
using UnityEngine;

namespace SuperPorkOut.Levels
{
    /// <summary>
    /// Provides a centralized event bus for broadcasting and tracking game state changes such as capture and level
    /// completion events.
    /// </summary>
    /// <remarks>GameStateBus enables decoupled communication between game systems by exposing events for
    /// significant game state transitions. It maintains flags indicating whether the game has been captured or the
    /// level has ended, and provides methods to raise these events and reset the state. This class is intended to be
    /// used as a singleton or shared component within a Unity scene to coordinate game flow.</remarks>
    public class GameStateBus : MonoBehaviour
    {
        public event Action<CapturedEvent> Captured;
        public event Action<LevelEndedEvent> LevelEnded;

        public bool IsCaptured { get; private set; }
        public bool IsLevelEnded { get; private set; }

        public bool IsGameOver => IsCaptured || IsLevelEnded;

        public void RaiseCaptured(CapturedEvent evt)
        {
            if (IsCaptured) return; // fire once
            IsCaptured = true;
            Captured?.Invoke(evt);
        }

        public void RaiseLevelEnded(LevelEndedEvent evt)
        {
            if (IsGameOver) return;
            IsLevelEnded = true;
            LevelEnded?.Invoke(evt);
        }

        public void ResetFlags()
        {
            IsCaptured = false;
            IsLevelEnded = false;
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

    public readonly struct LevelEndedEvent
    {
        public readonly GameObject pig;
        public readonly GameObject farmer;
        public readonly Vector3 point;
        public readonly string reason;

        public LevelEndedEvent(GameObject pig, GameObject farmer, Vector3 point, string reason)
        {
            this.pig = pig;
            this.farmer = farmer;
            this.point = point;
            this.reason = reason;
        }
    }
}
