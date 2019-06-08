using UnityEngine;

using System.Collections.Generic;

using FullInspector;

using LineUp = System.Collections.Generic.List<string>;

public class tnTeamDataDescriptor : BaseScriptableObject
{
    [SerializeField]
    private string m_TeamName = "";

    [SerializeField]
    private Sprite m_Flag = null;
    [SerializeField]
    private Sprite m_Icon = null;
    [SerializeField]
    private Sprite m_BaseSprite = null;
    [SerializeField]

    private Color m_FirstColor = Color.white;
    [SerializeField]
    private Color m_SecondColor = Color.black;

    [SerializeField]
    private Color m_SupportersFirstColor = Color.white;
    [SerializeField]
    private Color m_SupportersSecondColor = Color.white;

    [SerializeField]
    private tnStatsDatabase m_TeamStats = null;

    [SerializeField]
    private LineUp m_CharactersKeys = new LineUp();
    [SerializeField]
    private List<LineUp> m_DefaultLineUps = new List<LineUp>();

    public string teamName
    {
        get { return m_TeamName; }
    }

    public int charactersCount
    {
        get { return m_CharactersKeys.Count; }
    }

    public int defaultLineUpsCount
    {
        get
        {
            return m_DefaultLineUps.Count;
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

    public string GetCharacterKey(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_CharactersKeys.Count)
        {
            return "";
        }

        return m_CharactersKeys[i_Index];
    }

    public LineUp GetDefaultLineUp(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_DefaultLineUps.Count)
        {
            return null;
        }

        LineUp lineUp = m_DefaultLineUps[i_Index];
        if (lineUp == null)
        {
            return null;
        }

        LineUp target = new LineUp(lineUp);
        return target;
    }
}
