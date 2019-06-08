using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

using GoUI;

public class tnView_Celebration : GoUI.UIView
{
    // STATIC

    private static int s_Animator_Anim_BoolId = Animator.StringToHash("Anim");

    // Serializeable fields

    [SerializeField]
    private Animator m_CelebrationAnimator = null;
    [SerializeField]
    [GreaterOrEqual(0f)]
    private float m_CelebrationDuration = 2f;
    [SerializeField]
    private Text m_CelebrationText = null;

    // Fields

    private Coroutine m_Coroutine = null;
    private Action m_Callback = null;

    // UIView's interface

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

        Internal_StopCoroutine();
    }

    // LOGIC

    public void SetCelebrationText(string i_Text)
    {
        Internal_SetCelebrationText(i_Text);
    }

    public void StartCelebration(Action i_CelebrationCompleted = null)
    {
        Internal_StopCoroutine();

        m_Callback = i_CelebrationCompleted;

        IEnumerator celebration = Celebration();
        m_Coroutine = StartCoroutine(celebration);
    }

    // INTERNALS

    private void Internal_SetCelebrationText(string i_Text)
    {
        if (m_CelebrationText != null)
        {
            m_CelebrationText.text = i_Text;
        }
    }

    private void Internal_StopCoroutine()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;

            if (m_CelebrationAnimator != null)
            {
                m_CelebrationAnimator.SetBool(s_Animator_Anim_BoolId, false);
            }
        }

        Callback();
    }

    private IEnumerator Celebration()
    {
        if (m_CelebrationDuration > 0f)
        {
            if (m_CelebrationAnimator != null)
            {
                m_CelebrationAnimator.SetBool(s_Animator_Anim_BoolId, true);

                float time = 0f;
                while (time < m_CelebrationDuration)
                {
                    yield return null;
                    time += Time.deltaTime;
                }

                m_CelebrationAnimator.SetBool(s_Animator_Anim_BoolId, false);
            }
        }

        Callback();
    }

    private void Callback()
    {
        if (m_Callback != null)
        {
            m_Callback();
            m_Callback = null;
        }
    }
}