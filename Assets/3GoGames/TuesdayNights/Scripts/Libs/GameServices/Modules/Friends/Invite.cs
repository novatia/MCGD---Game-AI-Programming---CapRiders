public class Invite
{
    private Friend m_Friend = null;
    private string m_Args = "";

    // ACCESSORS

    public Friend friend
    {
        get
        {
            return m_Friend;
        }
    }

    public string args
    {
        get
        {
            return m_Args;
        }
    }

    // CTOR

    public Invite(Friend i_Friend, string i_Args = "")
    {
        m_Friend = i_Friend;
        m_Args = i_Args;
    }
}