using UnityEngine;

public class tnCharacterInfo : MonoBehaviour
{
    private int m_CharacterIndex = -1;
    private int m_CharacterId = Hash.s_NULL;
    private int m_TeamId = Hash.s_NULL;
    private int m_TeamIndex = -1;
    private Color m_TeamColor = Color.white;

    public int characterIndex
    {
        get { return m_CharacterIndex; }
    }

    public int characterId
    {
        get { return m_CharacterId; }
    }

    public int teamId
    {
        get { return m_TeamId; }
    }

    public int teamIndex
    {
        get { return m_TeamIndex; }
    }

    public Color teamColor
    {
        get { return m_TeamColor; }
    }

    public void SetCharacterIndex(int i_Index)
    {
        m_CharacterIndex = i_Index;
    }

    public void SetTeamIndex(int i_Index)
    {
        m_TeamIndex = i_Index;
    }

    public void SetCharacterId(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetCharacterId(hash);
    }

    public void SetCharacterId(int i_Id)
    {
        m_CharacterId = i_Id;
    } 

    public void SetTeamId(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetTeamId(hash);
    }

    public void SetTeamId(int i_Id)
    {
        m_TeamId = i_Id;
    }

    public void SetTeamColor(Color i_Color)
    {
        m_TeamColor = i_Color;
    }
}
