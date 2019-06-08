using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Audio;

using TuesdayNights;

public class tnOptionsController : UIViewController
{
    [SerializeField]
    private AudioMixer m_MasterMixer = null;

    private UISelector m_ResolutionSelector = null;

    private Toggle m_FullscreenToggle = null;

    private Button m_ApplyButton = null;

    private Slider m_SfxSlider = null;
    private Slider m_MusicSlider = null;

    private Toggle m_ScreenShakeToggle = null;
    //private Toggle m_SlowMotionToggle = null;
    private Toggle m_CameraMovementToggle = null;

    private Toggle m_XInputToggle = null;
    private Toggle m_RumbleToggle = null;

    private static string s_WidgetId_ResolutionSelector = "WIDGET_SELECTOR_RESOLUTION";
    private static string s_WidgetId_FullscreenToggle = "WIDGET_TOGGLE_FULLSCREEN";
    private static string s_WidgetId_ApplyButton = "WIDGET_BUTTON_APPLY";
    private static string s_WidgetId_SfxSlider = "WIDGET_SLIDER_SFX";
    private static string s_WidgetId_MusicSlider = "WIDGET_SLIDER_MUSIC";
    private static string s_WidgetId_ScreenShakeToggle = "WIDGET_TOGGLE_SCREENSHAKE";
    //private static string s_WidgetId_SlowMotionToggle = "WIDGET_TOGGLE_SLOWMOTION";
    private static string s_WidgetId_CameraMovementToggle = "WIDGET_TOGGLE_CAMERA";
    private static string s_WidgetId_XInputToggle = "WIDGET_TOGGLE_XINPUT";
    private static string s_WidgetId_RumbleToggle = "WIDGET_TOGGLE_RUMBLE";

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        UIPageDescriptor pageDescriptor = GetComponentInChildren<UIPageDescriptor>();

        if (pageDescriptor == null)
            return;

