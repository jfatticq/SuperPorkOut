using UnityEngine;

namespace SuperPorkOut.Gameplay.Pickups
{
    public readonly struct PickupEventData
    {
        public FoodType Type { get; }

        public float StaminaAmount { get; }

        public Vector3 Position { get; }

        public PickupEventData(FoodType type, float staminaAmount, Vector3 position)
        {
            Type = type;
            StaminaAmount = staminaAmount;
            Position = position;
        }
    }
}