using UnityEngine;

using System;
using System.Collections;

using GoUI;

public class tnPanel_Countdown : UIPanel<tnView_Countdown>
{
    [SerializeField]
    private float m_Delay = 1f;
    [SerializeField]
    private float m_Duration = 3f;

    // MEMBERS

    private Coroutine m_Coroutine = null;

    // EVENTS

    private event Action m_CountdownCompletedEvent = null;

    public event Action countdownCompletedEvent
    {
        add { m_CountdownCompletedEvent += value; }
        remove { m_CountdownCompletedEvent -= value; }
    }

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // LOGIC

    public void StartCountdown()
    {
        Internal_StopCoroutine();

        IEnumerator countdown = Countdown();
        m_Coroutine = StartCoroutine(countdown);
    }

    // INTERNALS

    private void Internal_StopCoroutine()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }

    // ROUTINES

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(m_Delay); // Wait for a small amount of time before proceed.

        // Update countdown.

        float duration = m_Duration;

        int lastSec = Mathf.FloorToInt(duration);
        int index = -1;

        while (duration > 0f)
        {
            index = Mathf.FloorToInt(duration);
            if (index != lastSec)
            {
                if (viewInstance != null)
                {
                    viewInstance.SetIndex(index);
                }
            }

            lastSec = index;

            yield return null;

            duration -= Time.deltaTime;
        }

        if (viewInstance != null)
        {
            viewInstance.Clear();
        }

        // Notify listeners.

        if (m_CountdownCompletedEvent != null)
        {
            m_CountdownCompletedEvent();
        }
    }
}