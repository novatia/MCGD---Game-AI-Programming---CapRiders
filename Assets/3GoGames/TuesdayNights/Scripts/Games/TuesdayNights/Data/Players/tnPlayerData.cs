using UnityEngine;

public class tnPlayerData
{
    private string m_Name = "";
    private Color m_Color = Color.white;
    private string m_PlayerInputName = "NULL";
    private string m_WiFiPlayerInputName = "NULL";

    public string name
    {
        get { return m_Name; }
    }

    public Color color
    {
        get { return m_Color; }
    }

    public string playerInputName
    {
        get { return m_PlayerInputName; }
    }

    public string wifiPlayerInputName
    {
        get { return m_WiFiPlayerInputName; }
    }

    // CTOR

    public tnPlayerData(tnPlayerDataDescriptor i_Descriptor)
    {
        if (i_Descriptor != null)
        {
            m_Name = i_Descriptor.playerName;
            m_Color = i_Descriptor.color;
            m_PlayerInputName = i_Descriptor.playerInputName;
            m_WiFiPlayerInputName = i_Descriptor.wifiPlayerInputName;
        }
    }
}
