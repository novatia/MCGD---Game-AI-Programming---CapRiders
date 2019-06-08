using UnityEngine;

using System;
using System.Collections.Generic;

using TrueSync;

[Serializable]
public class tnStatEntry
{
    [SerializeField]
    private string m_AttributeId = "";
    [SerializeField]
    private FP m_BaseValue = 50f;

    public string attributeId
    {
        get { return m_AttributeId; }
    }

    public FP baseValue
    {
        get { return m_BaseValue; }
    }
}

public class tnStatsDatabase : ScriptableObject
{
    [SerializeField]
    private List<tnStatEntry> m_Stats = new List<tnStatEntry>();

    public int statsCount
    {
        get { return m_Stats.Count; }
    }

    public tnStatEntry GetStat(int i_Index)
    {
        return m_Stats[i_Index];
    }
}
