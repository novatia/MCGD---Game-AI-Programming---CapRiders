using UnityEngine;

using System.Collections.Generic;

using PlayerInputEvents;

public class InputSystem : Singleton<InputSystem>
{
    private InputProvider m_InputProvider = null;
    private List<PlayerInput> m_Players = new List<PlayerInput>();

    private event OnControllerConnected m_OnControllerConnectedEvent;
    private event OnControllerDisconnected m_OnControllerDisconnectedEvent;

    private event OnEditorRecompileEvent m_OnEditorRecompileEvent;

    private event OnInputSystemReset m_OnInputSystemResetEvent;

    private bool m_Initialized = false;

    // INPUT PROVIDER - MAIN

    public static Vector3 mousePositionMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.mousePosition;
            }

            return Vector3.zero;
        }
    }

    public static bool GetMouseButtonMain(int i_Button)
    {
        if (Instance != null)
        {
            return Instance.GetMouseButton(i_Button);
        }

        return false;
    }

    public static bool GetMouseButtonDownMain(int i_Button)
    {
        if (Instance != null)
        {
            return Instance.GetMouseButtonDown(i_Button);
        }

        return false;
    }

    public static bool GetMouseButtonUpMain(int i_Button)
    {
        if (Instance != null)
        {
            return Instance.GetMouseButtonUp(i_Button);
        }

        return false;
    }

    public static bool GetKeyMain(KeyCode i_Key)
    {
        if (Instance != null)
        {
            return Instance.GetKey(i_Key);
        }

        return false;
    }

    public static bool GetKeyDownMain(KeyCode i_Key)
    {
        if (Instance != null)
        {
            return Instance.GetKeyDown(i_Key);
        }

        return false;
    }

    public static bool GetKeyUpMain(KeyCode i_Key)
    {
        if (Instance != null)
        {
            return Instance.GetKeyUp(i_Key);
        }

        return false;
    }

    public static bool GetButtonMain(string i_ButtonName)
    {
        if (Instance != null)
        {
            return Instance.GetButton(i_ButtonName);
        }

        return false;
    }

    public static bool GetButtonDownMain(string i_ButtonName)
    {
        if (Instance != null)
        {
            return Instance.GetButtonDown(i_ButtonName);
        }

        return false;
    }

    public static bool GetButtonUpMain(string i_ButtonName)
    {
        if (Instance != null)
        {
            return Instance.GetButtonUp(i_ButtonName);
        }

        return false;
    }

    public static float GetAxisMain(string i_AxisName)
    {
        if (Instance != null)
        {
            return Instance.GetAxis(i_AxisName);
        }

        return 0f;
    }

    public static float GetAxisRawMain(string i_AxisName)
    {
        if (Instance != null)
        {
            return Instance.GetAxisRaw(i_AxisName);
        }

        return 0f;
    }

    // PLAYER INPUT - MAIN

    public static PlayerInput player0Main
    {
        get
        {
            if (Instance != null)
            {
                return Instance.player0;
            }

            return null;
        }
    }

    public static PlayerInput player1Main
    {
        get
        {
            if (Instance != null)
            {
                return Instance.player1;
            }

            return null;
        }
    }

    public static PlayerInput player2Main
    {
        get
        {
            if (Instance != null)
            {
                return Instance.player2;
            }

            return null;
        }
    }

    public static PlayerInput player3Main
    {
        get
        {
            if (Instance != null)
            {
                return Instance.player3;
            }

            return null;
        }
    }

    public static int numPlayersMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.numPlayers;
            }

            return 0;
        }
    } 

    public static void AddPlayerMain(string i_Name)
    {
        if (Instance != null)
        {
            Instance.AddPlayer(i_Name);
        }
    }

    public static PlayerInput GetPlayerByIndexMain(int i_Index)
    {
        if (Instance != null)
        {
            return Instance.GetPlayerByIndex(i_Index);
        }

        return null;
    }

    public static PlayerInput GetPlayerByIdMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetPlayerById(i_Id);
        }

        return null;
    }

    public static PlayerInput GetPlayerByNameMain(string i_Name)
    {
        if (Instance != null)
        {
            return Instance.GetPlayerByName(i_Name);
        }

        return null;
    }

    public static void RefreshMapsMain()
    {
        if (Instance != null)
        {
            Instance.RefreshMaps();
        }
    }

    // EVENTS - MAIN

    public static event OnControllerConnected onControllerConnectedEventMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onControllerConnectedEvent += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onControllerConnectedEvent -= value;
            }
        }
    }

    public static event OnControllerDisconnected onControllerDisconnectedEventMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onControllerDisconnectedEvent += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onControllerDisconnectedEvent -= value;
            }
        }
    }

    public static event OnEditorRecompileEvent onEditorRecompileEventMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onEditorRecompileEvent += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onEditorRecompileEvent -= value;
            }
        }
    }

    public static event OnInputSystemReset onInputSystemResetEventMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onInputSystemResetEvent += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onInputSystemResetEvent -= value;
            }
        }
    }

    // BUSINESS LOGIC - MAIN

    public static bool isReadyMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.isReady;
            }

            return false;
        }
    }

    public static bool useXInputMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.useXInput;
            }

            return false;
        }

        set
        {
            if (Instance != null)
            {
                if (Instance.useXInput != value)
                {
                    Instance.useXInput = value;
                }
            }
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void ResetMain()
    {
        if (Instance != null)
        {
            Instance.Reset();
        }
    }

    // INPUT PROVIDER

    public Vector3 mousePosition
    {
        get
        {
            if (m_InputProvider == null)
            {
                return Vector3.zero;
            }

            return m_InputProvider.mousePosition;
        }
    }

    public bool GetMouseButton(int i_Button)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetMouseButton(i_Button);
    }

    public bool GetMouseButtonDown(int i_Button)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetMouseButtonDown(i_Button);
    }

    public bool GetMouseButtonUp(int i_Button)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetMouseButtonUp(i_Button);
    }

    public bool GetKey(KeyCode i_Key)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetKey(i_Key);
    }

    public bool GetKeyDown(KeyCode i_Key)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetKeyDown(i_Key);
    }

    public bool GetKeyUp(KeyCode i_Key)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetKeyUp(i_Key);
    }

    public bool GetButton(string i_ButtonName)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetButton(i_ButtonName);
    }

    public bool GetButtonDown(string i_ButtonName)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetButtonDown(i_ButtonName);
    }

    public bool GetButtonUp(string i_ButtonName)
    {
        if (m_InputProvider == null)
        {
            return false;
        }

        return m_InputProvider.GetButtonUp(i_ButtonName);
    }

    public float GetAxis(string i_AxisName)
    {
        if (m_InputProvider == null)
        {
            return 0f;
        }

        return m_InputProvider.GetAxis(i_AxisName);
    }

    public float GetAxisRaw(string i_AxisName)
    {
        if (m_InputProvider == null)
        {
            return 0f;
        }

        return m_InputProvider.GetAxisRaw(i_AxisName);
    }

    // PLAYER INPUT

    public PlayerInput player0
    {
        get
        {
            return GetPlayerByIndex(0);
        }
    }

    public PlayerInput player1
    {
        get
        {
            return GetPlayerByIndex(1);
        }
    }

    public PlayerInput player2
    {
        get
        {
            return GetPlayerByIndex(2);
        }
    }

    public PlayerInput player3
    {
        get
        {
            return GetPlayerByIndex(3);
        }
    }

    public int numPlayers
    {
        get
        {
            return m_Players.Count;
        }
    }

    public void AddPlayer(string i_Name)
    {
        PlayerInput player = new PlayerInput(i_Name);
        m_Players.Add(player);
    }

    public PlayerInput GetPlayerByIndex(int i_Index)
    {
        if (i_Index < m_Players.Count)
        {
            return m_Players[i_Index];
        }

        return null;
    }

    public PlayerInput GetPlayerById(int i_Id)
    {
        for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
        {
            PlayerInput playerInput = m_Players[playerIndex];

            if (playerInput != null)
            {
                if (playerInput.Id == i_Id)
                {
                    return playerInput;
                }
            }
        }

        return null;
    }

    public PlayerInput GetPlayerByName(string i_Name)
    {
        for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
        {
            PlayerInput playerInput = m_Players[playerIndex];

            if (playerInput != null)
            {
                if (playerInput.Name == i_Name)
                {
                    return playerInput;
                }
            }
        }

        return null;
    }

    public void RefreshMaps()
    {
        bool keyboardAssigned = false;

        for (int i = 0; i < m_Players.Count; ++i)
        {
            PlayerInput currentPlayer = GetPlayerByIndex(i);

            currentPlayer.DisableAllMaps();
            currentPlayer.HasMouse = false;

            currentPlayer.SetActive(false);

            if (currentPlayer.JoystickCount != 0)
            {
                currentPlayer.EnableAllMapsFor(InputUtils.InputSourceType.eJoystick);

                currentPlayer.SetActive(true);
            }
            else
            {
                if (!keyboardAssigned)
                {
                    currentPlayer.EnableAllMapsFor(InputUtils.InputSourceType.eKeyboard);
                    currentPlayer.EnableAllMapsFor(InputUtils.InputSourceType.eMouse);
                    currentPlayer.HasMouse = true;

                    currentPlayer.SetActive(true);

                    keyboardAssigned = true;
                }
            }
        }
    }

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

    public event OnInputSystemReset onInputSystemResetEvent
    {
        add
        {
            m_OnInputSystemResetEvent += value;
        }
        remove
        {
            m_OnInputSystemResetEvent -= value;
        }
    }

    // BUSINESS LOGIC

    public bool isReady
    {
        get
        {
            return m_Initialized && (m_InputProvider != null && m_InputProvider.isReady);
        }
    }

    public bool useXInput
    {
        get
        {
            if (m_InputProvider != null)
            {
                return m_InputProvider.useXInput;
            }

            return false;
        }

        set
        {
            if (m_InputProvider != null)
            {
                if (m_InputProvider.useXInput != value)
                {
                    m_InputProvider.useXInput = value;
                    Reset();
                }
            }
        }
    }

    public void Initialize()
    {
        if (m_Initialized)
            return;

        m_InputProvider = new InputProvider();
        m_InputProvider.Initialize();

        m_InputProvider.onControllerConnectedEvent += InternalControllerConnected;
        m_InputProvider.onControllerDisconnectedEvent += InternalControllerDisconnected;

        m_InputProvider.onEditorRecompileEvent += InternalEditorRecompile;

        m_Initialized = true;
    }

    public void Reset()
    {
        List<string> playerNames = new List<string>();

        // Backup players list.

        for (int index = 0; index < m_Players.Count; ++index)
        {
            PlayerInput playerInput = m_Players[index];
            if (playerInput != null)
            {
                string playerName = playerInput.Name;
                playerNames.Add(playerName);
            }
        }

        // Clear players list.

        m_Players.Clear();

        // Re-Init players and assignments.

        m_InputProvider.Reset();

        for (int index = 0; index < playerNames.Count; ++index)
        {
            string playerName = playerNames[index];
            AddPlayer(playerName);
        }

        RefreshMaps();

        // Callback.

        if (m_OnInputSystemResetEvent != null)
        {
            m_OnInputSystemResetEvent();
        }
    }

    // EVENTS

    private void InternalControllerConnected(ControllerEventParams i_EventParams)
    {
        RefreshMaps();

        // Notify listeners.

        if (m_OnControllerConnectedEvent != null)
        {
            m_OnControllerConnectedEvent(i_EventParams);
        }
    }

    private void InternalControllerDisconnected(ControllerEventParams i_EventParams)
    {
        RefreshMaps();

        // Notify listeners.

        if (m_OnControllerDisconnectedEvent != null)
        {
            m_OnControllerDisconnectedEvent(i_EventParams);
        }
    }

    private void InternalEditorRecompile()
    {
        if (m_OnEditorRecompileEvent != null)
        {
            m_OnEditorRecompileEvent();
        }
    }
}