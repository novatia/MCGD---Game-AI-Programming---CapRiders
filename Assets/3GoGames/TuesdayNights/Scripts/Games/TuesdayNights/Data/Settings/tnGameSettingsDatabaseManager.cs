using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tnGameSettingsDatabaseManager
{
    private Dictionary<int, string> m_Data = null;
    private List<int> m_Keys = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // BUSINESS LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnGameSettingsDatabase database = Resources.Load<tnGameSettingsDatabase>(i_DatabasePath);

        if (database != null)
        {
            foreach (string key in database.keys)
            {
                int hash = StringUtils.GetHashCode(key);
                string value = database.GetValue(key);

                m_Data.Add(hash, value);
                m_Keys.Add(hash);
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

    public bool HasKey(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return HasKey(hash);
    }

    public bool HasKey(int i_Id)
    {
        return m_Keys.Contains(i_Id);
    }

    public string GetValue(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetValue(hash);
    }

    public string GetValue(int i_Id)
    {
        string value;
        m_Data.TryGetValue(i_Id, out value);
        return value;
    }

    // CTOR

    public tnGameSettingsDatabaseManager()
    {
        m_Data = new Dictionary<int, string>();
        m_Keys = new List<int>();
    }
}
