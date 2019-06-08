public class StatsModule : GameServicesModule
{
    private UserStatsManager m_UserStatsManager = null;
    private RemoteStatsMapper m_RemoteStatsMapper = null;

    private IStatsModuleImpl m_Impl = null;

    // GameServicesModule's methods

    public override void Initialize()
    {
        m_UserStatsManager.Initialize();
        m_RemoteStatsMapper.Initialize();

        m_Impl.Initialize(m_UserStatsManager, m_RemoteStatsMapper);
    }

    public override void Update()
    {
        m_Impl.Update();
    }

    // LOGIC

    public void StoreStats()
    {
        m_Impl.StoreStats();
    }

    // User stats manager proxy

    public UserStatType GetStatStype(string i_StatId)
    {
        return m_UserStatsManager.GetUserStatStype(i_StatId);
    }

    public void UpdateIntStat(string i_StatId, int i_Value, IntStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateIntStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateIntStat(int i_StatId, int i_Value, IntStatUpdateFunction i_UpdateFunction)
    {
        m_UserStatsManager.UpdateIntStat(i_StatId, i_Value, i_UpdateFunction);
    }

    public void UpdateBoolStat(string i_StatId, bool i_Value, BoolStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateBoolStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateBoolStat(int i_StatId, bool i_Value, BoolStatUpdateFunction i_UpdateFunction)
    {
        m_UserStatsManager.UpdateBoolStat(i_StatId, i_Value, i_UpdateFunction);
    }

    public void UpdateFloatStat(string i_StatId, float i_Value, FloatStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateFloatStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateFloatStat(int i_StatId, float i_Value, FloatStatUpdateFunction i_UpdateFunction)
    {
        m_UserStatsManager.UpdateFloatStat(i_StatId, i_Value, i_UpdateFunction);
    }

    public void UpdateStringStat(string i_StatId, string i_Value, StringStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateStringStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateStringStat(int i_StatId, string i_Value, StringStatUpdateFunction i_UpdateFunction)
    {
        m_UserStatsManager.UpdateStringStat(i_StatId, i_Value, i_UpdateFunction);
    }

    public void RegisterIntStatHandler(string i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterIntStatHandler(hash, i_Handler);
    }

    public void RegisterIntStatHandler(int i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        m_UserStatsManager.RegisterIntStatHandler(i_StatId, i_Handler);
    }

    public void RegisterBoolStatHandler(string i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterBoolStatHandler(hash, i_Handler);
    }

    public void RegisterBoolStatHandler(int i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        m_UserStatsManager.RegisterBoolStatHandler(i_StatId, i_Handler);
    }

    public void RegisterFloatStatHandler(string i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterFloatStatHandler(hash, i_Handler);
    }

    public void RegisterFloatStatHandler(int i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        m_UserStatsManager.RegisterFloatStatHandler(i_StatId, i_Handler);
    }

    public void RegisterStringStatHandler(string i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterStringStatHandler(hash, i_Handler);
    }

    public void RegisterStringStatHandler(int i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        m_UserStatsManager.RegisterStringStatHandler(i_StatId, i_Handler);
    }

    public void UnregisterIntStatHandler(string i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterIntStatHandler(hash, i_Handler);
    }

    public void UnregisterIntStatHandler(int i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        m_UserStatsManager.UnregisterIntStatHandler(i_StatId, i_Handler);
    }

    public void UnregisterBoolStatHandler(string i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterBoolStatHandler(hash, i_Handler);
    }

    public void UnregisterBoolStatHandler(int i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        m_UserStatsManager.UnregisterBoolStatHandler(i_StatId, i_Handler);
    }

    public void UnregisterFloatStatHandler(string i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterFloatStatHandler(hash, i_Handler);
    }

    public void UnregisterFloatStatHandler(int i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        m_UserStatsManager.UnregisterFloatStatHandler(i_StatId, i_Handler);
    }

    public void UnregisterStringStatHandler(string i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterStringStatHandler(hash, i_Handler);
    }

    public void UnregisterStringStatHandler(int i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        m_UserStatsManager.UnregisterStringStatHandler(i_StatId, i_Handler);
    }

    public UserStatInt GetIntStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetIntStat(hash);
    }

    public UserStatInt GetIntStat(int i_StatId)
    {
        return m_UserStatsManager.GetIntStat(i_StatId);
    }

    public UserStatBool GetBoolStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetBoolStat(hash);
    }

    public UserStatBool GetBoolStat(int i_StatId)
    {
        return m_UserStatsManager.GetBoolStat(i_StatId);
    }

    public UserStatFloat GetFloatStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetFloatStat(hash);
    }

    public UserStatFloat GetFloatStat(int i_StatId)
    {
        return m_UserStatsManager.GetFloatStat(i_StatId);
    }

    public UserStatString GetStringStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetStringStat(hash);
    }

    public UserStatString GetStringStat(int i_StatId)
    {
        return m_UserStatsManager.GetStringStat(i_StatId);
    }

    public bool TryGetIntStatValue(string i_StatId, out int o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetIntStatValue(hash, out o_Value);
    }

    public bool TryGetIntStatValue(int i_StatId, out int o_Value)
    {
        return m_UserStatsManager.TryGetIntStatValue(i_StatId, out o_Value);
    }

    public bool TryGetBoolStatValue(string i_StatId, out bool o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetBoolStatValue(hash, out o_Value);
    }

    public bool TryGetBoolStatValue(int i_StatId, out bool o_Value)
    {
        return m_UserStatsManager.TryGetBoolStatValue(i_StatId, out o_Value);
    }

    public bool TryGetFloatStatValue(string i_StatId, out float o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetFloatStatValue(hash, out o_Value);
    }

    public bool TryGetFloatStatValue(int i_StatId, out float o_Value)
    {
        return m_UserStatsManager.TryGetFloatStatValue(i_StatId, out o_Value);
    }

    public bool TryGetStringStatValue(string i_StatId, out string o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetStringStatValue(hash, out o_Value);
    }

    public bool TryGetStringStatValue(int i_StatId, out string o_Value)
    {
        return m_UserStatsManager.TryGetStringStatValue(i_StatId, out o_Value);
    }

    // CTOR

    public StatsModule()
    {
        m_UserStatsManager = new UserStatsManager("Database/UserStats/UserStatsDatabase");
        m_RemoteStatsMapper = new RemoteStatsMapper("Database/UserStats/RemoteStatsMap");

#if STEAM
        m_Impl = new SteamStatsModuleImpl();
#else
        m_Impl = new NullStatsModuleImpl();
#endif
    }
}
