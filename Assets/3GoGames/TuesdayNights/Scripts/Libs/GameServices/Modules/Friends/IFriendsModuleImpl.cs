using System;

public interface IFriendsModuleImpl
{
    event Action friendsChangedEvent;
    event Action inviteReceivedEvent;
    event Action<Invite> inviteAcceptedEvent;

    int friendsCount { get; }

    void Initialize();
    void Update();

    Friend GetFriendByIndex(int i_Index);

    void InviteFriend(int i_Index, string i_Args);
    void InviteFriend(Friend i_Friend, string i_Args);
}
