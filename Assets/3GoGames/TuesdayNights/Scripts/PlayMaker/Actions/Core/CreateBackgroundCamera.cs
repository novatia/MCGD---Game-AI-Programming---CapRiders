using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Create Background camera.")]
    public class CreateBackgroundCamera : FsmStateAction
    {
        public override void OnEnter()
        {
            GameObject cameraGo = new GameObject("BackgroundCamera");
            cameraGo.transform.position = new Vector3(0f, 0f, -10f);

            Camera camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.cullingMask = 0;
            camera.orthographic = true;
            camera.orthographicSize = 5.4f;
            camera.depth = -1f;
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;

            GameObject.DontDestroyOnLoad(cameraGo);

            Finish();
        }
    }
}