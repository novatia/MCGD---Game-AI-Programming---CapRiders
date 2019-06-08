using System.Collections;

public enum RoutineState
{
    None,
    Running,
    Paused,
    Stopped,
    Finished,
}

public class Routine
{
    private IEnumerator m_Enumerator = null;
    private RoutineState m_State = RoutineState.None;

    // ACCESSORS

    public RoutineState state
    {
        get
        {
            return m_State;
        }
    }
    
    public bool isRunning
    {
        get
        {
            return (m_State == RoutineState.Running);
        }
    }

    public bool isPaused
    {
        get
        {
            return (m_State == RoutineState.Paused);
        }
    }

    public bool isFinished
    {
        get
        {
            return (m_State == RoutineState.Finished);
        }
    }

    public bool isStopped
    {
        get
        {
            return (m_State == RoutineState.Stopped);
        }
    }

    // LOGIC

    public void Start()
    {
        if (m_State != RoutineState.None)
            return;

        m_State = RoutineState.Running;
    }

    public IEnumerator Step()
    {
        if (m_State == RoutineState.Running)
        {
            if (m_Enumerator != null)
            {
                if (!m_Enumerator.MoveNext())
                {
                    m_State = RoutineState.Finished;
                }
                else
                {
                    yield return m_Enumerator.Current;
                }
            }
        }
    }

    public void Stop()
    {

    }

    public void Pause()
    {
        if (m_State == RoutineState.Running)
        {
            m_State = RoutineState.Paused;
        }
    }

    public void Resume()
    {
        if (m_State != RoutineState.Paused)
            return;

        m_State = RoutineState.Running;
    }

    // CTOR

    public Routine(IEnumerator i_Enumerator)
    {
        m_Enumerator = i_Enumerator;
        m_State = RoutineState.None;
    }
}
