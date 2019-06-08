using UnityEngine;

public class Less : PropertyAttribute
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

    public Less(int i_Threshold)
    {
        m_IntThreshold = i_Threshold;
    }
}
