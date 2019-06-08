using UnityEngine;
using System.Collections.Generic;

public class tnTeamsModule : GameModule
{
    // Fields

    private List<tnTeamDescription> m_Teams = null;

    // ACCESSORS

    public int teamsCount
    {
        get { return m_Teams.Count; }
    }

    // LOGIC

    public void Clear()
    {
        m_Teams.Clear();
    }

    public void AddTeamDescription(tnTeamDescription i_Team)
    {
        if (i_Team == null)
            return;

        m_Teams.Add(i_Team);
    }

    public tnTeamDescription GetTeamDescription(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Teams.Count)
        {
            return null;
        }

        return m_Teams[i_Index];
    }

    public int GetMaxTeamSize()
    {
        int maxTeamSize = 0;

        for (int teamIndex = 0; teamIndex < m_Teams.Count; ++teamIndex)
        {
            tnTeamDescription teamDescription = m_Teams[teamIndex];

            if (teamDescription == null)
                continue;

            maxTeamSize = Mathf.Max(teamDescription.charactersCount, maxTeamSize);
        }

        return maxTeamSize;
    }

    public int GetMinTeamSize()
    {
        int minTeamSize = int.MaxValue;

        for (int teamIndex = 0; teamIndex < m_Teams.Count; ++teamIndex)
        {
            tnTeamDescription teamDescription = m_Teams[teamIndex];

            if (teamDescription == null)
                continue;

            minTeamSize = Mathf.Min(teamDescription.charactersCount, minTeamSize);
        }

        return minTeamSize;
    }

    // CTOR

    public tnTeamsModule()
    {
        m_Teams = new List<tnTeamDescription>();
    }
}
