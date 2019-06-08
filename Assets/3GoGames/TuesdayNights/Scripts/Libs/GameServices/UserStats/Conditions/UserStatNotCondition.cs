public class UserStatNotCondition : UserStatCondition
{
    private UserStatCondition m_Condition = null;

    // OVERRIDE

    public override void Initialize(StatsModule i_StatsModule)
    {
        if (m_Condition != null)
        {
            m_Condition.Initialize(i_StatsModule);
        }
    }

    protected override bool OnEvaluate()
    {
        return InternalEvaluate();
    }

    // INTERNALS

    private bool InternalEvaluate()
    {
        bool result = false;

        if (m_Condition != null)
        {
            result = m_Condition.Evaluate();
        }

        return !result;
    }

    // CTOR

    public UserStatNotCondition(UserStatCondition i_Condition)
    {
        m_Condition = i_Condition;
    }
}
