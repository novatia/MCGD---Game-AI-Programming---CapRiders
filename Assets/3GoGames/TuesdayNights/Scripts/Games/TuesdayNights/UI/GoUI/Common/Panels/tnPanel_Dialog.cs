using System;

using GoUI;

public class tnPanel_Dialog : UIPanel<tnView_Dialog>
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

    public void SetTitle(string i_Text)
    {
        if (viewInstance != null)
        {
            viewInstance.SetTitle(i_Text);
        }
    }

    public void SetDeatilText(string i_Text)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeatilText(i_Text);
        }
    }

    public void ShowDialog(string i_Title, string i_DetailText, Action i_Callback = null)
    {
        if (viewInstance != null)
        {
            viewInstance.ShowDialog(i_Title, i_DetailText, i_Callback);
        }
    }

    public void ShowDialog(Action i_Callback = null)
    {
        if (viewInstance != null)
        {
            viewInstance.ShowDialog(i_Callback);
        }
    }
}