using UnityEngine;
using UnityEngine.Audio;

using System.Collections.Generic;

public class SfxPlayer : Singleton<SfxPlayer>
{
    private const int maxAudioSources = 128;
    private Deque<AudioSource> pool;

    private List<AudioSource> activatedInstances;

    // STATIC METHODS

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void PlayMain(AudioClip i_AudioClip)
    {
        if (Instance != null)
        {
            Instance.Play(i_AudioClip);
        }
    }

    public static void PlayMain(AudioClip i_AudioClip, AudioMixerGroup i_Group)
    {
        if (Instance != null)
        {
            Instance.Play(i_AudioClip, i_Group);
        }
    }

    public static void PlayMain(AudioClip i_AudioClip, AudioMixerGroup i_Group, float i_Volume)
    {
        if (Instance != null)
        {
            Instance.Play(i_AudioClip, i_Group, i_Volume);
        }
    }

    public static void PlayMain(AudioClip i_AudioClip, AudioMixerGroup i_Group, float i_Volume, float i_Pitch)
    {
        if (Instance != null)
        {
            Instance.Play(i_AudioClip, i_Group, i_Volume, i_Pitch);
        }
    }

    public static void PlayMain(AudioClip i_AudioClip, AudioMixerGroup i_Group, float i_Volume, float i_Pitch, Vector3 i_Position, Quaternion i_Rotation)
    {
        if (Instance != null)
        {
            Instance.Play(i_AudioClip, i_Group, i_Volume, i_Pitch, i_Position, i_Rotation);
        }
    }

    public static void PlayMain(SfxDescriptor i_Sfx)
    {
        if (Instance != null)
        {
            Instance.Play(i_Sfx);
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        pool = new Deque<AudioSource>();
        activatedInstances = new List<AudioSource>();

        for (int audioSourceIndex = 0; audioSourceIndex < maxAudioSources; ++audioSourceIndex)
        {
            GameObject sfxEffect = new GameObject("SfxEffect");
            sfxEffect.transform.parent = transform;

            AudioSource audioSource = sfxEffect.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = null;
            audioSource.clip = null;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.loop = false;
            audioSource.playOnAwake = false;

            sfxEffect.SetActive(false);

            pool.Add(audioSource);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        for (int audioSourceIndex = 0; audioSourceIndex < activatedInstances.Count; ++audioSourceIndex)
        {
            AudioSource audioSource = activatedInstances[audioSourceIndex];

            if (!audioSource.isPlaying)
            {
                audioSource.Stop();

                activatedInstances.RemoveAt(audioSourceIndex);

                audioSource.volume = 0f;
                audioSource.clip = null;

                audioSource.outputAudioMixerGroup = null;

                audioSource.transform.position = Vector3.zero;
                audioSource.transform.rotation = Quaternion.identity;

                audioSource.gameObject.SetActive(false);

                pool.AddBack(audioSource);
            }
        }
    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    public void Play(AudioClip i_AudioClip)
    {
        Play(i_AudioClip, null, 1f, 1f, Vector3.zero, Quaternion.identity);
    }

    public void Play(AudioClip i_AudioClip, AudioMixerGroup i_Group)
    {
        Play(i_AudioClip, i_Group, 1f, 1f, Vector3.zero, Quaternion.identity);
    }

    public void Play(AudioClip i_AudioClip, AudioMixerGroup i_Group, float i_Volume)
    {
        Play(i_AudioClip, i_Group, i_Volume, 1f, Vector3.zero, Quaternion.identity);
    }

    public void Play(AudioClip i_AudioClip, AudioMixerGroup i_Group, float i_Volume, float i_Pitch)
    {
        Play(i_AudioClip, i_Group, i_Volume, i_Pitch, Vector3.zero, Quaternion.identity);
    }

    public void Play(AudioClip i_AudioClip, AudioMixerGroup i_Group, float i_Volume, float i_Pitch, Vector3 i_Position, Quaternion i_Rotation)
    {
        if (i_AudioClip != null)
        {
            if (pool.Count > 0)
            {
                AudioSource audioSource = pool.RemoveBack();

                audioSource.gameObject.SetActive(true);

                audioSource.transform.position = i_Position;
                audioSource.transform.rotation = i_Rotation;

                audioSource.outputAudioMixerGroup = i_Group;

                audioSource.clip = i_AudioClip;
                audioSource.volume = i_Volume;

                audioSource.pitch = i_Pitch;

                activatedInstances.Add(audioSource);

                audioSource.Play();
            }
        }
    }

    public void Play(SfxDescriptor i_Sfx)
    {
        if (i_Sfx == null)
            return;

        AudioClip audioClip = i_Sfx.audioClip;

        if (audioClip == null)
            return;

        Play(audioClip, i_Sfx.audioMixerGroup, i_Sfx.volume);
    }
}
