using UnityEditor;

[CustomEditor(typeof(tnHoleTarget))]
public class tnHoleTargetInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            tnHoleTarget holeTarget = target as tnHoleTarget;

            if (holeTarget == null)
                return;

            EditorGUILayout.Space();
        
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Can enter hole: " + holeTarget.canEnterHole, EditorStyles.label);
            EditorGUILayout.LabelField("Teleporting: " + holeTarget.isTeleporting, EditorStyles.label);
        }
    }
}