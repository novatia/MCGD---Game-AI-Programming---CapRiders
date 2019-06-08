#if STEAM

using UnityEngine;

using System.Text;

using Steamworks;

public class SteamManager : Singleton<SteamManager>
{
    private static bool s_EverInitialized = false;

    private bool m_Initialized = false;
    private bool m_StoreStatsRequest = false;
    private bool m_StatsRequested = false;

    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook = null;

    // STATIC

    private static void SteamAPIDebugTextHook(int i_Severity, StringBuilder i_PchDebugText)
    {
        Debug.LogWarning(i_PchDebugText);
    }

    public static bool initializedMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.initialized;
            }

            return false;
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void SetStatsDirtyMain()
    {
        if (Instance != null)
        {
            Instance.SetStatsDirty();
        }
    }

    // MonoBehaviour's interface

    void Update()
    {
        if (!m_Initialized)
            return;

        if (!m_StatsRequested)
        {
            bool success = SteamUserStats.RequestCurrentStats();
            m_StatsRequested = success;
        }

        if (m_StoreStatsRequest)
        {
            bool success = SteamUserStats.StoreStats();
            m_StoreStatsRequest = !success;
        }

        SteamAPI.RunCallbacks();
    }

    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();

        if (!m_Initialized)
            return;

        SteamAPI.Shutdown();
    }

    // LOGIC

    public bool initialized
    {
        get
        {
            return m_Initialized;
        }
    }

    public void Initialize()
    {
        if (s_EverInitialized)
        {
            LogManager.LogError(this, "Tried to initialize the SteamAPI twice in one session.");
        }

        if (!Packsize.Test())
        {
            LogManager.LogError(this, "[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
        }

        if (!DllCheck.Test())
        {
            LogManager.LogError(this, "[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
        }

        if (SteamAPI.RestartAppIfNecessary(SteamAppInfo.s_AppId))
        {
            Application.Quit();
            return;
        }

        m_Initialized = SteamAPI.Init();
        if (!m_Initialized)
        {
            LogManager.LogError(this, "SteamAPI.Init() failed.");
            return;
        }

        if (m_SteamAPIWarningMessageHook == null)
        {
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }

        s_EverInitialized = true;
    }

    public void SetStatsDirty()
    {
        m_StoreStatsRequest = true;
    }
}

#endif