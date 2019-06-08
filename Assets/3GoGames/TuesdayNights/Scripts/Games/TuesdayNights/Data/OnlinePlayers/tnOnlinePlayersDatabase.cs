using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnOnlinePlayerDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnOnlinePlayerDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnOnlinePlayerDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnOnlinePlayersDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnOnlinePlayerDataEntry> m_Players = new List<tnOnlinePlayerDataEntry>();

    public int playersCount
    {
        get { return m_Players.Count; }
    }

    public tnOnlinePlayerDataEntry GetPlayerDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Players.Count)
        {
            return null;
        }

        return m_Players[i_Index];
    }
}