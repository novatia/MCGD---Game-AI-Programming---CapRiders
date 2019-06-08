using UnityEngine;

using System;
using System.Collections;

using GoUI;

public class tnView_StandardMatchInstructions : GoUI.UIView
{
    // Static

    private static int s_PlayTrigger = Animator.StringToHash("Play");
    private static int s_IdleState = Animator.StringToHash("Idle");

    // Fields

    [SerializeField]
    private UIAnimatorEntry[] m_UIAnimators = null;

    private float[] m_Timers = null;

    private event Action m_OnProceedEvent = null;

    [Header("Widgets")]

    [SerializeField]
    private UIEventTrigger m_StartTrigger = null;

    // ACCESSORS

    public event Action onProceedEvent
    {
        add { m_OnProceedEvent += value; }
        remove { m_OnProceedEvent -= value; }
    }

    // UIView's interface

    protected override void Awake()
    {
        base.Awake();

        if (m_UIAnimators != null && m_UIAnimators.Length > 0)
        {
            m_Timers = new float[m_UIAnimators.Length];
        }
    }

    protected override void OnEnter()
    {
        base.OnEnter();

        ClearAnimatorsState();

        ResetTimers();

        if (m_StartTrigger != null)
        {
            m_StartTrigger.onEvent.AddListener(OnStartTriggerEvent);
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);

        if (m_UIAnimators != null)
        {
            for (int index = 0; index < m_UIAnimators.Length; ++index)
            {
                UpdateAnimator(index);
            }
        }
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (m_StartTrigger != null)
        {
            m_StartTrigger.onEvent.RemoveListener(OnStartTriggerEvent);
        }
    }

    // INTERNALS

    private void ClearAnimatorsState()
    {
        if (m_UIAnimators != null)
        {
            for (int index = 0; index < m_UIAnimators.Length; ++index)
            {
                UIAnimatorEntry entry = m_UIAnimators[index];
                if (entry != null)
                {
                    Animator animator = entry.animator;
                    if (animator != null)
                    {
                        animator.CrossFade(s_IdleState, 0f);
                    }
                }
            }
        }
    }

    private void ResetTimers()
    {
        if (m_Timers == null || m_UIAnimators == null)
            return;

        for (int index = 0; index < m_Timers.Length; ++index)
        {
            if (index < m_UIAnimators.Length)
            {
                UIAnimatorEntry entry = m_UIAnimators[index];
                if (entry != null)
                {
                    m_Timers[index] = entry.delay;
                }
            }
        }
    }

    private void UpdateAnimator(int i_Index)
    {
        if (m_Timers == null || m_UIAnimators == null)
            return;

        if (i_Index < 0 || i_Index >= m_UIAnimators.Length || i_Index >= m_Timers.Length)
            return;

        UIAnimatorEntry entry = m_UIAnimators[i_Index];

        if (entry == null)
            return;

        Animator animator = entry.animator;

        if (animator == null)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == s_IdleState)
        {
            if (m_Timers[i_Index] < 0f)
            {
                m_Timers[i_Index] = entry.delay;
            }
            else
            {
                m_Timers[i_Index] -= Time.deltaTime;

                if (m_Timers[i_Index] < 0f)
                {
                    animator.SetTrigger(s_PlayTrigger);
                }
            }
        }
    }

    // EVENTS

    private void OnStartTriggerEvent()
    {
        if (m_OnProceedEvent != null)
        {
            m_OnProceedEvent();
        }
    }
}