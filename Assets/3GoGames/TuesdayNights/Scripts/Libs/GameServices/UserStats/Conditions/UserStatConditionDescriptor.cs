using UnityEngine;

using System;

[Serializable]
public class UserStatConditionDescriptor
{
    [SerializeField]
    private UserStatConditionType m_ConditionType = UserStatConditionType.Int;

    [SerializeField]
    private UserStatConditionDescriptor m_FirstCondition = null;
    [SerializeField]
    private UserStatConditionDescriptor m_SecondCondition = null;

    [SerializeField]
    private UserStatConditionDescriptor m_Condition = null;

    [SerializeField]
    private string m_StatId = "";

    [SerializeField]
    private NumericConditionType m_NumericConditionType = NumericConditionType.Equal;
    [SerializeField]
    private BooleanConditionType m_BooleanConditionType = BooleanConditionType.isTrue;
    [SerializeField]
    private StringConditionType m_StringConditionType = StringConditionType.Equal;

    [SerializeField]
    private int m_IntValue = 0;
    [SerializeField]
    private float m_FloatValue = 0f;
    [SerializeField]
    private string m_StringValue = "";

    public UserStatCondition BuildCondition()
    {
        UserStatCondition condition = null;

        UserStatCondition firstCondition = null;
        UserStatCondition secondCondition = null;

        UserStatCondition subCondition = null;

        switch (m_ConditionType)
        {
            case UserStatConditionType.Int:
                condition = new UserStatIntCondition(m_StatId, m_NumericConditionType, m_IntValue);
                break;

            case UserStatConditionType.Bool:
                condition = new UserStatBoolCondition(m_StatId, m_BooleanConditionType);
                break;

            case UserStatConditionType.Float:
                condition = new UserStatFloatCondition(m_StatId, m_NumericConditionType, m_FloatValue);
                break;

            case UserStatConditionType.String:
                condition = new UserStatStringCondition(m_StatId, m_StringConditionType, m_StringValue);
                break;

            case UserStatConditionType.And:

                firstCondition = m_FirstCondition.BuildCondition();
                secondCondition = m_SecondCondition.BuildCondition();

                condition = new UserStatAndCondition(firstCondition, secondCondition);

                break;

            case UserStatConditionType.Or:

                firstCondition = m_FirstCondition.BuildCondition();
                secondCondition = m_SecondCondition.BuildCondition();

                condition = new UserStatOrCondition(firstCondition, secondCondition);

                break;

            case UserStatConditionType.Xor:

                firstCondition = m_FirstCondition.BuildCondition();
                secondCondition = m_SecondCondition.BuildCondition();

                condition = new UserStatXorCondition(firstCondition, secondCondition);

                break;

            case UserStatConditionType.Not:

                subCondition = m_Condition.BuildCondition();

                condition = new UserStatNotCondition(subCondition);

                break;
        }

        return condition;
    }
}
