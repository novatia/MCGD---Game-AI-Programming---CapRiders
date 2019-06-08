using UnityEngine;
using System.Collections;

using GoUI;

public class tnPanel_WaitForRematch : UIPanel<tnView_WaitForRematch>
{
    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // LOGIC

    public void SetPlayers(int i_ReadyPlayers, int i_TotalPlayers)
    {
        if (viewInstance != null)
        {
            viewInstance.SetPlayers(i_ReadyPlayers, i_TotalPlayers);
        }
    }
}
