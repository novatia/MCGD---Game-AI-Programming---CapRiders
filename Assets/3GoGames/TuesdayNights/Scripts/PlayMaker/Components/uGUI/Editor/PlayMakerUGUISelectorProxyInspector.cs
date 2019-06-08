using UnityEditor;

[CustomEditor(typeof(PlayMakerUGUISelectorProxy))]
public class PlayMakerUGUISelectorProxyInspector : Editor
{
    private SerializedProperty m_UITargetProperty = null;
    private SerializedProperty m_EventTypeProperty = null;
    private SerializedProperty m_FsmEventSetupProperty = null;

    void OnEnable()
    {
        m_UITargetProperty = serializedObject.FindProperty("m_UITarget");
        m_EventTypeProperty = serializedObject.FindProperty("m_EventType");
        m_FsmEventSetupProperty = serializedObject.FindProperty("m_FsmEventSetup");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_UITargetProperty);
        EditorGUILayout.PropertyField(m_EventTypeProperty);

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
