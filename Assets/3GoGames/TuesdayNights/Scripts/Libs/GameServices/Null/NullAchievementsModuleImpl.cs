public class NullAchievementsModuleImpl : IAchievementsModuleImpl
{
    public void Initialize(AchievementsDatabaseManager m_AchievementsManager)
    {

    }

    public bool GetAchievement(int i_AchievementId, out bool o_Achieved)
    {
        o_Achieved = false;
        return true;
    }

    public void UnlockAchievement(int i_AchievementId)
    {

    }

    public void ClearAchievement(int i_AchievementId)
    {

    }
}
