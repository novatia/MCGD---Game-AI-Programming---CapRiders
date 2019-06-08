using UnityEngine;
using UnityEngine.UI;

using System;

using GoUI;

public class tnView_InGameOptions : GoUI.UIView
{
    // Serializable fields

    [Header("Video")]

    [SerializeField]
    private GameObject m_Video = null;

    [SerializeField]
    private UISelector m_ResolutionSelector = null;
    [SerializeField]
    private Toggle m_FullscreenToggle = null;
    [SerializeField]
    private Button m_ApplyButton = null;

    [Header("Audio")]

    [SerializeField]
    private GameObject m_Audio = null;

    [SerializeField]
    private Slider m_SfxSlider = null;
    [SerializeField]
    private Slider m_MusicSlider = null;

    [Header("Game")]

    [SerializeField]
    private GameObject m_Game = null;

    [SerializeField]
    private Toggle m_ScreenShakeToggle = null;
    [SerializeField]
    private Toggle m_SlowMotionToggle = null;
    [SerializeField]
    private Toggle m_CameraMovementToggle = null;

    [Header("Triggers")]

    [SerializeField]
    private UIEventTrigger m_CancelTrigger = null;

    // Fields

    private event Action m_OnApplyResolutionEvent = null;
    private event Action<float> m_OnSfxVolumeChangedEvent = null;
    private event Action<float> m_OnMusicVolumeChangedEvent = null;
    private event Action<bool> m_OnScreenShakeValueChangedEvent = null;
    private event Action<bool> m_OnSlowMotionValueChangedEvent = null;
    private event Action<bool> m_OnCameraMovementValueChangedEvent = null;

    private event Action m_OnBackEvent = null;

    // ACCESSORS

    public SelectorItem selectedResolution
    {
        get { return ((m_ResolutionSelector != null) ? m_ResolutionSelector.currentItem : null); }
    }

    public bool fullscreen
    {
        get { return ((m_FullscreenToggle != null) ? m_FullscreenToggle.isOn : false); }
    }

    public float sfxVolume
    {
        get { return ((m_SfxSlider != null) ? m_SfxSlider.value : 0f); }
    }

    public float musicVolume
    {
        get { return ((m_MusicSlider != null) ? m_MusicSlider.value : 0f); }
    }

    public bool screenShake
    {
        get { return ((m_ScreenShakeToggle != null) ? m_ScreenShakeToggle.isOn : false); }
    }

    public bool slowMotion
    {
        get { return ((m_SlowMotionToggle != null) ? m_SlowMotionToggle.isOn : false); }
    }

    public bool cameraMovement
    {
        get { return ((m_CameraMovementToggle != null) ? m_CameraMovementToggle.isOn : false); }
    }

    public event Action onApplyResolutionEvent
    {
        add { m_OnApplyResolutionEvent += value; }
        remove { m_OnApplyResolutionEvent -= value; }
    }

    public event Action<float> onSfxVolumeChangedEvent
    {
        add { m_OnSfxVolumeChangedEvent += value; }
        remove { m_OnSfxVolumeChangedEvent -= value; }
    }

    public event Action<float> onMusicVolumeChangedEvent
    {
        add { m_OnMusicVolumeChangedEvent += value; }
        remove { m_OnMusicVolumeChangedEvent -= value; }
    }

    public event Action<bool> onScreenShakeValueChangedEvent
    {
        add { m_OnScreenShakeValueChangedEvent += value; }
        remove { m_OnScreenShakeValueChangedEvent -= value; }
    }

    public event Action<bool> onSlowMotionValueChangedEvent
    {
        add { m_OnSlowMotionValueChangedEvent += value; }
        remove { m_OnSlowMotionValueChangedEvent -= value; }
    }

    public event Action<bool> onCameraMovementValueChangedEvent
    {
        add { m_OnCameraMovementValueChangedEvent += value; }
        remove { m_OnCameraMovementValueChangedEvent -= value; }
    }

