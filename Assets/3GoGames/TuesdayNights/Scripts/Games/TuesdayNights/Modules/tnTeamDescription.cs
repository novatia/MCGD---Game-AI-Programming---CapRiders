using UnityEngine;

using System.Collections.Generic;

public class tnTeamDescription
{
    // Fields

    private int m_TeamId = Hash.s_NULL;

    private Color m_TeamColor = Color.white;

    private List<tnCharacterDescription> m_Characters = null;

    // ACCESSORS

    public int teamId
    {
        get { return m_TeamId; }
    }

    public Color teamColor
    {
        get { return m_TeamColor; }
    }

    public tnCharacterDescription captain
    {
        get
        {
            if (m_Characters.Count > 0)
            {
                return m_Characters[0];
            }

            return null;
        }
    }
    
    public int captainPlayerId
    {
        get
        {
            if (captain != null)
            {
                return captain.playerId;
            }

            return Hash.s_NULL;
        }
    }

    public int captainOnlinePlayerIndex
    {
        get
        {
            if (captain != null)
            {
                return captain.onlinePlayerIndex;
            }

            return -1;
        }
    }

    public int charactersCount
    {
        get { return m_Characters.Count; }
    }

    // LOGIC

    public void Clear()
    {
        m_TeamId = Hash.s_NULL;

        m_TeamColor = Color.white;

        m_Characters.Clear();
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

    public void AddCharacterDescription(tnCharacterDescription i_Character)
    {
        if (i_Character == null)
            return;

        m_Characters.Add(i_Character);
    }

    public tnCharacterDescription GetCharacterDescription(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Characters.Count)
        {
            return null;
        }

        return m_Characters[i_Index];
    }

    // Used by Single player.

    public tnCharacterDescription GetCharacterDescriptionBySpawnOrder(int i_SpawnOrder)
    {
        tnCharacterDescription characterDescription = null;

        for (int characterIndex = 0; characterIndex < m_Characters.Count; ++characterIndex)
        {
            tnCharacterDescription currentDescription = m_Characters[characterIndex];

            if (currentDescription != null && currentDescription.spawnOrder == i_SpawnOrder)
            {
                characterDescription = currentDescription;
                break;
            }
        }

        return characterDescription;
    }

    // CTOR

    public tnTeamDescription()
    {
        m_TeamId = Hash.s_NULL;
        m_TeamColor = Color.white;

        m_Characters = new List<tnCharacterDescription>();
    }
}
