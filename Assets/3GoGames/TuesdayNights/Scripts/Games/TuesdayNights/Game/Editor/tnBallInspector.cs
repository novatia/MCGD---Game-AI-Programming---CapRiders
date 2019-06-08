using UnityEngine;
using UnityEditor;

using TrueSync;

[CustomEditor(typeof(tnBall))]
public class tnBallInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            tnBall ball = target as tnBall;

            if (ball == null)
                return;

            TSRigidBody2D rigidbody = ball.GetComponent<TSRigidBody2D>();
            TSVector2 velocity = (rigidbody != null) ? rigidbody.velocity : TSVector2.zero;
            FP speed = velocity.magnitude;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Speed: " + speed.ToString(2), EditorStyles.label);
        }
    }
}