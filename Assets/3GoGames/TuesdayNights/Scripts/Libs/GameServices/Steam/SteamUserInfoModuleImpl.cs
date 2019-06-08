#if STEAM

using UnityEngine;
using System.Collections;

using Steamworks;

public class SteamUserInfoModuleImpl : IUserInfoModuleImpl
{
    private CSteamID m_UserSteamId;
    private string m_Username = "";

    // IUserInfoModuleImpl's interface

    public string username
    {
        get
        {
            if (SteamManager.initializedMain)
            {
                return SteamFriends.GetPersonaName();
            }

            return "";
        }
    }

    public void Initialize()
    {

    }

    // CTOR

    public SteamUserInfoModuleImpl()
    {
        
    }
}

#endif // STEAM