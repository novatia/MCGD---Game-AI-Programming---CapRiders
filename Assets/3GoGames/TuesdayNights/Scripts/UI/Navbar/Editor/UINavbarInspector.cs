using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UINavbar))]
public class UINavbarInspector : Editor
{
    private SerializedProperty m_RootProperty = null;
    private SerializedProperty m_AutomaticGenerateItemsProperty = null;
    private SerializedProperty m_NavbarItemPrefabProperty = null;
    private SerializedProperty m_NavbarItemsProperty = null;
    private SerializedProperty m_MaxItemsProperty = null;

    void OnEnable()
    {
        m_RootProperty = serializedObject.FindProperty("m_Root");
        m_AutomaticGenerateItemsProperty = serializedObject.FindProperty("m_AutomaticGenerateItems");
        m_NavbarItemPrefabProperty = serializedObject.FindProperty("m_NavbarItemPrefab");
        m_NavbarItemsProperty = serializedObject.FindProperty("m_NavbarItems");
        m_MaxItemsProperty = serializedObject.FindProperty("m_MaxItems");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Navbar - General", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_RootProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Navbar - Items", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_AutomaticGenerateItemsProperty);

        EditorGUILayout.Space();

        if (m_AutomaticGenerateItemsProperty.boolValue)
        {
            m_NavbarItemsProperty.ClearArray();

            EditorGUILayout.PropertyField(m_NavbarItemPrefabProperty);
            EditorGUILayout.PropertyField(m_MaxItemsProperty);
        }
        else
        {
            m_NavbarItemPrefabProperty.objectReferenceValue = null;

            EditorGUILayout.PropertyField(m_NavbarItemsProperty, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.Update();
    }
}
