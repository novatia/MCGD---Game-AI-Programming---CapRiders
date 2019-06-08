using System;

using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Initialize UI Canvas.")]
    public class InitUICanvas : FsmStateAction
    {
        public override void OnEnter()
        {
            // Create UI Camera.

            GameObject uiCamera = new GameObject("UICamera");

            uiCamera.transform.position = new Vector3(0f, 0f, -10f);
            uiCamera.transform.rotation = Quaternion.identity;

            Camera cam = uiCamera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.cullingMask = 0;
            cam.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
            cam.cullingMask |= (1 << LayerMask.NameToLayer("GUI"));
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.depth = float.MaxValue;
            cam.useOcclusionCulling = false;
            cam.allowHDR = false;

            /* FixedAspectRatio fixedAspectRatio = */
            uiCamera.AddComponent<FixedAspectRatio>();
            // fixedAspectRatio.targetAspectRatio = 1920f / 1080f;

            // uiCamera.AddComponent<GUILayer>();

            GameObject.DontDestroyOnLoad(uiCamera);

            // Create UI Canvas.

            GameObject uiCanvas = new GameObject("UICanvas");
            uiCanvas.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = uiCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.pixelPerfect = false;
            canvas.worldCamera = cam;

            uiCanvas.AddComponent<UICanvas>();

            CanvasScaler canvasScaler = uiCanvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.referencePixelsPerUnit = 100;

            GraphicRaycaster graphicRaycaster = uiCanvas.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;

            uiCanvas.tag = "MainCanvas";

            GameObject.DontDestroyOnLoad(uiCanvas);

            Finish();
        }
    }
}