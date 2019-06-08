using System;
using WiFiInput.Server;

public class tnUIPhone : tnUIDevice
{
    private WiFiPlayerInput m_PlayerInput = null;

    // tnUIDevice's interface

    protected override bool isInputActive
    {
        get
        {
            if (m_PlayerInput != null)
            {
                return m_PlayerInput.isActive;
            }

            return false;
        }
    }

    protected override void InternalBindToPlayer(string i_PlayerName)
    {
        BindTo(i_PlayerName);
    }

    protected override bool InternalGetProceedButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetButtonDown("Submit");
        }

        return false;
    }

    protected override bool InternalGetCancelButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetButtonDown("Cancel");
        }

        return false;
    }

    protected override bool InternalGetRightButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetPositiveButtonDown("Horizontal");
        }

        return false;
    }

    protected override bool InternalGetLeftButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetNegativeButtonDown("Horizontal");
        }

        return false;
    }

    protected override bool InternalGetAddBotButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetButtonDown("Action1");
        }

        return false;
    }

    protected override bool InternalGetRemoveBotButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetButtonDown("Action2");
        }

        return false;
    }

    // INTERNALS

    private void BindTo(string i_PlayerName)
    {
        if (StringUtils.IsNullOrEmpty(i_PlayerName))
            return;

        WiFiPlayerInput playerInput = WiFiInputSystem.GetPlayerByNameMain(i_PlayerName);
        m_PlayerInput = playerInput;
    }
}
