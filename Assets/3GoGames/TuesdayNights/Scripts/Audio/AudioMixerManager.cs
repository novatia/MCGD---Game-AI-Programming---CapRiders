using UnityEngine;
using UnityEngine.Audio;

using System.Collections;
using System.Collections.Generic;

using Priority_Queue;

public class AudioMixerManager : Singleton<AudioMixerManager>
{
    private class AudioMixerSnapshotData
    {
        private AudioMixerSnapshot m_Snapshot;
        private float m_FadeTime;

        public AudioMixerSnapshot snapshot
        {
            get
            {
                return m_Snapshot;
            }
        }

        public float fadeTime
        {
            get
            {
                return m_FadeTime;
            }
        }

        public AudioMixerSnapshotData(AudioMixerSnapshot i_Snapshot, float i_FadeTime = 0f)
        {
            m_Snapshot = i_Snapshot;
            m_FadeTime = i_FadeTime;
        }
    }

    private SimplePriorityQueue<AudioMixerSnapshotData> m_Queue = new SimplePriorityQueue<AudioMixerSnapshotData>();

    // STATIC INTERFACE

    public static AudioMixerSnapshot currentSnapshotMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.currentSnapshot;
            }

            return null;
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void SetSnapshotMain(AudioMixerSnapshot i_Snapshot, float i_FadeTime, double i_Priority = 0.0)
    {
        if (Instance != null)
        {
            Instance.SetSnapshpt(i_Snapshot, i_FadeTime, i_Priority);
        }
    }

    public static void RemoveMain(AudioMixerSnapshot i_Snapshot)
    {
        if (Instance != null)
        {
            Instance.Remove(i_Snapshot);
        }
    }

    public static void ClearMain()
    {
        if (Instance != null)
        {
            Instance.Clear();
        }
    }

    // BUSINESS LOGIC

    public AudioMixerSnapshot currentSnapshot
    {
        get
        {
            if (m_Queue.Count == 0 || m_Queue.First == null)
            {
                return null;
            }

            AudioMixerSnapshotData snapshotData = m_Queue.First;
            return snapshotData.snapshot;
        }
    }

    public void Initialize()
    {

    }

    public void SetSnapshpt(AudioMixerSnapshot i_Snapshot, float i_FadeTime, double i_Priority = 0.0)
    {
       if (AddToQueue(i_Snapshot, i_FadeTime, i_Priority))
        {
            UpdateMixer();
        }
    }

    public void Remove(AudioMixerSnapshot i_Snapshot)
    {
        if (RemoveFromQueue(i_Snapshot))
        {
            UpdateMixer();
        }
    }

    public void Clear()
    {
        ClearQueue();

        UpdateMixer();
    }

    // INTERNALS

    private bool AddToQueue(AudioMixerSnapshot i_Snapshot, float i_FadeTime, double i_Priority)
    {
        if (i_Snapshot == null)
        {
            return false;
        }

        AudioMixerSnapshotData newSnapshot = new AudioMixerSnapshotData(i_Snapshot, i_FadeTime);
        m_Queue.Enqueue(newSnapshot, i_Priority);
        return true;
    }

    private bool RemoveFromQueue(AudioMixerSnapshot i_Snapshot)
    {
        AudioMixerSnapshotData snapshotData = GetDataFromQueue(i_Snapshot);
        if (snapshotData == null)
        {
            return false;
        }

        m_Queue.Remove(snapshotData);
        return true;
    }

    private void ClearQueue()
    {
        m_Queue.Clear();
    }

    private void UpdateMixer()
    {
        if (m_Queue.Count == 0)
            return;

        AudioMixerSnapshotData nextSnapshot = m_Queue.First;
        if (nextSnapshot != null)
        {
            AudioMixerSnapshot s = nextSnapshot.snapshot;
            if (s != null)
            {
                s.TransitionTo(nextSnapshot.fadeTime);
            }
        }
    }

    private AudioMixerSnapshotData GetDataFromQueue(AudioMixerSnapshot i_Snapshot)
    {
        AudioMixerSnapshotData targetSnapshot = null;

        foreach (AudioMixerSnapshotData s in m_Queue)
        {
            if (s != null)
            {
                if (s.snapshot == i_Snapshot)
                {
                    targetSnapshot = s;
                    break;
                }
            }
        }

        return targetSnapshot;
    }
}