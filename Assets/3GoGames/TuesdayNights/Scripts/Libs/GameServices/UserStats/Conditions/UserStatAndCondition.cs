public class UserStatAndCondition : UserStatCondition
{
    private UserStatCondition m_FirstCondition = null;
    private UserStatCondition m_SecondCondition = null;

    // OVERRIDE

    public override void Initialize(StatsModule i_StatsModule)
    {
        if (m_FirstCondition != null)
        {
            m_FirstCondition.Initialize(i_StatsModule);
        }

        if (m_SecondCondition != null)
        {
            m_FirstCondition.Initialize(i_StatsModule);
        }
    }

    protected override bool OnEvaluate()
    {
        return InternalEvaluate();
    }

    // INTERNALS

    private bool InternalEvaluate()
    {
        bool first = false;

        if (m_FirstCondition != null)
        {
            first = m_FirstCondition.Evaluate();
        }

        bool second = false;

        if (m_SecondCondition != null)
        {
            second = m_SecondCondition.Evaluate();
        }

        return (first && second);
    }

    // CTOR

    public UserStatAndCondition(UserStatCondition i_FirstCondition, UserStatCondition i_SecondCondition)
    {
        m_FirstCondition = i_FirstCondition;
        m_SecondCondition = i_SecondCondition;
    }
}
