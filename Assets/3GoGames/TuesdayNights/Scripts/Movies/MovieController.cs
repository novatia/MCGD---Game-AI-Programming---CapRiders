#pragma warning disable 618

using UnityEngine;
using UnityEngine.UI;

using System.Collections;

[RequireComponent(typeof(RawImage))]
public class MovieController : MonoBehaviour
{
    [SerializeField]
    private MovieTexture m_MovieTexture = null;
    [SerializeField]
    private bool m_Loop = false;

    [SerializeField]
    private bool m_EnableAudio = true;

    [SerializeField]
    private bool m_Autoplay = true;

    private bool m_IsPlaying = false;

    private RawImage m_RawImage = null;
    private AudioSource m_AudioSource = null;

    void Awake()
    {
        m_RawImage = GetComponent<RawImage>();
        m_AudioSource = GetComponent<AudioSource>();

        if (m_MovieTexture != null)
        {
            m_MovieTexture.loop = m_Loop;

            if (m_EnableAudio)
            {
                m_AudioSource.clip = m_MovieTexture.audioClip;
            }
        }

        m_RawImage.texture = null;

        m_RawImage.enabled = false;
    }

    void OnEnable()
    {
        if (m_Autoplay)
        {
            Play();
        }
    }

    void OnDisable()
    {
        Stop();
    }

    // BUSINESS LOGIC

    public bool isPlaying
    {
        get
        {
            return m_IsPlaying;
        }
    }

    public void Play()
    {
        if (isPlaying)
            return;

        if (m_MovieTexture == null)
            return;

        // Start playing MovieTexture.

        m_MovieTexture.Play();

        // Play AudioSource, if any.

        if (m_AudioSource != null)
        {
            m_AudioSource.Play();
        }

        // Bind MovieTexture with my RawImage.

        m_RawImage.enabled = true;
        m_RawImage.texture = m_MovieTexture;

        m_IsPlaying = true;
    }

    public void Stop()
    {
        if (!isPlaying)
            return;

        if (m_MovieTexture == null)
            return;

        // Restore original RawImage's texture.

        m_RawImage.texture = null;
        m_RawImage.enabled = false;

        // Stop playing MovieTexture.

        m_MovieTexture.Stop();

        // Stop AudioSource, if any.

        if (m_AudioSource != null)
        {
            m_AudioSource.Stop();
        }

        m_IsPlaying = false;
    }
}

#pragma warning restore 618