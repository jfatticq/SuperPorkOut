using SuperPorkOut.Characters.Player;
using SuperPorkOut.Levels;
using UnityEngine;

namespace SuperPorkOut.Gameplay
{
    public class EndLevelCoordinator : MonoBehaviour
    {
        [SerializeField] private GameStateBus gameStateBus;
        [SerializeField] private EndLevelScreenController endLevelScreen;

        [Header("Freeze")]
        [SerializeField] private bool freezePig = true;
        [SerializeField] private bool freezeFarmer = true;

        [Header("Optional Animations")]
        [SerializeField] private string pigEndLevelTrigger = "";     // e.g. "Win"
        [SerializeField] private string farmerEndLevelTrigger = "";  // e.g. "Stop"

        private void Awake()
        {
            if (gameStateBus == null)
                gameStateBus = FindFirstObjectByType<GameStateBus>();
        }

        private void OnEnable()
        {
            if (gameStateBus != null)
                gameStateBus.LevelEnded += OnLevelEnded;
        }

        private void OnDisable()
        {
            if (gameStateBus != null)
                gameStateBus.LevelEnded -= OnLevelEnded;
        }

        private void OnLevelEnded(LevelEndedEvent evt)
        {
            // -------- Pig --------
            if (freezePig && evt.pig != null)
            {
                // Disable player movement logic
                if (!evt.pig.TryGetComponent<CaptureState>(out var cap))
                {
                    cap = evt.pig.GetComponentInParent<CaptureState>();
                }
                if (cap != null)
                {
                    cap.SetCaptured(true);
                }

                // Freeze transform (no rigidbody required)
                if (!evt.pig.TryGetComponent<FreezeTransformMotion>(out var pigFreeze))
                {
                    pigFreeze = evt.pig.GetComponentInParent<FreezeTransformMotion>();
                }
                if (pigFreeze != null)
                {
                    pigFreeze.Freeze(true);
                }

                TryTrigger(evt.pig, pigEndLevelTrigger);
            }

            // -------- Farmer --------
            if (freezeFarmer && evt.farmer != null)
            {
                if (!evt.farmer.TryGetComponent<FreezeTransformMotion>(out var farmerFreeze))
                {
                    farmerFreeze = evt.farmer.GetComponentInParent<FreezeTransformMotion>();
                }
                if (farmerFreeze != null)
                {
                    farmerFreeze.Freeze(true);
                }

                TryTrigger(evt.farmer, farmerEndLevelTrigger);
            }

            // -------- UI --------
            if (endLevelScreen != null)
            {
                endLevelScreen.Show();
            }
        }

        private static void TryTrigger(GameObject go, string triggerName)
        {
            if (go == null) return;
            if (string.IsNullOrEmpty(triggerName)) return;

            Animator anim = go.GetComponentInChildren<Animator>();
            if (anim == null) return;

            int hash = Animator.StringToHash(triggerName);

            AnimatorControllerParameter[] parameters = anim.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter p = parameters[i];
                if (p.type == AnimatorControllerParameterType.Trigger && p.nameHash == hash)
                {
                    anim.SetTrigger(hash);
                    return;
                }
            }
        }
    }
}
