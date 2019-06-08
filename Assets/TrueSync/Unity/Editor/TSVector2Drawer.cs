using UnityEditor;
using UnityEngine;

namespace TrueSync
{
    [CustomPropertyDrawer(typeof(TSVector2))]
    public class TSVector2Drawer : PropertyDrawer
    {
        private const int INDENT_OFFSET = 15;
        private const int LABEL_WIDTH = 12;
        private const int LABEL_MARGIN = 1;

        private static GUIContent xLabel = new GUIContent("X");
        private static GUIContent yLabel = new GUIContent("Y");

        public override void OnGUI(Rect i_Position, SerializedProperty i_Property, GUIContent i_Label)
        {
            EditorGUI.BeginProperty(i_Position, i_Label, i_Property);

            i_Position = EditorGUI.PrefixLabel(i_Position, i_Label);

            i_Position.width /= 2f;

            float indentOffsetLevel = (INDENT_OFFSET) * EditorGUI.indentLevel;
            i_Position.width += indentOffsetLevel;

            EditorGUIUtility.labelWidth = indentOffsetLevel + LABEL_WIDTH;

            SerializedProperty xSerProperty = i_Property.FindPropertyRelative("x");
            i_Position.x -= indentOffsetLevel;

            EditorGUI.PropertyField(i_Position, xSerProperty, xLabel);
            i_Position.x += i_Position.width;

            SerializedProperty ySerProperty = i_Property.FindPropertyRelative("y");
            i_Position.x -= indentOffsetLevel;

            EditorGUI.PropertyField(i_Position, ySerProperty, yLabel);

            EditorGUI.EndProperty();
        }
    }
}