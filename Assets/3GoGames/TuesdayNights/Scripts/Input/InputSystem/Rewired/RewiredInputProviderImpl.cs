#if INPUT_REWIRED

using UnityEngine;

using Rewired;
using PlayerInputEvents;

public class RewiredInputProviderImpl : IInputProviderImpl
{
    private event OnControllerConnected m_OnControllerConnectedEvent;
    private event OnControllerDisconnected m_OnControllerDisconnectedEvent;

    private event OnEditorRecompileEvent m_OnEditorRecompileEvent;

    // EVENTS

    public event OnControllerConnected onControllerConnectedEvent
    {
        add
        {
            m_OnControllerConnectedEvent += value;
        }

        remove
        {
            m_OnControllerConnectedEvent -= value;
        }
    }

    public event OnControllerDisconnected onControllerDisconnectedEvent
    {
        add
        {
            m_OnControllerDisconnectedEvent += value;
        }

        remove
        {
            m_OnControllerDisconnectedEvent -= value;
        }
    }

    public event OnEditorRecompileEvent onEditorRecompileEvent
    {
        add
        {
            m_OnEditorRecompileEvent += value;
        }

        remove
        {
            m_OnEditorRecompileEvent -= value;
        }
    }

    // MOUSE

    public bool mousePresent
    {
        get
        {
            if (!ReInput.isReady)
            {
                return false;
            }

            return ReInput.controllers.Mouse.isConnected;
        }
    }

    public Vector3 mousePosition
    {
        get
        {
            if (!ReInput.isReady)
            {
                return Vector3.zero;
            }

            return new Vector3(ReInput.controllers.Mouse.screenPosition.x, ReInput.controllers.Mouse.screenPosition.y, 0f);
        }
    }

    public bool GetMouseButton(int i_Button)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        return ReInput.controllers.Mouse.GetButton(i_Button);
    }

    public bool GetMouseButtonUp(int i_Button)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        return ReInput.controllers.Mouse.GetButtonUp(i_Button);
    }

    public bool GetMouseButtonDown(int i_Button)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        return ReInput.controllers.Mouse.GetButtonDown(i_Button);
    }

    // KEYS

    public bool GetKey(KeyCode i_Key)
    {
        if (!ReInput.isReady)
        {
            return false;
        }
        
        if (i_Key < KeyCode.Mouse0)
        {
            return ReInput.controllers.Keyboard.GetKey(i_Key);
        }
        else
        {
            if (i_Key < KeyCode.JoystickButton0)
            {
                return ReInput.controllers.Mouse.GetButton((int)i_Key - (int)KeyCode.Mouse0);
            }
        }

        return false;
    }   

    public bool GetKeyDown(KeyCode i_Key)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        if (i_Key < KeyCode.Mouse0)
        {
            return ReInput.controllers.Keyboard.GetKeyDown(i_Key);
        }
        else
        {
            if (i_Key < KeyCode.JoystickButton0)
            {
                return ReInput.controllers.Mouse.GetButtonDown((int)i_Key - (int)KeyCode.Mouse0);
            }
        }

        return false;
    }

    public bool GetKeyUp(KeyCode i_Key)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        if (i_Key < KeyCode.Mouse0)
        {
            return ReInput.controllers.Keyboard.GetKeyUp(i_Key);
        }
        else
        {
            if (i_Key < KeyCode.JoystickButton0)
            {
                return ReInput.controllers.Mouse.GetButtonUp((int)i_Key - (int)KeyCode.Mouse0);
            }
        }

        return false;
    }

    // ACTIONS

    public bool GetButton(string i_ButtonName)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        Player player = ReInput.players.GetPlayer(0);

        if (player != null)
        {
            return player.GetButton(i_ButtonName);
        }

        return false;
    }

    public bool GetButtonDown(string i_ButtonName)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        Player player = ReInput.players.GetPlayer(0);

        if (player != null)
        {
            return player.GetButtonDown(i_ButtonName);
        }

        return false;
    }

    public bool GetButtonUp(string i_ButtonName)
    {
        if (!ReInput.isReady)
        {
            return false;
        }

        Player player = ReInput.players.GetPlayer(0);

        if (player != null)
        {
            return player.GetButtonUp(i_ButtonName);
        }

        return false;
    }
    
    public float GetAxis(string i_AxisName)
    {
        if (!ReInput.isReady)
        {
            return 0f;
        }

        Player player = ReInput.players.GetPlayer(0);

        if (player != null)
        {
            return player.GetAxis(i_AxisName);
        }

        return 0f;
    }

    public float GetAxisRaw(string i_AxisName)
    {
        if (!ReInput.isReady)
        {
            return 0f;
        }

        Player player = ReInput.players.GetPlayer(0);

        if (player != null)
        {
            return player.GetAxisRaw(i_AxisName);
        }

        return 0f;
    }

    // BUSINESS LOGIC

    public bool isReady
    {
        get
        {
            return ReInput.isReady;
        }
    }

    public bool useXInput
    {
        get
        {
            return ReInput.configuration.useXInput;
        }

        set
        {
            ReInput.configuration.useXInput = value;
        }
    }

    public void Initialize()
    {
        InputManager inputManagerPrefab = Resources.Load<InputManager>("Input/p_InputManager");
        if (inputManagerPrefab != null)
        {
            InputManager inputManagerInstance = GameObject.Instantiate<InputManager>(inputManagerPrefab);
            inputManagerInstance.name = "RewiredInputManager";
        }
        else
        {
            LogManager.LogError(this, "Rewired input manager prefab not found. Create it in Resources/Input/ folder.");
        }

        ReInput.ControllerConnectedEvent += InternalOnControllerConnected;
        ReInput.ControllerDisconnectedEvent += InternalOnControllerDisconnected;

        ReInput.EditorRecompileEvent += InternalOnEditorRecompileEvent;
    }

    public void Reset()
    {
        // Remove events.

        {
            ReInput.ControllerConnectedEvent -= InternalOnControllerConnected;
            ReInput.ControllerDisconnectedEvent -= InternalOnControllerDisconnected;

            ReInput.EditorRecompileEvent -= InternalOnEditorRecompileEvent;
        }

        // Reset ReInput.

        ReInput.Reset();

        // Add events.

        {
            ReInput.ControllerConnectedEvent += InternalOnControllerConnected;
            ReInput.ControllerDisconnectedEvent += InternalOnControllerDisconnected;

            ReInput.EditorRecompileEvent += InternalOnEditorRecompileEvent;
        }
    }

    // INTERNALS

    private void InternalOnControllerConnected(ControllerStatusChangedEventArgs i_Args)
    {
        if (m_OnControllerConnectedEvent != null)
        {
            ControllerEventParams args = new ControllerEventParams(i_Args.name, i_Args.controllerId, RewiredUtils.Types.GetInputSourceType(i_Args.controllerType));
            m_OnControllerConnectedEvent(args);
        }
    }

    private void InternalOnControllerDisconnected(ControllerStatusChangedEventArgs i_Args)
    {
        if (m_OnControllerDisconnectedEvent != null)
        {
            ControllerEventParams args = new ControllerEventParams(i_Args.name, i_Args.controllerId, RewiredUtils.Types.GetInputSourceType(i_Args.controllerType));
            m_OnControllerDisconnectedEvent(args);
        }
    }

    private void InternalOnEditorRecompileEvent()
    {
        if (m_OnEditorRecompileEvent != null)
        {
            m_OnEditorRecompileEvent();
        }
    }

    // CTOR

    public RewiredInputProviderImpl()
    {

    }
}

#endif // INPUT_REWIRED