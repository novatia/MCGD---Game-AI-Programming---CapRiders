#if STEAM

using Steamworks;

public class SteamStatsModuleImpl : IStatsModuleImpl
{
    protected Steamworks.Callback<UserStatsReceived_t> m_UserStatsReceived = null;

    private CGameID m_GameId;

    private UserStatsManager m_UserStatsManager = null;
    private RemoteStatsMapper m_RemoteStatsMapper = null;

    private bool m_StatsValid = false;

    // CALLBACKS

    private void OnUserStatsReceived(UserStatsReceived_t i_CallbackParams)
    {
        if (m_UserStatsManager == null || m_RemoteStatsMapper == null)
            return;

        if (i_CallbackParams.m_nGameID == ((ulong)m_GameId))
        {
            EResult callbackResult = i_CallbackParams.m_eResult;
            if (callbackResult == EResult.k_EResultOK)
            {
                for (int userStatIndex = 0; userStatIndex < m_UserStatsManager.statsCount; ++userStatIndex)
                {
                    UserStat userStat = m_UserStatsManager.GetStatByIndex(userStatIndex);
                    if (userStat != null)
                    {
                        UserStatType userStatType = userStat.type;
                        switch (userStatType)
                        {
                            case UserStatType.Int:

                                UserStatInt userStatInt = (UserStatInt)userStat;
                                CombineIntUserStat(userStatInt);
                                
                                break;

                            case UserStatType.Bool:

                                UserStatBool userStatBool = (UserStatBool)userStat;
                                CombineBoolUserStat(userStatBool);

                                break;

                            case UserStatType.Float:

                                UserStatFloat userStatFloat = (UserStatFloat)userStat;
                                CombineFloatUserStat(userStatFloat);

                                break;

                            case UserStatType.String:

                                UserStatString userStatString = (UserStatString)userStat;
                                CombineStringUserStat(userStatString);

                                break;
                        }
                    }
                }

                m_StatsValid = true;
            }
        }
    }

    // INTERNALS

    private void CombineIntUserStat(UserStatInt i_UserStatInt)
    {
        string statId = i_UserStatInt.id;
        RemoteStatInfo remoteStatInfo = m_RemoteStatsMapper.GetRemoteStatInfo(statId);
        if (remoteStatInfo != null)
        {
            string steamStatId = remoteStatInfo.steamId;
            if (steamStatId != "")
            {
                int remoteValue = 0;
                if (SteamUserStats.GetStat(steamStatId, out remoteValue))
                {
                    i_UserStatInt.Combine(remoteValue);
                }
            }
        }
    }

    private void CombineBoolUserStat(UserStatBool i_UserStatBool)
    {
        // Steam does not implement bool stats.
    }

    private void CombineFloatUserStat(UserStatFloat i_UserStatFloat)
    {
        string statId = i_UserStatFloat.id;
        RemoteStatInfo remoteStatInfo = m_RemoteStatsMapper.GetRemoteStatInfo(statId);
        if (remoteStatInfo != null)
        {
            string steamStatId = remoteStatInfo.steamId;
            if (steamStatId != "")
            {
                float remoteValue = 0;
                if (SteamUserStats.GetStat(steamStatId, out remoteValue))
                {
                    i_UserStatFloat.Combine(remoteValue);
                }
            }
        }
    }

    private void CombineStringUserStat(UserStatString i_UserStatString)
    {
        // Steam does not implement string stats.
    }

    private void StoreIntUserStat(UserStatInt i_UserStatInt)
    {
        string statId = i_UserStatInt.id;
        RemoteStatInfo remoteStatInfo = m_RemoteStatsMapper.GetRemoteStatInfo(statId);
        if (remoteStatInfo != null)
        {
            string steamStatId = remoteStatInfo.steamId;
            if (steamStatId != "")
            {
                int statValue = i_UserStatInt.intValue;
                SteamUserStats.SetStat(steamStatId, statValue);
            }
        }
    }

    private void StoreBoolUserStat(UserStatBool i_UserStatBool)
    {
        // Steam does not implement bool stats
    }

    private void StoreFloatUserStat(UserStatFloat i_UserStatFloat)
    {
        string statId = i_UserStatFloat.id;
        RemoteStatInfo remoteStatInfo = m_RemoteStatsMapper.GetRemoteStatInfo(statId);
        if (remoteStatInfo != null)
        {
            string steamStatId = remoteStatInfo.steamId;
            if (steamStatId != "")
            {
                float statValue = i_UserStatFloat.floatValue;
                SteamUserStats.SetStat(steamStatId, statValue);
            }
        }
    }

    private void StoreStringUserStat(UserStatString i_UserStatString)
    {
        // Steam does not implement string stats
    }

    // IStatsModule's interface

    public void Initialize(UserStatsManager i_UserStatsManager, RemoteStatsMapper i_RemoteStatsMapper)
    {
        if (!SteamManager.initializedMain)
            return;

        m_UserStatsManager = i_UserStatsManager;
        m_RemoteStatsMapper = i_RemoteStatsMapper;

        m_StatsValid = false;

        m_UserStatsReceived = Steamworks.Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);

        m_GameId = new CGameID(SteamAppInfo.s_AppId);
    }

    public void Update()
    {

    }

    public void StoreStats()
    {
        if (!SteamManager.initializedMain)
            return;

        if (!m_StatsValid)
            return;

        for (int userStatIndex = 0; userStatIndex < m_UserStatsManager.statsCount; ++userStatIndex)
        {
            UserStat userStat = m_UserStatsManager.GetStatByIndex(userStatIndex);
            if (userStat != null)
            {
                UserStatType userStatType = userStat.type;
                switch (userStatType)
                {
                    case UserStatType.Int:

                        UserStatInt userStatInt = (UserStatInt)userStat;
                        StoreIntUserStat(userStatInt);

                        break;

                    case UserStatType.Bool:

                        UserStatBool userStatBool = (UserStatBool)userStat;
                        StoreBoolUserStat(userStatBool);

                        break;

                    case UserStatType.Float:

                        UserStatFloat userStatFloat = (UserStatFloat)userStat;
                        StoreFloatUserStat(userStatFloat);

                        break;

                    case UserStatType.String:

                        UserStatString userStatString = (UserStatString)userStat;
                        StoreStringUserStat(userStatString);

                        break;
                }
            }
        }

        SteamManager.SetStatsDirtyMain();
    }
}

#endif