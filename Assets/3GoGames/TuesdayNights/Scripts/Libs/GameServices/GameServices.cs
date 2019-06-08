using System.Collections.Generic;

public class GameServices : Singleton<GameServices>
{
    private ServiceProvider m_ServiceProvider = null;

    private List<GameServicesModule> m_Modules = null;

    private bool m_Initialized = false;

    // STATIC

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static T GetModuleMain<T>() where T : GameServicesModule
    {
        if (Instance != null)
        {
            return Instance.GetModule<T>();
        }

        return null;
    }

    // MonoBehaviour's interface

    void Update()
    {
        if (!m_Initialized)
            return;

        for (int moduleIndex = 0; moduleIndex < m_Modules.Count; ++moduleIndex)
        {
            GameServicesModule module = m_Modules[moduleIndex];
            if (module != null)
            {
                module.Update();
            }
        }
    }

    // LOGIC

    public void Initialize()
    {
        // Initialize providers

        m_ServiceProvider = new ServiceProvider();
        m_ServiceProvider.Initialize();

        // Initialize modules

        m_Modules = new List<GameServicesModule>();

        // User Info module

        UserInfoModule userInfoModule = new UserInfoModule();
        userInfoModule.Initialize();

        m_Modules.Add(userInfoModule);

        // Friends module

        FriendsModule friendsModule = new FriendsModule();
        friendsModule.Initialize();

        m_Modules.Add(friendsModule);

        // Matchmaking module

        MatchmakingModule matchmakingModule = new MatchmakingModule();
        matchmakingModule.Initialize();

        m_Modules.Add(matchmakingModule);

        // Achievements module

        AchievementsModule achievementsModule = new AchievementsModule();
        achievementsModule.Initialize();

        m_Modules.Add(achievementsModule);

        // Stats module

        StatsModule statsModule = new StatsModule();
        statsModule.Initialize();

        m_Modules.Add(statsModule);

        m_Initialized = true;
    }

    public T GetModule<T>() where T : GameServicesModule
    {
        if (!m_Initialized)
        {
            return null;
        }

        for (int modulesIndex = 0; modulesIndex < m_Modules.Count; ++modulesIndex)
        {
            GameServicesModule module = m_Modules[modulesIndex];
            if (module != null)
            {
                if (module is T)
                {
                    return (T)module;
                }
            }
        }

        return null;
    }
}
