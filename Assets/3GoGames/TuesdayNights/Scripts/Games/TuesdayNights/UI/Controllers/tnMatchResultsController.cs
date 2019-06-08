using UnityEngine;
using System.Collections;

public class tnMatchResultsController : UIViewController
{
    protected virtual void ShowResults(tnMatchController i_Controller)
    {

    }

    // MonoBehaviour's INTERFACE

    void OnEnable()
    {
        tnMatchController matchController = FindObjectOfType<tnMatchController>();
        if (matchController == null)
            return;

        ShowResults(matchController);
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