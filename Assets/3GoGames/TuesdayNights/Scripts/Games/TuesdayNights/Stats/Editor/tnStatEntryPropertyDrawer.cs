using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(tnStatEntry))]
public class tnStatEntryPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty attributeIdProperty = property.FindPropertyRelative("m_AttributeId");
        SerializedProperty baseValueProperty = property.FindPropertyRelative("m_BaseValue");

        Rect first = new Rect(position.x, position.y, position.width, position.height / 2f);
        Rect second = new Rect(position.x, position.y + position.height / 2f, position.width, position.height / 2f);

        EditorGUI.BeginChangeCheck();

        EditorGUI.PropertyField(first, attributeIdProperty, new GUIContent("Attribute Id"));
        EditorGUI.PropertyField(second, baseValueProperty, new GUIContent("Value"));

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 2 * base.GetPropertyHeight(property, label);
    }
}
