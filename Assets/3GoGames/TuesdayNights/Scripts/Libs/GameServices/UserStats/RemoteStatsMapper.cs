using UnityEngine;

using System.Collections.Generic;

public class RemoteStatsMapper
{
    private string m_MapResourcePath = "";
    private RemoteStatsMap m_Map = null;

    private Dictionary<int, RemoteStatInfo> m_StatsMap = null;

    // LOGIC

    public void Initialize()
    {
        m_Map = Resources.Load<RemoteStatsMap>(m_MapResourcePath);

        if (m_Map != null)
        {
            foreach (string key in m_Map.keys)
            {
                RemoteStatInfo statInfo = m_Map.GetRemoteStatInfo(key);
                if (statInfo != null)
                {
                    int hash = StringUtils.GetHashCode(key);
                    m_StatsMap.Add(hash, statInfo);
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Map could not be loaded.");
        }
    }

    public RemoteStatInfo GetRemoteStatInfo(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetRemoteStatInfo(hash);
    }

    public RemoteStatInfo GetRemoteStatInfo(int i_Id)
    {
        RemoteStatInfo info = null;
        m_StatsMap.TryGetValue(i_Id, out info);
        return info;
    }

    // CTOR

    public RemoteStatsMapper(string i_MapResourcePath)
    {
        m_StatsMap = new Dictionary<int, RemoteStatInfo>();
        m_MapResourcePath = i_MapResourcePath;
    }
}
