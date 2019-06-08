using UnityEngine;

using PlayerInputEvents;

public interface IInputProviderImpl
{
    // EVENTS

    event OnControllerConnected onControllerConnectedEvent;
    event OnControllerDisconnected onControllerDisconnectedEvent;

    event OnEditorRecompileEvent onEditorRecompileEvent;

    // MOUSE

    bool mousePresent { get; }
    Vector3 mousePosition { get; }

    bool GetMouseButton(int i_Button);
    bool GetMouseButtonUp(int i_Button);
    bool GetMouseButtonDown(int i_Button);

    // KEYBOARD

    bool GetKey(KeyCode i_Key);
    bool GetKeyDown(KeyCode i_Key);
    bool GetKeyUp(KeyCode i_Key);

    // ACTIONS

    bool GetButton(string i_ButtonName);
    bool GetButtonDown(string i_ButtonName);
    bool GetButtonUp(string i_ButtonName);
    float GetAxis(string i_AxisName);
    float GetAxisRaw(string i_AxisName);

    // BUSINESS LOGIC

    bool isReady { get; }
    bool useXInput { get; set; }

    void Initialize();
    void Reset();
}
