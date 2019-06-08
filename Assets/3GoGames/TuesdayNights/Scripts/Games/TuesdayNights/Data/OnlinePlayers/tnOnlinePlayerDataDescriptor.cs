using UnityEngine;

public class tnOnlinePlayerDataDescriptor : ScriptableObject
{
    [SerializeField]
    private Color m_Color = Color.white;

    public Color color
    {
        get
        {
            return m_Color;
        }
    }
}
