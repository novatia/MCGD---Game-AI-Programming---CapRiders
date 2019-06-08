using UnityEngine;

using System.Collections;
using System;

public class ParticleEffect : Effect
{
    public enum FinishType
    {
        AllParticlesDie,
        EmissionIsStopped
    }

    public bool clearOnStop = true;
    public FinishType finishWhen = FinishType.EmissionIsStopped;

    private ParticleSystem m_ParticleSystem = null;

    protected override void OnAwake()
    {
        m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        if (m_ParticleSystem == null)
            return;

        bool finished = (finishWhen == FinishType.EmissionIsStopped) ? (m_ParticleSystem.time == m_ParticleSystem.main.duration) : (!m_ParticleSystem.IsAlive());

        if (finished)
        {
            Finish();
        }
    }

    protected override void OnPlay(AnimEventCallback i_Unused = null)
    {
        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.Play();
        }
        else
        {
            Finish();
        }
    }

    protected override void OnStop()
    {
        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.Stop();
            if (clearOnStop)
            {
                m_ParticleSystem.Clear();
            }
        }
    }

    // BUSINESS LOGIC

    public void SetParticleSeed(uint i_Seed)
    {
        m_ParticleSystem.randomSeed = i_Seed;
    }
}