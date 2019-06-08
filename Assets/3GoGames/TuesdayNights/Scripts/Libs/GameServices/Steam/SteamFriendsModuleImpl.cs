#if STEAM

using System;

using Steamworks;

public class SteamFriendsModuleImpl : IFriendsModuleImpl
{
    private DictionaryList<CSteamID, SteamFriend> m_Friends = null;

    private event Action m_FriendsChangedEvent = null;
    private event Action<Invite> m_InviteAcceptedEvent = null;

    protected Steamworks.Callback<GameRichPresenceJoinRequested_t> m_GameRichPresenceJoinRequested = null;
    protected Steamworks.Callback<PersonaStateChange_t> m_PersonaStateChange = null;

    // IFriendsModuleImpl's interface

    public event Action friendsChangedEvent
    {
        add
        {
            m_FriendsChangedEvent += value;
        }

        remove
        {
            m_FriendsChangedEvent -= value;
        }
    }

    public event Action inviteReceivedEvent
    {
        add
        {

        }

        remove
        {

        }
    }

    public event Action<Invite> inviteAcceptedEvent
    {
        add
        {
            m_InviteAcceptedEvent += value;
        }

        remove
        {
            m_InviteAcceptedEvent -= value;
        }
    }

    public int friendsCount
    {
        get
        {
            return m_Friends.count;
        }
    }

    public void Initialize()
    {
        if (!SteamManager.initializedMain)
            return;

        m_GameRichPresenceJoinRequested = Steamworks.Callback<GameRichPresenceJoinRequested_t>.Create(OnGameRichPresenceJoinRequested);
        m_PersonaStateChange = Steamworks.Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);

        EFriendFlags friendFlags = EFriendFlags.k_EFriendFlagImmediate;
        int friendsCount = SteamFriends.GetFriendCount(friendFlags);
        for (int friendIndex = 0; friendIndex < friendsCount; ++friendIndex)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(friendIndex, friendFlags);

            if (!friendSteamId.IsValid())
                continue;

            SteamFriend steamFriend = new SteamFriend(friendSteamId);
            m_Friends.Add(friendSteamId, steamFriend);
        }
    }

    public void Update()
    {

    }

    public Friend GetFriendByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Friends.count)
        {
            return null;
        }

        return m_Friends.GetItem(i_Index);
    }

    public void InviteFriend(int i_Index, string i_Args)
    {
        Friend friend = GetFriendByIndex(i_Index);
        InviteFriend(friend, i_Args);
    }

    public void InviteFriend(Friend i_Friend, string i_Args)
    {
        if (i_Friend == null)
            return;

        if (!SteamManager.initializedMain)
            return;

        SteamFriend steamFriend = (SteamFriend)i_Friend;
        CSteamID friendId = steamFriend.steamId;

        SteamFriends.InviteUserToGame(friendId, i_Args);
    }

    // INTERNALS

    private SteamFriend Internal_GetFriend(CSteamID i_SteamId)
    {
        SteamFriend friend;
        m_Friends.TryGetValue(i_SteamId, out friend);
        return friend;
    }

    private void RaiseFriendsChangedEvent()
    {
        if (m_FriendsChangedEvent != null)
        {
            m_FriendsChangedEvent();
        }
    }

    private void RaiseInviteAcceptedEvent(Invite i_Invite)
    {
        if (m_InviteAcceptedEvent != null)
        {
            m_InviteAcceptedEvent(i_Invite);
        }
    }

    // EVENTS

    private void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t i_Params)
    {
        CSteamID friendId = i_Params.m_steamIDFriend;

        Friend friend = Internal_GetFriend(friendId);
        string args = i_Params.m_rgchConnect;

        Invite invite = new Invite(friend, args);

        RaiseInviteAcceptedEvent(invite);
    }

    private void OnPersonaStateChange(PersonaStateChange_t i_Params)
    {
        CSteamID steamId = new CSteamID(i_Params.m_ulSteamID);
        SteamFriend steamFriend = Internal_GetFriend(steamId);
        if (steamFriend != null)
        {
            bool isMyFriend = SteamFriends.HasFriend(steamId, EFriendFlags.k_EFriendFlagImmediate);
            if (isMyFriend)
            {
                steamFriend.Refresh();
            }
            else
            {
                m_Friends.Remove(steamId);
            }
        }
        else
        {
            steamFriend = new SteamFriend(steamId);
            m_Friends.Add(steamId, steamFriend);
        }

        RaiseFriendsChangedEvent();
    }

    // CTOR

    public SteamFriendsModuleImpl()
    {
        m_Friends = new DictionaryList<CSteamID, SteamFriend>();
    }
}

#endif // STEAM