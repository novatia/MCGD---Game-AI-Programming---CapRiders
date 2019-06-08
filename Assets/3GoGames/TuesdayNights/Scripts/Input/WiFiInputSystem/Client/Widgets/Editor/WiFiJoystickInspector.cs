using UnityEditor;

namespace WiFiInput.Client
{
    [CustomEditor(typeof(WiFiJoystick))]
    public class WiFiJoystickInspector : Editor
    {
        private SerializedProperty m_XControlNameProperty = null;
        private SerializedProperty m_YControlNameProperty = null;
        private SerializedProperty m_BackPanelProperty = null;
        private SerializedProperty m_BackImageProperty = null;
        private SerializedProperty m_NubImageProperty = null;
        private SerializedProperty m_LowThresholdProperty = null;
        private SerializedProperty m_HighThresholdProperty = null;
        private SerializedProperty m_SendXAsButtonProperty = null;
        private SerializedProperty m_XPositiveButtonControlNameProperty = null;
        private SerializedProperty m_XNegativeButtonControlNameProperty = null;
        private SerializedProperty m_XButtonThresholdProperty = null;
        private SerializedProperty m_SendYAsButtonProperty = null;
        private SerializedProperty m_YPositiveButtonControlNameProperty = null;
        private SerializedProperty m_YNegativeButtonControlNameProperty = null;
        private SerializedProperty m_YButtonThresholdProperty = null;

        void OnEnable()
        {
            m_XControlNameProperty = serializedObject.FindProperty("m_XControlName");
            m_YControlNameProperty = serializedObject.FindProperty("m_YControlName");
            m_BackPanelProperty = serializedObject.FindProperty("m_BackPanel");
            m_BackImageProperty = serializedObject.FindProperty("m_BackImage");
            m_NubImageProperty = serializedObject.FindProperty("m_NubImage");
            m_LowThresholdProperty = serializedObject.FindProperty("m_LowThreshold");
            m_HighThresholdProperty = serializedObject.FindProperty("m_HighThreshold");
            m_SendXAsButtonProperty = serializedObject.FindProperty("m_SendXAsButton");
            m_XPositiveButtonControlNameProperty = serializedObject.FindProperty("m_XPositiveButtonControlName");
            m_XNegativeButtonControlNameProperty = serializedObject.FindProperty("m_XNegativeButtonControlName");
            m_XButtonThresholdProperty = serializedObject.FindProperty("m_XButtonThreshold");
            m_SendYAsButtonProperty = serializedObject.FindProperty("m_SendYAsButton");
            m_YPositiveButtonControlNameProperty = serializedObject.FindProperty("m_YPositiveButtonControlName");
            m_YNegativeButtonControlNameProperty = serializedObject.FindProperty("m_YNegativeButtonControlName");
            m_YButtonThresholdProperty = serializedObject.FindProperty("m_YButtonThreshold");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            bool forceApply = false;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Axes", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_XControlNameProperty);
            EditorGUILayout.PropertyField(m_YControlNameProperty);
            EditorGUILayout.Slider(m_LowThresholdProperty, 0f, 1f);
            EditorGUILayout.Slider(m_HighThresholdProperty, 0f, 1f);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_BackPanelProperty);
            EditorGUILayout.PropertyField(m_BackImageProperty);
            EditorGUILayout.PropertyField(m_NubImageProperty);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_SendXAsButtonProperty);
            if (m_SendXAsButtonProperty.boolValue)
            {
                EditorGUILayout.PropertyField(m_XPositiveButtonControlNameProperty);
                EditorGUILayout.PropertyField(m_XNegativeButtonControlNameProperty);
                EditorGUILayout.Slider(m_XButtonThresholdProperty, 0f, 1f);
            }
            else
            {
                if (m_XPositiveButtonControlNameProperty.stringValue != "")
                {
                    m_XPositiveButtonControlNameProperty.stringValue = "";
                    forceApply = true;
                }

                if (m_XNegativeButtonControlNameProperty.stringValue != "")
                {
                    m_XNegativeButtonControlNameProperty.stringValue = "";
                    forceApply = true;
                }
            }
            
            EditorGUILayout.PropertyField(m_SendYAsButtonProperty);
            if (m_SendYAsButtonProperty.boolValue)
            {
                EditorGUILayout.PropertyField(m_YPositiveButtonControlNameProperty);
                EditorGUILayout.PropertyField(m_YNegativeButtonControlNameProperty);
                EditorGUILayout.Slider(m_YButtonThresholdProperty, 0f, 1f);
            }
            else
            {
                if (m_YPositiveButtonControlNameProperty.stringValue != "")
                {
                    m_YPositiveButtonControlNameProperty.stringValue = "";
                    forceApply = true;
                }

                if (m_YNegativeButtonControlNameProperty.stringValue != "")
                {
                    m_YNegativeButtonControlNameProperty.stringValue = "";
                    forceApply = true;
                }
            }

            if (EditorGUI.EndChangeCheck() || forceApply)
            {
                serializedObject.ApplyModifiedProperties();
            }

            serializedObject.Update();
        }
    }
}
