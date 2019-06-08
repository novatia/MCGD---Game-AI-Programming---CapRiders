using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IntRange))]
public class IntRangePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty minProperty = property.FindPropertyRelative("m_Min");
        SerializedProperty maxProperty = property.FindPropertyRelative("m_Max");

        float widthPart = position.width / 12f;

        Rect first = new Rect(position.x, position.y, 3f * widthPart, position.height);
        Rect second = new Rect(position.x + 5f * widthPart, position.y, 3f * widthPart, position.height);
        Rect third = new Rect(position.x + 9f * widthPart, position.y, 3f * widthPart, position.height);

        EditorGUI.BeginChangeCheck();

        EditorGUI.LabelField(first, label);

        EditorGUI.PropertyField(second, minProperty, GUIContent.none);
        // minProperty.intValue = Mathf.Clamp(minProperty.intValue, int.MinValue, maxProperty.intValue);
        if (minProperty.intValue >= maxProperty.intValue)
        {
            maxProperty.intValue = minProperty.intValue;
        }

        EditorGUI.PropertyField(third, maxProperty, GUIContent.none);
        // maxProperty.intValue = Mathf.Clamp(maxProperty.intValue, minProperty.intValue, int.MaxValue);
        if (maxProperty.intValue < minProperty.intValue)
        {
            minProperty.intValue = maxProperty.intValue;
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}

