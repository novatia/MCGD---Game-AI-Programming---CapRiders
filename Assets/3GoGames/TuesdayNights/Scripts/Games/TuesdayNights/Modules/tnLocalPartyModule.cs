using System.Collections.Generic;

public class tnLocalPartyModule : GameModule
{
    private List<int> m_PlayerIds = null;
    private List<int> m_OnlinePlayerIndices = null;

    // ACCESSORS

    public int captainId
    {
        get
        {
            if (m_PlayerIds.Count > 0)
            {
                return m_PlayerIds[0];
            }

            return Hash.s_NULL;
        }
    }

    public int playersCount
    {
        get
        {
            return m_PlayerIds.Count;
        }
    }

    public bool isEmpty
    {
        get
        {
            return (m_PlayerIds.Count == 0);
        }
    }

    // LOGIC

    public void AddPlayer(int i_PlayerId, int i_OnlinePlayerIndex = -1)
    {
        m_PlayerIds.Add(i_PlayerId);
        m_OnlinePlayerIndices.Add(i_OnlinePlayerIndex);
    }

    public void SetOnlinePlayerIndexByIndex(int i_Index, int i_OnlinePlayerIndex)
    {
        if (i_Index < 0 || i_Index >= m_OnlinePlayerIndices.Count)
            return;

        m_OnlinePlayerIndices[i_Index] = i_OnlinePlayerIndex;
    }

    public void SetOnlinePlayerIndexById(int i_PlayerId, int i_OnlinePlayerIndex)
    {
        for (int index = 0; index < m_PlayerIds.Count; ++index)
        {
            if (m_PlayerIds[index] == i_PlayerId)
            {
                SetOnlinePlayerIndexByIndex(index, i_OnlinePlayerIndex);
                return;
            }
        }
    }

    public int GetPlayerId(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_PlayerIds.Count)
        {
            return Hash.s_NULL;
        }

        return m_PlayerIds[i_Index];
    }

    public int GetOnlinePlayerIndexByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_OnlinePlayerIndices.Count)
        {
            return -1;
        }

        return m_OnlinePlayerIndices[i_Index];
    }

    public int GetOnlinePlayerIndexById(int i_PlayerId)
    {
        for (int index = 0; index < m_PlayerIds.Count; ++index)
        {
            if (m_PlayerIds[index] == i_PlayerId)
            {
                return GetOnlinePlayerIndexByIndex(index);
            }
        }

        return -1;
    }

    public int GetPlayerIdByOnlineIndex(int i_OnlinePlayerIndex)
    {
        for (int index = 0; index < m_OnlinePlayerIndices.Count; ++index)
        {
            int current = m_OnlinePlayerIndices[index];

            if (current == i_OnlinePlayerIndex)
            {
                return m_PlayerIds[index];
            }
        }

        return Hash.s_NULL;
    }

    public bool ContainsPlayerId(int i_PlayerId)
    {
        if (Hash.IsNullOrEmpty(i_PlayerId))
        {
            return false;
        }

        return m_PlayerIds.Contains(i_PlayerId);
    }

    public void Clear()
    {
        m_PlayerIds.Clear();
        m_OnlinePlayerIndices.Clear();
    }

    // CTOR

    public tnLocalPartyModule()
    {
        m_PlayerIds = new List<int>();
        m_OnlinePlayerIndices = new List<int>();
    }
}