using System;

namespace Characters.Player
{
    /// <summary>
    /// Which planar axes a speed modifier affects.
    /// Forward = player's forward direction in movement basis.
    /// Strafe  = player's right direction in movement basis.
    /// </summary>
    [Flags]
    public enum SpeedAxes
    {
        None = 0,
        Forward = 1 << 0,
        Strafe = 1 << 1,
        Both = Forward | Strafe
    }
}
