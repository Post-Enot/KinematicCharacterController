using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.KinematicCharacterController.Editor
{
    [CustomEditor(typeof(KinematicCharacterMotor))]
    internal sealed class KinematicCharacterMotorEditor : UnityEditor.Editor
    {
        private VisualElement _container;
        private PropertyField _planarConstraintAxisField;
        private PropertyField _maxStableDistanceFromLedgeField;
        private PropertyField _maxVelocityForLedgeSnapField;
        private PropertyField _maxStableDenivelationAngleField;
        private PropertyField _maxStepHeightField;
        private PropertyField _allowSteppingWithoutStableGroundingField;
        private PropertyField _minRequiredStepDepthField;
        private PropertyField _simulatedCharacterMassField;
        private PropertyField _preserveAttachedRigidbodyMomentumField;

        private void OnSceneGUI()
        {
            if (target is not KinematicCharacterMotor motor)
            {
                return;
            }
            Vector3 characterBottom = motor.transform.position + (motor.Capsule.center + (-Vector3.up * (motor.Capsule.height * 0.5f)));
            Handles.color = Color.yellow;
            Handles.CircleHandleCap(
                0,
                characterBottom + (motor.transform.up * motor.MaxStepHeight),
                Quaternion.LookRotation(motor.transform.up, motor.transform.forward),
                motor.Capsule.radius + 0.1f,
                EventType.Repaint);
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (target is KinematicCharacterMotor motor)
            {
                motor.Capsule.hideFlags = HideFlags.NotEditable;
            }
            _container = new VisualElement();
            InspectorElement.FillDefaultInspector(_container, serializedObject, this);
            PropertyField capsuleField = _container.Q<PropertyField>("PropertyField:capsule");
            capsuleField.SetEnabled(false);
            _planarConstraintAxisField = _container.Q<PropertyField>("PropertyField:planarConstraintAxis");
            _maxStableDistanceFromLedgeField = _container.Q<PropertyField>("PropertyField:maxStableDistanceFromLedge");
            _maxVelocityForLedgeSnapField = _container.Q<PropertyField>("PropertyField:maxVelocityForLedgeSnap");
            _maxStableDenivelationAngleField = _container.Q<PropertyField>("PropertyField:maxStableDenivelationAngle");
            _maxStepHeightField = _container.Q<PropertyField>("PropertyField:maxStepHeight");
            _allowSteppingWithoutStableGroundingField = _container.Q<PropertyField>("PropertyField:allowSteppingWithoutStableGrounding");
            _minRequiredStepDepthField = _container.Q<PropertyField>("PropertyField:minRequiredStepDepth");
            _simulatedCharacterMassField = _container.Q<PropertyField>("PropertyField:simulatedCharacterMass");
            _preserveAttachedRigidbodyMomentumField = _container.Q<PropertyField>("PropertyField:preserveAttachedRigidbodyMomentum");
            PropertyField hasPlanarConstraintField = _container.Q<PropertyField>("PropertyField:hasPlanarConstraint");
            PropertyField ledgeHandlingField = _container.Q<PropertyField>("PropertyField:ledgeAndDenivelationHandling");
            PropertyField stepHandlingField = _container.Q<PropertyField>("PropertyField:stepHandling");
            PropertyField interactiveRigidbodyField = _container.Q<PropertyField>("PropertyField:interactiveRigidbodyHandling");
            hasPlanarConstraintField?.RegisterValueChangeCallback(OnValueChanged);
            ledgeHandlingField?.RegisterValueChangeCallback(OnValueChanged);
            stepHandlingField?.RegisterValueChangeCallback(OnValueChanged);
            interactiveRigidbodyField?.RegisterValueChangeCallback(OnValueChanged);
            UpdateVisibility();
            return _container;
        }

        private void OnValueChanged(SerializedPropertyChangeEvent context) => UpdateVisibility();

        private void UpdateVisibility()
        {
            if (serializedObject.targetObject == null)
            {
                return;
            }

            // Плоскостное ограничение
            SerializedProperty hasPlanarProperty = serializedObject.FindProperty("hasPlanarConstraint");
            SetVisible(_planarConstraintAxisField, hasPlanarProperty.boolValue);

            // Обработка уступов/денивеляции
            SerializedProperty ledgeHandlingProperty = serializedObject.FindProperty("ledgeAndDenivelationHandling");
            bool ledgeHandling = ledgeHandlingProperty.boolValue;
            SetVisible(_maxStableDistanceFromLedgeField, ledgeHandling);
            SetVisible(_maxVelocityForLedgeSnapField, ledgeHandling);
            SetVisible(_maxStableDenivelationAngleField, ledgeHandling);

            // Обработка шагов
            SerializedProperty stepProp = serializedObject.FindProperty("stepHandling");
            StepHandlingMethod stepMethod = (StepHandlingMethod)stepProp.enumValueIndex;
            bool showStepCommon = stepMethod != StepHandlingMethod.None;
            bool showMinDepth = stepMethod == StepHandlingMethod.Extra;

            SetVisible(_maxStepHeightField, showStepCommon);
            SetVisible(_allowSteppingWithoutStableGroundingField, showStepCommon);
            SetVisible(_minRequiredStepDepthField, showMinDepth);

            // Взаимодействие с Rigidbody
            SerializedProperty interactiveProperty = serializedObject.FindProperty("interactiveRigidbodyHandling");
            bool interactive = interactiveProperty.boolValue;
            SetVisible(_simulatedCharacterMassField, interactive);
            SetVisible(_preserveAttachedRigidbodyMomentumField, interactive);
        }

        private void SetVisible(VisualElement element, bool visible)
        {
            if (element != null)
            {
                element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
