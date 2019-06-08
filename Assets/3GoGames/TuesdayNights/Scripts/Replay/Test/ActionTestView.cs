using UnityEngine;

public struct ActionTestData
{
    private Vector3 m_Position;

    public Vector3 position
    {
        get
        {
            return m_Position;
        }
    }

    // LOGIC

    public void SetPosition(Vector3 i_Position)
    {
        m_Position = i_Position;
    }

    // CTOR

    public ActionTestData(Vector3 i_Position)
    {
        m_Position = i_Position;
    }
}

[RequireComponent(typeof(ActionTest))]
public class ActionTestView : MonoBehaviour, IReplayable
{
    private ActionTest m_ActionTest = null;

    private float m_TimeAccumulator = 0f;

    private bool m_IsRecording = false;

    private TimedBuffer<bool> m_Buffer = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_ActionTest = GetComponent<ActionTest>();

        int bufferSize = 10;
        m_Buffer = new TimedBuffer<bool>(bufferSize);
    }

    void OnEnable()
    {
        m_ActionTest.onAction += OnAction;
    }

    void OnDisable()
    {
        m_ActionTest.onAction -= OnAction;
    }

    // IReplayable's interface

    // RECORD

    public void StartRecord()
    {
        m_TimeAccumulator = 0f;

        m_Buffer.Clear();

        m_IsRecording = true;
    }

    public void StopRecord()
    {
        m_IsRecording = false;
    }

    public void UpdateRecord(float i_DeltaTime)
    {
        m_TimeAccumulator += i_DeltaTime;
    }

    // PLAY
    
    public void StartPlay(float i_StartTime)
    {

    }

    public void StopPlay()
    {

    }

    public void UpdatePlay(float i_LastPlayedTime, float i_PlayTime)
    {
        int lastIndexPlayed;
        float lastPlayedTimestamp;
        if (m_Buffer.TryGetIndex(i_LastPlayedTime, out lastIndexPlayed, out lastPlayedTimestamp))
        {
            int index;
            float timestamp;
            if (m_Buffer.TryGetIndex(i_PlayTime, out index, out timestamp))
            {
                for (int i = lastIndexPlayed; i <= index; ++i)
                {
                    float t;
                    m_Buffer.GetData(i, out t);

                    if (t > i_LastPlayedTime && t <= i_PlayTime)
                    {
                        m_ActionTest.ForceEffect();
                    }
                }
            }
        }
    }

    // INTERNALS

    private void OnAction()
    {
        if (m_IsRecording)
        {
            m_Buffer.Push(m_TimeAccumulator, true);
        }
    }
}
