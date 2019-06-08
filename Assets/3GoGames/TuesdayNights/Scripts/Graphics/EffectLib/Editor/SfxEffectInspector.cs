using UnityEditor;

[CustomEditor(typeof(SfxEffect))]
public class SfxEffectInspector : Editor
{
    private SerializedProperty m_ClipsProperty = null;
    private SerializedProperty m_AudioMixerGroupProperty = null;

    private SerializedProperty m_MinVolumeProperty = null;
    private SerializedProperty m_MaxVolumeProperty = null;

    private SerializedProperty m_MinPitchProperty = null;
    private SerializedProperty m_MaxPitchProperty = null;

    void OnEnable()
    {
        m_ClipsProperty = serializedObject.FindProperty("clips");
        m_AudioMixerGroupProperty = serializedObject.FindProperty("audioMixerGroup");

        m_MinVolumeProperty = serializedObject.FindProperty("minVolume");
        m_MaxVolumeProperty = serializedObject.FindProperty("maxVolume");

        m_MinPitchProperty = serializedObject.FindProperty("minPitch");
        m_MaxPitchProperty = serializedObject.FindProperty("maxPitch");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_ClipsProperty, true);

        EditorGUILayout.LabelField("Mixer", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_AudioMixerGroupProperty);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Volume", EditorStyles.boldLabel);

        {
            float minVolume = m_MinVolumeProperty.floatValue;
            float maxVolume = m_MaxVolumeProperty.floatValue;

            EditorGUILayout.LabelField("Min Volume:", minVolume.ToString());
            EditorGUILayout.LabelField("Max Volume:", maxVolume.ToString());

            EditorGUILayout.MinMaxSlider(ref minVolume, ref maxVolume, 0f, 1f, null);

            m_MinVolumeProperty.floatValue = minVolume;
            m_MaxVolumeProperty.floatValue = maxVolume;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Pitch", EditorStyles.boldLabel);

        {
            float minPitch = m_MinPitchProperty.floatValue;
            float maxPitch = m_MaxPitchProperty.floatValue;

            EditorGUILayout.LabelField("Min Pitch:", minPitch.ToString());
            EditorGUILayout.LabelField("Max Pitch:", maxPitch.ToString());

            EditorGUILayout.MinMaxSlider(ref minPitch, ref maxPitch, -3f, 3f, null);

            m_MinPitchProperty.floatValue = minPitch;
            m_MaxPitchProperty.floatValue = maxPitch;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.Update();
    }
}