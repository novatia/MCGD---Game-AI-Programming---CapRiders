using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Readonly))]
public class ReadonlyInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool oldValue = GUI.enabled;
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property);

        GUI.enabled = oldValue;
    }
}
