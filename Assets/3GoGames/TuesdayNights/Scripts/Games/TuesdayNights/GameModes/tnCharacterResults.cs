public class tnCharacterResults
{
    private int m_Id;

    private int m_PlayerId;
    private bool m_IsHuman;

    public int id
    {
        get
        {
            return m_Id;
        }
    }
    
    public int playerId
    {
        get
        {
            return m_PlayerId;
        }

        set
        {
            m_PlayerId = value;
        }
    }

    public bool isHuman
    {
        get
        {
            return m_IsHuman;
        }

        set
        {
            m_IsHuman = value;
        }
    }

    // CTOR

    public tnCharacterResults(int i_Id)
    {
        m_Id = i_Id;
    }
}
