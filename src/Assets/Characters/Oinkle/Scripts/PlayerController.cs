using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(PlayerFacade))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Basis")]
        [Tooltip("If set, forward/strafe follow this transform (usually your camera rig or world forward anchor).")]
        [SerializeField] private Transform movementBasis;

        [Header("Physics")]
        [SerializeField, Min(0f)] private float extraDownForce = 10f;

        [Header("World X Clamp")]
        [SerializeField] private bool clampX = false;
        [SerializeField] private float minX = -8f;
        [SerializeField] private float maxX = 8f;

        private PlayerFacade player;
        private float strafeInput;

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

            Vector3 basisForward = movementBasis ? movementBasis.forward : Vector3.forward;
            Vector3 basisRight = movementBasis ? movementBasis.right : Vector3.right;

            Vector3 planarVel = player.SpeedModel.GetPlanarVelocity(basisForward, basisRight, strafeInput);

            // Preserve Y velocity
            float yVel = rb.linearVelocity.y;
            rb.linearVelocity = new Vector3(planarVel.x, yVel, planarVel.z);

            if (extraDownForce > 0f)
                rb.AddForce(Vector3.down * extraDownForce, ForceMode.Acceleration);

            if (clampX)
            {
                Vector3 pos = rb.position;
                float clampedX = Mathf.Clamp(pos.x, minX, maxX);
                if (!Mathf.Approximately(pos.x, clampedX))
                    rb.MovePosition(new Vector3(clampedX, pos.y, pos.z));
            }
        }
    }
}
