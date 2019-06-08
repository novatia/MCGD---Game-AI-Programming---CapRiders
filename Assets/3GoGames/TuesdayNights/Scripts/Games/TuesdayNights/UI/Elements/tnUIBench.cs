using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnUIBenchEntry
{
    [SerializeField]
    private RectTransform m_Anchor = null;

    private bool m_IsFree = true;

    public RectTransform anchor
    {
        get { return m_Anchor; }
    }

    public bool isFree
    {
        get { return m_IsFree; }
        set { m_IsFree = value; }
    }
}

public class tnUIBench : BaseBehavior
{
    [SerializeField]
    private List<tnUIBenchEntry> m_Entries = new List<tnUIBenchEntry>();
    
    public int entriesCount
    {
        get { return m_Entries.Count; }
    }

    // LOGIC

    public tnUIBenchEntry GetEntryByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Entries.Count)
        {
            return null;
        }

        return m_Entries[i_Index];
    }

    public tnUIBenchEntry GetFirstAvailableEntry()
    {
        for (int index = 0; index < m_Entries.Count; ++index)
        {
            tnUIBenchEntry entry = m_Entries[index];
            if (entry != null)
            {
                if (entry.isFree)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    public tnUIBenchEntry GetLastAvailableEntry()
    {
        for (int index = m_Entries.Count - 1; index >= 0; --index)
        {
            tnUIBenchEntry entry = m_Entries[index];
            if (entry != null)
            {
                if (entry.isFree)
                {
                    return entry;
                }
            }
        }

        return null;
    }
}
