using UnityEngine;

using System.Collections.Generic;

public class tnStadiumsDatabaseManager
{
    private Dictionary<int, tnStadiumData> m_Data = null;
    private List<int> m_Keys = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnStadiumsDatabase database = Resources.Load<tnStadiumsDatabase>(i_DatabasePath);
        if (database != null)
        {
            for (int index = 0; index < database.stadiumsCount; ++index)
            {
                tnStadiumDataEntry entry = database.GetStadiumDataEntry(index);
                if (entry != null)
                {
                    string key = entry.id;
                    tnStadiumDataDescriptor descriptor = entry.descriptor;
                    if (key != "" && descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(key);
                        tnStadiumData data = new tnStadiumData(descriptor);

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

    public tnStadiumData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnStadiumData GetData(int i_Id)
    {
        tnStadiumData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnStadiumsDatabaseManager()
    {
        m_Data = new Dictionary<int, tnStadiumData>();
        m_Keys = new List<int>();
    }
}
