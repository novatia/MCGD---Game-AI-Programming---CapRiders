using UnityEngine;

using PlayerInputEvents;

public class InputProvider
{
    private IInputProviderImpl m_Impl = null;

    // LOGIC

    public bool isReady
    {
        get
        {
            return m_Impl.isReady;
        }
    }

    public bool useXInput
    {
        get
        {
            return m_Impl.useXInput;
        }

        set
        {
            m_Impl.useXInput = value;
        }
    }

    public void Initialize()
    {
        m_Impl.Initialize();
    }

    public void Reset()
    {
        m_Impl.Reset();
    }

    public bool GetButton(string i_ButtonName)
    {
        return m_Impl.GetButton(i_ButtonName);
    }

    public bool GetButtonDown(string i_ButtonName)
    {
        return m_Impl.GetButtonDown(i_ButtonName);
    }

    public bool GetButtonUp(string i_ButtonName)
    {
        return m_Impl.GetButtonUp(i_ButtonName);
    }

    public float GetAxis(string i_AxisName)
    {
        return m_Impl.GetAxis(i_AxisName);
    }

    public float GetAxisRaw(string i_AxisName)
    {
        return m_Impl.GetAxisRaw(i_AxisName);
    }

    // EVENTS

    public event OnControllerConnected onControllerConnectedEvent
    {
        add
        {
            m_Impl.onControllerConnectedEvent += value;
        }

        remove
        {
            m_Impl.onControllerConnectedEvent -= value;
        }
    }

    public event OnControllerDisconnected onControllerDisconnectedEvent
    {
        add
        {
            m_Impl.onControllerDisconnectedEvent += value;
        }

        remove
        {
            m_Impl.onControllerDisconnectedEvent -= value;
        }
    }

    public event OnEditorRecompileEvent onEditorRecompileEvent
    {
        add
        {
            m_Impl.onEditorRecompileEvent += value;
        }

        remove
        {
            m_Impl.onEditorRecompileEvent += value;
        }
    }

    // MOUSE

    public bool mousePresent
    {
        get { return m_Impl.mousePresent; }
    }

    public Vector3 mousePosition
    {
        get
        {
            return m_Impl.mousePosition;
        }
    }

    public bool GetMouseButton(int i_Button)
    {
        return m_Impl.GetMouseButton(i_Button);
    }

    public bool GetMouseButtonUp(int i_Button)
    {
        return m_Impl.GetMouseButtonUp(i_Button);
    }

    public bool GetMouseButtonDown(int i_Button)
    {
        return m_Impl.GetMouseButtonDown(i_Button);
    }

    // KEYBOARD

    public bool GetKey(KeyCode i_Key)
    {
        return m_Impl.GetKey(i_Key);
    }

    public bool GetKeyDown(KeyCode i_Key)
    {
        return m_Impl.GetKeyDown(i_Key);
    }

    public bool GetKeyUp(KeyCode i_Key)
    {
        return m_Impl.GetKeyUp(i_Key);
    }

    // CTOR

    public InputProvider()
    {
#if INPUT_STANDARD
        m_Impl = new UInputProviderImpl();
#elif INPUT_REWIRED
        m_Impl = new RewiredInputProviderImpl();
#else
        m_Impl = new NullInputProviderImpl();
#endif
    }
}
