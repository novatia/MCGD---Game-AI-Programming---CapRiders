using UnityEngine;
using UnityEngine.Audio;

public class AudioEffect : Effect
{
    public AudioClip[] clips = null;

    private AudioSource m_AudioSource = null;

    protected override void OnAwake()
    {
        m_AudioSource = GetComponentInChildren<AudioSource>();
        m_AudioSource.playOnAwake = false;
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        if (m_AudioSource == null)
            return;

        if (!m_AudioSource.isPlaying)
        {
            Finish();
        }
    }

    protected override void OnPlay(AnimEventCallback i_Unused = null)
    {
        if (m_AudioSource == null || clips == null || clips.Length == 0)
        {
            Finish();
        }

        int randomIndex = Random.Range(0, clips.Length);
        AudioClip clip = clips[randomIndex];

        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }

    protected override void OnStop()
    {
        if (m_AudioSource != null)
        { 
            m_AudioSource.Stop();
            m_AudioSource.clip = null;
        }
    }
}