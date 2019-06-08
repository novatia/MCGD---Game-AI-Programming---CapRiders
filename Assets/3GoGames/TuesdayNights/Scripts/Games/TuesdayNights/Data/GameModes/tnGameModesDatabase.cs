using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnGameModeDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnGameModeDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnGameModeDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnGameModesDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnGameModeDataEntry> m_Modes = new List<tnGameModeDataEntry>();

    public int count
    {
        get { return m_Modes.Count; }
    }

    public tnGameModeDataEntry GetGameModeDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Modes.Count)
        {
            return null;
        }

        return m_Modes[i_Index];
    }
}
