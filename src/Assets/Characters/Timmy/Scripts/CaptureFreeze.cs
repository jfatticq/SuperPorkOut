using UnityEngine;

namespace SuperPorkOut.Characters.Farmer
{
    public class CaptureFreeze : MonoBehaviour
    {
        [SerializeField] private Behaviour[] behavioursToDisable;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private bool zeroPlanarVelocity = true;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
        }

        public void SetFrozen(bool frozen)
        {
            if (behavioursToDisable != null)
            {
                foreach (var b in behavioursToDisable)
                    if (b != null) b.enabled = !frozen;
            }

            if (rb != null && frozen && zeroPlanarVelocity)
            {
                var v = rb.linearVelocity;
                rb.linearVelocity = new Vector3(0f, v.y, 0f);
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
