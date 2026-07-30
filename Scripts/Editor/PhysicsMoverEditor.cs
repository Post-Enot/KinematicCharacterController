using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PostEnot.KinematicCharacterController.Editor
{
    [CustomEditor(typeof(PhysicsMover))]
    internal sealed class PhysicsMoverEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            PropertyField rigidbodyPropertyField = container.Q<PropertyField>("PropertyField:rigidbody");
            rigidbodyPropertyField.SetEnabled(false);
            return container;
        }
    }
}
