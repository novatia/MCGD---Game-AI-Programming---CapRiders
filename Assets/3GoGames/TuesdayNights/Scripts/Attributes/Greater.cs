using UnityEngine;

public class Greater : PropertyAttribute
{
    private int m_IntThreshold = 0;

    public int intThreshold
    {
        get
        {
            return m_IntThreshold;
        }
    }

    // CTOR

    public Greater(int i_Threshold)
    {
        m_IntThreshold = i_Threshold;
    }
}
