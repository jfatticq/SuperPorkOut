using System.Collections.Generic;
using UnityEngine;

namespace Characters.Player
{
    /// <summary>
    /// Holds temporary speed multipliers that can be applied to specific axes.
    /// Overlap rule: "lowest wins" per-axis (mud + water => worst slow applies).
    /// </summary>
    public class SpeedModifiers : MonoBehaviour
    {
        private readonly Dictionary<object, SpeedModifier> modifiers = new();

        public void SetModifier(object source, SpeedAxes axes, float multiplier)
        {
            if (source == null) return;
            modifiers[source] = new SpeedModifier(axes, Mathf.Max(0f, multiplier));
        }

        public void RemoveModifier(object source)
        {
            if (source == null) return;
            modifiers.Remove(source);
        }

        public (float forward, float strafe) GetAxisFactors()
        {
            if (modifiers.Count == 0) return (1f, 1f);

            float forward = 1f;
            float strafe = 1f;

            foreach (var mod in modifiers.Values)
            {
                float m = Mathf.Max(0f, mod.Multiplier);

                if ((mod.Axes & SpeedAxes.Forward) != 0)
                    forward = Mathf.Min(forward, m);

                if ((mod.Axes & SpeedAxes.Strafe) != 0)
                    strafe = Mathf.Min(strafe, m);
            }

            return (forward, strafe);
        }
    }
}
