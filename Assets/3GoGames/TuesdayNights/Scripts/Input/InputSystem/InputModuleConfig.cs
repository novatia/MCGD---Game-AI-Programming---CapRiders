using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class InputModuleConfig : BaseScriptableObject
{
    [SerializeField]
    private List<string> m_DefaultPlayers = new List<string>();
    [SerializeField]
    private List<string> m_DefaultWifiPlayers = new List<string>();

    [SerializeField]
    private string m_HorizontalAxis = "Horizontal";
    [SerializeField]
    private string m_VerticalAxis = "Vertical";
    [SerializeField]
    private string m_SubmitButton = "Submit";
    [SerializeField]
    private string m_CancelButton = "Cancel";

    [SerializeField]
    private string m_WifiHorizontalAxis = "Horizontal";
    [SerializeField]
    private string m_WifiVerticalAxis = "Vertical";
    [SerializeField]
    private string m_WifiSubmitButton = "Submit";
    [SerializeField]
    private string m_WifiCancelButton = "Cancel";

    [SerializeField]
    private float m_InputActionsPerSecond = 4.5f;

    [SerializeField]
    private bool m_MoveOneElementPerAxisPress = false;

    [SerializeField]
    private float m_RepeatDelay = 0f;

    [SerializeField]
    private float m_AxisDeadzone = 0.2f;

    [SerializeField]
    private bool m_AllowMouseInput = false;
    [SerializeField]
    private bool m_AllowMouseInputIfTouchSupported = false;
    [SerializeField]
    private bool m_AllowActivationOnMobileDevice = false;

    // ACCESSORS

    public int defaultPlayersCount
    {
        get
        {
            return m_DefaultPlayers.Count;
        }
    }

    public int defaultWifiPlayersCount
    {
        get
        {
            return m_DefaultWifiPlayers.Count;
        }
    }

    public string horizontalAxis
    {
        get
        {
            return m_HorizontalAxis;
        }
    }

    public string verticalAxis
    {
        get
        {
            return m_VerticalAxis;
        }
    }

    public string submitButton
    {
        get
        {
            return m_SubmitButton;
        }
    }

    public string cancelButton
    {
        get
        {
            return m_CancelButton;
        }
    }

    public string wifiHorizontalAxis
    {
        get
        {
            return m_WifiHorizontalAxis;
        }
    }

    public string wifiVerticalAxis
    {
        get
        {
            return m_WifiVerticalAxis;
        }
    }

    public string wifiSubmitButton
    {
        get
        {
            return m_WifiSubmitButton;
        }
    }

    public string wifiCancelButton
    {
        get
        {
            return m_WifiCancelButton;
        }
    }

    public float inputActionsPerSecond
    {
        get
        {
            return m_InputActionsPerSecond;
        }
    }

    public bool moveOneElementPerAxisPress
    {
        get
        {
            return m_MoveOneElementPerAxisPress;
        }
    }

    public float repeatDelay
    {
        get
        {
            return m_RepeatDelay;
        }
    }

    public float axisDeadzone
    {
        get
        {
            return m_AxisDeadzone;
        }
    }

    public bool allowMouseInput
    {
        get
        {
            return m_AllowMouseInput;
        }
    }

    public bool allowMouseInputIfTouchSupported
    {
        get
        {
            return m_AllowMouseInputIfTouchSupported;
        }
    }

    public bool allowActivationOnMobileDevice
    {
        get
        {
            return m_AllowActivationOnMobileDevice;
        }
    }

    // LOGIC

    public string GetPlayer(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_DefaultPlayers.Count)
        {
            return StringUtils.s_EMPTY;
        }

        return m_DefaultPlayers[i_Index];
    }

    public string GetWifiPlayer(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_DefaultWifiPlayers.Count)
        {
            return StringUtils.s_EMPTY;
        }

        return m_DefaultWifiPlayers[i_Index];
    }
}
