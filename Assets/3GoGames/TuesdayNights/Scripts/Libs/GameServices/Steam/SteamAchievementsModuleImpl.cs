#if STEAM

using Steamworks;

public class SteamAchievementsModuleImpl : IAchievementsModuleImpl
{
    protected Steamworks.Callback<UserStatsReceived_t> m_UserStatsReceived = null;

    private CGameID m_GameId;

    private AchievementsDatabaseManager m_AchievementsManager = null;

    // CALLBACKS

    private void OnUserStatsReceived(UserStatsReceived_t i_CallbackParams)
    {
        if (m_AchievementsManager == null)
            return;

        if (i_CallbackParams.m_nGameID == ((ulong)m_GameId))
        {
            EResult callbackResult = i_CallbackParams.m_eResult;
            if (callbackResult == EResult.k_EResultOK)
            {
                for (int achievementIndex = 0; achievementIndex < m_AchievementsManager.achievementsCount; ++achievementIndex)
                {
                    Achievement achievement = m_AchievementsManager.GetAchievementByIndex(achievementIndex);
                    if (achievement != null)
                    {
                        string achievementSteamId = achievement.steamId;
                        bool achieved = false;
                        if (SteamUserStats.GetAchievement(achievementSteamId, out achieved))
                        {
                            achievement.SetAchieved(achieved);
                        }
                    }
                }
            }
        }
    }

    // INTERNALS

    // IAchievementsModuleImpl's interface

    public void Initialize(AchievementsDatabaseManager i_AchievementsManager)
    {
        m_AchievementsManager = i_AchievementsManager;

        m_UserStatsReceived = Steamworks.Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);

        m_GameId = new CGameID(SteamAppInfo.s_AppId);
    }

    public bool GetAchievement(int i_AchievementId, out bool o_Achieved)
    {
        o_Achieved = false;

        if (!SteamManager.initializedMain)
            return false;

        if (m_AchievementsManager == null)
            return false;

        Achievement achievement = m_AchievementsManager.GetAchievement(i_AchievementId);
        if (achievement != null)
        {
            o_Achieved = achievement.isAchieved;
            return true;
        }

        return false;
    }

    public void UnlockAchievement(int i_AchievementId)
    {
        if (!SteamManager.initializedMain)
            return;

        if (m_AchievementsManager == null)
            return;

        Achievement achievement = m_AchievementsManager.GetAchievement(i_AchievementId);
        if (achievement != null)
        {
            if (!achievement.isAchieved)
            {
                string steamAchievementId = achievement.steamId;
                SteamUserStats.SetAchievement(steamAchievementId);
                achievement.SetAchieved(true);
                SteamManager.SetStatsDirtyMain();
            }
        }
    }

    public void ClearAchievement(int i_AchievementId)
    {
        if (!SteamManager.initializedMain)
            return;

        if (m_AchievementsManager == null)
            return;

        Achievement achievement = m_AchievementsManager.GetAchievement(i_AchievementId);
        if (achievement != null)
        {
            if (!achievement.isAchieved)
            {
                string steamAchievementId = achievement.steamId;
                SteamUserStats.ClearAchievement(steamAchievementId);
                achievement.SetAchieved(false);
                SteamManager.SetStatsDirtyMain();
            }
        }
    }
}

#endif