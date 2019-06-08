using UnityEngine;

using System;

[Serializable]
public class IntRange
{
    [SerializeField]
    private int m_Min = 0;
    [SerializeField]
    private int m_Max = 1;

    public int min
    {
        get { return m_Min; }
        set { m_Min = value; }
    }

    public int max
    {
        get { return m_Max; }
        set { m_Max = value; }
    }

    // LOGIC

    public bool IsValueValid(int i_Value)
    {
        return (m_Min <= i_Value && m_Max >= i_Value);
    }

    public int GetValueAt(float i_Percentage)
    {
        float p = Mathf.Clamp01(i_Percentage);

        int value = (int)(m_Min + p * (m_Max - m_Min));
        value = Mathf.Clamp(value, m_Min, m_Max);
        return value;
    }

    // CTOR

    public IntRange(int i_Min, int i_Max)
    {
        m_Min = i_Min;
        m_Max = i_Max;
    }
}
