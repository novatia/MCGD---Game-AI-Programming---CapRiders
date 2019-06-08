using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnAILevelDescriptor
{
    // Serializable fields

    [SerializeField]
    private string m_Label = "";
    [SerializeField]
    private int m_InputDelay = 0;

    // ACCESSORS

    public string label
    {
        get
        {
            return m_Label;
        }
        set
        {
            m_Label = value;
        }
    }

    public int inputDelay
    {
        get
        {
            return m_InputDelay;
        }
        set
        {
            m_InputDelay = value;
        }
    }
}

public class tnAIDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<tnAILevelDescriptor> m_AILevels = new List<tnAILevelDescriptor>();

    // ACCESSORS

    public int aiLevelCount
    {
        get { return m_AILevels.Count; }
    }

    // LOGIC

    public tnAILevelDescriptor GetAILevelDescriptor(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_AILevels.Count)
        {
            return null;
        }

        return m_AILevels[i_Index];
    }
}