using UnityEngine;

using System;

[Serializable]
public class UIAnimatorEntry
{
    [SerializeField]
    private Animator m_Animator = null;
    [SerializeField]
    private float m_Delay = 0f;

    public Animator animator
    {
        get { return m_Animator; }
    }

    public float delay
    {
        get { return m_Delay; }
    }
}

public class tnPreMatchController : UIViewController
{
    // Static

    private static int s_PlayTrigger = Animator.StringToHash("Play");
    private static int s_IdleState = Animator.StringToHash("Idle");

    // Fields

    [SerializeField]
    private UIAnimatorEntry[] m_UIAnimators = null;

    private float[] m_Timers = null;

    // MonoBehaviour's interface

    void Awake()
    {
        if (m_UIAnimators != null && m_UIAnimators.Length > 0)
        {
            m_Timers = new float[m_UIAnimators.Length];
        }
    }

    void OnEnable()
    {
        ClearAnimatorsState();

        ResetTimers();
    }

    void Update()
    {
        if (m_UIAnimators != null)
        {
            for (int index = 0; index < m_UIAnimators.Length; ++index)
            {
                UpdateAnimator(index);
            }
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
}
