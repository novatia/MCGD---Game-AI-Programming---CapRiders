using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnTeamDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnTeamDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnTeamDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnTeamsDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnTeamDataEntry> m_Teams = new List<tnTeamDataEntry>();

    public int teamsCount
    {
        get { return m_Teams.Count; }
    }

    public tnTeamDataEntry GetTeamDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Teams.Count)
        {
            return null;
        }

        return m_Teams[i_Index];
    }
}