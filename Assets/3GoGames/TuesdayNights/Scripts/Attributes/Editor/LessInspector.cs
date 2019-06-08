using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Less))]
public class LessInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null)
            return;

        EditorGUI.BeginChangeCheck();

        Less greaterAttribute = (Less)attribute;

        int intThreshold = greaterAttribute.intThreshold;

        EditorGUI.PropertyField(position, property, label);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            if (property.intValue >= intThreshold)
            {
                property.intValue = intThreshold - 1;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
