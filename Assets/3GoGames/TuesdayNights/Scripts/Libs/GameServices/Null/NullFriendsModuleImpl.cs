using System;

public class NullFriendsModuleImpl : IFriendsModuleImpl
{
    // IFriendsModuleImpl's interface

    public event Action friendsChangedEvent
    {
        add
        {

        }
        remove
        {

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

        }

        remove
        {

        }
    }

    public int friendsCount
    {
        get
        {
            return 0;
        }
    }

    public void Initialize()
    {

    }

    public void Update()
    {

    }

    public Friend GetFriendByIndex(int i_Index)
    {
        return null;
    }

    public void InviteFriend(int i_Index, string i_Args)
    {

    }

    public void InviteFriend(Friend i_Friend, string i_Args)
    {

    }
}