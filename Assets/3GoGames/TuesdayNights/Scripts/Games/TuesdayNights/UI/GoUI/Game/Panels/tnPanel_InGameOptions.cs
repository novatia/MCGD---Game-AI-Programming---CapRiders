using UnityEngine;
using UnityEngine.Audio;

using System;

using GoUI;

using TuesdayNights;

public class tnPanel_InGameOptions : UIPanel<tnView_InGameOptions>
{
    // STATIC

    private static string s_MasterMixer_SfxVolume_Id = "SfxVolume";
    private static string s_MasterMixer_VoiceoverVolume_Id = "VoiceoverVolume";
    private static string s_MasterMixer_MusicVolume_Id = "MusicVolume";
    private static string s_MasterMixer_AmbienceVolume_Id = "AmbienceVolume";

    // Serializable fields

    [Header("Audio")]

    [SerializeField]
    private AudioMixer m_MasterMixer = null;

    // Fields

    private event Action m_OnBackEvent = null;

    // ACCESSORS

    public event Action onBackEvent
    {
        add { m_OnBackEvent += value; }
        remove { m_OnBackEvent -= value; }
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        InitResolutionSelector();
    }

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        Setup();

        if (viewInstance != null)
        {
            viewInstance.onApplyResolutionEvent += OnApplyResolutionEvent;
            viewInstance.onSfxVolumeChangedEvent += OnSfxVolumeChangedEvent;
            viewInstance.onMusicVolumeChangedEvent += OnMusicVolumeChangedEvent;
            viewInstance.onScreenShakeValueChangedEvent += OnScreenShakeValueChangedEvent;
            //viewInstance.onSlowMotionValueChangedEvent += OnSlowMotionValueChangedEvent;
            viewInstance.onCameraMovementValueChangedEvent += OnCameraMovementValueChangedEvent;
            viewInstance.onBackEvent += OnBackEvent;
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (viewInstance != null)
        {
            viewInstance.onApplyResolutionEvent -= OnApplyResolutionEvent;
            viewInstance.onSfxVolumeChangedEvent -= OnSfxVolumeChangedEvent;
            viewInstance.onMusicVolumeChangedEvent -= OnMusicVolumeChangedEvent;
            viewInstance.onScreenShakeValueChangedEvent -= OnScreenShakeValueChangedEvent;
            //viewInstance.onSlowMotionValueChangedEvent -= OnSlowMotionValueChangedEvent;
            viewInstance.onCameraMovementValueChangedEvent -= OnCameraMovementValueChangedEvent;
            viewInstance.onBackEvent -= OnBackEvent;
        }
    }

    // LOGIC

