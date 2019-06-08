using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;

public class CompositeEffect : Effect
{
    public List<Effect> effects = new List<Effect>();

    private int m_RealEffectsCount;
    private int m_Finished;

    protected override void OnUpdate(float i_DeltaTime)
    {
        if (AllFinished())
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
        m_Finished = 0;
        m_RealEffectsCount = GetEffectsCount();

        for (int effectIndex = 0; effectIndex < effects.Count; ++effectIndex)
        {
            Effect currentEffect = effects[effectIndex];
            if (currentEffect != null)
            {
                currentEffect.Play(OnEffectCompleted, i_EventCallback);
            }
        }
    }

    protected override void NotifyStop()
    {
        for (int effectIndex = 0; effectIndex < effects.Count; ++effectIndex)
        {
            Effect currentEffect = effects[effectIndex];
            if (currentEffect != null)
            {
                currentEffect.Stop();
            }
        }
    }

    // INTERNALS

    private void OnEffectCompleted()
    {
        ++m_Finished;
    }

    private bool AllFinished()
    {
        return (m_Finished == m_RealEffectsCount);
    }

    private int GetEffectsCount()
    {
        int count = 0;
        for (int i = 0; i < effects.Count; ++i)
        {
            if (effects[i] != null)
            {
                ++count;
            }
        }

        return count;
    }
}