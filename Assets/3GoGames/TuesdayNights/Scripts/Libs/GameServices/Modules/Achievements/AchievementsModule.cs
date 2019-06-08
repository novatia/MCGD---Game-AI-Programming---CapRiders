public class AchievementsModule : GameServicesModule
{
    private AchievementsDatabaseManager m_AchievementsManager = null;

    private IAchievementsModuleImpl m_Impl = null;

    // GameServicesModule's methods

    public override void Initialize()
    {
        m_AchievementsManager.Initialize("Database/Achievements/AchievementsDatabase");

        m_Impl.Initialize(m_AchievementsManager);
    }

    public override void Update()
    {
        // Nothing to do
    }

    // LOGIC

    public void UnlockAchievement(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        UnlockAchievement(hash);
    }

    public void UnlockAchievement(int i_Id)
    {
        m_Impl.UnlockAchievement(i_Id);
    }

    public void ClearAchievement(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        ClearAchievement(hash);
    }

    public void ClearAchievement(int i_Id)
    {
        m_Impl.ClearAchievement(i_Id);
    }

    public bool GetAchievement(string i_Id, out bool o_Achieved)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetAchievement(hash, out o_Achieved);
    }

    public bool GetAchievement(int i_Id, out bool o_Achieved)
    {
        return m_Impl.GetAchievement(i_Id, out o_Achieved);
    }

    // CTOR

    public AchievementsModule()
    {
        m_AchievementsManager = new AchievementsDatabaseManager();

#if STEAM
        m_Impl = new SteamAchievementsModuleImpl();
#else
        m_Impl = new NullAchievementsModuleImpl();
#endif
    }
}
