using UnityEngine;

using System.Collections.Generic;

public class tnOnlinePlayersDatabaseManager
{
    private Dictionary<int, tnOnlinePlayerData> m_Data = null;
    private List<int> m_Keys = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnOnlinePlayersDatabase database = Resources.Load<tnOnlinePlayersDatabase>(i_DatabasePath);
        if (database != null)
        {
            for (int index = 0; index < database.playersCount; ++index)
            {
                tnOnlinePlayerDataEntry entry = database.GetPlayerDataEntry(index);
                if (entry != null)
                {
                    string key = entry.id;
                    tnOnlinePlayerDataDescriptor descriptor = entry.descriptor;
                    if (key != "" && descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(key);
                        tnOnlinePlayerData data = new tnOnlinePlayerData(descriptor);

                        m_Data.Add(hash, data);
                        m_Keys.Add(hash);
                    }
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database not loaded.");
        }
    }

    public List<int> GetKeys()
    {
        List<int> keys = new List<int>(m_Keys);
        return keys;
    }

    public tnOnlinePlayerData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnOnlinePlayerData GetData(int i_Id)
    {
        tnOnlinePlayerData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    public tnOnlinePlayerData GetDataByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Keys.Count)
        {
            return null;
        }

        int key = m_Keys[i_Index];
        return GetData(key);
    }

    // CTOR

    public tnOnlinePlayersDatabaseManager()
    {
        m_Data = new Dictionary<int, tnOnlinePlayerData>();
        m_Keys = new List<int>();
    }
}