    public event Action onBackEvent
    {
        add { m_OnBackEvent += value; }
        remove { m_OnBackEvent -= value; }
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        // Bind listeners.

        if (m_ApplyButton != null)
        {
            m_ApplyButton.onClick.AddListener(OnApplyResolutionButtonClicked);
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

        if (m_SlowMotionToggle != null)
        {
            m_SlowMotionToggle.onValueChanged.AddListener(OnSlowMotionValueChanged);
        }

        if (m_CameraMovementToggle != null)
        {
            m_CameraMovementToggle.onValueChanged.AddListener(OnCameraMovementValueChanged);
        }

        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.onEvent.AddListener(OnCancelTriggerEvent);
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        // Release listeners.

        if (m_ApplyButton != null)
        {
            m_ApplyButton.onClick.RemoveListener(OnApplyResolutionButtonClicked);
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

        if (m_SlowMotionToggle != null)
        {
            m_SlowMotionToggle.onValueChanged.RemoveListener(OnSlowMotionValueChanged);
        }

        if (m_CameraMovementToggle != null)
        {
            m_CameraMovementToggle.onValueChanged.RemoveListener(OnCameraMovementValueChanged);
        }

        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.onEvent.RemoveListener(OnCancelTriggerEvent);
        }
    }

    // LOGIC

    public void SetVideoOptionsActive(bool i_Active)
    {
        Internal_SetVideoOptionsActive(i_Active);
    }

    public void SetAudioOptionsActive(bool i_Active)
    {
        Internal_SetAudioOptionsActive(i_Active);
    }

    public void SetGameOptionsActive(bool i_Active)
    {
        Internal_SetGameOptionsActive(i_Active);
    }

    public void SetResolutionSelectorData(SelectorData i_SelectorData)
    {
        Internal_SetResolutionSelectorData(i_SelectorData);
    }

    public void SelectResolutionByIndex(int i_Index)
    {
        if (m_ResolutionSelector != null)
        {
            m_ResolutionSelector.SelectItemByIndex(i_Index);
        }
    }

    public void SelectResolutionById(int i_Id)
    {
        if (m_ResolutionSelector != null)
        {
            m_ResolutionSelector.SelectItem(i_Id);
        }
    }

    public void SetFullscreen(bool i_Value)
    {
        if (m_FullscreenToggle != null)
        {
            m_FullscreenToggle.isOn = i_Value;
        }
    }

    public void SetSfxVolume(float i_Value)
    {
        if (m_SfxSlider != null)
        {
            m_SfxSlider.value = i_Value;
        }
    }

    public void SetMusicVolume(float i_Value)
    {
        if (m_MusicSlider != null)
        {
            m_MusicSlider.value = i_Value;
        }
    }

    public void SetScreenShake(bool i_Value)
    {
        if (m_ScreenShakeToggle != null)
        {
            m_ScreenShakeToggle.isOn = i_Value;
        }
    }

    public void SetSlowMotion(bool i_Value)
    {
        if (m_SlowMotionToggle != null)
        {
            m_SlowMotionToggle.isOn = i_Value;
        }
    }

    public void SetCameraMovement(bool i_Value)
    {
        if (m_CameraMovementToggle != null)
        {
            m_CameraMovementToggle.isOn = i_Value;
        }
    }

    // INTERNALS

    private void Internal_SetVideoOptionsActive(bool i_Active)
    {
        if (m_Video != null)
        {
            m_Video.SetActive(i_Active);
        }
    }

    private void Internal_SetAudioOptionsActive(bool i_Active)
    {
        if (m_Audio != null)
        {
            m_Audio.SetActive(i_Active);
        }
    }

    private void Internal_SetGameOptionsActive(bool i_Active)
    {
        if (m_Game != null)
        {
            m_Game.SetActive(i_Active);
        }
    }

    private void Internal_SetResolutionSelectorData(SelectorData i_SelectorData)
    {
        if (m_ResolutionSelector == null)
            return;

        m_ResolutionSelector.SetData(i_SelectorData);
    }

    // EVENTS

    private void OnApplyResolutionButtonClicked()
    {
        if (m_OnApplyResolutionEvent != null)
        {
            m_OnApplyResolutionEvent();
        }
    }

    private void OnSfxVolumeChanged(float i_Value)
    {
        if (m_OnSfxVolumeChangedEvent != null)
        {
            m_OnSfxVolumeChangedEvent(i_Value);
        }
    }

    private void OnMusicVolumeChanged(float i_Value)
    {
        if (m_OnMusicVolumeChangedEvent != null)
        {
            m_OnMusicVolumeChangedEvent(i_Value);
        }
    }

    private void OnScreenShakeValueChanged(bool i_Value)
    {
        if (m_OnScreenShakeValueChangedEvent != null)
        {
            m_OnScreenShakeValueChangedEvent(i_Value);
        }
    }

    private void OnSlowMotionValueChanged(bool i_Value)
    {
        if (m_OnSlowMotionValueChangedEvent != null)
        {
            m_OnSlowMotionValueChangedEvent(i_Value);
        }
    }

    private void OnCameraMovementValueChanged(bool i_Value)
    {
        if (m_OnCameraMovementValueChangedEvent != null)
        {
            m_OnCameraMovementValueChangedEvent(i_Value);
        }
    }

    private void OnCancelTriggerEvent()
    {
        if (m_OnBackEvent != null)
        {
            m_OnBackEvent();
        }
    }
}