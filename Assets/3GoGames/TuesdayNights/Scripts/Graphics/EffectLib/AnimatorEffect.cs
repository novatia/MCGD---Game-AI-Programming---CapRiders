using UnityEngine;

public class AnimatorEffect : Effect
{
    private static int s_WaitStateHash = Animator.StringToHash("Wait");
    private static int s_FinishedStateHash = Animator.StringToHash("Finish");
    private static int s_PlayingStateHash = Animator.StringToHash("Playing");

    private Animator m_Animator = null;
    private SpriteRenderer m_SpriteRenderer = null;

    protected override void OnAwake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        if (m_Animator == null || m_SpriteRenderer == null)
            return;

        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == s_FinishedStateHash)
        {
            Finish();
        }
    }

    protected override void OnPlay(AnimEventCallback i_Unused = null)
    {
        if (m_Animator != null && m_SpriteRenderer != null)
        {
            m_Animator.CrossFade(s_PlayingStateHash, 0f);
        }
        else
        {
            Finish();
        }
    }

    protected override void OnStop()
    {
        if (m_Animator != null)
        {
            m_Animator.CrossFade(s_WaitStateHash, 0f);
        }

        if (m_SpriteRenderer != null)
        {
            m_SpriteRenderer.sprite = null;
        }
    }
}