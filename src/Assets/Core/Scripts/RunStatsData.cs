using System;
using System.Collections.Generic;
using SuperPorkOut.Gameplay.Pickups;

namespace SuperPorkOut.Core
{
    [Serializable]
    public class RunStatsEntry
    {
        public float distanceTraveled;
        public float timeElapsed;
        public int carrotCount;
        public int cabbageCount;
        public int tomatoCount;
        public int otherCount;
        public string sceneName;
        public string dateTime;

        public int GetPickupCount(FoodType type)
        {
            return type switch
            {
                FoodType.Carrot => carrotCount,
                FoodType.Cabbage => cabbageCount,
                FoodType.Tomato => tomatoCount,
                FoodType.Other => otherCount,
                _ => 0
            };
        }

        public int TotalPickups => carrotCount + cabbageCount + tomatoCount + otherCount;
    }

    [Serializable]
    public class RunStatsBoard
    {
        public List<RunStatsEntry> entries = new();
    }
}
