using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class RemoteStatsMap : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, RemoteStatInfo> m_StatsMap = new Dictionary<string, RemoteStatInfo>();

    public int count
    {
        get { return m_StatsMap.Count; }
    }

    public Dictionary<string, RemoteStatInfo>.KeyCollection keys
    {
        get { return m_StatsMap.Keys; }
    }

    public RemoteStatInfo GetRemoteStatInfo(string i_StatId)
    {
        RemoteStatInfo info = null;
        m_StatsMap.TryGetValue(i_StatId, out info);
        return info;
    }
}
