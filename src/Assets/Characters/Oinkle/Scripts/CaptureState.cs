using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerFacade))]
    public class CaptureState : MonoBehaviour
    {
        [SerializeField] private bool freezeRigidbodyPlanarVelocity = true;

        private PlayerController controller;

        private PlayerFacade facade;

        private bool isCaptured;

        public bool IsCaptured => isCaptured;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            facade = GetComponent<PlayerFacade>();
        }

        public void SetCaptured(bool captured)
        {
            isCaptured = captured;

            // Disable player movement logic (cleanest)
            controller.enabled = !captured;

            if (captured && freezeRigidbodyPlanarVelocity)
            {
                var rb = facade.Rigidbody;
                Vector3 v = rb.linearVelocity;
                rb.linearVelocity = new Vector3(0f, v.y, 0f);
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
