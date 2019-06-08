public class UserInfoModule : GameServicesModule
{
    private IUserInfoModuleImpl m_Impl = null;

    // ACCESSORS

    public string username
    {
        get
        {
            return m_Impl.username;
        }
    }

    // GameServicesModule's interface

    public override void Initialize()
    {
        m_Impl.Initialize();
    }

    public override void Update()
    {

    }

    // CTOR

    public UserInfoModule()
    {
#if STEAM
        m_Impl = new SteamUserInfoModuleImpl();
#else
        m_Impl = new NullUserInfoModuleImpl();
#endif
    }
}
