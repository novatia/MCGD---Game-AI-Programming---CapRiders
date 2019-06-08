using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (AdjustBoxColliderHelper))]
public class AdjustBoxColliderHelperInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if(GUILayout.Button("Calcola"))
        {
            AdjustBoxColliderHelper executor = target as AdjustBoxColliderHelper;
            executor.Compute();
        }
    }

}
