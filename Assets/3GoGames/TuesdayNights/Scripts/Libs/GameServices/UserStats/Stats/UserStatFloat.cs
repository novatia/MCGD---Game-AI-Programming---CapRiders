using UnityEngine;

public class UserStatFloat : UserStat
{
    private float m_Value;

    private bool m_UsingLowLimit = false;
    private bool m_UsingHighLimit = false;

    private float m_MinValue;
    private float m_MaxValue;

    private NumericCombineFunction m_NumericCombineFunction = NumericCombineFunction.Set;

    public float floatValue
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

    public UserStatValueChangedEvent<float> onValueChangedEvent = null;

    // UserStat methods

    public override UserStatType type
    {
        get
        {
            return UserStatType.Float;
        }
    }

    // LOGIC

    public void Set(float i_Value)
    {
        floatValue = i_Value;
    }

    public void Add(float i_Value)
    {
        floatValue += i_Value;
    }

    public void Min(float i_Value)
    {
        floatValue = Mathf.Min(floatValue, i_Value);
    }

    public void Max(float i_Value)
    {
        floatValue = Mathf.Max(floatValue, i_Value);
    }

    public void Multiply(float i_Value)
    {
        floatValue *= i_Value;
    }

    public void Combine(float i_Value)
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

    private void InternalSetValue(float i_Value)
    {
        float oldValue = m_Value;

        if (oldValue != i_Value)
        {
            m_Value = i_Value;
            OnValueChanged(oldValue, m_Value);
        }
    }

    private void OnValueChanged(float i_OldValue, float i_NewValue)
    {
        if (onValueChangedEvent != null)
        {
            onValueChangedEvent(i_OldValue, i_NewValue);
        }
    }

    private float InternalClamp(float i_Value)
    {
        float min = (m_UsingLowLimit) ? m_MinValue : float.MinValue;
        float max = (m_UsingHighLimit) ? m_MaxValue : float.MaxValue;

        float value = i_Value;

        value = Mathf.Clamp(value, min, max);

        return value;
    }

    // CTOR

    public UserStatFloat(string i_Id, float i_DefaultValue, bool i_UseLowLimit, bool i_UseHighLimit, float i_MinValue, float i_MaxValue, NumericCombineFunction i_CombineFunction)
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