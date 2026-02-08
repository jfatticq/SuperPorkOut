namespace SuperPorkOut.Characters.Player
{
    /// <summary>
    /// A single modifier affecting one or both planar axes.
    /// Stored in SpeedModifiers and evaluated by SpeedModel.
    /// </summary>
    public readonly struct SpeedModifier
    {
        public readonly SpeedAxes Axes;

        public readonly float Multiplier;

        public SpeedModifier(SpeedAxes axes, float multiplier)
        {
            Axes = axes;
            Multiplier = multiplier;
        }
    }
}
