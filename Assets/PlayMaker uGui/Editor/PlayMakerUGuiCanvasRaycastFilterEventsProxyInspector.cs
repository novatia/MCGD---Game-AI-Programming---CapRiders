using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;

using System.Collections;

/// <summary>
/// PlayMaker UGui Canvas Raycast Filter Events Proxy Inspector inspector.
/// This is only to remove the first script pointer field, which is totally unnecessary and takes vertical space in the editor
/// </summary>
[CustomEditor(typeof(PlayMakerUGuiCanvasRaycastFilterEventsProxy))]
public class PlayMakerUGuiCanvasRaycastFilterEventsProxyInspector : Editor {
	
	public override void OnInspectorGUI()
	{
#pragma warning disable 618
        serializedObject.UpdateIfDirtyOrScript();
#pragma warning restore 618

        SerializedProperty RayCastingEnabled = serializedObject.FindProperty("RayCastingEnabled");
		EditorGUILayout.PropertyField(RayCastingEnabled);

		serializedObject.ApplyModifiedProperties();
	}
}
