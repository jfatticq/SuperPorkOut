using System;
using System.Collections.Generic;
using SuperPorkOut.Gameplay.Pickups;
using SuperPorkOut.Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperPorkOut.Core
{
    [RequireComponent(typeof(GameStateBus))]
    public class RunStatsRecorder : MonoBehaviour
    {
        [Tooltip("The player transform to track distance traveled. If not set, distance will not be tracked.")]
        [SerializeField] private Transform player;

        private GameStateBus gameStateBus;

        private float elapsedSeconds;
        private float distanceTraveled;
        private Vector3 lastPlayerPos;
        private bool hasLastPlayerPos;
        private bool frozen;

        private readonly Dictionary<FoodType, int> pickupCounts = new()
        {
            { FoodType.Carrot, 0 },
            { FoodType.Cabbage, 0 },
            { FoodType.Tomato, 0 },
            { FoodType.Other, 0 }
        };

        public float DistanceTraveled => distanceTraveled;
        public float ElapsedSeconds => elapsedSeconds;
        public IReadOnlyDictionary<FoodType, int> PickupCounts => pickupCounts;

        private void Awake()
        {
            if (player != null)
            {
                lastPlayerPos = player.position;
                hasLastPlayerPos = true;
            }

            gameStateBus = GetComponent<GameStateBus>();
        }

        private void OnEnable()
        {
            PowerUp.PickedUp += OnPickedUp;

            if (gameStateBus != null)
            {
                gameStateBus.LevelEnded += OnLevelEnded;
                gameStateBus.LevelEnded += OnCaptured;
            }
        }

        private void OnDisable()
        {
            PowerUp.PickedUp -= OnPickedUp;

            if (gameStateBus != null)
            {
                gameStateBus.LevelEnded -= OnLevelEnded;
                gameStateBus.LevelEnded -= OnCaptured;
            }
        }

        private void Update()
        {
            if (frozen) return;
            if (InputManager.Instance == null || InputManager.Instance.Mode != GameMode.Playing)
                return;

            elapsedSeconds += Time.deltaTime;

            if (player == null) return;

            if (!hasLastPlayerPos)
            {
                lastPlayerPos = player.position;
                hasLastPlayerPos = true;
                return;
            }

            distanceTraveled += Vector3.Distance(lastPlayerPos, player.position);
            lastPlayerPos = player.position;
        }

        private void OnPickedUp(PickupEventData data)
        {
            if (frozen) return;

            if (!pickupCounts.ContainsKey(data.Type))
                pickupCounts[data.Type] = 0;

            pickupCounts[data.Type]++;
        }

        private void OnLevelEnded(LevelEndedEvent evt)
        {
            frozen = true;

            var entry = new RunStatsEntry
            {
                distanceTraveled = distanceTraveled,
                timeElapsed = elapsedSeconds,
                carrotCount = pickupCounts[FoodType.Carrot],
                cabbageCount = pickupCounts[FoodType.Cabbage],
                tomatoCount = pickupCounts[FoodType.Tomato],
                otherCount = pickupCounts[FoodType.Other],
                sceneName = SceneManager.GetActiveScene().name,
                dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            RunStatsStore.Save(entry.sceneName, entry);
            RunStatsStore.Load(entry.sceneName);
        }

        private void OnCaptured(LevelEndedEvent evt)
        {
            frozen = true;

            var entry = new RunStatsEntry
            {
                distanceTraveled = distanceTraveled,
                timeElapsed = elapsedSeconds,
                carrotCount = pickupCounts[FoodType.Carrot],
                cabbageCount = pickupCounts[FoodType.Cabbage],
                tomatoCount = pickupCounts[FoodType.Tomato],
                otherCount = pickupCounts[FoodType.Other],
                sceneName = SceneManager.GetActiveScene().name,
                dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            RunStatsStore.Save(entry.sceneName, entry);
            RunStatsStore.Load(entry.sceneName);
        }
    }
}
