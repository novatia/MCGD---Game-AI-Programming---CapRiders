using UnityEngine;

using System.Collections.Generic;

public class tnTeamsDatabaseManager
{
    private Dictionary<int, tnTeamData> m_Data = null;
    private List<int> m_Keys = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnTeamsDatabase database = Resources.Load<tnTeamsDatabase>(i_DatabasePath);
        if (database != null)
        {
            for (int index = 0; index < database.teamsCount; ++index)
            {
                tnTeamDataEntry entry = database.GetTeamDataEntry(index);
                if (entry != null)
                {
                    string key = entry.id;
                    tnTeamDataDescriptor descriptor = entry.descriptor;
                    if (key != "" && descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(key);
                        tnTeamData data = new tnTeamData(descriptor);

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

    public tnTeamData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnTeamData GetData(int i_Id)
    {
        tnTeamData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnTeamsDatabaseManager()
    {
        m_Data = new Dictionary<int, tnTeamData>();
        m_Keys = new List<int>();
    }
}
