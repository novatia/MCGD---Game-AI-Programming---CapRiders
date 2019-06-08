using UnityEngine;
using UnityEngine.Audio;

using TuesdayNights;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Fake Game initialization.")]
    public class tnFakeInitialization : FsmStateAction
    {
        [ObjectType(typeof(AudioMixer))]
        public FsmObject mixer;

        public override void Reset()
        {
            mixer = null;
        }

        public override void OnEnter()
        {
            // Create Audio Listener.

            {
                GameObject audioListenerGo = new GameObject("AudioListener");
                audioListenerGo.AddComponent<AudioListener>();
                GameObject.DontDestroyOnLoad(audioListenerGo);
            }

            // Load game DB.

            {
                tnGameData.InitializeMain();
            }

            // Load Game Settings

            {
                GameSettings.LoadMain();

                // SFX volume.

                {
                    if (!GameSettings.HasFloatKeyMain(Settings.s_SfxVolumeSetting))
                    {
                        GameSettings.SetFloatMain(Settings.s_SfxVolumeSetting, 1f);
                    }
                }

                // Music volume.

                {
                    if (!GameSettings.HasFloatKeyMain(Settings.s_MusicVolumeSetting))
                    {
                        GameSettings.SetFloatMain(Settings.s_MusicVolumeSetting, 1f);
                    }
                }

                // Screenshake.

                {
                    if (!GameSettings.HasBoolKeyMain(Settings.s_ScreenshakeSetting))
                    {
                        GameSettings.SetBoolMain(Settings.s_ScreenshakeSetting, true);
                    }
                }

                // Slow motion.

                //{
                //    if (!GameSettings.HasBoolKeyMain(Settings.s_SlowMotionSetting))
                //    {
                //        GameSettings.SetBoolMain(Settings.s_SlowMotionSetting, true);
                //    }
                //}

                // Camera movement.

                {
                    if (!GameSettings.HasBoolKeyMain(Settings.s_CameraMovementSetting))
                    {
                        GameSettings.SetBoolMain(Settings.s_CameraMovementSetting, true);
                    }
                }

                // XInput.

                {
                    if (!GameSettings.HasBoolKeyMain(Settings.s_UseXInput))
                    {
                        GameSettings.SetBoolMain(Settings.s_UseXInput, true);
                    }
                }

                // Rumble.

                {
                    if (!GameSettings.HasBoolKeyMain(Settings.s_UseRumble))
                    {
                        GameSettings.SetBoolMain(Settings.s_UseRumble, true);
                    }
                }

                // Apply settings.

                if (mixer != null && !mixer.IsNone && mixer.Value != null)
                {
                    AudioMixer audioMixer = (AudioMixer)mixer.Value;

                    {
                        float sfxVolume = GameSettings.GetFloatMain(Settings.s_SfxVolumeSetting);

                        float sfxVolumeDb = AudioUtils.LinearToDecibel(sfxVolume);
                        audioMixer.SetFloat("SfxVolume", sfxVolumeDb);
                        audioMixer.SetFloat("VoiceoverVolume", sfxVolumeDb);
                        audioMixer.SetFloat("AmbienceVolume", sfxVolumeDb);

                    }

                    {
                        float musicVolume = GameSettings.GetFloatMain(Settings.s_MusicVolumeSetting);

                        float musicVolumeDb = AudioUtils.LinearToDecibel(musicVolume);
                        audioMixer.SetFloat("MusicVolume", musicVolumeDb);
                    }
                }

                {
                    InputSystem.useXInputMain = true;
                }
            }

            Finish();
        }
    }
}
