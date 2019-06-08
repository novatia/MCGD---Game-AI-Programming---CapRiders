using UnityEngine;

public class tnCreditsTextEntry
{
    private string m_Label = "";

    private bool m_OverrideProperties = false;

    private Color m_Color = Color.white;

    private Font m_Font = null;
    private int m_FontSize = 0;

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

    // CTOR

    public tnCreditsTextEntry(tnCreditsTextEntryDescriptor i_Descriptor)
    {
        if (i_Descriptor != null)
        {
            m_Label = i_Descriptor.label;

            m_OverrideProperties = i_Descriptor.overrideProperties;

            m_Color = i_Descriptor.color;

            m_Font = i_Descriptor.font;
            m_FontSize = i_Descriptor.fontSize;
        }
    }
}
