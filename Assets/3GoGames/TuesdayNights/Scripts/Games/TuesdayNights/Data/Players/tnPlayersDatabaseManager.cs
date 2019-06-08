using UnityEngine;

using System.Collections.Generic;

public class tnPlayerDatabaseManager
{
    private Dictionary<int, tnPlayerData> m_Data = null;
    private List<int> m_Keys = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnPlayersDatabase database = Resources.Load<tnPlayersDatabase>(i_DatabasePath);
        if (database != null)
        {
            for (int index = 0; index < database.playersCount; ++index)
            {
                tnPlayerDataEntry entry = database.GetPlayerDataEntry(index);
                if (entry != null)
                {
                    string key = entry.id;
                    tnPlayerDataDescriptor descriptor = entry.descriptor;
                    if (key != "" && descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(key);
                        tnPlayerData data = new tnPlayerData(descriptor);

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

    public tnPlayerData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnPlayerData GetData(int i_Id)
    {
        tnPlayerData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnPlayerDatabaseManager()
    {
        m_Data = new Dictionary<int, tnPlayerData>();
        m_Keys = new List<int>();
    }
}