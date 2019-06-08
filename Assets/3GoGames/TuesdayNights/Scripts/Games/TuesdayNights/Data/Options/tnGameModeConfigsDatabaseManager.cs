using UnityEngine;

using System.Collections.Generic;

public class tnGameModeConfigsDatabaseManager
{
    private Dictionary<int, tnGameModeConfig> m_Data = null;

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnGameModeConfigsDatabase database = Resources.Load<tnGameModeConfigsDatabase>(i_DatabasePath);

        if (database != null)
        {
            foreach (string key in database.keys)
            {
                tnGameModeConfigDescriptor descriptor = database.GetConfig(key);
                if (descriptor != null)
                {
                    int hash = StringUtils.GetHashCode(key);
                    tnGameModeConfig data = new tnGameModeConfig(descriptor);
                    m_Data.Add(hash, data);
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database not loaded.");
        }
    }

    public tnGameModeConfig GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnGameModeConfig GetData(int i_Id)
    {
        tnGameModeConfig data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnGameModeConfigsDatabaseManager()
    {
        m_Data = new Dictionary<int, tnGameModeConfig>();
    }
}
