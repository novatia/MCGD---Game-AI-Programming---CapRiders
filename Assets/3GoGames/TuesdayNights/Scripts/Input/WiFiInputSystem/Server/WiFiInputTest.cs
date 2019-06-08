using UnityEngine;

using System;
using System.Collections.Generic;

using WiFiInput.Server;

using FullInspector;

[Serializable]
public class PlayerEntry
{
    [SerializeField]
    private string m_PlayerName = "";
    [SerializeField]
    private List<string> m_Buttons = new List<string>();
    [SerializeField]
    private List<string> m_Axes = new List<string>();


    public string playerName
    {
        get { return m_PlayerName; }
    }

    public int buttonsCount
    {
        get { return m_Buttons.Count; }
    }

    public int axesCount
    {
        get { return m_Axes.Count; }
    }

    // LOGIC

    public string GetButton(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Buttons.Count)
        {
            return "";
        }

        return m_Buttons[i_Index];
    }

    public string GetAxis(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Axes.Count)
        {
            return "";
        }

        return m_Axes[i_Index];
    }
}

public class WiFiPlayer
{
    private string m_PlayerName = "";
    private WiFiPlayerInput m_PlayerInput = null;
    private List<string> m_Buttons = null;
    private List<string> m_Axes = null;

    public int buttonsCount
    {
        get { return m_Buttons.Count; }
    }

    public int axesCount
    {
        get { return m_Axes.Count; }
    }

    // LOGIC

    public void AddButton(string i_Button)
    {
        m_Buttons.Add(i_Button);
    }

    public void AddAxis(string i_Axis)
    {
        m_Axes.Add(i_Axis);
    }

    public string GetButton(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Buttons.Count)
        {
            return "";
        }

        return m_Buttons[i_Index];
    }

    public string GetAxis(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Axes.Count)
        {
            return "";
        }

        return m_Axes[i_Index];
    }

    public void Test()
    {
        if (m_PlayerInput == null)
            return;

        for (int index = 0; index < m_Buttons.Count; ++index)
        {
            string button = m_Buttons[index];
            if (button != "")
            {
                bool buttonPressed = m_PlayerInput.GetButtonDown(button);
                if (buttonPressed)
                {
                    Debug.Log("[" + m_PlayerName + "] " + button);
                }
            }
        }

        for (int index = 0; index < m_Axes.Count; ++index)
        {
            string axis = m_Axes[index];
            if (axis != "")
            {
                float axisValue = m_PlayerInput.GetAxis(axis);
                if (axisValue != 0f)
                {
                    Debug.Log("[" + m_PlayerName + "] " + axis + ": " + axisValue);
                }
            }
        }
    }

    // CTOR

    public WiFiPlayer(string i_PlayerName, WiFiPlayerInput i_PlayerInput)
    {
        m_PlayerName = i_PlayerName;
        m_PlayerInput = i_PlayerInput;
        m_Buttons = new List<string>();
        m_Axes = new List<string>();
    }
}

public class WiFiInputTest : BaseBehavior
{
    [SerializeField]
    private List<PlayerEntry> m_Players = new List<PlayerEntry>();

    private List<WiFiPlayer> m_WiFiPlayers = new List<WiFiPlayer>();

    protected override void Awake()
    {
        WiFiInputSystem.InitializeMain();

        for (int index = 0; index < m_Players.Count; ++index)
        {
            PlayerEntry entry = m_Players[index];
            if (entry != null)
            {
                WiFiPlayerInput playerInput = WiFiInputSystem.GetPlayerByNameMain(entry.playerName);
                if (playerInput != null)
                {
                    WiFiPlayer player = new WiFiPlayer(entry.playerName, playerInput);

                    for (int buttonIndex = 0; buttonIndex < entry.buttonsCount; ++buttonIndex)
                    {
                        string button = entry.GetButton(buttonIndex);
                        if (button != "")
                        {
                            player.AddButton(button);
                        }
                    }

                    for (int axisIndex = 0; axisIndex < entry.axesCount; ++axisIndex)
                    {
                        string axis = entry.GetAxis(axisIndex);
                        if (axis != "")
                        {
                            player.AddAxis(axis);
                        }
                    }
                    m_WiFiPlayers.Add(player);
                }
            }
        }
    }

    void OnEnable()
    {
        WiFiInputSystem.controllerRegistredEventMain += OnControllerRegistered;
        WiFiInputSystem.connectionsChangedEventMain += OnConnectionsChanged;
    }

    void OnDisable()
    {
        WiFiInputSystem.controllerRegistredEventMain -= OnControllerRegistered;
        WiFiInputSystem.connectionsChangedEventMain -= OnConnectionsChanged;
    }

    void Update()
    {
        for (int index = 0; index < m_Players.Count; ++index)
        {
            WiFiPlayer player = m_WiFiPlayers[index];
            if (player != null)
            {
                player.Test();
            }
        }
    }

    // EVENTS

    private void OnConnectionsChanged(bool i_IsConnect, int i_PlayerNumber)
    {
        Debug.Log("Connections changed");
    }

    private void OnControllerRegistered(string i_Key)
    {
        Debug.Log("Controller registered: " + i_Key + ".");
    }
}
