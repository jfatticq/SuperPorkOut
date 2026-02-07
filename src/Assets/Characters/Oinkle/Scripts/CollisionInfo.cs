using UnityEngine;

namespace Characters.Player
{
    /// <summary>
    /// Lightweight collision info passed to IPlayerCollisionInteractable.
    /// </summary>
    public readonly struct CollisionInfo
    {
        public readonly Vector3 Point;

        public readonly Vector3 Normal;

        public readonly Vector3 RelativeVelocity;

        public readonly float ImpulseMagnitude;

        public readonly Collider OtherCollider;

        public CollisionInfo(Vector3 point, Vector3 normal, Vector3 relativeVelocity, float impulseMagnitude, Collider otherCollider)
        {
            Point = point;
            Normal = normal;
            RelativeVelocity = relativeVelocity;
            ImpulseMagnitude = impulseMagnitude;
            OtherCollider = otherCollider;
        }

        public static CollisionInfo FromCollision(Collision c)
        {
            ContactPoint cp = c.GetContact(0);
            return new CollisionInfo(
                cp.point,
                cp.normal,
                c.relativeVelocity,
                c.impulse.magnitude,
                c.collider
            );
        }
    }
}
