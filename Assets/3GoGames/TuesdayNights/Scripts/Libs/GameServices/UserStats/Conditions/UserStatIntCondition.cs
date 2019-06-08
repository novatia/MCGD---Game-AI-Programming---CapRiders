public class UserStatIntCondition : UserStatCondition
{
    private string m_StatId = "";
    private NumericConditionType m_ConditionType = NumericConditionType.Equal;
    private int m_Value = 0;

    private UserStatInt m_Stat = null;

    // OVERRIDE

    public override void Initialize(StatsModule i_StatsModule)
    {
        m_Stat = i_StatsModule.GetIntStat(m_StatId);
    }

    protected override bool OnEvaluate()
    {
        return InternalEvaluate();
    }

    // INTERNALS

    private bool InternalEvaluate()
    {
        if (m_Stat != null)
        {
            int statValue = m_Stat.intValue;

            switch(m_ConditionType)
            {
                case NumericConditionType.Equal:
                    return (statValue == m_Value);

                case NumericConditionType.Greater:
                    return (statValue > m_Value);

                case NumericConditionType.GreaterOrEqual:
                    return (statValue >= m_Value);

                case NumericConditionType.Less:
                    return (statValue < m_Value);

                case NumericConditionType.LessOrEqual:
                    return (statValue <= m_Value);

                case NumericConditionType.NotEqual:
                    return (statValue != m_Value);
            }
        }

        return false;
    }

    // CTOR

    public UserStatIntCondition(string i_StatId, NumericConditionType i_ConditionType, int i_Value)
    {
        m_StatId = i_StatId;
        m_ConditionType = i_ConditionType;
        m_Value = i_Value;
    }
}
