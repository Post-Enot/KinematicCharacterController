using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PostEnot.KinematicCharacterController
{
    [CustomPropertyDrawer(typeof(CharacterControllerShape))]
    public class CharacterControllerShapeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty heightProperty = property.FindPropertyRelative("<Height>k__BackingField");
            SerializedProperty radiusProperty = property.FindPropertyRelative("<Radius>k__BackingField");

            Foldout container = new()
            {
                text = property.displayName,
                value = property.isExpanded
            };
            FloatField heightField = new("Height")
            {
                value = heightProperty.floatValue,
                isDelayed = true
            };
            FloatField radiusField = new("Radius")
            {
                value = radiusProperty.floatValue,
                isDelayed = true
            };

            heightField.AddToClassList(BaseField<float>.alignedFieldUssClassName);
            radiusField.AddToClassList(BaseField<float>.alignedFieldUssClassName);

            container.Add(heightField);
            container.Add(radiusField);

            container.RegisterValueChangedCallback(context =>
            {
                property.isExpanded = context.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            heightField.RegisterValueChangedCallback(context =>
            {
                float height = Mathf.Max(context.newValue, KCC_Utility.MinHeight);
                float radius = KCC_Utility.CalculateSafeCharacterRadius(height, radiusField.value);
                SaveValues(height, radius, heightField, radiusField, heightProperty, radiusProperty);
            });
            radiusField.RegisterValueChangedCallback(context =>
            {
                float radius = Mathf.Max(context.newValue, KCC_Utility.MinRadius);
                float height = KCC_Utility.CalculateSafeCharacterHeight(heightField.value, radius);
                SaveValues(height, radius, heightField, radiusField, heightProperty, radiusProperty);
            });
            return container;
        }

        private void SaveValues(
            float height,
            float radius,
            FloatField heightField,
            FloatField radiusField,
            SerializedProperty heightProperty,
            SerializedProperty radiusProperty)
        {
            heightField.SetValueWithoutNotify(height);
            radiusField.SetValueWithoutNotify(radius);
            heightProperty.floatValue = height;
            radiusProperty.floatValue = radius;
            heightProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
