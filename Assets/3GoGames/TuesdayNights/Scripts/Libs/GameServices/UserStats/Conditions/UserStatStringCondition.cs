public class UserStatStringCondition : UserStatCondition
{
    private string m_StatId = "";
    private StringConditionType m_ConditionType = StringConditionType.Equal;
    private string m_Value = "";

    private UserStatString m_Stat = null;

    // OVERRIDE

    public override void Initialize(StatsModule i_StatsModule)
    {
        m_Stat = i_StatsModule.GetStringStat(m_StatId);
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
            string statValue = m_Stat.stringValue;

            switch(m_ConditionType)
            {
                case StringConditionType.Equal:
                    return (statValue == m_Value);

                case StringConditionType.NotEqual:
                    return (statValue != m_Value);
            }
        }

        return false;
    }

    // CTOR

    public UserStatStringCondition(string i_StatId, StringConditionType i_ConditionType, string i_Value)
    {
        m_StatId = i_StatId;
        m_ConditionType = i_ConditionType;
        m_Value = i_Value;
    }
}
