using UnityEngine;

using System.Collections.Generic;

public class tnAILevel
{
    // Fields

    private string m_Label = "";
    private int m_InputDelay = 0;

    // ACCESSORS

    public string label
    {
        get
        {
            return m_Label;
        }
    }

    public int inputDelay
    {
        get
        {
            return m_InputDelay;
        }
    }

    // CTOR

    public tnAILevel(tnAILevelDescriptor i_Descriptor)
    {
        if (i_Descriptor == null)
            return;

        m_Label = i_Descriptor.label;
        m_InputDelay = i_Descriptor.inputDelay;
    }
}

public class tnAIDatabaseManager
{
    // Serializable fields

    private List<tnAILevel> m_AILevels = null;

    // ACCESSORS

    public int aiLevelCount
    {
        get
        {
            return m_AILevels.Count;
        }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnAIDatabase database = Resources.Load<tnAIDatabase>(i_DatabasePath);
        if (database != null)
        {
            for (int index = 0; index < database.aiLevelCount; ++index)
            {
                tnAILevelDescriptor descriptor = database.GetAILevelDescriptor(index);

                if (descriptor == null)
                    continue;

                tnAILevel aiLevel = new tnAILevel(descriptor);
                m_AILevels.Add(aiLevel);
            }
        }
    }

    public tnAILevel GetAILevel(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_AILevels.Count)
        {
            return null;
        }

        return m_AILevels[i_Index];
    }

    // CTOR

    public tnAIDatabaseManager()
    {
        m_AILevels = new List<tnAILevel>();
    }
}