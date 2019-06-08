using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

[CustomEditor(typeof(tnCharacterDataDescriptor))]
public class tnCharacterDataDescriptorInspector : Editor
{
    private SerializedProperty m_FirstNameProperty = null;
    private SerializedProperty m_LastNameProperty = null;
    private SerializedProperty m_DisplayNameProperty = null;
    private SerializedProperty m_RoleProperty = null;
    private SerializedProperty m_NumberProperty = null;
    private SerializedProperty m_SpecifyPrefabProperty = null;
    private SerializedProperty m_PrefabPathProperty = null;
    private SerializedProperty m_LeftFramesProperty = null;
    private SerializedProperty m_RightFramesProperty = null;
    private SerializedProperty m_AnimatorControllerProperty = null;
    private SerializedProperty m_UIIconFacingRightProperty = null;
    private SerializedProperty m_UIIconFacingLeftProperty = null;

    void OnEnable()
    {
        m_FirstNameProperty = serializedObject.FindProperty("m_FirstName");
        m_LastNameProperty = serializedObject.FindProperty("m_LastName");
        m_DisplayNameProperty = serializedObject.FindProperty("m_DisplayName");
        m_RoleProperty = serializedObject.FindProperty("m_Role");
        m_NumberProperty = serializedObject.FindProperty("m_Number");
        m_SpecifyPrefabProperty = serializedObject.FindProperty("m_SpecifyPrefab");
        m_PrefabPathProperty = serializedObject.FindProperty("m_PrefabPath");
        m_LeftFramesProperty = serializedObject.FindProperty("m_Left");
        m_RightFramesProperty = serializedObject.FindProperty("m_Right");
        m_AnimatorControllerProperty = serializedObject.FindProperty("m_AnimatorController");
        m_UIIconFacingRightProperty = serializedObject.FindProperty("m_UIIconFacingRight");
        m_UIIconFacingLeftProperty = serializedObject.FindProperty("m_UIIconFacingLeft");
    }

    public override void OnInspectorGUI()
    {
        bool forceApply = false;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Character Info", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_FirstNameProperty);
        EditorGUILayout.PropertyField(m_LastNameProperty);
        EditorGUILayout.PropertyField(m_DisplayNameProperty);

        EditorGUILayout.PropertyField(m_RoleProperty);

        EditorGUILayout.PropertyField(m_NumberProperty);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_SpecifyPrefabProperty);
        EditorGUILayout.Space();

        if (m_SpecifyPrefabProperty.boolValue)
        {
            EditorGUILayout.PropertyField(m_PrefabPathProperty);
            if (m_AnimatorControllerProperty.objectReferenceValue != null)
            {
                m_AnimatorControllerProperty.objectReferenceValue = null;
                forceApply = true;
            }
        }
        else
        {
            EditorGUILayout.LabelField("Assets", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_LeftFramesProperty, true);
            // DrawArray("Left Sprites", m_LeftFramesProperty);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_RightFramesProperty, true);
            // DrawArray("Right Sprites", m_RightFramesProperty);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_AnimatorControllerProperty);
            SerializedProperty path = m_PrefabPathProperty.FindPropertyRelative("m_Path");
            if (path.stringValue != "")
            {
                path.stringValue = "";
                forceApply = true;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_UIIconFacingRightProperty);
        EditorGUILayout.PropertyField(m_UIIconFacingLeftProperty);

        if (EditorGUI.EndChangeCheck() || forceApply)
        {
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.Update();
    }

    // private List<int> m_IndexToRemove = new List<int>();

    // INTERNALS

    //private void DrawArray(string i_Title, SerializedProperty i_SerializedProperty)
    //{
    //    if (i_SerializedProperty == null)
    //        return;

    //    EditorGUILayout.LabelField(i_Title, EditorStyles.label);

    //    int arraySize = i_SerializedProperty.arraySize;
    //    EditorGUILayout.LabelField("(" + arraySize + " elements)", EditorStyles.miniLabel);
    //    //int desiredSize = arraySize;
    //    //desiredSize = EditorGUILayout.IntField("Size", desiredSize);

    //    //desiredSize = Mathf.Max(desiredSize, 0);

    //    //Event e = Event.current;
    //    //if (arraySize != desiredSize && e != null && e.isKey == true && e.keyCode == KeyCode.Return)
    //    //{
    //    //    int currentSize = arraySize;
    //    //    if (arraySize > desiredSize)
    //    //    {
    //    //        while (currentSize > desiredSize)
    //    //        {
    //    //            i_SerializedProperty.DeleteArrayElementAtIndex(i_SerializedProperty.arraySize - 1);
    //    //            --currentSize;
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        while (currentSize < desiredSize)
    //    //        {
    //    //            i_SerializedProperty.InsertArrayElementAtIndex(i_SerializedProperty.arraySize);
    //    //            ++currentSize;
    //    //        }
    //    //    }
    //    //}

    //    m_IndexToRemove.Clear();

    //    for (int index = 0; index < i_SerializedProperty.arraySize; ++index)
    //    {
    //        SerializedProperty p = i_SerializedProperty.GetArrayElementAtIndex(index);

    //        EditorGUILayout.BeginHorizontal();

    //        EditorGUILayout.PropertyField(p);
    //        bool remove = GUILayout.Button("-");
    //        if (remove)
    //        {
    //            m_IndexToRemove.Add(index);
    //        }

    //        EditorGUILayout.EndHorizontal();
    //    }

    //    for (int index = 0; index < m_IndexToRemove.Count; ++index)
    //    {
    //        int indexToRemove = m_IndexToRemove[index];
    //        i_SerializedProperty.DeleteArrayElementAtIndex(indexToRemove);
    //    }

    //    bool add = GUILayout.Button("Add");
    //    if (add)
    //    {
    //        i_SerializedProperty.InsertArrayElementAtIndex(arraySize);
    //    }
    //}
}
