using UnityEngine;
using UnityEditor;

using TrueSync;

[CustomEditor(typeof(tnSubbuteoController))]
public class tnSubbuteoControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            tnSubbuteoController subbuteoController = target as tnSubbuteoController;

            if (subbuteoController == null)
                return;

            TSRigidBody2D rigidbody = subbuteoController.GetComponent<TSRigidBody2D>();
            TSVector2 velocity = (rigidbody != null) ? rigidbody.velocity : TSVector2.zero;
            FP speed = velocity.magnitude;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Speed: " + speed.ToString(2), EditorStyles.label);
        }
    }
}