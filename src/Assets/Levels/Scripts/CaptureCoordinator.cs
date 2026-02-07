using SuperPorkOut.Characters.Farmer;
using SuperPorkOut.Characters.Player;
using UnityEngine;

namespace SuperPorkOut.Levels
{
    public class CaptureCoordinator : MonoBehaviour
    {
        [SerializeField] private GameStateBus gameStateBus;

        [SerializeField] private CapturedScreenController capturedScreen;

        [Header("Freeze Options")]
        [SerializeField] private bool freezeFarmerAlso = false;

        [Header("Optional Animations")]
        [SerializeField] private string pigCapturedTrigger = "Captured";
        [SerializeField] private string farmerCapturedTrigger = "Captured";

        private void Awake()
        {
            if (gameStateBus == null)
                gameStateBus = FindFirstObjectByType<GameStateBus>();
        }

        private void OnEnable()
        {
            if (gameStateBus != null)
                gameStateBus.Captured += OnCaptured;
        }

        private void OnDisable()
        {
            if (gameStateBus != null)
                gameStateBus.Captured -= OnCaptured;
        }

        private void OnCaptured(CapturedEvent evt)
        {
            // Stop pig movement
            var pigState = evt.pig.GetComponent<CaptureState>() ?? evt.pig.GetComponentInParent<CaptureState>();
            if (pigState != null) pigState.SetCaptured(true);

            // Optional farmer freeze
            if (freezeFarmerAlso)
            {
                var farmerFreeze = evt.farmer.GetComponent<CaptureFreeze>() ?? evt.farmer.GetComponentInParent<CaptureFreeze>();
                if (farmerFreeze != null) farmerFreeze.SetFrozen(true);
            }

            // Optional animations
            TryTrigger(evt.pig, pigCapturedTrigger);
            TryTrigger(evt.farmer, farmerCapturedTrigger);

            // Show UI + fade
            if (capturedScreen != null)
                capturedScreen.Show();
        }

        private static void TryTrigger(GameObject go, string triggerName)
        {
            if (go == null || string.IsNullOrEmpty(triggerName)) return;

            var anim = go.GetComponentInChildren<Animator>();
            if (anim == null) return;

            int hash = Animator.StringToHash(triggerName);
            if (!HasTrigger(anim, hash)) return; // <- no warning spam

            anim.SetTrigger(hash);
        }

        private static bool HasTrigger(Animator anim, int triggerHash)
        {
            // Small and cheap (only called on capture)
            foreach (var p in anim.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger && p.nameHash == triggerHash)
                    return true;
            }
            return false;
        }
    }
}
