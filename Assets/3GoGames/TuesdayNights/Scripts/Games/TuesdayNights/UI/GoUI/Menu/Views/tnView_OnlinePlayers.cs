using UnityEngine;
using UnityEngine.UI;

using GoUI;

public class tnView_OnlinePlayers : GoUI.UIView
{
    // Serializable fields

    [Header("Widgets")]

    [SerializeField]
    private Text m_PlayerCount = null;

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

    public void SetPlayerCount(int i_PlayerCount)
    {
        int playerCount = Mathf.Max(0, i_PlayerCount);

        string text = playerCount.ToString();
        Internal_SetPlayerCount(text);
    }

    // INTERNALS

    private void Internal_SetPlayerCount(string i_Text)
    {
        if (m_PlayerCount != null)
        {
            m_PlayerCount.text = i_Text;
        }
    }
}