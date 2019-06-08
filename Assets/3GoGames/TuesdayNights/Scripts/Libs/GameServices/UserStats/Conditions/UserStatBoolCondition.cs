public class UserStatBoolCondition : UserStatCondition
{
    private string m_StatId = "";
    private BooleanConditionType m_ConditionType = BooleanConditionType.isTrue;

    private UserStatBool m_Stat = null;

    // OVERRIDE

    public override void Initialize(StatsModule i_StatsModule)
    {
        m_Stat = i_StatsModule.GetBoolStat(m_StatId);
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
            bool statValue = m_Stat.boolValue;

            switch(m_ConditionType)
            {
                case BooleanConditionType.isTrue:
                    return statValue;

                case BooleanConditionType.isFalse:
                    return !statValue;
            }
        }

        return false;
    }

    // CTOR

    public UserStatBoolCondition(string i_StatId, BooleanConditionType i_ConditionType)
    {
        m_StatId = i_StatId;
        m_ConditionType = i_ConditionType;
    }
}
