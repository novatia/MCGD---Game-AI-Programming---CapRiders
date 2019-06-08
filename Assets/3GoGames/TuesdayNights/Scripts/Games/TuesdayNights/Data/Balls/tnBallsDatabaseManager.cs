using UnityEngine;

using System.Collections.Generic;

public class tnBallsDatabaseManager
{
    private string m_BallPrefabPath = "";

    private Dictionary<int, tnBallData> m_Data = null;
    private List<int> m_Keys = null;

    public string ballPrefabPath
    {
        get { return m_BallPrefabPath; }
    }

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnBallsDatabase database = Resources.Load<tnBallsDatabase>(i_DatabasePath);
        if (database != null)
        {
            m_BallPrefabPath = database.ballPrefabPath;

            for (int index = 0; index < database.ballsCount; ++index)
            {
                tnBallDataEntry entry = database.GetBallDataEntry(index);
                if (entry != null)
                {
                    string key = entry.id;
                    tnBallDataDescriptor descriptor = entry.descriptor;
                    if (key != "" && descriptor != null)
                    {
                        int hash = StringUtils.GetHashCode(key);
                        tnBallData data = new tnBallData(descriptor);

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

    public tnBall LoadAndGetBallPrefab()
    {
        tnBall prefab = Resources.Load<tnBall>(m_BallPrefabPath);
        return prefab;
    }

    public List<int> GetKeys()
    {
        List<int> keys = new List<int>(m_Keys);
        return keys;
    }

    public tnBallData GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnBallData GetData(int i_Id)
    {
        tnBallData data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnBallsDatabaseManager()
    {
        m_Data = new Dictionary<int, tnBallData>();
        m_Keys = new List<int>();
    }
}
