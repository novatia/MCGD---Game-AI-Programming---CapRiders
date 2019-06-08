public delegate void UserStatValueChangedEvent<T>(T i_OldValue, T i_NewValue);

public abstract class UserStat
{
    private string m_Id;
    private int m_HashId;

    public string id
    {
        get
        {
            return m_Id;
        }
    }

    public int hashId
    {
        get
        {
            return m_HashId;
        }
    }

    public abstract UserStatType type
    {
        get;
    }

    // CTOR

    public UserStat(string i_Id)
    {
        m_Id = i_Id;
        m_HashId = StringUtils.GetHashCode(m_Id);
    }
}
