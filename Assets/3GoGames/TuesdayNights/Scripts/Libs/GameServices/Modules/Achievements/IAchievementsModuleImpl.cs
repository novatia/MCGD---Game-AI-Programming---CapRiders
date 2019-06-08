public interface IAchievementsModuleImpl
{
    void Initialize(AchievementsDatabaseManager i_AchievementsManager);

    bool GetAchievement(int i_AchievementId, out bool o_Achieved);
    void UnlockAchievement(int i_AchievementId);
    void ClearAchievement(int i_AchievementId);
}
