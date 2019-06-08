using UnityEngine;

using System.Collections.Generic;

public class tnCreditsDatabaseManager
{
    private List<tnCreditsData> m_Data = null;
    private List<tnCreditsTextEntry> m_Texts = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    public int textsCount
    {
        get { return m_Texts.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnCreditsDatabase database = Resources.Load<tnCreditsDatabase>(i_DatabasePath);
        if (database != null)
        {
            for (int index = 0; index < database.entriesCount; ++index)
            {
                tnCreditsDataDescriptor descriptor = database.GetEntry(index);
                if (descriptor != null)
                {
                    tnCreditsData data = new tnCreditsData(descriptor);
                    m_Data.Add(data);
                }
            }

            for (int index = 0; index < database.specialThanksEntriesCount; ++index)
            {
                tnCreditsTextEntryDescriptor descriptor = database.GetTextEntry(index);
                if (descriptor != null)
                {
                    tnCreditsTextEntry data = new tnCreditsTextEntry(descriptor);
                    m_Texts.Add(data);
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database not loaded.");
        }
    }

    public tnCreditsData GetData(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Data.Count)
        {
            return null;
        }

        return m_Data[i_Index];
    }

    public tnCreditsTextEntry GetText(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Texts.Count)
        {
            return null;
        }

        return m_Texts[i_Index];
    }

    // CTOR

    public tnCreditsDatabaseManager()
    {
        m_Data = new List<tnCreditsData>();
        m_Texts = new List<tnCreditsTextEntry>();
    }
}
