using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Greater))]
public class GreaterInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null)
            return;

        EditorGUI.BeginChangeCheck();

        Greater greaterAttribute = (Greater)attribute;

        int intThreshold = greaterAttribute.intThreshold;

        EditorGUI.PropertyField(position, property, label);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            if (property.intValue <= intThreshold)
            {
                property.intValue = intThreshold + 1;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
