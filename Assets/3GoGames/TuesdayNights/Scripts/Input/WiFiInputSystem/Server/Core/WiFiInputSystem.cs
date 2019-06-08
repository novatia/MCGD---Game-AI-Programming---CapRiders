using UnityEngine;

using System.Collections.Generic;

using WiFiInput.Common;

namespace WiFiInput.Server
{
    public class WiFiInputSystem : Singleton<WiFiInputSystem>
    {
        private WiFiInputManager m_InputManager = null;

        private List<WiFiPlayerInput> m_Players = null;

        public OnControllerRegisteredCallback controllerRegisteredEvent;
        public controllerConnectionsChangedHandler connectionsChangedEvent;

        // STATIC

        public static event OnControllerRegisteredCallback controllerRegistredEventMain
        {
            add
            {
                if (Instance != null)
                {
                    Instance.controllerRegisteredEvent += value;
                }
            }
            remove
            {
                if (Instance != null)
                {
                    Instance.controllerRegisteredEvent -= value;
                }
            }
        }

        public static event controllerConnectionsChangedHandler connectionsChangedEventMain
        {
            add
            {
                if (Instance != null)
                {
                    Instance.connectionsChangedEvent += value;
                }
            }
            remove
            {
                if (Instance != null)
                {
                    Instance.connectionsChangedEvent -= value;
                }
            }
        }

        public static int playersCountMain
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.playersCount;
                }

                return 0;
            }
        }

        public static void InitializeMain()
        {
            if (Instance != null)
            {
                Instance.Initialize();
            }
        }

        public static WiFiPlayerInput GetPlayerByNameMain(string i_PlayerName)
        {
            if (Instance != null)
            {
                return Instance.GetPlayerByName(i_PlayerName);
            }

            return null;
        }

        public static WiFiPlayerInput GetPlayerByIndexMain(int i_Index)
        {
            if (Instance != null)
            {
                return Instance.GetPlayerByIndex(i_Index);
            }

            return null;
        }

        // MonoBehaviour' sinterface

        void Awake()
        {
            m_Players = new List<WiFiPlayerInput>();
        }

        void OnEnable()
        {
            WiFiInputController.On_ConnectionsChanged += OnConnectionsChanged;
            WiFiInputController.OnControllerRegisteredEvent += OnControllerRegistered;
        }

        void OnDisable()
        {
            WiFiInputController.On_ConnectionsChanged -= OnConnectionsChanged;
            WiFiInputController.OnControllerRegisteredEvent -= OnControllerRegistered;
        }

        void Update()
        {
            for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
            {
                WiFiPlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    if (playerInput.isActive)
                    {
                        playerInput.Update();
                    }
                }
            }
        }

        // LOGIC

        public int playersCount
        {
            get { return m_Players.Count; }
        }

        public void Initialize()
        {
            CreateInputManager();
            CreatePlayers();
            CreateControls();
        }

        public WiFiPlayerInput GetPlayerByName(string i_Name)
        {
            for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
            {
                WiFiPlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    if (playerInput.playerName.Equals(i_Name))
                    {
                        return m_Players[playerIndex];
                    }
                }
            }
            return null;
        }

        public WiFiPlayerInput GetPlayerByIndex(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Players.Count)
            {
                return null;
            }

            return m_Players[i_Index];
        }

        // INTERNALS

        private void CreateInputManager()
        {
            m_InputManager = gameObject.AddComponent<WiFiInputManager>();

            WiFiInputManagerConfig config = Resources.Load<WiFiInputManagerConfig>("Input/WiFi/Server_WiFiInputManagerConfig");
            if (config != null)
            {
                m_InputManager.serverSocketPort = config.serverSocketPort;
                m_InputManager.clientSocketPort = config.clientSocketPort;
                m_InputManager.clientTimeout = config.clientTimeout;
                m_InputManager.serverSendBackchannel = config.serverSendBackchannel;
                m_InputManager.serverSendHeartbeatRate = config.serverSendHeartbeatRate;
                m_InputManager.logVerbose = config.logVerbose;

                m_InputManager.applicationName = config.applicationName;
            }
            else
            {
                m_InputManager.serverSocketPort = 2015;
                m_InputManager.clientSocketPort = 2016;
                m_InputManager.clientTimeout = 3f;
                m_InputManager.serverSendBackchannel = false;
                m_InputManager.serverSendHeartbeatRate = 0.5f;
                m_InputManager.logVerbose = false;

                m_InputManager.applicationName = "Default";
            }

            m_InputManager.Initialize();
        }

        private void CreatePlayers()
        {
            WiFiPlayersDatabase database = Resources.Load<WiFiPlayersDatabase>("Input/WiFi/WiFiPlayersDatabase");
            if (database != null)
            {
                for (int playerIndex = 0; playerIndex < database.count; ++playerIndex)
                {
                    WiFiPlayerDescriptor descriptor = database.GetPlayer(playerIndex);
                    if (descriptor != null)
                    {
                        WiFiPlayerInput playerInput = new WiFiPlayerInput(descriptor);

                        playerInput.Initialize();

                        playerInput.SetActive(false);
                        m_Players.Add(playerInput);
                    }
                }
            }
        }

        private void CreateControls()
        {
            WiFiControlsDatabase database = Resources.Load<WiFiControlsDatabase>("Input/WiFi/WiFiControlsDatabase");
            if (database != null)
            {
                for (int controlIndex = 0; controlIndex < database.count; ++controlIndex)
                {
                    WiFiControlDescriptor descriptor = database.GetControl(controlIndex);
                    if (descriptor != null)
                    {
                        ControlType actionType = descriptor.type;
                        switch (actionType)
                        {
                            case ControlType.Button:
                                InternalAddButton(descriptor);
                                break;

                            case ControlType.Axis:
                                InternalAddAxis(descriptor);
                                break;
                        }
                    }
                }
            }
        }

        private void InternalAddButton(WiFiControlDescriptor i_Descriptor)
        {
            string controlName = i_Descriptor.controlName;

            for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
            {
                WiFiPlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    ButtonServerController buttonServerController = new ButtonServerController(controlName, (PLAYER_NUMBER)playerIndex);
                    playerInput.AddButton(controlName, buttonServerController);
                }
            }
        }

        private void InternalAddAxis(WiFiControlDescriptor i_Descriptor)
        {
            string controlName = i_Descriptor.controlName;

            for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
            {
                WiFiPlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    AxisServerController axisServerController = new AxisServerController(controlName, (PLAYER_NUMBER)playerIndex);
                    playerInput.AddAxis(controlName, axisServerController);
                }
            }
        }

        // EVENTS

        private void OnConnectionsChanged(bool i_IsConnect, int i_PlayerNumber)
        {
            // Notify players

            for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
            {
                WiFiPlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    playerInput.OnConnectionsChanged();
                }

                if (playerIndex == i_PlayerNumber)
                {
                    playerInput.SetActive(i_IsConnect);
                }
            }

            // Raise event

            if (connectionsChangedEvent != null)
            {
                connectionsChangedEvent(i_IsConnect, i_PlayerNumber);
            }
        }

        private void OnControllerRegistered(string i_Key)
        {
            if (controllerRegisteredEvent != null)
            {
                controllerRegisteredEvent(i_Key);
            }
        }
    }
}