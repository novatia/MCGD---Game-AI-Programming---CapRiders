using UnityEditor;
using UnityEngine;

namespace TrueSync
{
    [CustomPropertyDrawer(typeof(FP))]
    public class TSFPDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect i_Position, SerializedProperty i_Property, GUIContent i_Label)
        {
            EditorGUI.BeginProperty(i_Position, i_Label, i_Property);

            EditorGUI.BeginChangeCheck();

            SerializedProperty serializedValueProperty = i_Property.FindPropertyRelative("_serializedValue");
            string value = serializedValueProperty.stringValue;

            FP fpValue = FP.Zero;
            if (value != "")
            {
                fpValue = FP.FromRaw(long.Parse(value));
            }

            fpValue = EditorGUI.FloatField(i_Position, i_Label, (float)fpValue);

            if (EditorGUI.EndChangeCheck())
            {
                serializedValueProperty.stringValue = fpValue.RawValue.ToString();
            }

            EditorGUI.EndProperty();
        }
    }
}