    public void SetVideoOptionsActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetVideoOptionsActive(i_Active);
        }
    }

    public void SetAudioOptionsActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetAudioOptionsActive(i_Active);
        }
    }

    public void SetGameOptionsActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetGameOptionsActive(i_Active);
        }
    }

    // UTILS

    private void SetScreenResolution(int i_Width, int i_Height, bool i_IsFullscreen)
    {
        bool changed = false;

        changed |= (i_Width != Screen.width);
        changed |= (i_Height != Screen.height);
        changed |= (i_IsFullscreen != Screen.fullScreen);

        if (!changed)
            return;

        Screen.SetResolution(i_Width, i_Height, i_IsFullscreen);
    }

    private void GetScreenResolution(out int o_Width, out int o_Height)
    {
        o_Width = Screen.width;
        o_Height = Screen.height;
    }

    private void GetFullscreen(out bool o_Fullscreen)
    {
        o_Fullscreen = Screen.fullScreen;
    }

    private void SetSfxVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat(s_MasterMixer_SfxVolume_Id, attenuation);
        }
    }

    private void GetSfxVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat(s_MasterMixer_SfxVolume_Id, out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void SetVoiceoverVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat(s_MasterMixer_VoiceoverVolume_Id, attenuation);
        }
    }

    private void GetVoiceoverVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat(s_MasterMixer_VoiceoverVolume_Id, out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void SetMusicVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat(s_MasterMixer_MusicVolume_Id, attenuation);
        }
    }

    private void GetMusicVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat(s_MasterMixer_MusicVolume_Id, out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void SetAmbienceVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat(s_MasterMixer_AmbienceVolume_Id, attenuation);
        }
    }

    private void GetAmbienceVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat(s_MasterMixer_AmbienceVolume_Id, out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void GetScreenShake(out bool o_ScreenShake)
    {
        o_ScreenShake = GameSettings.GetBoolMain(Settings.s_ScreenshakeSetting);
    }

    //private void GetSlowMotion(out bool o_SlowMotion)
    //{
    //    o_SlowMotion = GameSettings.GetBoolMain(Settings.s_SlowMotionSetting);
    //}

    private void GetCameraMovement(out bool o_CameraMovement)
    {
        o_CameraMovement = GameSettings.GetBoolMain(Settings.s_CameraMovementSetting);
    }

    // INTERNALS

    private void Internal_SetSfxVolume(float i_Value)
    {
        SetSfxVolume(i_Value);
        SetVoiceoverVolume(i_Value);
        SetAmbienceVolume(i_Value);
    }

    private void Internal_SetMusicVolume(float i_Value)
    {
        SetMusicVolume(i_Value);
    }

    private void InitResolutionSelector()
    {
        SelectorData selectorData = new SelectorData();

        Resolution[] resolutions = Screen.resolutions;

        if (resolutions != null)
        {
            for (int resolutionIndex = 0; resolutionIndex < resolutions.Length; ++resolutionIndex)
            {
                Resolution resolution = resolutions[resolutionIndex];
                string label = resolution.ToString();
                SelectorItem selectorItem = new SelectorItem(resolutionIndex, label, "", null);
                selectorData.AddItem(selectorItem);
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetResolutionSelectorData(selectorData);
        }
    }

    private void Setup()
    {
        SetupResolution();
        SetupFullscreen();

        SetupSfxVolume();
        SetupMusicVolume();

        SetupScreenShake();
        //SetupSlowMotion();
        SetupCameraMovement();
    }

    private void SetupResolution()
    {
        if (viewInstance == null)
            return;

        int currentResolutionIndex = -1;

        Resolution currentResolution = Screen.currentResolution;

        int screenWidth = currentResolution.width;
        int screenHeight = currentResolution.height;

        if (!Screen.fullScreen)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        int screenRefreshRate = currentResolution.refreshRate;

        for (int resolutionIndex = 0; resolutionIndex < Screen.resolutions.Length; ++resolutionIndex)
        {
            Resolution resolution = Screen.resolutions[resolutionIndex];

            if (resolution.width == screenWidth && resolution.height == screenHeight && resolution.refreshRate == screenRefreshRate)
            {
                currentResolutionIndex = resolutionIndex;
                break;
            }
        }

        if (currentResolutionIndex > 0)
        {
            viewInstance.SelectResolutionByIndex(currentResolutionIndex);
        }
    }

    private void SetupFullscreen()
    {
        if (viewInstance == null)
            return;

        bool isFullscreen;
        GetFullscreen(out isFullscreen);

        viewInstance.SetFullscreen(isFullscreen);
    }

    private void SetupSfxVolume()
    {
        if (viewInstance == null)
            return;

        float sfxVolume;
        GetSfxVolume(out sfxVolume);

        viewInstance.SetSfxVolume(sfxVolume);
    }

    private void SetupMusicVolume()
    {
        if (viewInstance == null)
            return;

        float musicVolume;
        GetMusicVolume(out musicVolume);

        viewInstance.SetMusicVolume(musicVolume);
    }

    private void SetupScreenShake()
    {
        if (viewInstance == null)
            return;

        bool screenShake;
        GetScreenShake(out screenShake);

        viewInstance.SetScreenShake(screenShake);
    }

    //private void SetupSlowMotion()
    //{
    //    if (viewInstance == null)
    //        return;

    //    bool slowMotion;
    //    GetSlowMotion(out slowMotion);

    //    viewInstance.SetSlowMotion(slowMotion);
    //}

    private void SetupCameraMovement()
    {
        if (viewInstance == null)
            return;

        bool cameraMovement;
        GetCameraMovement(out cameraMovement);

        viewInstance.SetCameraMovement(cameraMovement);
    }

    // EVENTS

    private void OnApplyResolutionEvent()
    {
        if (viewInstance == null)
            return;

        SelectorItem selectedResolution = viewInstance.selectedResolution;

        if (selectedResolution == null)
            return;

        int resolutionIndex = selectedResolution.id; // NOTE: Index are used as id.

        if (resolutionIndex < 0 || resolutionIndex >= Screen.resolutions.Length)
            return;

        Resolution newResolution = Screen.resolutions[resolutionIndex];

        int screenWidth = newResolution.width;
        int screenHeight = newResolution.height;

        bool isFullscreen = viewInstance.fullscreen;

        // Apply resolution.

        SetScreenResolution(screenWidth, screenHeight, isFullscreen);
    }

    private void OnSfxVolumeChangedEvent(float i_Value)
    {
        GameSettings.SetFloatMain(Settings.s_SfxVolumeSetting, i_Value);

        Internal_SetSfxVolume(i_Value);
    }

    private void OnMusicVolumeChangedEvent(float i_Value)
    {
        GameSettings.SetFloatMain(Settings.s_MusicVolumeSetting, i_Value);

        Internal_SetMusicVolume(i_Value);
    }

    private void OnScreenShakeValueChangedEvent(bool i_Value)
    {
        GameSettings.SetBoolMain(Settings.s_ScreenshakeSetting, i_Value);
    }

    //private void OnSlowMotionValueChangedEvent(bool i_Value)
    //{
    //    GameSettings.SetBoolMain(Settings.s_SlowMotionSetting, i_Value);
    //}

    private void OnCameraMovementValueChangedEvent(bool i_Value)
    {
        GameSettings.SetBoolMain(Settings.s_CameraMovementSetting, i_Value);
    }

    private void OnBackEvent()
    {
        if (m_OnBackEvent != null)
        {
            m_OnBackEvent();
        }
    }
}