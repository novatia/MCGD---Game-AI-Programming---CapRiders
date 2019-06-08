using UnityEngine;

using PlayerInputEvents;

public class NullInputProviderImpl : IInputProviderImpl
{
    // EVENTS

    public event OnControllerConnected onControllerConnectedEvent
    {
        add
        {
            // Nothing to do
        }
        remove
        {
            // Nothing to do
        }
    }

    public event OnControllerDisconnected onControllerDisconnectedEvent
    {
        add
        {
            // Nothing to do
        }
        remove
        {
            // Nothing to do
        }
    }

    public event OnEditorRecompileEvent onEditorRecompileEvent
    {
        add
        {
            // Nothing to do
        }
        remove
        {
            // Nothing to do
        }
    }

    // MOUSE

    public bool mousePresent
    {
        get { return false; }
    }

    public Vector3 mousePosition
    {
        get
        {
            return Vector3.zero;
        }
    }

    public bool GetMouseButton(int i_Button)
    {
        return false;
    }

    public bool GetMouseButtonUp(int i_Button)
    {
        return false;
    }

    public bool GetMouseButtonDown(int i_Button)
    {
        return false;
    }

    // KEYBOARD

    public bool GetKey(KeyCode i_Key)
    {
        return false;
    }

    public bool GetKeyDown(KeyCode i_Key)
    {
        return false;
    }

    public bool GetKeyUp(KeyCode i_Key)
    {
        return false;
    }

    // ACTIONS

    public bool GetButton(string i_ButtonName)
    {
        return false;
    }

    public bool GetButtonDown(string i_ButtonName)
    {
        return false;
    }

    public bool GetButtonUp(string i_ButtonName)
    {
        return false;
    }

    public float GetAxis(string i_AxisName)
    {
        return 0f;
    }

    public float GetAxisRaw(string i_AxisName)
    {
        return 0f;
    }

    // BUSINESS LOGIC

    public bool isReady
    {
        get
        {
            return true;
        }
    }

    public bool useXInput
    {
        get
        {
            return false;
        }
        set
        {

        }
    }

    public void Initialize()
    {

    }

    public void Reset()
    {

    }

    // CTOR

    public NullInputProviderImpl()
    {

    }
}
