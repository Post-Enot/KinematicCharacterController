using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    [AddComponentMenu(KCC_Inner.AddComponentMenu + "/Kinematic Character System")]
    [DefaultExecutionOrder(-100), DisallowMultipleComponent]
    public sealed partial class KinematicCharacterSystem : MonoBehaviour
    {
        #region Inspector
        [SerializeField] private KCC_Settings settings;
        #endregion

        public KCC_Settings Settings => settings;
        public IReadOnlyList<KinematicCharacterMotor> CharacterMotors { get; private set; }
        public int CharacterMotorsCapacity => _characterMotors.Capacity;
        public IReadOnlyList<PhysicsMover> PhysicsMovers { get; private set; }
        public int PhysicsMoversCapacity => _physicsMovers.Capacity;

        private const string _gameObjectName = "KinematicCharacterSystem";

        [AutoStaticsCleanup] private static KinematicCharacterSystem _instance;

        private List<KinematicCharacterMotor> _characterMotors;
        private List<PhysicsMover> _physicsMovers;
        private float _lastCustomInterpolationStartTime = -1.0f;
        private float _lastCustomInterpolationDeltaTime = -1.0f;

        public void EnsureCharacterMotorsCapacity(int capacity)
        {
            if (capacity > _characterMotors.Count)
            {
                _characterMotors.Capacity = capacity;
            }
        }

        public void SetCharacterMotorsCapacity(int capacity) => _characterMotors.Capacity = capacity;

        public void EnsurePhysicsMoversCapacity(int capacity)
        {
            if (capacity > _physicsMovers.Count)
            {
                _physicsMovers.Capacity = capacity;
            }
        }

        public void SetPhysicsMoversCapacity(int capacity) => _physicsMovers.Capacity = capacity;

        public static void RegisterCharacterMotor(KinematicCharacterMotor motor)
        {
            _ = GetInstance();
            _instance._characterMotors.Add(motor);
        }

        public static void UnregisterCharacterMotor(KinematicCharacterMotor motor)
        {
            if (_instance != null)
            {
                _ = _instance._characterMotors.Remove(motor);
            }
        }

        public static void RegisterPhysicsMover(PhysicsMover mover)
        {
            _ = GetInstance();
            _instance._physicsMovers.Add(mover);
            mover.Rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public static void UnregisterPhysicsMover(PhysicsMover mover)
        {
            if (_instance != null)
            {
                _ = _instance._physicsMovers.Remove(mover);
            }
        }

        public static void PreSimulationInterpolationUpdate(
            IReadOnlyList<KinematicCharacterMotor> motors,
            IReadOnlyList<PhysicsMover> movers)
        {
            // Save pre-simulation poses and place transform at transient pose.
            for (int i = 0; i < motors.Count; i += 1)
            {
                KinematicCharacterMotor motor = motors[i];
                motor.InitialTickPosition = motor.TransientPosition;
                motor.InitialTickRotation = motor.TransientRotation.normalized;
                motor.transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            }

            for (int i = 0; i < movers.Count; i += 1)
            {
                PhysicsMover mover = movers[i];
                mover.InitialTickPosition = mover.TransientPosition;
                mover.InitialTickRotation = mover.TransientRotation.normalized;
                mover.transform.SetPositionAndRotation(mover.TransientPosition, mover.TransientRotation);
                mover.Rigidbody.position = mover.TransientPosition;
                mover.Rigidbody.rotation = mover.TransientRotation.normalized;
            }
        }

        public static void Simulate(float deltaTime, IReadOnlyList<KinematicCharacterMotor> motors, IReadOnlyList<PhysicsMover> movers)
        {
            int characterMotorsCount = motors.Count;
            int physicsMoversCount = movers.Count;

            // Update PhysicsMover velocities.
            for (int i = 0; i < physicsMoversCount; i += 1)
            {
                PhysicsMover mover = movers[i];
                mover.VelocityUpdate(deltaTime, mover.MoverController);
            }

            // Character controller update phase 1.
            for (int i = 0; i < characterMotorsCount; i += 1)
            {
                motors[i].UpdatePhase1(deltaTime);
            }

            // Simulate PhysicsMover displacement.
            for (int i = 0; i < physicsMoversCount; i += 1)
            {
                PhysicsMover mover = movers[i];
                mover.transform.SetPositionAndRotation(mover.TransientPosition, mover.TransientRotation);
                mover.Rigidbody.position = mover.TransientPosition;
                mover.Rigidbody.rotation = mover.TransientRotation.normalized;
            }

            // Character controller update phase 2 and move.
            for (int i = 0; i < characterMotorsCount; i += 1)
            {
                KinematicCharacterMotor motor = motors[i];
                motor.UpdatePhase2(deltaTime);
                motor.transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            }
        }

        public static void PostSimulationInterpolationUpdate(
            IReadOnlyList<KinematicCharacterMotor> motors,
            IReadOnlyList<PhysicsMover> movers)
        {
            // Return interpolated roots to their initial poses.
            for (int i = 0; i < motors.Count; i += 1)
            {
                KinematicCharacterMotor motor = motors[i];
                motor.transform.SetPositionAndRotation(motor.InitialTickPosition, motor.InitialTickRotation);
            }

            for (int i = 0; i < movers.Count; i += 1)
            {
                PhysicsMover mover = movers[i];
                if (mover.MoveWithPhysics)
                {
                    mover.Rigidbody.position = mover.InitialTickPosition;
                    mover.Rigidbody.rotation = mover.InitialTickRotation;
                    mover.Rigidbody.MovePosition(mover.TransientPosition);
                    mover.Rigidbody.MoveRotation(mover.TransientRotation.normalized);
                }
                else
                {
                    mover.Rigidbody.position = mover.TransientPosition;
                    mover.Rigidbody.rotation = mover.TransientRotation.normalized;
                }
            }
        }

        public static void CustomInterpolationUpdate(
            float lastStartTime,
            float lastDeltaTime,
            IReadOnlyList<KinematicCharacterMotor> motors,
            IReadOnlyList<PhysicsMover> movers)
        {
            float deltaTime = Time.time - lastStartTime;
            float interpolationFactorUnclamped = deltaTime / lastDeltaTime;
            float interpolationFactor = Mathf.Clamp01(interpolationFactorUnclamped);

            // Handle characters interpolation:
            for (int i = 0; i < motors.Count; i += 1)
            {
                KinematicCharacterMotor motor = motors[i];
                Vector3 position = Vector3.Lerp(motor.InitialTickPosition, motor.TransientPosition, interpolationFactor);
                Quaternion rotation = Quaternion.Slerp(motor.InitialTickRotation, motor.TransientRotation, interpolationFactor);
                motor.transform.SetPositionAndRotation(position, rotation);
            }

            // Handle PhysicsMovers interpolation:
            for (int i = 0; i < movers.Count; i += 1)
            {
                PhysicsMover mover = movers[i];
                Vector3 position = Vector3.Lerp(mover.InitialTickPosition, mover.TransientPosition, interpolationFactor);
                Quaternion rotation = Quaternion.Slerp(mover.InitialTickRotation, mover.TransientRotation, interpolationFactor);
                mover.transform.SetPositionAndRotation(position, rotation);
                mover.transform.GetPositionAndRotation(out Vector3 newPosition, out Quaternion newRot);
                mover.PositionDeltaFromInterpolation = newPosition - mover.LatestInterpolationPosition;
                mover.RotationDeltaFromInterpolation = Quaternion.Inverse(mover.LatestInterpolationRotation) * newRot;
                mover.LatestInterpolationPosition = newPosition;
                mover.LatestInterpolationRotation = newRot;
            }
        }

        public static KinematicCharacterSystem GetInstance()
        {
            if (_instance == null)
            {
                GameObject systemGameObject = new(_gameObjectName);
                _instance = systemGameObject.AddComponent<KinematicCharacterSystem>();
                systemGameObject.hideFlags = HideFlags.NotEditable;
                _instance.hideFlags = HideFlags.NotEditable;
                DontDestroyOnLoad(systemGameObject);
                _instance.settings = ScriptableObject.CreateInstance<KCC_Settings>();
                _instance.Init();
            }
            return _instance;
        }

        private void Awake()
        {
            if (settings == null)
            {
                return;
            }
            _instance = this;
            Init();
        }

        private void Start()
        {
            if ((settings == null) || (_instance != this))
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (!settings.AutoSimulation)
            {
                return;
            }
            float deltaTime = Time.deltaTime;
            if (settings.Interpolate)
            {
                PreSimulationInterpolationUpdate(_characterMotors, _physicsMovers);
            }
            Simulate(deltaTime, _characterMotors, _physicsMovers);
            if (settings.Interpolate)
            {
                _lastCustomInterpolationStartTime = Time.time;
                _lastCustomInterpolationDeltaTime = deltaTime;
                PostSimulationInterpolationUpdate(_characterMotors, _physicsMovers);
            }
        }

        private void LateUpdate()
        {
            if (settings.Interpolate)
            {
                CustomInterpolationUpdate(
                    _lastCustomInterpolationStartTime,
                    _lastCustomInterpolationDeltaTime,
                    _characterMotors,
                    _physicsMovers);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            _characterMotors.Clear();
            _physicsMovers.Clear();
        }

        private void Init()
        {
            if (settings.MotorsListInitialCapacity <= 0)
            {
                _characterMotors = new List<KinematicCharacterMotor>();
            }
            else
            {
                _characterMotors = new List<KinematicCharacterMotor>(settings.MotorsListInitialCapacity);
            }
            if (settings.MoversListInitialCapacity <= 0)
            {
                _physicsMovers = new List<PhysicsMover>();
            }
            else
            {
                _physicsMovers = new List<PhysicsMover>(settings.MoversListInitialCapacity);
            }
            CharacterMotors = new ReadOnlyCollection<KinematicCharacterMotor>(_characterMotors);
            PhysicsMovers = new ReadOnlyCollection<PhysicsMover>(_physicsMovers);
        }
    }
}
