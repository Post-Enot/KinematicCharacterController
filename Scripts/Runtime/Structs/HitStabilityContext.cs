#nullable enable

using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    public readonly struct HitStabilityContext
    {
        public HitStabilityContext(
            Collider hitCollider,
            Vector3 hitNormal,
            Vector3 hitPoint,
            Vector3 atCharacterPosition,
            Quaternion atCharacterRotation)
        {
            HitCollider = hitCollider;
            HitNormal = hitNormal;
            HitPoint = hitPoint;
            AtCharacterPosition = atCharacterPosition;
            AtCharacterRotation = atCharacterRotation;
        }

        public readonly Collider HitCollider {get; }
        public readonly Vector3 HitNormal {get; }
        public readonly Vector3 HitPoint {get; }
        public readonly Vector3 AtCharacterPosition {get; }
        public readonly Quaternion AtCharacterRotation { get; }
    }
}
