using UnityEditor;

[CustomEditor(typeof(NonDrawableImage))]
public class NonDrawableImageInspector : Editor
{
    private SerializedProperty m_RaycastTargetProperty = null;

    void OnEnable()
    {
        m_RaycastTargetProperty = serializedObject.FindProperty("m_RaycastTarget");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_RaycastTargetProperty);
    }
}
