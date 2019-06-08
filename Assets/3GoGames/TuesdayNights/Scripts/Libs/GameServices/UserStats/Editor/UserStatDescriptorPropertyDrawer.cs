using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UserStatDescriptor))]
public class UserStatDescriptorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty m_IdProperty = property.FindPropertyRelative("m_Id");
        SerializedProperty m_TypeProperty = property.FindPropertyRelative("m_Type");

        SerializedProperty m_LowLimitProperty = property.FindPropertyRelative("m_LowLimit");
        SerializedProperty m_HighLimitProperty = property.FindPropertyRelative("m_HighLimit");

        SerializedProperty m_DefaultIntValueProperty = property.FindPropertyRelative("m_DefaultIntValue");
        SerializedProperty m_MinIntValueProperty = property.FindPropertyRelative("m_MinIntValue");
        SerializedProperty m_MaxIntValueProperty = property.FindPropertyRelative("m_MaxIntValue");

        SerializedProperty m_DefaultBoolValueProperty = property.FindPropertyRelative("m_DefaultBoolValue");

        SerializedProperty m_DefaultFloatValueProperty = property.FindPropertyRelative("m_DefaultFloatValue");
        SerializedProperty m_MinFloatValueProperty = property.FindPropertyRelative("m_MinFloatValue");
        SerializedProperty m_MaxFloatValueProperty = property.FindPropertyRelative("m_MaxFloatValue");

        SerializedProperty m_DefaultStringValueProperty = property.FindPropertyRelative("m_DefaultStringValue");

        SerializedProperty m_NumericCombineFunctionProperty = property.FindPropertyRelative("m_NumericCombineFunction");
        SerializedProperty m_BooleanCombineFunctionProperty = property.FindPropertyRelative("m_BooleanCombineFunction");
        SerializedProperty m_StringCombineFunctionProperty = property.FindPropertyRelative("m_StringCombineFunction");

        //public enum UserStatType
        //{
        //    Invalid = 0,
        //    Int = 1,
        //    Bool = 2,
        //    Float = 3,
        //    String = 4,
        //}

        float partWidth = position.width / 5f;
        float halfHeight = position.height / 2f;
        float heightThirdPart = position.height / 3f;
        float limitsPart = position.width / 10f;

        Rect invalid = new Rect(position.x, position.y, partWidth, position.height);

        Rect noLimitFirst =         new Rect(position.x,                        position.y,                         1 * partWidth,      halfHeight);
        Rect noLimitSecond =        new Rect(position.x + 1 * partWidth,        position.y,                         2 * partWidth,      halfHeight);
        Rect noLimitThird  =        new Rect(position.x + 3 * partWidth,        position.y,                         2 * partWidth,      halfHeight);
        Rect noLimitFourth =        new Rect(position.x,                        position.y + 1 * halfHeight,   position.width,     halfHeight);

        Rect withLimitFirst =       new Rect(position.x,                      position.y,                     1 * partWidth,  1 * heightThirdPart);
        Rect withLimitSecond =      new Rect(position.x + 1 * partWidth,      position.y,                     2 * partWidth,  1 * heightThirdPart);
        Rect withLimitThird =       new Rect(position.x + 3 * partWidth,      position.y,                     2 * partWidth,  1 * heightThirdPart);

        Rect withLimitFourth =      new Rect(position.x,                        position.y + 1 * heightThirdPart,    1 * limitsPart,  1 * heightThirdPart);
        Rect withLimitFifth =       new Rect(position.x + 1 * limitsPart,       position.y + 1 * heightThirdPart,    4 * limitsPart,  1 * heightThirdPart);
        Rect withLimitSixth =       new Rect(position.x + 5 * limitsPart,       position.y + 1 * heightThirdPart,    1 * limitsPart,  1 * heightThirdPart);
        Rect withLimitSeventh =     new Rect(position.x + 6 * limitsPart,       position.y + 1 * heightThirdPart,    4 * limitsPart,  1 * heightThirdPart);

        Rect withLimitEighth =      new Rect(position.x, position.y + 2 * heightThirdPart, position.width, 1 * heightThirdPart);

        EditorGUI.BeginChangeCheck();

        int typeEnumIndex = m_TypeProperty.enumValueIndex;

        if (typeEnumIndex > 0)
        {
            if (typeEnumIndex == 1)
            {
                EditorGUI.PropertyField(withLimitFirst, m_TypeProperty, GUIContent.none);
                EditorGUI.PropertyField(withLimitSecond, m_IdProperty, GUIContent.none);
                EditorGUI.PropertyField(withLimitThird, m_DefaultIntValueProperty, GUIContent.none);

                EditorGUI.PropertyField(withLimitFourth, m_LowLimitProperty, GUIContent.none);

                GUI.enabled = m_LowLimitProperty.boolValue;
                EditorGUI.PropertyField(withLimitFifth, m_MinIntValueProperty, new GUIContent("Low"));
                GUI.enabled = true;

                EditorGUI.PropertyField(withLimitSixth, m_HighLimitProperty, GUIContent.none);

                GUI.enabled = m_HighLimitProperty.boolValue;
                EditorGUI.PropertyField(withLimitSeventh, m_MaxIntValueProperty, new GUIContent("High"));
                GUI.enabled = true;

                EditorGUI.PropertyField(withLimitEighth, m_NumericCombineFunctionProperty, new GUIContent("Combine"));
            }
            else if (typeEnumIndex == 2)
            {
                EditorGUI.PropertyField(noLimitFirst, m_TypeProperty, GUIContent.none);
                EditorGUI.PropertyField(noLimitSecond, m_IdProperty, GUIContent.none);
                EditorGUI.PropertyField(noLimitThird, m_DefaultBoolValueProperty, GUIContent.none);

                EditorGUI.PropertyField(noLimitFourth, m_BooleanCombineFunctionProperty, new GUIContent("Combine"));
            }
            else if (typeEnumIndex == 3)
            {
                EditorGUI.PropertyField(withLimitFirst, m_TypeProperty, GUIContent.none);
                EditorGUI.PropertyField(withLimitSecond, m_IdProperty, GUIContent.none);
                EditorGUI.PropertyField(withLimitThird, m_DefaultFloatValueProperty, GUIContent.none);

                EditorGUI.PropertyField(withLimitFourth, m_LowLimitProperty, GUIContent.none);

                GUI.enabled = m_LowLimitProperty.boolValue;
                EditorGUI.PropertyField(withLimitFifth, m_MinFloatValueProperty, new GUIContent("Low"));
                GUI.enabled = true;

                EditorGUI.PropertyField(withLimitSixth, m_HighLimitProperty, GUIContent.none);

                GUI.enabled = m_HighLimitProperty.boolValue;
                EditorGUI.PropertyField(withLimitSeventh, m_MaxFloatValueProperty, new GUIContent("High"));
                GUI.enabled = true;

                EditorGUI.PropertyField(withLimitEighth, m_NumericCombineFunctionProperty, new GUIContent("Combine"));
            }
            else if (typeEnumIndex == 4)
            {
                EditorGUI.PropertyField(noLimitFirst, m_TypeProperty, GUIContent.none);
                EditorGUI.PropertyField(noLimitSecond, m_IdProperty, GUIContent.none);
                EditorGUI.PropertyField(noLimitThird, m_DefaultStringValueProperty, GUIContent.none);

                EditorGUI.PropertyField(noLimitFourth, m_StringCombineFunctionProperty, new GUIContent("Combine"));
            }
        }
        else
        {
            EditorGUI.PropertyField(invalid, m_TypeProperty, GUIContent.none);
        }

        if (EditorGUI.EndChangeCheck()) // TO REMOVE forceApply
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty m_TypeProperty = property.FindPropertyRelative("m_Type");

        int typeEnumIndex = m_TypeProperty.enumValueIndex;

        if (typeEnumIndex == 0)
        {
            return base.GetPropertyHeight(property, label);
        }

        if (typeEnumIndex == 1 || typeEnumIndex == 3)
        {
            return 3 * base.GetPropertyHeight(property, label);
        }

        return 2 * base.GetPropertyHeight(property, label);
    }
}
