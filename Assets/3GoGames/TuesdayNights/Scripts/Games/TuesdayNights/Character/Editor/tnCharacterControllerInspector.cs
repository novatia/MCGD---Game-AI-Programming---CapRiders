using UnityEngine;
using UnityEditor;

using TrueSync;

[CustomEditor(typeof(tnCharacterController))]
public class tnCharacterControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            tnCharacterController characterController = target as tnCharacterController;

            if (characterController == null)
                return;

            TSRigidBody2D rigidbody = characterController.GetComponent<TSRigidBody2D>();
            TSVector2 velocity = (rigidbody != null) ? rigidbody.velocity : TSVector2.zero;
            FP speed = velocity.magnitude;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Speed: " + speed.ToString(2), EditorStyles.label);
        }
    }
}