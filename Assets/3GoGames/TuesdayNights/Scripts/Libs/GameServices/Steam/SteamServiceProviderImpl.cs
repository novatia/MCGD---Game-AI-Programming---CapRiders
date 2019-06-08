#if STEAM

using UnityEngine;
using System.Collections;

using Steamworks;

public class SteamServiceProviderImpl : IServiceProviderImpl
{
    // IServiceProviderImpl's interface

    public void Initialize()
    {
        SteamManager.InitializeMain();
    }

}
#endif
