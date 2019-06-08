using UnityEngine;
using UnityEngine.Audio;

public class UISfxEffect : MonoBehaviour
{
    [SerializeField]
    private AudioMixerGroup m_AudioMixerGroup = null;

    [Range(0f, 1f)]
    [SerializeField]
    private float m_Volume = 1f;

    public void Play(AudioClip i_Clip)
    {
        SfxPlayer.PlayMain(i_Clip, m_AudioMixerGroup, m_Volume);
    }
}
