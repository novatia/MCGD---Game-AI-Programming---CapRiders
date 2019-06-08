using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisallowEditInPlayMode))]
public class DisallowEditInPlayModeInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool oldValue = GUI.enabled;
        GUI.enabled = !EditorApplication.isPlaying;

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = oldValue;
    }
}
