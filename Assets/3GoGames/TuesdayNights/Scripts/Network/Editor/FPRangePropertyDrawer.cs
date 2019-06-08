#if PHOTON_TRUE_SYNC

using UnityEngine;
using UnityEditor;

using TrueSync;

[CustomPropertyDrawer(typeof(FPRange))]
public class FPRangePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty minProperty = property.FindPropertyRelative("m_Min");
        SerializedProperty maxProperty = property.FindPropertyRelative("m_Max");

        float widthPart = position.width / 12f;

        Rect first = new Rect(position.x, position.y, 6f * widthPart, position.height);
        Rect second = new Rect(position.x + 6f * widthPart, position.y, 3f * widthPart, position.height);
        Rect third = new Rect(position.x + 9f * widthPart, position.y, 3f * widthPart, position.height);

        EditorGUI.BeginChangeCheck();

        EditorGUI.LabelField(first, label);

        EditorGUI.PropertyField(second, minProperty, GUIContent.none);
        EditorGUI.PropertyField(third, maxProperty, GUIContent.none);

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

#endif // PHOTON_TRUE_SYNC