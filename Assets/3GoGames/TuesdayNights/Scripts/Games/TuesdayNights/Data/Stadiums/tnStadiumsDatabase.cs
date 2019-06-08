using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnStadiumDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnStadiumDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnStadiumDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnStadiumsDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnStadiumDataEntry> m_Stadiums = new List<tnStadiumDataEntry>();

    public int stadiumsCount
    {
        get { return m_Stadiums.Count; }
    }

    public tnStadiumDataEntry GetStadiumDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Stadiums.Count)
        {
            return null;
        }

        return m_Stadiums[i_Index];
    }
}
