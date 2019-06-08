using TuesdayNights;

public class tnMatchSettingsModule : GameModule
{
    private int m_GameModeId;

    private int m_StadiumId;
    private int m_BallId;

    private int m_MatchDurationOption;
    private int m_RefereeOption;
    private int m_GoldenGoalOption;

    private int m_AILevelIndex;

    public int gameModeId
    {
        get { return m_GameModeId; }
    }

    public int stadiumId
    {
        get { return m_StadiumId; }
    }

    public int ballId
    {
        get { return m_BallId; }
    }

    public int matchDurationOption
    {
        get { return m_MatchDurationOption; }
    }

    public int refereeOption
    {
        get { return m_RefereeOption; }
    }

    public int goldenGoalOption
    {
        get { return m_GoldenGoalOption; }
    }

    public int aiLevelIndex
    {
        get { return m_AILevelIndex; }
    }

    // LOGIC

    public void Clear()
    {
        m_GameModeId = Hash.s_NULL;
        m_StadiumId = Hash.s_NULL;
        m_BallId = Hash.s_NULL;
        m_MatchDurationOption = Hash.s_NULL;
        m_RefereeOption = Hash.s_NULL;
        m_GoldenGoalOption = Hash.s_NULL;
        m_AILevelIndex = 0;
    }

    public void SetGameModeId(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetGameModeId(hash);
    }

    public void SetGameModeId(int i_Id)
    {
        m_GameModeId = i_Id;
    }

    public void SetStadiumId(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetStadiumId(hash);
    }

    public void SetStadiumId(int i_Id)
    {
        m_StadiumId = i_Id;
    }

    public void SetBallId(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetBallId(hash);
    }

    public void SetBallId(int i_Id)
    {
        m_BallId = i_Id;
    }

    public void SetMatchDurationOption(string i_Key)
    {
        int hash = StringUtils.GetHashCode(i_Key);
        SetMatchDurationOption(hash);
    }

    public void SetMatchDurationOption(int i_Key)
    {
        m_MatchDurationOption = i_Key;
    }

    public void SetRefereeOption(string i_Key)
    {
        int hash = StringUtils.GetHashCode(i_Key);
        SetRefereeOption(hash);
    }

    public void SetRefereeOption(int i_Key)
    {
        m_RefereeOption = i_Key;
    }

    public void SetGoldenGoalOption(string i_Key)
    {
        int hash = StringUtils.GetHashCode(i_Key);
        SetGoldenGoalOption(hash);
    }

    public void SetGoldenGoalOption(int i_Key)
    {
        m_GoldenGoalOption = i_Key;
    }

    public void SetAILevelIndex(int i_Index)
    {
        m_AILevelIndex = i_Index;
    }

    // CTOR

    public tnMatchSettingsModule()
    {
        m_GameModeId = Hash.s_NULL;
        m_StadiumId = Hash.s_NULL;
        m_BallId = Hash.s_NULL;
        m_MatchDurationOption = Hash.s_NULL;
        m_RefereeOption = Hash.s_NULL;
        m_GoldenGoalOption = Hash.s_NULL;
        m_AILevelIndex = 0;
    }
}
