using UnityEngine;

using System;

using GoUI;

public class tnPanel_PauseMenu : UIPanel<tnView_PauseMenu>
{
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

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (viewInstance != null)
        {
            viewInstance.onResumeEvent += OnViewResumeEvent;
            viewInstance.onOptionsEvent += OnViewOptionsEvent;
            viewInstance.onRestartEvent += OnViewRestartEvent;
            viewInstance.onExitEvent += OnViewExitEvent;
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
            viewInstance.onResumeEvent -= OnViewResumeEvent;
            viewInstance.onOptionsEvent -= OnViewOptionsEvent;
            viewInstance.onRestartEvent -= OnViewRestartEvent;
            viewInstance.onExitEvent -= OnViewExitEvent;
        }
    }

    // LOGIC

    public void SetResumeButtonActive(bool i_Acitve)
    {
        if (viewInstance != null)
        {
            viewInstance.SetResumeButtonActive(i_Acitve);
        }
    }

    public void SetOptionsButtonActive(bool i_Acitve)
    {
        if (viewInstance != null)
        {
            viewInstance.SetOptionsButtonActive(i_Acitve);
        }
    }

    public void SetRestartButtonActive(bool i_Acitve)
    {
        if (viewInstance != null)
        {
            viewInstance.SetRestartButtonActive(i_Acitve);
        }
    }

    public void SetExitButtonActive(bool i_Acitve)
    {
        if (viewInstance != null)
        {
            viewInstance.SetExitButtonActive(i_Acitve);
        }
    }

    // EVENTS

    private void OnViewResumeEvent()
    {
        if (m_OnResumeEvent != null)
        {
            m_OnResumeEvent();
        }
    }

    private void OnViewOptionsEvent()
    {
        if (m_OnOptionsEvent != null)
        {
            m_OnOptionsEvent();
        }
    }

    private void OnViewRestartEvent()
    {
        if (m_OnRestartEvent != null)
        {
            m_OnRestartEvent();
        }
    }

    private void OnViewExitEvent()
    {
        if (m_OnExitEvent != null)
        {
            m_OnExitEvent();
        }
    }
}