public class tnGameModeConfig
{
    private int m_MatchDurationOption = Hash.s_NULL;
    private int m_RefereeOption = Hash.s_NULL;
    private int m_GoldenGoalOption = Hash.s_NULL;

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

    // CTOR

    public tnGameModeConfig(tnGameModeConfigDescriptor i_Descriptor)
    {
        if (i_Descriptor != null)
        {
            m_MatchDurationOption = StringUtils.GetHashCode(i_Descriptor.matchDurationOption);
            m_RefereeOption = StringUtils.GetHashCode(i_Descriptor.refereeOption);
            m_GoldenGoalOption = StringUtils.GetHashCode(i_Descriptor.goldenGoalOption);
        }
    }
}
