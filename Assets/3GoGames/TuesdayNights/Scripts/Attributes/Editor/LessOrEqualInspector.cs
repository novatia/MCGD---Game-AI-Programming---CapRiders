using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LessOrEqual))]
public class LessOrEqualInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null)
            return;

        EditorGUI.BeginChangeCheck();

        LessOrEqual greaterOrEqualAttribute = (LessOrEqual)attribute;

        int intThreshold = greaterOrEqualAttribute.intThreshold;
        float floatThreshold = greaterOrEqualAttribute.floatThreshold;

        EditorGUI.PropertyField(position, property, label);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            if (property.intValue > intThreshold)
            {
                property.intValue = intThreshold;
            }
        }
        else
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                if (property.floatValue > floatThreshold)
                {
                    property.floatValue = floatThreshold;
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