        m_ResolutionSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_ResolutionSelector);
        m_FullscreenToggle = pageDescriptor.GetWidget<Toggle>(s_WidgetId_FullscreenToggle);

        m_ApplyButton = pageDescriptor.GetWidget<Button>(s_WidgetId_ApplyButton);

        m_SfxSlider = pageDescriptor.GetWidget<Slider>(s_WidgetId_SfxSlider);
        m_MusicSlider = pageDescriptor.GetWidget<Slider>(s_WidgetId_MusicSlider);

        m_ScreenShakeToggle = pageDescriptor.GetWidget<Toggle>(s_WidgetId_ScreenShakeToggle);
        //m_SlowMotionToggle = pageDescriptor.GetWidget<Toggle>(s_WidgetId_SlowMotionToggle);
        m_CameraMovementToggle = pageDescriptor.GetWidget<Toggle>(s_WidgetId_CameraMovementToggle);

        m_XInputToggle = pageDescriptor.GetWidget<Toggle>(s_WidgetId_XInputToggle);
        m_RumbleToggle = pageDescriptor.GetWidget<Toggle>(s_WidgetId_RumbleToggle);

        InitResolutionSelector();
    }

    void OnEnable()
    {
        // Configure initial values.

        RefreshWidgets();

        // Bind listeners.

        if (m_ApplyButton != null)
        {
            m_ApplyButton.onClick.AddListener(OnApplyResolution);
        }

        if (m_SfxSlider != null)
        {
            m_SfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        if (m_MusicSlider != null)
        {
            m_MusicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (m_ScreenShakeToggle != null)
        {
            m_ScreenShakeToggle.onValueChanged.AddListener(OnScreenShakeValueChanged);
        }

        //if (m_SlowMotionToggle != null)
        //{
        //    m_SlowMotionToggle.onValueChanged.AddListener(OnSlowMotionValueChanged);
        //}

        if (m_CameraMovementToggle != null)
        {
            m_CameraMovementToggle.onValueChanged.AddListener(OnCameraMovementValueChanged);
        }

        if (m_XInputToggle != null)
        {
            m_XInputToggle.onValueChanged.AddListener(OnXInputChanged);
        }

        if (m_RumbleToggle != null)
        {
            m_RumbleToggle.onValueChanged.AddListener(OnRumbleChanged);
        }
    }

    void OnDisable()
    {
        if (m_ApplyButton != null)
        {
            m_ApplyButton.onClick.RemoveListener(OnApplyResolution);
        }

        if (m_SfxSlider != null)
        {
            m_SfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        }

        if (m_MusicSlider != null)
        {
            m_MusicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }

        if (m_ScreenShakeToggle != null)
        {
            m_ScreenShakeToggle.onValueChanged.RemoveListener(OnScreenShakeValueChanged);
        }

        //if (m_SlowMotionToggle != null)
        //{
        //    m_SlowMotionToggle.onValueChanged.RemoveListener(OnSlowMotionValueChanged);
        //}

        if (m_CameraMovementToggle != null)
        {
            m_CameraMovementToggle.onValueChanged.RemoveListener(OnCameraMovementValueChanged);
        }

        if (m_XInputToggle != null)
        {
            m_XInputToggle.onValueChanged.RemoveListener(OnXInputChanged);
        }

        if (m_RumbleToggle != null)
        {
            m_RumbleToggle.onValueChanged.RemoveListener(OnRumbleChanged);
        }
    }

    // UIViewController's interface

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    // EVENTS

    private void OnApplyResolution()
    {
        if (m_ResolutionSelector == null || m_ResolutionSelector.currentItem == null)
            return;

        if (m_FullscreenToggle == null)
            return;

        int resolutionIndex = m_ResolutionSelector.currentItem.id;

        if (resolutionIndex < 0 || resolutionIndex >= Screen.resolutions.Length)
            return;

        Resolution newResolution = Screen.resolutions[resolutionIndex];

        int screenWidth = newResolution.width;
        int screenHeight = newResolution.height;

        bool isFullscreen = m_FullscreenToggle.isOn;

        // Apply resolution.

        SetScreenResolution(screenWidth, screenHeight, isFullscreen);
    }

    private void OnSfxVolumeChanged(float i_Value)
    {
        GameSettings.SetFloatMain(Settings.s_SfxVolumeSetting, i_Value);

        InternalSetSfxVolume(i_Value);
    }

    private void OnMusicVolumeChanged(float i_Value)
    {
        GameSettings.SetFloatMain(Settings.s_MusicVolumeSetting, i_Value);

        InternalSetMusicVolume(i_Value);
    }

    private void OnScreenShakeValueChanged(bool i_Value)
    {
        GameSettings.SetBoolMain(Settings.s_ScreenshakeSetting, i_Value);
    }

    //private void OnSlowMotionValueChanged(bool i_Value)
    //{
    //    GameSettings.SetBoolMain(Settings.s_SlowMotionSetting, i_Value);
    //}

    private void OnCameraMovementValueChanged(bool i_Value)
    {
        GameSettings.SetBoolMain(Settings.s_CameraMovementSetting, i_Value);
    }

    private void OnXInputChanged(bool i_Value)
    {
        GameSettings.SetBoolMain(Settings.s_UseXInput, i_Value);

        InternalSetXInput(i_Value);

        SetRumbleToggleInteractable(i_Value);
    }

    private void OnRumbleChanged(bool i_Value)
    {
        GameSettings.SetBoolMain(Settings.s_UseRumble, i_Value);
    }

    // UTILS

    private void SetScreenResolution(int i_Width, int i_Height, bool i_IsFullscreen)
    {
        if (i_Width != Screen.width || i_Height != Screen.height || i_IsFullscreen != Screen.fullScreen)
        {
            Screen.SetResolution(i_Width, i_Height, i_IsFullscreen);
        }
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
            m_MasterMixer.SetFloat("SfxVolume", attenuation);
        }
    }

    private void GetSfxVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat("SfxVolume", out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void SetVoiceoverVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat("VoiceoverVolume", attenuation);
        }
    }

    private void GetVoiceoverVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat("VoiceoverVolume", out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void SetMusicVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat("MusicVolume", attenuation);
        }
    }

    private void GetMusicVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat("MusicVolume", out attenuation);

            o_Volume = AudioUtils.DecibelToLinear(attenuation);
        }
    }

    private void SetAmbienceVolume(float i_Volume)
    {
        if (m_MasterMixer != null)
        {
            float attenuation = AudioUtils.LinearToDecibel(i_Volume);
            m_MasterMixer.SetFloat("AmbienceVolume", attenuation);
        }
    }

    private void GetAmbienceVolume(out float o_Volume)
    {
        o_Volume = 0f;

        if (m_MasterMixer != null)
        {
            float attenuation;
            m_MasterMixer.GetFloat("AmbienceVolume", out attenuation);

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

    private void GetXInput(out bool o_UseXInput)
    {
        o_UseXInput = GameSettings.GetBoolMain(Settings.s_UseXInput);
    }

    private void GetRumble(out bool o_UseRumble)
    {
        o_UseRumble = GameSettings.GetBoolMain(Settings.s_UseRumble);
    }

    // INTERNALS

    private void InternalSetSfxVolume(float i_Value)
    {
        SetSfxVolume(i_Value);
        SetVoiceoverVolume(i_Value);
        SetAmbienceVolume(i_Value);
    }

    private void InternalSetMusicVolume(float i_Value)
    {
        SetMusicVolume(i_Value);
    }

    private void InternalSetXInput(bool i_Value)
    {
        InputSystem.useXInputMain = i_Value;
    }

    private void InitResolutionSelector()
    {
        if (m_ResolutionSelector == null)
            return;

        SelectorData selectorData = new SelectorData();

        for (int resolutionIndex = 0; resolutionIndex < Screen.resolutions.Length; ++resolutionIndex)
        {
            Resolution resolution = Screen.resolutions[resolutionIndex];
            string label = resolution.ToString();
            SelectorItem selectorItem = new SelectorItem(resolutionIndex, label, "", null);
            selectorData.AddItem(selectorItem);
        }

        m_ResolutionSelector.SetData(selectorData);
    }

    private void RefreshWidgets()
    {
        RefreshResolutionSelector();

        RefreshFullscreenToggle();

        RefreshSfxSlider();
        RefreshMusicSlider();

        RefreshScreenShakeToggle();
        //RefreshSlowMotionToggle();

        RefreshCameraMovementToggle();

        RefreshXInputToggle();
        RefreshRumbleToggle();
    }

    private void RefreshResolutionSelector()
    {
        if (m_ResolutionSelector == null)
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

        if (currentResolutionIndex >= 0)
        {
            m_ResolutionSelector.SelectItemByIndex(currentResolutionIndex);
        }
    }

    private void RefreshFullscreenToggle()
    {
        if (m_FullscreenToggle == null)
            return;

        bool isFullscreen;
        GetFullscreen(out isFullscreen);

        m_FullscreenToggle.isOn = isFullscreen;
    }

    private void RefreshSfxSlider()
    {
        if (m_SfxSlider == null)
            return;

        float sfxVolume;
        GetSfxVolume(out sfxVolume);

        m_SfxSlider.value = sfxVolume;
    }

    private void RefreshMusicSlider()
    {
        if (m_MusicSlider == null)
            return;

        float musicVolume;
        GetMusicVolume(out musicVolume);

        m_MusicSlider.value = musicVolume;
    }

    private void RefreshScreenShakeToggle()
    {
        if (m_ScreenShakeToggle == null)
            return;

        bool screenShake;
        GetScreenShake(out screenShake);

        m_ScreenShakeToggle.isOn = screenShake;
    }

    //private void RefreshSlowMotionToggle()
    //{
    //    if (m_SlowMotionToggle == null)
    //        return;

    //    bool slowMotion;
    //    GetSlowMotion(out slowMotion);

    //    m_SlowMotionToggle.isOn = slowMotion;
    //}

    private void RefreshCameraMovementToggle()
    {
        if (m_CameraMovementToggle == null)
            return;

        bool cameraMovement;
        GetCameraMovement(out cameraMovement);

        m_CameraMovementToggle.isOn = cameraMovement;
    }

    private void RefreshXInputToggle()
    {
        if (m_XInputToggle == null)
            return;

        bool useXInput;
        GetXInput(out useXInput);

        m_XInputToggle.isOn = useXInput;

        SetRumbleToggleInteractable(useXInput);
    }

    private void RefreshRumbleToggle()
    {
        if (m_RumbleToggle == null)
            return;

        bool useRumble;
        GetRumble(out useRumble);

        m_RumbleToggle.isOn = useRumble;
    }

    private void SetRumbleToggleInteractable(bool i_Value)
    {
        if (m_RumbleToggle == null)
            return;

        m_RumbleToggle.interactable = i_Value;
    }
}