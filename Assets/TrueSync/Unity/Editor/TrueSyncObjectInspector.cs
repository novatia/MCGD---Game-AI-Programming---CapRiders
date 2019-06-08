using UnityEngine;
using UnityEditor;

namespace TrueSync
{
    [CustomEditor(typeof(TrueSyncObject))]
    public class TrueSyncObjectInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TrueSyncObject tsObject = target as TrueSyncObject;

            if (tsObject == null)
                return;

            GameObject go = tsObject.gameObject;

            if (go == null)
                return;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);

            if (!EditorApplication.isPlaying)
            {
                TSTransform2D[] tsTransforms2D = go.GetComponentsInChildren<TSTransform2D>();
                TSCollider2D[] tsColliders2D = go.GetComponentsInChildren<TSCollider2D>();
                TrueSyncBehaviour[] tsBehaviours = go.GetComponentsInChildren<TrueSyncBehaviour>();

                int transformCount = (tsTransforms2D != null) ? tsTransforms2D.Length : 0;
                int colliderCount = (tsColliders2D != null) ? tsColliders2D.Length : 0;
                int behaviourCount = (tsBehaviours != null) ? tsBehaviours.Length : 0;

                EditorGUILayout.LabelField("Transforms (" + transformCount + ")", EditorStyles.label);
                EditorGUILayout.LabelField("Colliders (" + colliderCount + ")", EditorStyles.label);
                EditorGUILayout.LabelField("Behaviours (" + behaviourCount + ")", EditorStyles.label);
            }
            else
            {
                int transformCount = (tsObject != null) ? tsObject.transformCount: 0;
                int colliderCount = (tsObject != null) ? tsObject.colliderCount: 0;
                int behaviourCount = (tsObject != null) ? tsObject.behaviourCount: 0;

                int registeredTransformCount = (tsObject != null) ? tsObject.registeredTransformCount : 0;
                int registeredColliderCount = (tsObject != null) ? tsObject.registeredColliderCount : 0;
                int registeredBehaviourCount = (tsObject != null) ? tsObject.registeredBehaviourCount : 0;

                EditorGUILayout.LabelField("Transforms (" + transformCount + " / " + registeredTransformCount + ")", EditorStyles.label);
                EditorGUILayout.LabelField("Colliders (" + colliderCount + " / " + registeredColliderCount + ")", EditorStyles.label);
                EditorGUILayout.LabelField("Behaviours (" + behaviourCount + " / " + registeredBehaviourCount + ")", EditorStyles.label);
            }
        }
    }
}