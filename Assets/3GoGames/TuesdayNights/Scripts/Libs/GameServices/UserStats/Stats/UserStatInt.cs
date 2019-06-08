using UnityEngine;

public class UserStatInt : UserStat
{
    private int m_Value;

    private bool m_UsingLowLimit = false;
    private bool m_UsingHighLimit = false;

    private int m_MinValue;
    private int m_MaxValue;

    private NumericCombineFunction m_NumericCombineFunction = NumericCombineFunction.Set;

    public int intValue
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

    public UserStatValueChangedEvent<int> onValueChangedEvent = null;

    // UserStat methods

    public override UserStatType type
    {
        get
        {
            return UserStatType.Int;
        }
    }

    // LOGIC

    public void Set(int i_Value)
    {
        intValue = i_Value;
    }

    public void Add(int i_Value)
    {
        intValue += i_Value;
    }

    public void Min(int i_Value)
    {
        intValue = Mathf.Min(intValue, i_Value);
    }

    public void Max(int i_Value)
    {
        intValue = Mathf.Max(intValue, i_Value);
    }

    public void Multiply(int i_Value)
    {
        intValue *= i_Value;
    }

    public void Combine(int i_Value)
    {
        switch (m_NumericCombineFunction)
        {
            case NumericCombineFunction.Add:
                Add(i_Value);
                break;

            case NumericCombineFunction.Max:
                Max(i_Value);
                break;

            case NumericCombineFunction.Min:
                Min(i_Value);
                break;

            case NumericCombineFunction.Multiply:
                Multiply(i_Value);
                break;

            case NumericCombineFunction.Set:
                Set(i_Value);
                break;
        }
    }

    // INTERNALS

    private void InternalSetValue(int i_Value)
    {
        int oldValue = m_Value;
        int newValue = InternalClamp(i_Value);

        if (oldValue != newValue)
        {
            m_Value = newValue;
            OnValueChanged(oldValue, m_Value);
        }
    }

    private void OnValueChanged(int i_OldValue, int i_NewValue)
    {
        if (onValueChangedEvent != null)
        {
            onValueChangedEvent(i_OldValue, i_NewValue);
        }
    }

    private int InternalClamp(int i_Value)
    {
        int min = (m_UsingLowLimit) ? m_MinValue : int.MinValue;
        int max = (m_UsingHighLimit) ? m_MaxValue : int.MaxValue;

        int value = i_Value;

        value = Mathf.Clamp(value, min, max);

        return value;
    }

    // CTOR

    public UserStatInt(string i_Id, int i_DefaultValue, bool i_UseLowLimit, bool i_UseHighLimit, int i_MinValue, int i_MaxValue, NumericCombineFunction i_CombineFunction)
        : base(i_Id)
    {
        m_Value = i_DefaultValue;

        m_UsingLowLimit = i_UseLowLimit;
        m_UsingHighLimit = i_UseHighLimit;

        m_MinValue = i_MinValue;
        m_MaxValue = i_MaxValue;

        m_Value = InternalClamp(m_Value);

        m_NumericCombineFunction = i_CombineFunction;
    }
}