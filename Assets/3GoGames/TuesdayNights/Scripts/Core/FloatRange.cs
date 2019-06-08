using UnityEngine;

using System;

[Serializable]
public class FloatRange
{
    [SerializeField]
    private float m_Min = 0;
    [SerializeField]
    private float m_Max = 1;

    public float min
    {
        get { return m_Min; }
        set { m_Min = value; }
    }

    public float max
    {
        get { return m_Max; }
        set { m_Max = value; }
    }

    // LOGIC

    public bool IsValueValid(float i_Value)
    {
        return (m_Min <= i_Value && m_Max >= i_Value);
    }

    public float GetValueAt(float i_Percentage)
    {
        float p = Mathf.Clamp01(i_Percentage);

        float value = m_Min + p * (m_Max - m_Min);
        return value;
    }

    // CTOR

    public FloatRange(float i_Min, float i_Max)
    {
        m_Min = i_Min;
        m_Max = i_Max;
    }
}