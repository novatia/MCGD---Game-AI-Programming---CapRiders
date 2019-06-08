using UnityEngine;
using UnityEngine.UI;

using System;

using GoUI;

public class tnView_PauseMenu : GoUI.UIView
{
    // Serializable fields

    [Header("Widgets")]

    [SerializeField]
    private Button m_ResumeButton = null;
    [SerializeField]
    private Button m_OptionsButton = null;
    [SerializeField]
    private Button m_RestartButton = null;
    [SerializeField]
    private Button m_ExitButton = null;

    [SerializeField]
    private UIEventTrigger m_CancelTrigger = null;

    [Header("Sfx")]

    [SerializeField]
    private SfxDescriptor m_OnEnter = null;
    [SerializeField]
    private SfxDescriptor m_OnExit = null;

    // Fields

    private event Action m_OnResumeEvent = null;
    private event Action m_OnOptionsEvent = null;
    private event Action m_OnRestartEvent = null;
    private event Action m_OnExitEvent = null;

    // ACCESSORS

    public event Action onResumeEvent
    {
        add { m_OnResumeEvent += value; }
        remove { m_OnResumeEvent -= value; }
    }

    public event Action onOptionsEvent
    {
        add { m_OnOptionsEvent += value; }
        remove { m_OnOptionsEvent -= value; }
    }

    public event Action onRestartEvent
    {
        add { m_OnRestartEvent += value; }
        remove { m_OnRestartEvent -= value; }
    }

    public event Action onExitEvent
    {
        add { m_OnExitEvent += value; }
        remove { m_OnExitEvent -= value; }
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        // Play SFX.

        {
            SfxPlayer.PlayMain(m_OnEnter);
        }

        // Bind events.

        if (m_ResumeButton != null)
        {
            m_ResumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        if (m_OptionsButton != null)
        {
            m_OptionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }

        if (m_RestartButton != null)
        {
            m_RestartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.onClick.AddListener(OnExitButtonClicked);
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

        // Release events.

        if (m_ResumeButton != null)
        {
            m_ResumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        }

        if (m_OptionsButton != null)
        {
            m_OptionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        }

        if (m_RestartButton != null)
        {
            m_RestartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.onEvent.RemoveListener(OnCancelTriggerEvent);
        }

        // Play SFX.

        {
            SfxPlayer.PlayMain(m_OnExit);
        }
    }

    // LOGIC

    public void SetResumeButtonActive(bool i_Acitve)
    {
        if (m_ResumeButton != null)
        {
            m_ResumeButton.gameObject.SetActive(i_Acitve);
        }
    }

    public void SetOptionsButtonActive(bool i_Acitve)
    {
        if (m_OptionsButton != null)
        {
            m_OptionsButton.gameObject.SetActive(i_Acitve);
        }
    }

    public void SetRestartButtonActive(bool i_Acitve)
    {
        if (m_RestartButton != null)
        {
            m_RestartButton.gameObject.SetActive(i_Acitve);
        }
    }

    public void SetExitButtonActive(bool i_Acitve)
    {
        if (m_ExitButton != null)
        {
            m_ExitButton.gameObject.SetActive(i_Acitve);
        }
    }

    // EVENTS

    private void OnCancelTriggerEvent()
    {
        OnResumeButtonClicked();
    }

    private void OnResumeButtonClicked()
    {
        if (m_OnResumeEvent != null)
        {
            m_OnResumeEvent();
        }
    }

    private void OnOptionsButtonClicked()
    {
        if (m_OnOptionsEvent != null)
        {
            m_OnOptionsEvent();
        }
    }

    private void OnRestartButtonClicked()
    {
        if (m_OnRestartEvent != null)
        {
            m_OnRestartEvent();
        }
    }

    private void OnExitButtonClicked()
    {
        if (m_OnExitEvent != null)
        {
            m_OnExitEvent();
        }
    }
}