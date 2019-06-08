using UnityEngine;
using UnityEngine.Audio;

using System;

[Serializable]
public class SfxDescriptor
{
    [SerializeField]
    private AudioClip m_AudioClip = null;
    [SerializeField]
    private AudioMixerGroup m_AudioMixerGroup = null;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_Volume = 1f;

    public AudioClip audioClip
    {
        get
        {
            return m_AudioClip;
        }
    }

    public AudioMixerGroup audioMixerGroup
    {
        get
        {
            return m_AudioMixerGroup;
        }
    }

    public float volume
    {
        get
        {
            return Mathf.Clamp01(m_Volume);
        }
    }
}
