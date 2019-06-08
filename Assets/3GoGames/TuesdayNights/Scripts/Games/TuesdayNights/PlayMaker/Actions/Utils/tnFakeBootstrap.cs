using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using WiFiInput.Server;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Fake Bootstrap sequence.")]
    public class tnFakeBootstrap : FsmStateAction
    {
        [ObjectType(typeof(AudioMixerGroup))]
        public FsmObject musicPlayerOutputChannel;
        [ObjectType(typeof(AudioMixerSnapshot))]
        public FsmObject audioMixerSnapshot;

        public override void Reset()
        {
            audioMixerSnapshot = null;
        }

        public override void OnEnter()
        {
            // Create background camera.

            {
                GameObject cameraGo = new GameObject("BackgroundCamera");
                cameraGo.transform.position = new Vector3(0f, 0f, -10f);

                Camera camera = cameraGo.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.cullingMask = 0;
                camera.orthographic = true;
                camera.orthographicSize = 5f;
                camera.depth = -1f;
                camera.useOcclusionCulling = false;
                camera.allowHDR = false;

                GameObject.DontDestroyOnLoad(cameraGo);
            }

            // Initialize Input system.

            {
                InputSystem.InitializeMain();

                InputSystem.AddPlayerMain("Player0");
                InputSystem.AddPlayerMain("Player1");
                InputSystem.AddPlayerMain("Player2");
                InputSystem.AddPlayerMain("Player3");
                InputSystem.AddPlayerMain("Player4");
                InputSystem.AddPlayerMain("Player5");
                InputSystem.AddPlayerMain("Player6");
                InputSystem.AddPlayerMain("Player7");

                InputSystem.RefreshMapsMain();
            }

            // Initialize WiFi Input system.

            {
                WiFiInputSystem.InitializeMain();
            }

            // Init systems.

            {
                Messenger.Cleanup();

                GameModulesManager.InitializeMain();
                ObjectPool.InitializeMain();

                MusicPlayer.InitializeMain();
                if (musicPlayerOutputChannel != null && !musicPlayerOutputChannel.IsNone && musicPlayerOutputChannel.Value != null)
                {
                    MusicPlayer.SetChannelMain((AudioMixerGroup)musicPlayerOutputChannel.Value);
                }

                // TODO: Set MusicPlayer output channel.

                SfxPlayer.InitializeMain();

                AudioMixerManager.InitializeMain();
                if (audioMixerSnapshot != null && !audioMixerSnapshot.IsNone && audioMixerSnapshot.Value != null)
                {
                    AudioMixerManager.SetSnapshotMain((AudioMixerSnapshot)audioMixerSnapshot.Value, 0f);
                }

                GameServices.InitializeMain();
                GameSettings.InitializeMain();

                UIEventSystem.InitializeMain();
            }

            // Init UI.

            {
                GameObject uiCamera = new GameObject("UICamera");

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
            }

            // PlayMaker Proxy.

            {
                GameObject playmakerProxy = new GameObject("PlayMaker Proxy");

                GameObject playmakerGUIPrefab = (GameObject)Resources.Load("Core/PlayMakerGUI");
                if (playmakerGUIPrefab != null)
                {
                    GameObject playmakerGUI = (GameObject)GameObject.Instantiate(playmakerGUIPrefab, Vector3.zero, Quaternion.identity);
                    playmakerGUI.SetParent(playmakerProxy);
                }

                GameObject playmakerUGUIProxyPrefab = (GameObject)Resources.Load("Core/PlayMakerUGUI");
                if (playmakerUGUIProxyPrefab != null)
                {
                    GameObject playmakerUGUIProxy = (GameObject)GameObject.Instantiate(playmakerUGUIProxyPrefab, Vector3.zero, Quaternion.identity);
                    playmakerUGUIProxy.SetParent(playmakerProxy);
                }

                GameObject.DontDestroyOnLoad(playmakerProxy);
            }

            Finish();
        }
    }
}