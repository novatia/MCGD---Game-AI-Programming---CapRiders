using UnityEngine;
using System.Collections;

public class tnMatchStatsController : UIViewController
{
    protected virtual void ShowStats(tnMatchController i_Controller)
    {

    }

    // MonoBehaviour's INTERFACE

    void OnEnable()
    {
        tnMatchController matchController = FindObjectOfType<tnMatchController>();
        if (matchController == null)
            return;

        ShowStats(matchController);
    }

    void OnDisable()
    {

    }

    // UIViewController's INTERFACE

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
