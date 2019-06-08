using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnCharacterDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnCharacterDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnCharacterDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnCharactersDatabase : BaseScriptableObject
{
    [SerializeField]
    private ResourcePath m_DefaultCharacterPrefabPath = null;
    [SerializeField]
    private List<tnCharacterDataEntry> m_Characters = new List<tnCharacterDataEntry>();

    public int charactersCount
    {
        get { return m_Characters.Count; }
    }

    public string defaultCharacterPrefabPath
    {
        get { return m_DefaultCharacterPrefabPath; }
    }

    public tnCharacterDataEntry GetCharacterDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Characters.Count)
        {
            return null;
        }

        return m_Characters[i_Index];
    }
}
