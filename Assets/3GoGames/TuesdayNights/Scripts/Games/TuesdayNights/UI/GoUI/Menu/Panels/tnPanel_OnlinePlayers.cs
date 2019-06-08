using UnityEngine;
using System.Collections;

using GoUI;

public class tnPanel_OnlinePlayers : UIPanel<tnView_OnlinePlayers>
{
    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);

        int countOfPlayers = PhotonNetwork.countOfPlayers;
        Internal_SetPlayerCount(countOfPlayers);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // INTERNALS

    private void Internal_SetPlayerCount(int i_PlayerCount)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetPlayerCount(i_PlayerCount);
    }
}