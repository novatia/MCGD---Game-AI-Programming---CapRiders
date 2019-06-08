using UnityEngine;
using UnityEngine.UI;

using GoUI;

public class tnView_WaitForRematch : GoUI.UIView
{
    [SerializeField]
    private Text m_Text = null;

    // UIView's interface

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
        string text = "REMATCH VOTED (" + i_ReadyPlayers + " / " + i_TotalPlayers + ")";
        Internal_SetText(text);
    }

    // INTERNALS

    private void Internal_SetText(string i_Text)
    {
        if (m_Text != null)
        {
            m_Text.text = i_Text;
        }
    }
}