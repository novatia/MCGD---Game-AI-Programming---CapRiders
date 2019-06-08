using UnityEngine;

using System.Collections.Generic;

using LineUp = System.Collections.Generic.List<int>;

public class tnTeamData
{
    private string m_Name = "";

    private LineUp m_CharactersKeys = null;
    private List<LineUp> m_DefaultLineUps = null;

    private Sprite m_Flag = null;
    private Sprite m_Icon = null;

    private Sprite m_BaseSprite = null;
    private Color m_FirstColor = Color.white;
    private Color m_SecondColor = Color.white;
    private Color m_SupportersFirstColor = Color.white;
    private Color m_SupportersSecondColor = Color.white;

    private tnStatsDatabase m_TeamStats = null;

    public string name
    {
        get { return m_Name; }
    }

    public int charactersCount
    {
        get
        {
            return m_CharactersKeys.Count;
        }
    }

    public Sprite flag
    {
        get { return m_Flag; }
    }

    public Sprite icon
    {
        get
        {
            return m_Icon;
        }
    }

    public Sprite baseSprite
    {
        get { return m_BaseSprite; }
    }

    public Color firstColor
    {
        get { return m_FirstColor; }
    }

    public Color secondColor
    {
        get { return m_SecondColor; }
    }

    public Color supportersFirstColor
    {
        get { return m_SupportersFirstColor; }
    }

    public Color supportersSecondColor
    {
        get { return m_SupportersSecondColor; }
    }

    public tnStatsDatabase teamStats
    {
        get
        {
            return m_TeamStats;
        }
    }

    // LOGIC

    public int GetCharacterKey(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_CharactersKeys.Count)
        {
            return Hash.s_NULL;
        }

        return m_CharactersKeys[i_Index];
    }

    public LineUp GetDefaultLineUp(int i_TeamSize)
    {
        for (int lineUpIndex = 0; lineUpIndex < m_DefaultLineUps.Count; ++lineUpIndex)
        {
            LineUp lineUp = m_DefaultLineUps[lineUpIndex];
            int size = lineUp.Count;
            if (size == i_TeamSize)
            {
                LineUp target = new LineUp(lineUp);
                return target;
            }
        }

        return null;
    }

    public bool Contains(int i_CharacterId)
    {
        return m_CharactersKeys.Contains(i_CharacterId);
    }

    // CTOR

    public tnTeamData(tnTeamDataDescriptor i_Descriptor)
    {
        m_CharactersKeys = new LineUp();
        m_DefaultLineUps = new List<LineUp>();

        if (i_Descriptor != null)
        {
            m_Name = i_Descriptor.teamName;

            for (int characterIndex = 0; characterIndex < i_Descriptor.charactersCount; ++characterIndex)
            {
                string key = i_Descriptor.GetCharacterKey(characterIndex);
                if (key != "")
                {
                    int hash = StringUtils.GetHashCode(key);
                    m_CharactersKeys.Add(hash);
                }
            }

            for (int defaultLineUpIndex = 0; defaultLineUpIndex < i_Descriptor.defaultLineUpsCount; ++defaultLineUpIndex)
            {
                LineUp lineUp = new LineUp();
                List<string> lineUpDescriptor = i_Descriptor.GetDefaultLineUp(defaultLineUpIndex);
                if (lineUp != null)
                {
                    for (int characterIndex = 0; characterIndex < lineUpDescriptor.Count; ++characterIndex)
                    {
                        string key = lineUpDescriptor[characterIndex];
                        if (key != "")
                        {
                            int hash = StringUtils.GetHashCode(key);
                            lineUp.Add(hash);
                        }
                    }
                }

                if (lineUp.Count > 0)
                {
                    m_DefaultLineUps.Add(lineUp);
                }
            }

            m_Flag = i_Descriptor.flag;
            m_Icon = i_Descriptor.icon;

            m_BaseSprite = i_Descriptor.baseSprite;

            m_FirstColor = i_Descriptor.firstColor;
            m_SecondColor = i_Descriptor.secondColor;

            m_SupportersFirstColor = i_Descriptor.supportersFirstColor;
            m_SupportersSecondColor = i_Descriptor.supportersSecondColor;

            m_TeamStats = i_Descriptor.teamStats;
        }
    }
}
