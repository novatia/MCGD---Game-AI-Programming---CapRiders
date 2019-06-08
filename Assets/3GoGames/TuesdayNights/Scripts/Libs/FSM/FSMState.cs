using System.Collections.Generic;

public abstract class FSMState
{
    private string m_Id = "";
    private int m_HashId = 0;

    private List<FSMTransition> m_Transitions = null;

    // GETTERS and SETTERS

    public string id
    {
        get { return m_Id; }
    }

    public int hashId
    {
        get { return m_HashId; }
    }

    public int transitionsCount
    {
        get
        {
            return m_Transitions.Count;
        }
    }

    // LOGIC

    public void AddTransition(FSMTransition i_Transition)
    {
        if (i_Transition == null)
            return;

        m_Transitions.Add(i_Transition);
    }

    public void Update(float i_DeltaTime)
    {
        OnUpdate(i_DeltaTime);
    }

    // VIRTUALS

    public virtual int DoTransitions(float i_DeltaTime)
    {
        int targetStateId = Hash.s_NULL;

        for (int transitionIndex = 0; transitionIndex < m_Transitions.Count; ++transitionIndex)
        {
            FSMTransition transition = m_Transitions[transitionIndex];

            if (transition == null)
                continue;

            bool verified = transition.Evaluate(i_DeltaTime);

            if (verified)
            {
                targetStateId = transition.targetStateHashId;
            }
        }

        return targetStateId;
    }

    public virtual void OnStateEnter()
    {

    }

    public virtual void OnStateExit()
    {

    }

    protected virtual void OnUpdate(float i_DeltaTime)
    {

    }

    // CTOR

    public FSMState(string i_Id)
    {
        m_Id = i_Id;
        m_HashId = StringUtils.GetHashCode(m_Id);

        m_Transitions = new List<FSMTransition>();
    }
}
