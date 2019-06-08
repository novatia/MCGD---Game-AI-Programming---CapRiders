using UnityEngine;

using System.Collections.Generic;

public class tnGameModesDatabaseManager
{
    private Dictionary<int, tnGameModeData> m_Data = null;
    private List<int> m_Keys = null;

    // GETTERS

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnGameModesDatabase database = Resources.Load<tnGameModesDatabase>(i_DatabasePath);

        if (database != null)
        {
            for (int index = 0; index < database.count; ++index)
            {
                tnGameModeDataEntry entry = database.GetGameModeDataEntry(index);
                if (entry != null)
                {
                    tnGameModeDataDescriptor descriptor = entry.descriptor;
                    if (descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(entry.id);
                        tnGameModeData data = new tnGameModeData(descriptor);
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

    public tnGameModeData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnGameModeData GetData(int i_Id)
    {
        tnGameModeData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    public List<int> GetKeys()
    {
        List<int> list = new List<int>(m_Keys);
        return list;
    }

    // CTOR

    public tnGameModesDatabaseManager()
    {
        m_Data = new Dictionary<int, tnGameModeData>();
        m_Keys = new List<int>();
    }
}
