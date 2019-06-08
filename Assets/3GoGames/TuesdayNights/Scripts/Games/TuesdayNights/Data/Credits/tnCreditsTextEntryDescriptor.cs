using UnityEngine;

using System;

[Serializable]
public class tnCreditsTextEntryDescriptor
{
    [SerializeField]
    private string m_Label = "";
    [SerializeField]
    private bool m_OverrideProperties = false;
    [SerializeField]
    private Color m_Color = Color.white;
    [SerializeField]
    private Font m_Font = null;
    [SerializeField]
    private int m_FontSize = 16;

    public string label
    {
        get
        {
            return m_Label;
        }
    }

    public bool overrideProperties
    {
        get
        {
            return m_OverrideProperties;
        }
    }

    public Color color
    {
        get
        {
            return m_Color;
        }
    }

    public Font font
    {
        get
        {
            return m_Font;
        }
    }
    
    public int fontSize
    {
        get
        {
            return m_FontSize;
        }
    }
}
