using UnityEngine;

using System.Collections.Generic;

public class tnCharactersDatabaseManager
{
    private string m_DefaultCharacterPrefabPath = "";

    private Dictionary<int, tnCharacterData> m_Data = null;
    private List<int> m_Keys = null;

    public string defaultCharacterPrefabPath
    {
        get { return m_DefaultCharacterPrefabPath; }
    }

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnCharactersDatabase database = Resources.Load<tnCharactersDatabase>(i_DatabasePath);
        if (database != null)
        {
            m_DefaultCharacterPrefabPath = database.defaultCharacterPrefabPath;

            for (int index = 0; index < database.charactersCount; ++index)
            {
                tnCharacterDataEntry entry = database.GetCharacterDataEntry(index);
                if (entry != null)
                {
                    string key = entry.id;
                    tnCharacterDataDescriptor descriptor = entry.descriptor;
                    if (key != "" && descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(key);
                        tnCharacterData data = new tnCharacterData(descriptor);

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

    public GameObject LoadAndGetDefaultCharacterPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>(m_DefaultCharacterPrefabPath);
        return prefab;
    }

    public List<int> GetKeys()
    {
        List<int> keys = new List<int>(m_Keys);
        return keys;
    }

    public tnCharacterData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnCharacterData GetData(int i_Id)
    {
        tnCharacterData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnCharactersDatabaseManager()
    {
        m_Data = new Dictionary<int, tnCharacterData>();
        m_Keys = new List<int>();
    }
}
