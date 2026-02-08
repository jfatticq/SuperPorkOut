using UnityEngine;

namespace SuperPorkOut.Gameplay
{
    /// <summary>
    /// Freezes an object in place without requiring a Rigidbody.
    /// - Disables specified movement behaviours (e.g., FarmerController)
    /// - Optionally pins the Transform in LateUpdate so it can't be moved by anything else
    /// </summary>
    public class FreezeTransformMotion : MonoBehaviour
    {
        [Header("Behaviours to disable on freeze (FarmerController, AI scripts, etc.)")]
        [SerializeField] private Behaviour[] behavioursToDisable;

        [Header("Pin transform even if something else tries to move it")]
        [SerializeField] private bool pinPosition = true;

        [Tooltip("Usually false so idle animations can still rotate bones/children. Turn on if something rotates the whole farmer transform.")]
        [SerializeField] private bool pinRotation = false;

        private bool frozen;
        private Vector3 frozenPosition;
        private Quaternion frozenRotation;

        public bool IsFrozen => frozen;

        /// <summary>Freeze or unfreeze movement.</summary>
        public void Freeze(bool shouldFreeze)
        {
            if (frozen == shouldFreeze) return;

            frozen = shouldFreeze;

            if (shouldFreeze)
            {
                frozenPosition = transform.position;
                frozenRotation = transform.rotation;
            }

            if (behavioursToDisable != null)
            {
                foreach (var b in behavioursToDisable)
                {
                    if (b != null)
                        b.enabled = !shouldFreeze;
                }
            }
        }

        private void LateUpdate()
        {
            if (!frozen) return;

            if (pinPosition)
                transform.position = frozenPosition;

            if (pinRotation)
                transform.rotation = frozenRotation;
        }
    }
}
