using System.Collections.Generic;
using UnityEngine;

namespace SuperPorkOut.Characters.Player
{
    [RequireComponent(typeof(PlayerFacade))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Basis")]
        [Tooltip("If set, forward/strafe follow this transform (usually your camera rig or world forward anchor).")]
        [SerializeField] private Transform movementBasis;

        [Header("Physics")]
        [Tooltip("Extra downward force applied to the player to help them stick to the ground. Adjust as needed for your physics settings.")]
        [SerializeField, Min(0f)] private float extraDownForce = 10f;

        [Header("World X Clamp")]
        [SerializeField] private bool clampX = false;
        [SerializeField] private float minX = -8f;
        [SerializeField] private float maxX = 8f;

        private PlayerFacade player;
        private float strafeInput;

        /// <summary>
        /// Track collision normals from the last physics step
        /// </summary>
        private readonly List<Vector3> planarNormals = new(8);

        private void Awake()
        {
            player = GetComponent<PlayerFacade>();
        }

        private void Update()
        {
            // You said InputManager exists in your project. This keeps your current approach.
            InputManager im = InputManager.Instance;
            if (im == null || im.Actions == null)
            {
                strafeInput = 0f;
                return;
            }

            strafeInput = Mathf.Clamp(im.Actions.Gameplay.Move.ReadValue<Vector2>().x, -1f, 1f);
        }

        private void FixedUpdate()
        {
            var rb = player.Rigidbody;

            Vector3 fwd = movementBasis ? movementBasis.forward : Vector3.forward;
            Vector3 right = movementBasis ? movementBasis.right : Vector3.right;

            // 1) Desired planar velocity (what we WANT to do)
            Vector3 desiredPlanar = player.SpeedModel.GetPlanarVelocity(fwd, right, strafeInput);

            // 2) Adjust it based on current collision planes to produce sliding
            Vector3 adjustedPlanar = ConstrainToCollisionPlanes(desiredPlanar);

            // 3) Apply, preserve gravity/Y
            float y = rb.linearVelocity.y;
            rb.linearVelocity = new Vector3(adjustedPlanar.x, y, adjustedPlanar.z);

            if (extraDownForce > 0f)
                rb.AddForce(Vector3.down * extraDownForce, ForceMode.Acceleration);

            // --- Horizontal bounds clamp (position-based, slide-safe) ---
            if (clampX)
            {
                Vector3 pos = rb.position;

                float clampedX = Mathf.Clamp(pos.x, minX, maxX);
                if (!Mathf.Approximately(pos.x, clampedX))
                {
                    rb.position = new Vector3(clampedX, pos.y, pos.z);

                    // Optional: zero X velocity so you don't "buzz" against the wall
                    Vector3 v = rb.linearVelocity;
                    rb.linearVelocity = new Vector3(0f, v.y, v.z);
                }
            }

            // Clear normals for next step (they'll be repopulated by OnCollisionStay)
            planarNormals.Clear();
        }

        private Vector3 ConstrainToCollisionPlanes(Vector3 desiredPlanar)
        {
            Vector3 v = desiredPlanar;

            // Remove components pushing into any contact plane.
            // This naturally:
            // - blocks forward when hitting head-on (normal ~ -forward)
            // - blocks strafe when scraping side (normal ~ +/- right)
            // - allows sliding along the plane.
            for (int i = 0; i < planarNormals.Count; i++)
            {
                Vector3 n = planarNormals[i];

                float into = Vector3.Dot(v, n);
                if (into < 0f)
                    v -= into * n; // subtract the "into-wall" component
            }

            return v;
        }

        private void OnCollisionStay(Collision collision)
        {
            // Collect planar normals for this physics step.
            // We ignore mostly-vertical normals (ground) so movement isn't affected.
            int count = collision.contactCount;
            for (int i = 0; i < count; i++)
            {
                Vector3 n = collision.GetContact(i).normal;

                // Ignore floor/ceiling-ish normals
                if (Mathf.Abs(n.y) > 0.5f)
                    continue;

                n.y = 0f;
                float mag = n.magnitude;
                if (mag < 0.0001f) continue;

                planarNormals.Add(n / mag);
            }
        }
    }
}
