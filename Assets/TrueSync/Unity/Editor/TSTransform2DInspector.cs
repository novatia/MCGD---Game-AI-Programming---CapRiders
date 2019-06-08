using UnityEngine;
using UnityEditor;

namespace TrueSync
{
    [CustomEditor(typeof(TSTransform2D))]
    public class TSTransform2DInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TSTransform2D tsTransform2D = target as TSTransform2D;

            if (tsTransform2D == null)
                return;

            string positionLabel = "Position --> (" + tsTransform2D.position.x + ", " + tsTransform2D.position.y + ")";
            string rotationLabel = "Rotation --> (" + tsTransform2D.rotation + ")";

            string serializedLable = (tsTransform2D.serialized) ? "Serialized: OK!" : "WARNING: Not serialized";

            EditorGUILayout.LabelField(positionLabel, EditorStyles.label);
            EditorGUILayout.LabelField(rotationLabel, EditorStyles.label);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(serializedLable, EditorStyles.label);
        }
    }
}
