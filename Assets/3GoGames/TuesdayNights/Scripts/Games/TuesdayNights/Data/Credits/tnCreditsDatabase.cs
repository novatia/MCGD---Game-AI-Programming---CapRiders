using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class tnCreditsDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnCreditsDataDescriptor> m_Entries = new List<tnCreditsDataDescriptor>();

    [SerializeField]
    private List<tnCreditsTextEntryDescriptor> m_TextsEntries = new List<tnCreditsTextEntryDescriptor>();

    public int entriesCount
    {
        get
        {
            return m_Entries.Count;
        }
    }

    public int specialThanksEntriesCount
    {
        get
        {
            return m_TextsEntries.Count;
        }
    }

    public tnCreditsDataDescriptor GetEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Entries.Count)
        {
            return null;
        }

        return m_Entries[i_Index];
    }

    public tnCreditsTextEntryDescriptor GetTextEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_TextsEntries.Count)
        {
            return null;
        }

        return m_TextsEntries[i_Index];
    }
}