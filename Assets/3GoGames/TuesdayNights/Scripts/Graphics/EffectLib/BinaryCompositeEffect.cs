using UnityEngine;

using System.Collections;
using System;

public class BinaryCompositeEffect : Effect
{
    public Effect firstEffect = null;
    public Effect secondEffect = null;

    private bool m_FirstEffectCompleted = false;
    private bool m_SecondEffectCompleted = false;

    protected override void OnUpdate(float i_DeltaTime)
    {
        if (m_FirstEffectCompleted && m_SecondEffectCompleted)
        {
            Finish();
        }
    }

    protected override void OnPlay(AnimEventCallback i_EventCallback = null)
    {

    }

    protected override void OnStop()
    {

    }

    protected override void NotifyPlay(AnimEventCallback i_EventCallback = null)
    {
        m_FirstEffectCompleted = false;
        m_SecondEffectCompleted = false;

        if (firstEffect != null)
        {
            firstEffect.Play(OnFirstEffectCompleted, i_EventCallback);
        }
        else
        {
            m_FirstEffectCompleted = true;
        }

        if (secondEffect != null)
        {
            secondEffect.Play(OnSecondEffectCompleted, i_EventCallback);
        }
        else
        {
            m_SecondEffectCompleted = true;
        }
    }

    protected override void NotifyStop()
    {
        if (firstEffect != null)
        {
            firstEffect.Stop();
        }

        if (secondEffect != null)
        {
            secondEffect.Stop();
        }
    }

    // INTERNALS

    private void OnFirstEffectCompleted()
    {
        m_FirstEffectCompleted = true;
    }

    private void OnSecondEffectCompleted()
    {
        m_SecondEffectCompleted = true;
    }
}