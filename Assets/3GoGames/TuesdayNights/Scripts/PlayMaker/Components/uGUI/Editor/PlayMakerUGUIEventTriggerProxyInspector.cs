using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayMakerUGUIEventTriggerProxy))]
public class PlayMakerUGUIEventTriggerProxyInspector : Editor
{
    private SerializedProperty m_UITargetProperty = null;
    private SerializedProperty m_FsmEventSetupProperty = null;

    void OnEnable()
    {
        m_UITargetProperty = serializedObject.FindProperty("m_UITarget");
        m_FsmEventSetupProperty = serializedObject.FindProperty("m_FsmEventSetup");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_UITargetProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PlayMaker Event", EditorStyles.boldLabel);

        SerializedProperty fsmEventTargetProperty = m_FsmEventSetupProperty.FindPropertyRelative("target");
        SerializedProperty fsmEventGameObjectProperty = m_FsmEventSetupProperty.FindPropertyRelative("gameObject");
        SerializedProperty fsmEventEventNameProperty = m_FsmEventSetupProperty.FindPropertyRelative("eventName");

        EditorGUILayout.PropertyField(fsmEventTargetProperty);

        if (fsmEventTargetProperty.enumValueIndex == 1)
        {
            // GAME OBJECT
            EditorGUILayout.PropertyField(fsmEventGameObjectProperty);
        }
        else
        {
            fsmEventGameObjectProperty.objectReferenceValue = null;
        }

        EditorGUILayout.PropertyField(fsmEventEventNameProperty);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.Update();
    }
}
