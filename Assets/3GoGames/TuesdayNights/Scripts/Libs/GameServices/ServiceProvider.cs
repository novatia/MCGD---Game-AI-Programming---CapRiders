public class ServiceProvider
{
    private IServiceProviderImpl m_Impl = null;

    // BUSINESS LOGIC

    public void Initialize()
    {
        m_Impl.Initialize();
    }

    // CTOR

    public ServiceProvider()
    {
#if STEAM
        m_Impl = new SteamServiceProviderImpl();
#else
        m_Impl = new NullServiceProviderImpl();
#endif
    }
}
