using UnityEngine;

public class GreaterOrEqual : PropertyAttribute
{
    private float m_FloatThreshold = 0f;
    private int m_IntThreshold = 0;

    public float floatThreshold
    {
        get
        {
            return m_FloatThreshold;
        }
    }

    public int intThreshold
    {
        get
        {
            return m_IntThreshold;
        }
    }

    // CTOR

    public GreaterOrEqual(int i_Threshold)
    {
        m_FloatThreshold = float.MinValue;
        m_IntThreshold = i_Threshold;
    }

    public GreaterOrEqual(float i_Threshold)
    {
        m_FloatThreshold = i_Threshold;
        m_IntThreshold = int.MinValue;
    }
}
