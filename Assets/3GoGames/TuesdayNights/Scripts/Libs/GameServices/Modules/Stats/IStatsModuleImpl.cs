public interface IStatsModuleImpl
{
    void Initialize(UserStatsManager i_UserStatsManager, RemoteStatsMapper i_RemoteStatsMapper);
    void Update();
    void StoreStats();
}
