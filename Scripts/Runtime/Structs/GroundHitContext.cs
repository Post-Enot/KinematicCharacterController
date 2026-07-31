#nullable enable

using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    public readonly struct GroundHitContext
    {
        public GroundHitContext(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint)
        {
            HitCollider = hitCollider;
            HitNormal = hitNormal;
            HitPoint = hitPoint;
        }

        public readonly Collider HitCollider { get; }
        public readonly Vector3 HitNormal { get; }
        public readonly Vector3 HitPoint { get; }
    }
}
