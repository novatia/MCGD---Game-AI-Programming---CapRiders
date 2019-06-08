using UnityEngine;

using System;
using System.Collections;

public class MatchmakingModule : GameServicesModule
{
    private IMatchmakingModuleImpl m_Impl = null;

    // GameServicesModule's interface

    public override void Initialize()
    {
        m_Impl.Initialize();
    }

    public override void Update()
    {
        m_Impl.Update();
    }

    // CTOR

    public MatchmakingModule()
    {
#if STEAM
        m_Impl = new SteamMatchmakingModuleImpl();
#else
        m_Impl = new NullMatchmakingModuleImpl();
#endif
    }
}