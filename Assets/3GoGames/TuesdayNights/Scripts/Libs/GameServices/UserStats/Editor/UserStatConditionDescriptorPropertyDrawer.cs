using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UserStatConditionDescriptor))]
public class UserStatConditionDescriptorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty m_ConditionTypeProperty = property.FindPropertyRelative("m_ConditionType");

        //SerializedProperty m_FirstConditionProperty = property.FindPropertyRelative("m_FirstCondition");
        //SerializedProperty m_SecondConditionProperty = property.FindPropertyRelative("m_SecondCondition");
        //SerializedProperty m_ConditionProperty = property.FindPropertyRelative("m_Condition");

        SerializedProperty m_StatIdProperty = property.FindPropertyRelative("m_StatId");

        SerializedProperty m_NumericConditionTypeProperty = property.FindPropertyRelative("m_NumericConditionType");
        SerializedProperty m_BooleanConditionTypeProperty = property.FindPropertyRelative("m_BooleanConditionType");
        SerializedProperty m_StringConditionTypeProperty = property.FindPropertyRelative("m_StringConditionType");

        SerializedProperty m_IntValueProperty = property.FindPropertyRelative("m_IntValue");
        SerializedProperty m_FloatValueProperty = property.FindPropertyRelative("m_FloatValue");
        SerializedProperty m_StringValueProperty = property.FindPropertyRelative("m_StringValue");

        //public enum UserStatConditionType
        //{
        //    Int = 0,
        //    Bool = 1,
        //    Float = 2,
        //    String = 3,
        //    Not = 4,
        //    And = 5,
        //    Or = 6,
        //    Xor = 7,
        //}

        int conditionTypeEnumIndex = m_ConditionTypeProperty.enumValueIndex;

        bool forceApply = false; // TO REMOVE

        EditorGUI.BeginChangeCheck();

        if (conditionTypeEnumIndex < 4) // It's a simple condition
        {
            float widthQuart = position.width / 4f;
            Rect first = new Rect(position.x, position.y, widthQuart, position.height);
            Rect second = new Rect(position.x + 1f * widthQuart, position.y, widthQuart, position.height);
            Rect thirdA = new Rect(position.x + 2f * widthQuart, position.y, widthQuart, position.height);
            Rect thirdB = new Rect(position.x + 2f * widthQuart, position.y, 2f * widthQuart, position.height);
            Rect fourth = new Rect(position.x + 3f * widthQuart, position.y, widthQuart, position.height);

            EditorGUI.PropertyField(first, m_ConditionTypeProperty, GUIContent.none);
            EditorGUI.PropertyField(second, m_StatIdProperty, GUIContent.none);

            if (conditionTypeEnumIndex == 0)
            {
                EditorGUI.PropertyField(thirdA, m_NumericConditionTypeProperty, GUIContent.none);
                EditorGUI.PropertyField(fourth, m_IntValueProperty, GUIContent.none);
            }
            else if (conditionTypeEnumIndex == 1)
            {
                EditorGUI.PropertyField(thirdB, m_BooleanConditionTypeProperty, GUIContent.none);
            }
            else if (conditionTypeEnumIndex == 2)
            {
                EditorGUI.PropertyField(thirdA, m_NumericConditionTypeProperty, GUIContent.none);
                EditorGUI.PropertyField(fourth, m_FloatValueProperty, GUIContent.none);
            }
            else if (conditionTypeEnumIndex == 3)
            {
                EditorGUI.PropertyField(thirdA, m_StringConditionTypeProperty, GUIContent.none);
                EditorGUI.PropertyField(fourth, m_StringValueProperty, GUIContent.none);
            }
        }
        else
        {
            m_ConditionTypeProperty.enumValueIndex = 0; // TO REMOVE
            forceApply = true;                          // TO REMOVE

            // Composite condition

            //if (conditionTypeEnumIndex > 4)
            //{
            //    float heightThirdPart = position.height / 3f;

            //    Rect first = new Rect(position.x, position.y, position.width, heightThirdPart);
            //    Rect second = new Rect(position.x, position.y + 1f * heightThirdPart, position.width, heightThirdPart);
            //    Rect third = new Rect(position.x, position.y + 2f * heightThirdPart, position.width, heightThirdPart);

            //    EditorGUI.PropertyField(first, m_ConditionTypeProperty);
            //    EditorGUI.PropertyField(second, m_FirstConditionProperty);
            //    EditorGUI.PropertyField(third, m_SecondConditionProperty);
            //}
            //else
            //{
            //    float halfHeight = position.height / 2f;

            //    Rect first = new Rect(position.x, position.y, position.width, halfHeight);
            //    Rect second = new Rect(position.x, position.y + 1f * halfHeight, position.width, halfHeight);

            //    EditorGUI.PropertyField(first, m_ConditionTypeProperty);
            //    EditorGUI.PropertyField(second, m_ConditionProperty);
            //}
        }

        if (EditorGUI.EndChangeCheck() || forceApply) // TO REMOVE forceApply
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label); // TO REMOVE

        //SerializedProperty m_ConditionTypeProperty = property.FindPropertyRelative("m_ConditionType");

        //int conditionTypeEnumIndex = m_ConditionTypeProperty.enumValueIndex;

        //if (conditionTypeEnumIndex < 4)
        //{
        //    return base.GetPropertyHeight(property, label);
        //}
        //else
        //{
        //    if (conditionTypeEnumIndex == 4)
        //    {
        //        return 2f * base.GetPropertyHeight(property, label);
        //    }

        //    return 3f * base.GetPropertyHeight(property, label);
        //}
    }
}
