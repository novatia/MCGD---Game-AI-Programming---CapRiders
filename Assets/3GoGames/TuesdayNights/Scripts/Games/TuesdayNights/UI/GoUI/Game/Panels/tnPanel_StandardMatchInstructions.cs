using UnityEngine;

using System;
using System.Collections;

using GoUI;

public class tnPanel_StandardMatchInstructions : UIPanel<tnView_StandardMatchInstructions>
{
    private event Action m_OnProceedEvent = null;

    // ACCESSORS

    public event Action onProceedEvent
    {
        add { m_OnProceedEvent += value; }
        remove { m_OnProceedEvent -= value; }
    }

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (viewInstance != null)
        {
            viewInstance.onProceedEvent += OnViewStartEvent;
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
            viewInstance.onProceedEvent -= OnViewStartEvent;
        }
    }

    // EVENTS

    private void OnViewStartEvent()
    {
        if (m_OnProceedEvent != null)
        {
            m_OnProceedEvent();
        }
    }
}
