using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    /// <summary>
    /// Describes an overlap between the character capsule and another collider
    /// </summary>
    public struct OverlapResult
    {
        public OverlapResult(Vector3 normal, Collider collider)
        {
            Normal = normal;
            Collider = collider;
        }

        public Vector3 Normal;
        public Collider Collider;
    }
}
