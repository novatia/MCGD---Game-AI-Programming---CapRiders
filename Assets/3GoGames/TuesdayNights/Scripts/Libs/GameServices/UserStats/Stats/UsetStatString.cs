public class UserStatString : UserStat
{
    private string m_Value;

    private StringCombineFunction m_StringCombineFunction = StringCombineFunction.Set;

    public string stringValue
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

    public UserStatValueChangedEvent<string> onValueChangedEvent = null;

    // UserStat methods

    public override UserStatType type
    {
        get
        {
            return UserStatType.String;
        }
    }

    // LOGIC

    public void Set(string i_Value)
    {
        stringValue = i_Value;
    }

    public void Add(string i_Value)
    {
        stringValue += i_Value;
    }

    public void Min(string i_Value)
    {
        if (string.Compare(stringValue, i_Value) > 0)
        {
            stringValue = i_Value;
        }
    }

    public void Max(string i_Value)
    {
        if (string.Compare(stringValue, i_Value) < 0)
        {
            stringValue = i_Value;
        }
    }

    public void Combine(string i_Value)
    {
        switch (m_StringCombineFunction)
        {
            case StringCombineFunction.Add:
                Add(i_Value);
                break;

            case StringCombineFunction.Max:
                Max(i_Value);
                break;

            case StringCombineFunction.Min:
                Min(i_Value);
                break;

            case StringCombineFunction.Set:
                Set(i_Value);
                break;
        }
    }

    // INTERNALS

    private void InternalSetValue(string i_Value)
    {
        string oldValue = m_Value;

        if (oldValue != i_Value)
        {
            m_Value = i_Value;
            OnValueChanged(oldValue, m_Value);
        }
    }

    private void OnValueChanged(string i_OldValue, string i_NewValue)
    {
        if (onValueChangedEvent != null)
        {
            onValueChangedEvent(i_OldValue, i_NewValue);
        }
    }

    // CTOR

    public UserStatString(string i_Id, string i_DefaultValue, StringCombineFunction i_CombineFunction)
        : base (i_Id)
    {
        m_Value = i_DefaultValue;
        m_StringCombineFunction = i_CombineFunction;
    }
}