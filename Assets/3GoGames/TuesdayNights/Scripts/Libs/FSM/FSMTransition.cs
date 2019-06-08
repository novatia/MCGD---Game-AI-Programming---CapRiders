public abstract class FSMTransition
{
    private string m_Id = "";
    private int m_HashId = 0; 

    private string m_TargetStateId = "";
    private int m_TargetStateHashId = 0;

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

    public string targetStateId
    {
        get
        {
            return m_TargetStateId;
        }
    }

    public int targetStateHashId
    {
        get
        {
            return m_TargetStateHashId;
        }
    }
    
    // LOGIC

    public bool Evaluate(float i_DeltaTime)
    {
        return OnEvaluate(i_DeltaTime);
    }

    // VIRTUAL

    protected abstract bool OnEvaluate(float i_DeltaTime);

    // CTOR

    public FSMTransition(string i_Id, string i_TargetStateId)
    {
        m_Id = i_Id;
        m_HashId = StringUtils.GetHashCode(m_Id);

        m_TargetStateId = i_TargetStateId;
        m_TargetStateHashId = StringUtils.GetHashCode(m_TargetStateId);
    }
}
