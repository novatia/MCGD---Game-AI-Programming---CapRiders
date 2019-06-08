using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnPlayerDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnPlayerDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnPlayerDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnPlayersDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnPlayerDataEntry> m_Players = new List<tnPlayerDataEntry>();

    public int playersCount
    {
        get { return m_Players.Count; }
    }

    public tnPlayerDataEntry GetPlayerDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Players.Count)
        {
            return null;
        }

        return m_Players[i_Index];
    }
}
