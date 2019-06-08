using UnityEngine;

using PlayerInputEvents;

public sealed class UInputProviderImpl : IInputProviderImpl
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
        get { return Input.mousePresent; }
    }

    public Vector3 mousePosition
    {
        get
        {
            return Input.mousePosition;
        }
    }

    public bool GetMouseButton(int i_Button)
    {
        return Input.GetMouseButton(i_Button);
    }

    public bool GetMouseButtonDown(int i_Button)
    {
        return Input.GetMouseButtonDown(i_Button);
    }

    public bool GetMouseButtonUp(int i_Button)
    {
        return Input.GetMouseButtonUp(i_Button);
    }

    // KEYBOARD

    public bool GetKey(KeyCode i_Key)
    {
        return Input.GetKey(i_Key);
    }

    public bool GetKeyDown(KeyCode i_Key)
    {
        return Input.GetKeyDown(i_Key);
    }

    public bool GetKeyUp(KeyCode i_Key)
    {
        return Input.GetKeyUp(i_Key);
    }

    // ACTIONS

    public bool GetButton(string i_ButtonName)
    {
        return Input.GetButton(i_ButtonName);
    }

    public bool GetButtonDown(string i_ButtonName)
    {
        return Input.GetButtonDown(i_ButtonName);
    }

    public bool GetButtonUp(string i_ButtonName)
    {
        return Input.GetButtonUp(i_ButtonName);
    }

    public float GetAxis(string i_AxisName)
    {
        return Input.GetAxis(i_AxisName);
    }

    public float GetAxisRaw(string i_AxisName)
    {
        return Input.GetAxisRaw(i_AxisName);
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
            return true;
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

    public UInputProviderImpl()
    {

    }
}
