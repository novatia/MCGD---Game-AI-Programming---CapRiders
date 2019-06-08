using UnityEngine;

public class tnCharacterDescription
{
    // Common

    private int m_CharacterId = Hash.s_NULL;

    // Used by Multiplayer

    private int m_OnlinePlayerIndex = -1;

    // Used by Single player

    private int m_PlayerId = Hash.s_NULL;
    private int m_SpawnOrder = -1;

    // ACCESSORS

    public int onlinePlayerIndex
    {
        get
        {
            return m_OnlinePlayerIndex;
        }
    }

    public int characterId
    {
        get
        {
            return m_CharacterId;
        }
    }

    public int playerId
    {
        get
        {
            return m_PlayerId;
        }
    }

    public int spawnOrder
    {
        get
        {
            return m_SpawnOrder;
        }
    }

    // LOGIC

    public void Clear()
    {
        m_CharacterId = Hash.s_NULL;

        m_OnlinePlayerIndex = -1;

        m_PlayerId = Hash.s_NULL;
        m_SpawnOrder = -1;
    }

    public void SetOnlinePlayerIndex(int i_Index)
    {
        m_OnlinePlayerIndex = Mathf.Max(-1, i_Index);
    }

    public void SetCharacterId(string i_CharacterId)
    {
        int hash = StringUtils.GetHashCode(i_CharacterId);
        SetCharacterId(hash);
    }

    public void SetCharacterId(int i_CharacterId)
    {
        m_CharacterId = i_CharacterId;
    }

    public void SetPlayerId(string i_PlayerId)
    {
        int hash = StringUtils.GetHashCode(i_PlayerId);
        SetPlayerId(hash);
    }

    public void SetPlayerId(int i_PlayerId)
    {
        m_PlayerId = i_PlayerId;
    }

    public void SetSpawnOrder(int i_SpawnOrder)
    {
        m_SpawnOrder = i_SpawnOrder;
    }

    // CTOR

    public tnCharacterDescription()
    {
        m_CharacterId = Hash.s_NULL;

        m_OnlinePlayerIndex = -1;

        m_PlayerId = Hash.s_NULL;
        m_SpawnOrder = -1;
    }
}
