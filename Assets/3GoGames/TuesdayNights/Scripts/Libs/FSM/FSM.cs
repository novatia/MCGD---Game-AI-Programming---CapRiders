using System.Collections.Generic;

public class FSM
{
    private string m_Id = "";
    private int m_HashId = 0;

    private List<FSMState> m_States = null;

    private FSMState m_CurrentState = null;

    // GETTERS and SETTERS

    public string id
    {
        get
        {
            return m_Id;
        }
    }

    public int hashId
    {
        get
        {
            return m_HashId;
        }
    }

    public int statesCount
    {
        get
        {
            return m_States.Count;
        }
    }

    // LOGIC
    public void AddState(FSMState i_State)
    {
        if (i_State == null)
            return;

        InternalAddState(i_State);
    }

    public void Update(float i_DeltaTime)
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.Update(i_DeltaTime);

            int nextStateId = m_CurrentState.DoTransitions(i_DeltaTime);

            if (nextStateId != Hash.s_NULL)
            {
                SetState(nextStateId);
            }
        }
    }

    public void SetState(string i_StateId)
    {
        int hash = StringUtils.GetHashCode(i_StateId);
        SetState(hash);
    }

    public void SetState(int i_StateId)
    {
        InternalSetState(i_StateId);
    }

    // INTERNALS

    private void InternalAddState(FSMState i_State)
    {
        if (i_State == null)
            return;

        bool duplicated = false;

        for (int stateIndex = 0; stateIndex < m_States.Count; ++stateIndex)
        {
            FSMState state = m_States[stateIndex];

            if (state == null)
                continue;

            if (state.hashId == i_State.hashId)
            {
                duplicated = true;
                break;
            }
        }

        if (!duplicated)
        {
            m_States.Add(i_State);
        }

    }

    private void InternalSetState(int i_StateId)
    {
        FSMState targetState = null;

        for (int stateIndex = 0; stateIndex < m_States.Count; ++stateIndex)
        {
            FSMState state = m_States[stateIndex];

            if (state == null)
                continue;

            if (state.hashId == i_StateId)
            {
                targetState = state;
                break;
            }
        }

        if (targetState != null)
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnStateExit();
            }

            m_CurrentState = targetState;

            m_CurrentState.OnStateEnter();   
        }
    }

    // CTOR

    public FSM(string i_Id)
    {
        m_Id = i_Id;
        m_HashId = StringUtils.GetHashCode(m_Id);

        m_States = new List<FSMState>();
    }
}
