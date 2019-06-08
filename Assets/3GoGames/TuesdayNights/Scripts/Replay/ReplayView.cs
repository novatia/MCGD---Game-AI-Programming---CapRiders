using UnityEngine;

using System.Collections.Generic;

using FullInspector;

[DisallowMultipleComponent]
public class ReplayView : BaseBehavior
{
    [SerializeField]
    private List<IReplayable> m_Replayables = new List<IReplayable>();

    private bool m_IsRecording = false;
    private bool m_IsPlaying = false;

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

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        if (!ReplaySystem.initializedMain)
        {
            ReplaySystem.InitializeMain();
        }
    }

    void OnEnable()
    {
        ReplaySystem.RegisterViewMain(this);
    }

    void OnDisable()
    {
        ReplaySystem.UnregisterViewMain(this);
    }

    // LOGIC

    // Record

    public void StartRecord()
    {
        if (m_IsRecording || m_IsPlaying)
            return;

        // TODO

        CallStartRecordMethods();

        m_IsRecording = true;
    }

    public void UpdateRecord(float i_DeltaTime)
    {
        if (m_IsRecording)
        {
            CallUpdateRecordMethods(i_DeltaTime);
        }
    }

    public void StopRecord()
    {
        if (!m_IsRecording)
            return;

        // TODO

        CallStopRecordMethods();

        m_IsRecording = false;
    }

    // Play

    public void StartPlay(float i_StartTime)
    {
        if (m_IsPlaying || m_IsRecording)
            return;

        // TODO

        float startTime = GetClampedPlayTime(i_StartTime);
        CallStartPlayMethods(startTime);

        m_IsPlaying = true;
    }

    public void UpdatePlay(float i_LastPlayedTime, float i_PlayTime)
    {
        if (m_IsPlaying)
        {
            float lastPlayedTime = GetClampedPlayTime(i_LastPlayedTime);
            float playTime = GetClampedPlayTime(i_PlayTime);

            CallUpdatePlayMethods(lastPlayedTime, playTime);
        }
    }

    public void StopPlay()
    {
        if (!m_IsPlaying)
            return;

        // TODO

        CallStopPlayMethods();

        m_IsPlaying = false;
    }

    // UTILS

    private void CallStartRecordMethods()
    {
        for (int index = 0; index < m_Replayables.Count; ++index)
        {
            IReplayable replayable = m_Replayables[index];
            if (replayable != null)
            {
                replayable.StartRecord();
            }
        }
    }

    private void CallStopRecordMethods()
    {
        for (int index = 0; index < m_Replayables.Count; ++index)
        {
            IReplayable replayable = m_Replayables[index];
            if (replayable != null)
            {
                replayable.StopRecord();
            }
        }
    }

    private void CallUpdateRecordMethods(float i_DeltaTime)
    {
        for (int index = 0; index < m_Replayables.Count; ++index)
        {
            IReplayable replayable = m_Replayables[index];
            if (replayable != null)
            {
                replayable.UpdateRecord(i_DeltaTime);
            }
        }
    }

    private void CallStartPlayMethods(float i_StartTime)
    {
        for (int index = 0; index < m_Replayables.Count; ++index)
        {
            IReplayable replayable = m_Replayables[index];
            if (replayable != null)
            {
                replayable.StartPlay(i_StartTime);
            }
        }
    }

    private void CallStopPlayMethods()
    {
        for (int index = 0; index < m_Replayables.Count; ++index)
        {
            IReplayable replayable = m_Replayables[index];
            if (replayable != null)
            {
                replayable.StopPlay();
            }
        }
    }

    private void CallUpdatePlayMethods(float i_LastPlayedTime, float i_PlayTime)
    {
        for (int index = 0; index < m_Replayables.Count; ++index)
        {
            IReplayable replayable = m_Replayables[index];
            if (replayable != null)
            {
                replayable.UpdatePlay(i_LastPlayedTime, i_PlayTime);
            }
        }
    }

    private float GetClampedPlayTime(float i_PlayTime)
    {
        float t = Mathf.Clamp(i_PlayTime, 0f, ReplayConfig.s_RecordTime);
        return t;
    }
}
