#nullable enable

using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    /// <summary>
    /// Component that manages the movement of moving kinematic rigidbodies for
    /// proper interaction with characters
    /// </summary>
    [AddComponentMenu(KCC_Inner.AddComponentMenu + "/Physics Mover")]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsMover : MonoBehaviour
    {
        #region Inspector
        [Tooltip("The mover's Rigidbody.")]
        [SerializeField] private Rigidbody rigidbody = default!;
        [Tooltip("Determines if the platform moves with rigidbody.MovePosition (when true), or with rigidbody.position (when false).")]
        [SerializeField] private bool moveWithPhysics = true;

        private void Reset() => ValidateData();

        private void OnValidate() => ValidateData();
        #endregion

        public IMoverController? MoverController { get; set; }
        /// <summary>
        /// Remembers latest position in interpolation.
        /// </summary>
        public Vector3 LatestInterpolationPosition { get; internal set; }
        /// <summary>
        /// Remembers latest rotation in interpolation.
        /// </summary>
        public Quaternion LatestInterpolationRotation { get; internal set; }
        /// <summary>
        /// The latest movement made by interpolation.
        /// </summary>
        public Vector3 PositionDeltaFromInterpolation { get; internal set; }
        /// <summary>
        /// The latest rotation made by interpolation.
        /// </summary>
        public Quaternion RotationDeltaFromInterpolation { get; internal set; }
        /// <summary>
        /// Remembers initial position before all simulation are done.
        /// </summary>
        public Vector3 Velocity { get; protected set; }
        public Vector3 AngularVelocity { get; protected set; }
        public Vector3 InitialTickPosition { get; internal set; }
        /// <summary>
        /// Remembers initial rotation before all simulation are done.
        /// </summary>
        public Quaternion InitialTickRotation { get; internal set; }
        /// <summary>
        /// The character's position before the movement calculations began.
        /// </summary>
        public Vector3 InitialSimulationPosition { get; private set; }
        /// <summary>
        /// The character's rotation before the movement calculations began.
        /// </summary>
        public Quaternion InitialSimulationRotation { get; private set; }
        /// <summary>
        /// The mover's Rigidbody.
        /// </summary>
        public Rigidbody Rigidbody => rigidbody;
        /// <summary>
        /// Determines if the platform moves with <see cref="Rigidbody.MovePosition"/>(when <see langword="true"/>),
        /// or with <see cref="Rigidbody.position"/> (when <see langword="false"/>).
        /// </summary>
        public bool MoveWithPhysics
        {
            get => moveWithPhysics;
            set => moveWithPhysics = value;
        }
        /// <summary>
        /// The mover's position (always up-to-date during the character update phase)
        /// </summary>
        public Vector3 TransientPosition
        {
            get => _transientPosition;
            private set => _transientPosition = value;
        }
        /// <summary>
        /// The mover's rotation (always up-to-date during the character update phase)
        /// </summary>
        public Quaternion TransientRotation
        {
            get => _transientRotation;
            private set => _transientRotation = value;
        }

        private Vector3 _transientPosition;
        private Quaternion _transientRotation;

        /// <summary>
        /// Handle validating all required values
        /// </summary>
        public void ValidateData()
        {
            rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.centerOfMass = Vector3.zero;
            rigidbody.maxAngularVelocity = Mathf.Infinity;
            rigidbody.maxDepenetrationVelocity = Mathf.Infinity;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        /// <summary>
        /// Sets the mover's position directly
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            rigidbody.position = position;
            InitialSimulationPosition = position;
            _transientPosition = position;
        }

        /// <summary>
        /// Sets the mover's rotation directly
        /// </summary>
        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
            rigidbody.rotation = rotation;
            InitialSimulationRotation = rotation;
            _transientRotation = rotation;
        }

        /// <summary>
        /// Sets the mover's position and rotation directly
        /// </summary>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            rigidbody.position = position;
            rigidbody.rotation = rotation;
            InitialSimulationPosition = position;
            InitialSimulationRotation = rotation;
            _transientPosition = position;
            _transientRotation = rotation;
        }

        /// <summary>
        /// Returns all the state information of the mover that is pertinent for simulation
        /// </summary>
        public PhysicsMoverState GetState() => new()
            {
                Position = _transientPosition,
                Rotation = _transientRotation,
                Velocity = Velocity,
                AngularVelocity = AngularVelocity
            };

        /// <summary>
        /// Applies a mover state instantly
        /// </summary>
        public void ApplyState(PhysicsMoverState state)
        {
            SetPositionAndRotation(state.Position, state.Rotation);
            Velocity = state.Velocity;
            AngularVelocity = state.AngularVelocity;
        }

        /// <summary>
        /// Caches velocity values based on deltatime and target position/rotations
        /// </summary>
        internal void VelocityUpdate(float deltaTime, IMoverController mover)
        {
            InitialSimulationPosition = _transientPosition;
            InitialSimulationRotation = _transientRotation;
            mover.UpdateMovement(out _transientPosition, out _transientRotation, deltaTime);
            if (deltaTime > 0.0f)
            {
                Velocity = (_transientPosition - InitialSimulationPosition) / deltaTime;                                
                Quaternion rotationFromCurrentToGoal = _transientRotation * Quaternion.Inverse(InitialSimulationRotation);
                AngularVelocity = Mathf.Deg2Rad * rotationFromCurrentToGoal.eulerAngles / deltaTime;
            }
        }

        private void Awake()
        {
            ValidateData();
            _transientPosition = rigidbody.position;
            _transientRotation = rigidbody.rotation;
            InitialSimulationPosition = rigidbody.position;
            InitialSimulationRotation = rigidbody.rotation;
            LatestInterpolationPosition = transform.position;
            LatestInterpolationRotation = transform.rotation;
        }

        private void OnEnable() => KinematicCharacterSystem.RegisterPhysicsMover(this);

        private void OnDisable() => KinematicCharacterSystem.UnregisterPhysicsMover(this);
    }
}
