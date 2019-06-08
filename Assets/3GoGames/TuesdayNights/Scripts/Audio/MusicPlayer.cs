using UnityEngine;
using UnityEngine.Audio;

using System;
using System.Collections;

using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : Singleton<MusicPlayer> 
{
    private AudioSource m_AudioSource = null;

    private MusicPlaylist m_Playlist = null;
    private int m_NextIndex = -1;

    private bool m_Playing = false;
    private bool m_Paused = false;

    private IEnumerator m_FadeRoutine = null;

    // STATIC INTERFACE

    public static bool isPlayingMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.isPlaying;
            }

            return false;
        }
    }

    public static float volumeMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.volume;
            }

            return 0f;
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void SetChannelMain(AudioMixerGroup i_Group)
    {
        if (Instance != null)
        {
            Instance.SetChannel(i_Group);
        }
    }

    public static void SetPlaylistMain(MusicPlaylist i_Playlist)
    {
        if (Instance != null)
        {
            Instance.SetPlaylist(i_Playlist);
        }
    }

    public static void SetVolumeMain(float i_Volume)
    {
        if (Instance != null)
        {
            Instance.SetVolume(i_Volume);
        }
    }

    public static void PlayMain()
    {
        if (Instance != null)
        {
            Instance.Play();
        }
    }

    public static void StopMain()
    {
        if (Instance != null)
        {
            Instance.Stop();
        }
    }

    public static void PauseMain()
    {
        if (Instance != null)
        {
            Instance.Pause();
        }
    }

    public static void UnPauseMain()
    {
        if (Instance != null)
        {
            Instance.UnPause();
        }
    }

    public static void FadeToMain(float i_TargetVolume, float i_FadeTime, Action i_Callback = null)
    {
        if (Instance != null)
        {
            Instance.FadeTo(i_TargetVolume, i_FadeTime, i_Callback);
        }
    }

    // GETTERS

    public bool isPlaying
    {
        get
        {
            return m_Playing;
        }
    }

    public float volume
    {
        get
        {
            return m_AudioSource.volume;
        }
    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    public void SetChannel(AudioMixerGroup i_Group)
    {
        m_AudioSource.outputAudioMixerGroup = i_Group;
    }

    public void SetPlaylist(MusicPlaylist i_Playlist)
    {
        Stop();

        m_Playlist = i_Playlist;
        m_NextIndex = -1;

        m_AudioSource.clip = null;

        int tracksCount = (i_Playlist != null) ? i_Playlist.tracksCount : 0;
        m_AudioSource.loop = (tracksCount == 1);
    }

    public void SetVolume(float i_Volume)
    {
        if (m_FadeRoutine != null)
        {
            StopCoroutine(m_FadeRoutine);
            m_FadeRoutine = null;
        }

        InternalSetVolume(i_Volume);
    }

    public void Play()
    {
        if (!m_Playing)
        {
            if (m_Playlist != null && m_Playlist.tracksCount > 0)
            {
                m_NextIndex = Random.Range(0, m_Playlist.tracksCount);
            }
        }

        m_Paused = false;
        m_Playing = true;
    }

    public void Stop()
    {
        m_AudioSource.Stop();

        m_Paused = false;
        m_Playing = false;
    }

    public void Pause()
    {
        m_AudioSource.Pause();

        m_Paused = true;
    }

    public void UnPause()
    {
        m_AudioSource.UnPause();

        m_Paused = false;
    }

    // UTILS

    public void FadeTo(float i_TargetVolume, float i_FadeTime, Action i_Callback = null)
    {
        if (m_FadeRoutine != null)
        {
            StopCoroutine(m_FadeRoutine);
            m_FadeRoutine = null;
        }

        m_FadeRoutine = InternalChangeVolumeTo(i_TargetVolume, i_FadeTime, i_Callback);
        StartCoroutine(m_FadeRoutine);
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.ignoreListenerVolume = true;

        m_AudioSource.Stop();
    }

    void Update()
    {
        if (!m_Playing || m_Paused)
            return;

        if (!m_AudioSource.isPlaying)
        {
            if (m_Playlist == null || m_Playlist.tracksCount == 0)
                return;

            if (m_NextIndex < 0 || m_NextIndex >= m_Playlist.tracksCount)
                return;

            AudioClip track = m_Playlist.GetTrack(m_NextIndex);
            m_AudioSource.clip = track;
            m_AudioSource.Play();

            m_NextIndex = (m_NextIndex + 1) % m_Playlist.tracksCount;
        }
    }

    // COROUTINES

    private IEnumerator InternalChangeVolumeTo(float i_TargetVolume, float i_FadeTime, Action i_Callback = null)
    {
        if (i_TargetVolume > 0f && m_Paused)
        {
            UnPause();
        }

        float originalVolume = volume;

        if (i_FadeTime > 0f)
        {
            float timer = 0f;

            while (timer < i_FadeTime)
            {
                float timePercentage = timer / i_FadeTime;
                timePercentage = Mathf.Clamp01(timePercentage);

                float newVolume = Mathf.Lerp(originalVolume, i_TargetVolume, timePercentage);

                InternalSetVolume(newVolume);

                timer += Time.deltaTime;

                yield return null;
            }
        }

        InternalSetVolume(i_TargetVolume);

        if (i_TargetVolume == 0f && !m_Paused)
        {
            Pause();
        }

        if (i_Callback != null)
        {
            i_Callback();
        }
    }

    // INTERNALS

    private void InternalSetVolume(float i_Volume)
    {
        m_AudioSource.volume = Mathf.Clamp01(i_Volume);
    }
}