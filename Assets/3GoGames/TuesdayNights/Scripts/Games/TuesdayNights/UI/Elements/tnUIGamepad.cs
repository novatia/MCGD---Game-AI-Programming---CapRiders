using UnityEngine;

using PlayerInputEvents;
using System;

public class tnUIGamepad : tnUIDevice
{
    [SerializeField]
    private Sprite m_KeyboardImage = null;
    [SerializeField]
    private Sprite m_GamepadImage = null;

    private PlayerInput m_PlayerInput = null;

    // tnUIDevice's interface

    protected override bool isInputActive
    {
        get
        {
            if (m_PlayerInput != null)
            {
                return m_PlayerInput.bIsActive;
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
            return m_PlayerInput.GetButtonDown("HorizontalRight");
        }

        return false;
    }

    protected override bool InternalGetLeftButton()
    {
        if (m_PlayerInput != null)
        {
            return m_PlayerInput.GetButtonDown("HorizontalLeft");
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

    protected override void OnStateEnter(DeviceState i_OldState, DeviceState i_NewState)
    {
        if (i_NewState == DeviceState.Disabled)
        {
            SetImage(m_GamepadImage);
        }
    }

    protected override void OnDeactivate()
    {
        SetImage(m_GamepadImage);
    }

    protected override void OnActivate()
    {
        RefreshImage();
    }

    // MonoBehaviour's interface

    protected override void OnEnable()
    {
        base.OnEnable();

        BindTo(playerName); // Force a gamepad rebind. Previous PlayerInput could have been destroyed or could have been invalidated.

        InputSystem.onControllerConnectedEventMain += OnControllerConnectedEvent;
        InputSystem.onControllerDisconnectedEventMain += OnControllerDisconnectedEvent;

        InputSystem.onInputSystemResetEventMain += OnInputSystemResetEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Release();

        InputSystem.onInputSystemResetEventMain -= OnInputSystemResetEvent;

        InputSystem.onControllerConnectedEventMain -= OnControllerConnectedEvent;
        InputSystem.onControllerDisconnectedEventMain -= OnControllerDisconnectedEvent;
    }

    // EVENTS

    private void OnControllerConnectedEvent(ControllerEventParams i_Params)
    {
        RefreshImage();
    }

    private void OnControllerDisconnectedEvent(ControllerEventParams i_Params)
    {
        RefreshImage();
    }

    private void OnInputSystemResetEvent()
    {
        BindTo(playerName);

        RefreshImage();
    }

    // INTERNALS

    private void BindTo(string i_PlayerName)
    {
        if (StringUtils.IsNullOrEmpty(i_PlayerName))
            return;

        PlayerInput playerInput = InputSystem.GetPlayerByNameMain(i_PlayerName);
        m_PlayerInput = playerInput;
    }

    private void Release()
    {
        m_PlayerInput = null;
    }

    private void RefreshImage()
    {
        if (m_PlayerInput == null)
            return;

        if (deviceState == DeviceState.Disabled)
        {
            SetImage(m_GamepadImage);
            return;
        }

        int joystickCount = m_PlayerInput.JoystickCount;
        if (joystickCount > 0)
        {
            SetImage(m_GamepadImage);
        }
        else
        {
            SetImage(m_KeyboardImage);
        }
    }
}
