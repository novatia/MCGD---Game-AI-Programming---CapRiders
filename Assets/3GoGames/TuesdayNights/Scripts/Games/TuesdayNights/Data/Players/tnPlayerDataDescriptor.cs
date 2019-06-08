using UnityEngine;

using FullInspector;

public class tnPlayerDataDescriptor : BaseScriptableObject
{
    [SerializeField]
    private string m_PlayerName = "";
    [SerializeField]
    private Color m_Color = Color.white;
    [SerializeField]
    private string m_PlayerInputName = "NULL";
    [SerializeField]
    private string m_WiFiPlayerInputName = "NULL";

    public string playerName
    {
        get
        {
            return m_PlayerName;
        }
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
}
