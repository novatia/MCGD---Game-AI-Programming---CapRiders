using UnityEngine;

using System.Collections.Generic;

public class ReplaySystem : Singleton<ReplaySystem>
{
    // STATIC

    public static bool initializedMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.initialized;
            }

            return false;
        }
    }

    public static bool isRecordingMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.isRecording;
            }

            return false;
        }
    }

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

    public static int viewsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.viewsCount;
            }

            return 0;
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void RegisterViewMain(ReplayView i_ReplayView)
    {
        if (Instance != null)
        {
            Instance.RegisterView(i_ReplayView);
        }
    }

    public static void UnregisterViewMain(ReplayView i_ReplayView)
    {
        if (Instance != null)
        {
            Instance.UnregisterView(i_ReplayView);
        }
    }

    public static void StartRecordMain()
    {
        if (Instance != null)
        {
            Instance.StartRecord();
        }
    }

    public static void StopRecordMain()
    {
        if (Instance != null)
        {
            Instance.StopRecord();
        }
    }

    public static void StartPlayMain(float i_StartTime)
    {
        if (Instance != null)
        {
            Instance.StartPlay(i_StartTime);
        }
    }

    public static void StopPlayMain()
    {
        if (Instance != null)
        {
            Instance.StopPlay();
        }
    }

    // Fields

    private List<ReplayView> m_Views = null;

    private bool m_Initialized = false;

    private bool m_IsRecording = false;
    private bool m_IsPlaying = false;

    private float m_RecordTime = 0f;
    private float m_PlayTime = 0f;

    // MonoBehaviour's interface

    void Update()
    {
        if (m_IsRecording)
        {
            UpdateRecord();
        }
        else
        {
            if (m_IsPlaying)
            {
                UpdatePlay();
            }
        }
    }

    // LOGIC

    public bool initialized
    {
        get
        {
            return m_Initialized;
        }
    }

    public bool isRecording
    {
        get
        {
            return m_IsRecording;
        }
    }

    public bool isPlaying
    {
        get
        {
            return m_IsPlaying;
        }
    }

    public int viewsCount
    {
        get
        {
            if (m_Views != null)
            {
                return m_Views.Count;
            }

            return 0;
        }
    }

    public void Initialize()
    {
        if (m_Initialized)
            return;

        m_Views = new List<ReplayView>();

        m_Initialized = true;
    }

    public void RegisterView(ReplayView i_ReplayView)
    {
        if (m_Views == null)
            return;

        if (!m_Views.Contains(i_ReplayView))
        {
            m_Views.Add(i_ReplayView);
        }
    }

    public void UnregisterView(ReplayView i_ReplayView)
    {
        if (m_Views == null)
            return;

        m_Views.Remove(i_ReplayView);
    }

    public void StartRecord()
    {
        if (!initialized)
            return;

        if (m_IsRecording || m_IsPlaying)
            return;

        {
            // TODO

            m_RecordTime = 0f;

            for (int viewIndex = 0; viewIndex < m_Views.Count; ++viewIndex)
            {
                ReplayView view = m_Views[viewIndex];
                if (view != null)
                {
                    view.StartRecord();
                }
            }
        }

        m_IsRecording = true; 
    }

    public void StopRecord()
    {
        if (!initialized)
            return;

        if (!m_IsRecording)
            return;

        {
            // TODO

            for (int viewIndex = 0; viewIndex < m_Views.Count; ++viewIndex)
            {
                ReplayView view = m_Views[viewIndex];
                if (view != null)
                {
                    view.StopRecord();
                }
            }
        }

        m_IsRecording = false;
    }

    public void StartPlay(float i_StartTime)
    {
        if (!initialized)
            return;

        if (m_IsPlaying || m_IsRecording)
            return;

        {
            // TODO

            m_PlayTime = i_StartTime;

            for (int viewIndex = 0; viewIndex < m_Views.Count; ++viewIndex)
            {
                ReplayView view = m_Views[viewIndex];
                if (view != null)
                {
                    view.StartPlay(i_StartTime);
                }
            }
        }

        m_IsPlaying = true;
    }

    public void StopPlay()
    {
        if (!initialized)
            return;

        if (!m_IsPlaying)
            return;

        {
            // TODO

            for (int viewIndex = 0; viewIndex < m_Views.Count; ++viewIndex)
            {
                ReplayView view = m_Views[viewIndex];
                if (view != null)
                {
                    view.StopPlay();
                }
            }
        }

        m_IsPlaying = false;
    }

    // INTERNALS

    private void UpdateRecord()
    {
        float deltaTime = Time.deltaTime;

        for (int viewIndex = 0; viewIndex < m_Views.Count; ++viewIndex)
        {
            ReplayView view = m_Views[viewIndex];
            if (view != null)
            {
                view.UpdateRecord(deltaTime);
            }
        }

        m_RecordTime += deltaTime;
    }

    private void UpdatePlay()
    {
        float deltaTime = Time.deltaTime;

        float lastPlayedTime = m_PlayTime;
        m_PlayTime += deltaTime;

        for (int viewIndex = 0; viewIndex < m_Views.Count; ++viewIndex)
        {
            ReplayView view = m_Views[viewIndex];
            if (view != null)
            {
                view.UpdatePlay(lastPlayedTime, m_PlayTime);
            }
        }
    }
}
