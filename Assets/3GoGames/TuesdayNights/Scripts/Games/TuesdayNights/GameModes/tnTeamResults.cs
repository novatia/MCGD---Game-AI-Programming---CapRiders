using System.Collections.Generic;

public class tnTeamResults
{
    private int m_Id;

    private List<tnCharacterResults> m_CharactersResults = null;

    public int id
    {
        get { return m_Id; }
    }

    public int charactersResultsCount
    {
        get { return m_CharactersResults.Count; }
    }

    // LOGIC

    public void Clear()
    {
        m_CharactersResults.Clear();
    }

    public void AddCharacterResults(tnCharacterResults i_PlayerResults)
    {
        if (i_PlayerResults == null)
            return;

        m_CharactersResults.Add(i_PlayerResults);
    }

    public tnCharacterResults GetCharacterResults(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_CharactersResults.Count)
        {
            return null;
        }

        return m_CharactersResults[i_Index];
    }

    // CTOR

    public tnTeamResults(int i_Id)
    {
        m_Id = i_Id;
        m_CharactersResults = new List<tnCharacterResults>();
    }
}
