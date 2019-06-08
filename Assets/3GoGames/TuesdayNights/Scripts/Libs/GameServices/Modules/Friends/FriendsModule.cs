using System;

public class FriendsModule : GameServicesModule
{
    private IFriendsModuleImpl m_Impl = null;

    public event Action friendsChangedEvent = null;
    public event Action inviteReceivedEvent = null;
    public event Action<Invite> inviteAcceptedEvent = null;

    public int friendsCount
    {
        get
        {
            return m_Impl.friendsCount;
        }
    }

    // GameServicesModule's interface

    public override void Initialize()
    {
        m_Impl.Initialize();

        m_Impl.friendsChangedEvent += OnImplFriendsChangedEvent;
        m_Impl.inviteReceivedEvent += OnImplInviteReceivedEvent;
        m_Impl.inviteAcceptedEvent += OnImplInviteAcceptedEvent;
    }

    public override void Update()
    {
        m_Impl.Update();
    }

    // LOGIC

    public Friend GetFriendByIndex(int i_Index)
    {
        return m_Impl.GetFriendByIndex(i_Index);
    }

    public void InviteFriend(int i_Index, string i_Args)
    {
        m_Impl.InviteFriend(i_Index, i_Args);
    }
    
    public void InviteFriend(Friend i_Friend, string i_Args)
    {
        m_Impl.InviteFriend(i_Friend, i_Args);
    }

    // EVENTS

    private void OnImplFriendsChangedEvent()
    {
        if (friendsChangedEvent != null)
        {
            friendsChangedEvent();
        }
    }

    private void OnImplInviteReceivedEvent()
    {
        if (inviteReceivedEvent != null)
        {
            inviteReceivedEvent();
        }
    }

    private void OnImplInviteAcceptedEvent(Invite i_Invite)
    {
        if (inviteAcceptedEvent != null)
        {
            inviteAcceptedEvent(i_Invite);
        }
    }

    // CTOR

    public FriendsModule()
    {
#if STEAM
        m_Impl = new SteamFriendsModuleImpl();
#else
        m_Impl = new NullFriendsModuleImpl();
#endif
    }
}