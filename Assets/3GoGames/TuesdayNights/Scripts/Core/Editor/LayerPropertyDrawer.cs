using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Layer))]
public class LayerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        bool forceApply = false;

        SerializedProperty layerIndex = property.FindPropertyRelative("m_LayerIndex");
        if (layerIndex != null)
        {
            layerIndex.intValue = EditorGUI.LayerField(position, label, layerIndex.intValue);
        }

        if (EditorGUI.EndChangeCheck() || forceApply)
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
