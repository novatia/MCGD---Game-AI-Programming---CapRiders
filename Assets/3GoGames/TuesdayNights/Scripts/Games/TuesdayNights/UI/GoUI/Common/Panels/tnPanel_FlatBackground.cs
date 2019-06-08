using UnityEngine;

using GoUI;

public class tnPanel_FlatBackground : UIPanel<tnView_FlatBackground>
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

    public void SetBackgroundColor(Color i_Color)
    {
        if (viewInstance != null)
        {
            viewInstance.SetBackgroundColor(i_Color);
        }
    }

    public void SetBackgroundColorWithoutAlpha(Color i_Color)
    {
        if (viewInstance != null)
        {
            viewInstance.SetBackgroundColorWithoutAlpha(i_Color);
        }
    }

    public void SetBackgroundAlpha(float i_Alpha)
    {
        if (viewInstance != null)
        {
            viewInstance.SetBackgroundAlpha(i_Alpha);
        }
    }
}