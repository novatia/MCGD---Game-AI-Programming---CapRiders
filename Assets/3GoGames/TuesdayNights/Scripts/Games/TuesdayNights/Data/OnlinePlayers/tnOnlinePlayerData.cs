using UnityEngine;

public class tnOnlinePlayerData
{
    private Color m_Color = Color.white;

    // ACCESSORS

    public Color color
    {
        get
        {
            return m_Color;
        }
    }

    // CTOR

    public tnOnlinePlayerData(tnOnlinePlayerDataDescriptor i_Descriptor)
    {
        if (i_Descriptor == null)
            return;

        m_Color = i_Descriptor.color;
    }
}