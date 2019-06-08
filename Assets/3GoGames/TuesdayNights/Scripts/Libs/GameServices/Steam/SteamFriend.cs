#if STEAM

using UnityEngine;

using Steamworks;

public class SteamFriend : Friend
{
    // Fields

    private CSteamID m_SteamId = CSteamID.Nil;

    private string m_Name = "";
    private Sprite m_Icon = null;
    private EPersonaState m_State = EPersonaState.k_EPersonaStateMax;

    // ACCESSORS

    public CSteamID steamId
    {
        get
        {
            return m_SteamId;
        }
    }

    // LOGIC

    public void Refresh()
    {
        if (!SteamManager.initializedMain)
            return;

        string personaName = SteamFriends.GetFriendPersonaName(m_SteamId);
        Sprite icon = SteamUtilities.GetMediumAvatar(m_SteamId);
        EPersonaState state = SteamFriends.GetFriendPersonaState(m_SteamId);

        m_Name = personaName;
        m_Icon = icon;
        m_State = state;
    }

    // Friend's interface

    public override string name
    {
        get
        {
            return m_Name;
        }
    }

    public override Sprite icon
    {
        get
        {
            return m_Icon;
        }
    }

    public override PersonaStatus status
    {
        get
        {
            return GetPersonaStatus(m_State);
        }
    }

    // INTERNALS

    private PersonaStatus GetPersonaStatus(EPersonaState i_State)
    {
        PersonaStatus personaStatus = PersonaStatus.None;

        switch (i_State)
        {
            case EPersonaState.k_EPersonaStateAway:
                personaStatus = PersonaStatus.Online;
                break;

            case EPersonaState.k_EPersonaStateBusy:
                personaStatus = PersonaStatus.Online;
                break;

            case EPersonaState.k_EPersonaStateLookingToPlay:
                personaStatus = PersonaStatus.Online;
                break;

            case EPersonaState.k_EPersonaStateLookingToTrade:
                personaStatus = PersonaStatus.Online;
                break;

            case EPersonaState.k_EPersonaStateOffline:
                personaStatus = PersonaStatus.Offline;
                break;

            case EPersonaState.k_EPersonaStateOnline:
                personaStatus = PersonaStatus.Online;
                break;

            case EPersonaState.k_EPersonaStateSnooze:
                personaStatus = PersonaStatus.Online;
                break;
        }

        return personaStatus;
    }

    // CTOR

    public SteamFriend(CSteamID i_SteamId)
    {
        m_SteamId = i_SteamId;
        Refresh();
    }
}

#endif // STEAM