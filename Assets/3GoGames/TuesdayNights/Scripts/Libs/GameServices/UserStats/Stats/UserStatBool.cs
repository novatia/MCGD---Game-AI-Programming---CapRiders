public class UserStatBool : UserStat
{
    private bool m_Value;

    private BooleanCombineFunction m_BooleanCombineFunction = BooleanCombineFunction.Set;

    public bool boolValue
    {
        get
        {
            return m_Value;
        }

        set
        {
            InternalSetValue(value);
        }
    }

    public UserStatValueChangedEvent<bool> onValueChangedEvent = null;

    // UserStat methods

    public override UserStatType type
    {
        get
        {
            return UserStatType.Bool;
        }
    }

    // LOGIC

    public void Set(bool i_Value)
    {
        boolValue = i_Value;
    }

    public void Add(bool i_Value)
    {
        boolValue |= i_Value;
    }

    public void Multiply(bool i_Value)
    {
        boolValue &= i_Value;
    }

    public void Combine(bool i_Value)
    {
        switch (m_BooleanCombineFunction)
        {
            case BooleanCombineFunction.Add:
                Add(i_Value);
                break;

            case BooleanCombineFunction.Multiply:
                Multiply(i_Value);
                break;

            case BooleanCombineFunction.Set:
                Set(i_Value);
                break;
        }
    }

    // INTERNALS

    private void InternalSetValue(bool i_Value)
    {
        bool oldValue = m_Value;

        if (oldValue != i_Value)
        {
            m_Value = i_Value;
            OnValueChanged(oldValue, m_Value);
        }
    }

    private void OnValueChanged(bool i_OldValue, bool i_NewValue)
    {
        if (onValueChangedEvent != null)
        {
            onValueChangedEvent(i_OldValue, i_NewValue);
        }
    }

    // CTOR

    public UserStatBool(string i_Id, bool i_DefaultValue, BooleanCombineFunction i_CombineFunction)
        : base(i_Id)
    {
        m_Value = i_DefaultValue;
        m_BooleanCombineFunction = i_CombineFunction;
    }
}
