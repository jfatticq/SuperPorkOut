using SuperPorkOut.Characters.Player;
using SuperPorkOut.Core;
using SuperPorkOut.Gameplay.Pickups;
using SuperPorkOut.Levels;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SuperPorkOut.Screens.HUD
{
    /// <summary>
    /// Updates a Debug UI (UI Toolkit) with live gameplay stats, even while the UI is hidden.
    /// The UIDocument is disabled by default and toggled via the ` key (backquote).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public class DebugUIController : MonoBehaviour
    {
        [Header("Scene References")]
        [Tooltip("Player transform used for distance-to-farmer.")]
        [SerializeField] private Transform player;

        [SerializeField] private Stamina stamina;

        [Tooltip("Farmer transform used for distance-to-farmer.")]
        [SerializeField] private Transform farmer;

        [Tooltip("Reference to the GameStateBus for subscribing to game state events (captured, level end).")]
        [SerializeField] private GameStateBus gameStateBus;

        [Tooltip("Reference to the RunStatsRecorder for reading current run stats.")]
        [SerializeField] private RunStatsRecorder runStatsRecorder;

        [Header("Update Settings")]
        [Tooltip("How often (seconds) to refresh label text while the UI is enabled. 0 = every frame.")]
        [SerializeField, Min(0f)] private float uIRefreshInterval = 0.1f;

        private UIDocument debugUIDocument;

        private InputAction toggleDebugUIAction;

        // UI elements (queried when UIDocument is enabled)
        private Label lblDistanceToFarmer;
        private Label lblPickupsCollected;
        private Label lblTimeElapsed;
        private Label lblDistanceTraveled;
        private Label lblStamina;

        // Cached strings so we can keep "updating" even when UI is hidden/disabled
        private string cachedDistanceToFarmerText = "Distance To Farmer: ";
        private string cachedPickupsCollectedText = "Pickups Collected: ";
        private string cachedTimeElapsedText = "Time Elapsed: ";
        private string cachedDistanceTraveledText = "Distance Traveled: ";
        private string cachedStaminaText = "Stamina: ";

        private float nextUIRefreshTime;

        // Freeze on capture/end
        private bool frozenGameOver;

        private void Awake()
        {
            debugUIDocument = GetComponent<UIDocument>();

            // Default to hidden by disabling the document.
            if (debugUIDocument != null)
                debugUIDocument.enabled = false;

            if (stamina == null && player != null)
                stamina = player.GetComponent<Stamina>();
        }

        private void OnEnable()
        {
            WireInputAction();
            WireGameState();
        }

        private void OnDisable()
        {
            UnwireInputAction();
            UnwireGameState();
        }

        private void Update()
        {
            if (frozenGameOver)
            {
                PushToUIIfVisible();
                return;
            }

            UpdateCachedDistanceToFarmer();
            UpdateCachedRunStats();
            UpdateCachedStamina();

            PushToUIIfVisible();
        }

        private void UpdateCachedRunStats()
        {
            if (runStatsRecorder == null) return;

            cachedTimeElapsedText = $"Time Elapsed: {FormatTime(runStatsRecorder.ElapsedSeconds)}";
            cachedDistanceTraveledText = $"Distance Traveled: {runStatsRecorder.DistanceTraveled:0.0} m";

            var pickups = runStatsRecorder.PickupCounts;
            var sb = new StringBuilder(64);
            sb.Append("Pickups Collected: ");
            sb.Append("Carrot: ").Append(pickups[FoodType.Carrot]);
            sb.Append(" | Cabbage: ").Append(pickups[FoodType.Cabbage]);
            sb.Append(" | Tomato: ").Append(pickups[FoodType.Tomato]);

            int other = pickups[FoodType.Other];
            if (other > 0)
                sb.Append(" | Other: ").Append(other);

            cachedPickupsCollectedText = sb.ToString();
        }

        private void UpdateCachedStamina()
        {
            if (stamina != null)
            {
                float cur = stamina.Current;
                float max = stamina.Max;
                float pct = stamina.Normalized() * 100f;
                cachedStaminaText = $"Stamina: {cur:0}/{max:0} ({pct:0}%)";
            }
            else
            {
                cachedStaminaText = "Stamina: ";
            }
        }

        private void UpdateCachedDistanceToFarmer()
        {
            if (player == null || farmer == null)
            {
                cachedDistanceToFarmerText = "Distance To Farmer: ";
                return;
            }

            float d = Vector3.Distance(player.position, farmer.position);
            cachedDistanceToFarmerText = $"Distance To Farmer: {d:0.0} m";
        }

        private void PushToUIIfVisible()
        {
            if (!debugUIDocument.enabled)
                return;

            if (uIRefreshInterval > 0f && Time.unscaledTime < nextUIRefreshTime)
                return;

            EnsureUIReferences();
            if (lblDistanceToFarmer != null) lblDistanceToFarmer.text = cachedDistanceToFarmerText;
            if (lblPickupsCollected != null) lblPickupsCollected.text = cachedPickupsCollectedText;
            if (lblTimeElapsed != null) lblTimeElapsed.text = cachedTimeElapsedText;
            if (lblDistanceTraveled != null) lblDistanceTraveled.text = cachedDistanceTraveledText;
            if (lblStamina != null) lblStamina.text = cachedStaminaText;

            nextUIRefreshTime = Time.unscaledTime + uIRefreshInterval;
        }

        private void EnsureUIReferences()
        {
            // Query fresh when enabling (or if something got reloaded)
            var root = debugUIDocument.rootVisualElement;
            if (root == null) return;

            if (lblDistanceToFarmer != null &&
                lblPickupsCollected != null &&
                lblTimeElapsed != null &&
                lblDistanceTraveled != null)
            {
                return;
            }

            lblDistanceToFarmer = root.Q<Label>("lblDistanceToFarmer");
            lblPickupsCollected = root.Q<Label>("lblPickupsCollected");
            lblTimeElapsed = root.Q<Label>("lblTimeElapsed");
            lblDistanceTraveled = root.Q<Label>("lblDistanceTraveled");
            lblStamina = root.Q<Label>("lblStamina");
        }

        private void ToggleDocumentVisibility()
        {
            debugUIDocument.enabled = !debugUIDocument.enabled;

            if (debugUIDocument.enabled)
            {
                // Immediately push cached values so it "pops" correct on open
                EnsureUIReferences();
                nextUIRefreshTime = 0f;
                PushToUIIfVisible();
            }
            else
            {
                // Drop references; they�re invalid when UIDocument disabled
                lblDistanceToFarmer = null;
                lblPickupsCollected = null;
                lblTimeElapsed = null;
                lblDistanceTraveled = null;
            }
        }

        private void WireInputAction()
        {
            if (InputManager.Instance == null)
            {
                Debug.LogError("DebugUIController: InputManager.Instance is null. Ensure GameRoot is loaded first.", this);
                return;
            }

            toggleDebugUIAction = InputManager.Instance.Actions.Gameplay.ToggleDebugUI;
            toggleDebugUIAction.performed += OnTogglePerformed;
        }

        private void UnwireInputAction()
        {
            if (toggleDebugUIAction != null)
                toggleDebugUIAction.performed -= OnTogglePerformed;

            toggleDebugUIAction = null;
        }

        private void OnTogglePerformed(InputAction.CallbackContext _)
        {
            ToggleDocumentVisibility();
        }

        private void WireGameState()
        {
            if (gameStateBus == null) return;

            gameStateBus.Captured += OnCaptured;
            gameStateBus.LevelEnded += OnLevelEnded;
        }

        private void UnwireGameState()
        {
            if (gameStateBus == null) return;

            gameStateBus.Captured -= OnCaptured;
            gameStateBus.LevelEnded -= OnLevelEnded;
        }

        private void OnCaptured(CapturedEvent _)
        {
            FreezeOnGameOver();
        }

        private void OnLevelEnded(LevelEndedEvent _)
        {
            FreezeOnGameOver();
        }

        private void FreezeOnGameOver()
        {
            frozenGameOver = true;

            UpdateCachedDistanceToFarmer();
            UpdateCachedRunStats();
            UpdateCachedStamina();

            nextUIRefreshTime = 0f;
            PushToUIIfVisible();
        }

        private static string FormatTime(float seconds)
        {
            if (seconds < 0f) seconds = 0f;
            int total = Mathf.FloorToInt(seconds);
            int mins = total / 60;
            int secs = total % 60;
            return $"{mins:00}:{secs:00}";
        }
    }
}