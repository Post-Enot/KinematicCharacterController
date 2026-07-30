#nullable enable

using System;
using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    /// <summary>
    /// Represents the entire state of a character motor that is pertinent for simulation.
    /// Use this to save state or revert to past state
    /// </summary>
    [Serializable]
    public struct KinematicCharacterMotorState
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 BaseVelocity;

        public bool MustUnground;
        public float MustUngroundTime;
        public bool LastMovementIterationFoundAnyGround;
        public CharacterTransientGroundingReport GroundingStatus;

        public Rigidbody? AttachedRigidbody;
        public Vector3 AttachedRigidbodyVelocity;
    }
}
