using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using FullInspector;

using GoUI;

using PlayerInputEvents;

using TuesdayNights;

using WiFiInput.Server;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum tnMultiplayerState
{
    None = 0,

    Login = 1,
    Connection = 2,

    LocalParty = 3,

    Lobby = 4,

    CreatingRoom = 5,

    TryToJoin = 6,

    SideSelection = 7,

    TeamSelection = 8,
    CharacterSelection = 9,
    
    LoadGame = 10,
}

[fiInspectorOnly]
[RequireComponent(typeof(PhotonView))]
public class tnMultiplayer : tnGameFSM<tnMultiplayerState>
{
    // STATIC

    private static string s_MenuFSM_MultiplayerCancel_Event = "MULTIPLAYER / CANCEL";

    private static string s_MultiplayerGame_SceneName = "MultiplayerGame";

    // Serializeable Fields

    [InspectorHeader("Flow")]

    [SerializeField]
    [InspectorCategory("FLOW")]
    private PlayMakerFSM m_MenuFSM = null;

    [InspectorHeader("Timing")]

    [SerializeField]
    [InspectorCategory("FLOW")]
    private double m_TeamSelectionTime = 30.0;
    [SerializeField]
    [InspectorCategory("FLOW")]
    private double m_CharacterSelectionTime = 30.0;

    [SerializeField]
    [InspectorCategory("FLOW")]
    private float m_TeamSelectionProceedDelay = 1f;
    [SerializeField]
    [InspectorCategory("FLOW")]
    private float m_CharacterSelectionProceedDelay = 1f;

    [InspectorHeader("Panels")]

    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_LocalParty m_LocalParty_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_Lobby m_Lobby_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_OnlinePlayers m_OnlinePlayers_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_CreateRoom m_CreateRoom_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_SideSelection m_SideSelection_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_MatchInfo m_MatchInfo_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_TeamSelection m_TeamSelection_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_CharacterSelection m_CharacterSelection_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private UIPanel_Navbar m_Navbar_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_Waiting m_Waiting_Panel_Prefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_Dialog m_Dialog_Panel_Prefab = null;

    [InspectorHeader("Logic")]

    [SerializeField]
    [InspectorCategory("UI")]
    private bool m_ShowOnlinePlayersPanel = true;

    [InspectorHeader("Audio - Side selection")]

    [SerializeField]
    [InspectorCategory("AUDIO")]
    private SfxDescriptor m_SideSelection_Leave_Sfx = null;
    [SerializeField]
    [InspectorCategory("AUDIO")]
    private SfxDescriptor m_SideSelection_Proceed_Sfx = null;

    // Fields

    private tnPanel_LocalParty m_LocalParty_Panel = null;
    private tnPanel_Lobby m_Lobby_Panel = null;
    private tnPanel_OnlinePlayers m_OnlinePlayers_Panel = null;
    private tnPanel_CreateRoom m_CreateRoom_Panel = null;
    private tnPanel_SideSelection m_SideSelection_Panel = null;
    private tnPanel_MatchInfo m_MatchInfo_Panel = null;
    private tnPanel_TeamSelection m_TeamSelection_Panel = null;
    private tnPanel_CharacterSelection m_CharacterSelection_Panel = null;
    private UIPanel_Navbar m_Navbar_Panel = null;
    private tnPanel_Waiting m_Waiting_Panel = null;
    private tnPanel_Dialog m_Dialog_Panel = null;

    private UIController m_LoadingPanel = null;

    private PhotonView m_PhotonView = null;

    private Invite m_Invite = null;

    private int currentStateIndex
    {
        get
        {
            return (int)fsm.currentState;
        }
    }

    private bool isInMatchSetup
    {
        get
        {
            int teamSelectionStateIndex = (int)tnMultiplayerState.TeamSelection;
            int characterSelectionStateIndex = (int)tnMultiplayerState.CharacterSelection;

            return ((currentStateIndex >= teamSelectionStateIndex) && (currentStateIndex <= characterSelectionStateIndex));
        }
    }

    // STATES VARIABLES

    private bool m_PlayerDisconnectedDuringMatchSetup = false;

    // Connection

    private bool m_Connection_ConnectRequested = false;
    private bool m_Connection_JoinLobbyRequested = false;

    // Lobby

    private bool m_Lobby_JoinRoomRequested = false;

    // Creating Room

    private bool m_CreatingRoom_ProceedRequested = false;

    // Side selection.

    private bool m_SideSelection_ProceedRequested = false;
    private bool m_SideSelection_BackRequested = false;

    // Team selection.

    private bool m_TeamSelection_ProceedRequested = false;
    private int m_TeamSelection_PlayersReady = 0;

    private bool m_TeamSelectionTimeSynced = false;
    private bool m_TeamSelectionTimePropertyAlreadySet = false;

    // Character selection.

    private bool m_CharacterSelection_ProceedRequested = false;
    private int m_CharacterSelection_PlayerReady = 0;

    private bool m_CharacterSelectionTimeSynced = false;
    private bool m_CharacterSelectionTimePropertyAlreadySet = false;

    // btFSM's interface

    protected override tnMultiplayerState startingState
    {
        get
        {
            if (PhotonNetwork.connectionState == ConnectionState.Connected)
            {
                return tnMultiplayerState.LocalParty;
            }
            else
            {
                return tnMultiplayerState.Login;
            }
        }
    }

    protected override void OnFSMStarted()
    {
        base.OnFSMStarted();

        RegisterPhotonCommonEvents();
    }

    protected override void OnFSMReturn()
    {
        base.OnFSMReturn();

        UnregisterPhotonCommonEvents();

        ClearAllGroups();

        ClearInputModule();

        if (m_MenuFSM != null)
        {
            m_MenuFSM.SendEvent(s_MenuFSM_MultiplayerCancel_Event);
        }
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Get Components.

        m_PhotonView = GetComponent<PhotonView>();

        // Create panels.

        if (m_LocalParty_Panel_Prefab != null)
        {
            tnPanel_LocalParty localPartyPanelInstance = Instantiate<tnPanel_LocalParty>(m_LocalParty_Panel_Prefab);
            localPartyPanelInstance.transform.SetParent(transform);

            m_LocalParty_Panel = localPartyPanelInstance;
        }

        if (m_Lobby_Panel_Prefab != null)
        {
            tnPanel_Lobby lobbyPanelInstance = Instantiate<tnPanel_Lobby>(m_Lobby_Panel_Prefab);
            lobbyPanelInstance.transform.SetParent(transform);

            m_Lobby_Panel = lobbyPanelInstance;
        }

        if (m_OnlinePlayers_Panel_Prefab != null)
        {
            tnPanel_OnlinePlayers onlinePlayersPanelInstance = Instantiate<tnPanel_OnlinePlayers>(m_OnlinePlayers_Panel_Prefab);
            onlinePlayersPanelInstance.transform.SetParent(transform);

            m_OnlinePlayers_Panel = onlinePlayersPanelInstance;
        }

        if (m_CreateRoom_Panel_Prefab != null)
        {
            tnPanel_CreateRoom createRoomPanelInstance = Instantiate<tnPanel_CreateRoom>(m_CreateRoom_Panel_Prefab);
            createRoomPanelInstance.transform.SetParent(transform);

            m_CreateRoom_Panel = createRoomPanelInstance;
        }

        if (m_SideSelection_Panel_Prefab != null)
        {
            tnPanel_SideSelection sideSelectionPanelInstance = Instantiate<tnPanel_SideSelection>(m_SideSelection_Panel_Prefab);
            sideSelectionPanelInstance.transform.SetParent(transform);

            m_SideSelection_Panel = sideSelectionPanelInstance;
        }

        if (m_MatchInfo_Panel_Prefab != null)
        {
            tnPanel_MatchInfo matchInfoPanelInstance = Instantiate<tnPanel_MatchInfo>(m_MatchInfo_Panel_Prefab);
            matchInfoPanelInstance.transform.SetParent(transform);

            m_MatchInfo_Panel = matchInfoPanelInstance;
        }

        if (m_TeamSelection_Panel_Prefab != null)
        {
            tnPanel_TeamSelection teamSelectionPanelInstance = Instantiate<tnPanel_TeamSelection>(m_TeamSelection_Panel_Prefab);
            teamSelectionPanelInstance.transform.SetParent(transform);

            m_TeamSelection_Panel = teamSelectionPanelInstance;
        }

        if (m_CharacterSelection_Panel_Prefab != null)
        {
            tnPanel_CharacterSelection characterSelectionPanelInstance = Instantiate<tnPanel_CharacterSelection>(m_CharacterSelection_Panel_Prefab);
            characterSelectionPanelInstance.transform.SetParent(transform);

            m_CharacterSelection_Panel = characterSelectionPanelInstance;
        }

        if (m_Navbar_Panel_Prefab != null)
        {
            UIPanel_Navbar navbarPanelInstance = Instantiate<UIPanel_Navbar>(m_Navbar_Panel_Prefab);
            navbarPanelInstance.transform.SetParent(transform);

            m_Navbar_Panel = navbarPanelInstance;
        }

        if (m_Waiting_Panel_Prefab != null)
        {
            tnPanel_Waiting waitingPanelInstance = Instantiate<tnPanel_Waiting>(m_Waiting_Panel_Prefab);
            waitingPanelInstance.transform.SetParent(transform);

            m_Waiting_Panel = waitingPanelInstance;
        }

        if (m_Dialog_Panel_Prefab != null)
        {
            tnPanel_Dialog dialogPanelInstance = Instantiate<tnPanel_Dialog>(m_Dialog_Panel_Prefab);
            dialogPanelInstance.transform.SetParent(transform);

            m_Dialog_Panel = dialogPanelInstance;
        }

        GameObject loadingPanelGo = GameObject.FindGameObjectWithTag("Loading");
        if (loadingPanelGo != null)
        {
            m_LoadingPanel = loadingPanelGo.GetComponent<UIController>();
        }
    }

    // STATES

    // Login

    private void Login_Enter()
    {
        Debug.Log("[Login] Enter");

        // Register on photon callbacks.

        PhotonCallbacks.onConnectedToMasterMain += On_Login_ConnectedToMasterEvent;
        PhotonCallbacks.onFailedToConnectToPhotonMain += On_Login_FailedToConnectToPhotonEvent;

        // Clear local party module.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        if (localPartyModule != null)
        {
            localPartyModule.Clear();
        }

        // Get player name.

        UserInfoModule userInfoModule = GameServices.GetModuleMain<UserInfoModule>();
        string playerName = (userInfoModule != null) ? userInfoModule.username : "";
        playerName = (playerName == "") ? GeneratePlayerName() : playerName;

        // Login.

        OnLoginRequested(playerName);
    }

    private void Login_Update()
    {

    }

    private void Login_Exit()
    {
        Debug.Log("[Login] Exit");

        PhotonCallbacks.onConnectedToMasterMain -= On_Login_ConnectedToMasterEvent;
        PhotonCallbacks.onFailedToConnectToPhotonMain -= On_Login_FailedToConnectToPhotonEvent;
    }

    private void OnLoginRequested(string i_PlayerName)
    {
        if (StringUtils.IsNullOrEmpty(i_PlayerName))
            return;

        OpenWaiting();

        PhotonNetwork.playerName = i_PlayerName;
        PhotonNetwork.ConnectUsingSettings(AppInfo.s_GameVersion);
    }

    private void On_Login_ConnectedToMasterEvent()
    {
        fsm.ChangeState(tnMultiplayerState.Connection);
    }

    private void On_Login_FailedToConnectToPhotonEvent(DisconnectCause cause)
    {
        // Close waiting.

        CloseWaiting();

        // Show dialog.

        ShowDialog("OUCH", "Connection failed.", On_Login_FailedToConnectDialogCallback);
    }

    private void On_Login_FailedToConnectDialogCallback()
    {
        Return();
    }

    // Connection

    private void Connection_Enter()
    {
        Debug.Log("[Connection] Enter");

        // Clear variables.

        m_Connection_ConnectRequested = false;
        m_Connection_JoinLobbyRequested = false;

        // Open Waiting

        OpenWaiting();

        // Open Panels.

        ClearGroup(UIGroup.Group0);

        // Register photon callbacks.

        PhotonCallbacks.onConnectedToMasterMain += On_Connection_ConnectedToMasterEvent;
        PhotonCallbacks.onFailedToConnectToPhotonMain += On_Connection_FailedToConnectToPhotonEvent;
        PhotonCallbacks.onJoinedLobbyMain += On_Connection_JoinedLobbyEvent;
    }

    private void Connection_Update()
    {
        if (!m_Connection_ConnectRequested)
        {
            if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
            {
                m_Connection_ConnectRequested = true;
                PhotonNetwork.ConnectUsingSettings(AppInfo.s_GameVersion);
            }
        }

        if (!m_Connection_JoinLobbyRequested)
        {
            if (PhotonNetwork.connectionStateDetailed == ClientState.ConnectedToMaster)
            {
                m_Connection_JoinLobbyRequested = true;
                PhotonNetwork.JoinLobby();
            }
        }
    }

    private void Connection_Exit()
    {
        Debug.Log("[Connection] Enter");

        // Unregister photon callbacks.

        PhotonCallbacks.onConnectedToMasterMain -= On_Connection_ConnectedToMasterEvent;
        PhotonCallbacks.onFailedToConnectToPhotonMain -= On_Connection_FailedToConnectToPhotonEvent;
        PhotonCallbacks.onJoinedLobbyMain -= On_Connection_JoinedLobbyEvent;
    }

    private void On_Connection_ConnectedToMasterEvent()
    {

    }

    private void On_Connection_FailedToConnectToPhotonEvent(DisconnectCause cause)
    {
        // Close waiting.

        CloseWaiting();

        // Show dialog.

        ShowDialog("OUCH", "Connection failed.", On_Connection_FailedToConnectDialogCallback);
    }

    private void On_Connection_JoinedLobbyEvent()
    {
        // Close waiting.

        CloseWaiting();

        // Check if go to local party or to lobby.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        bool localPartyModuleEmpty = (localPartyModule != null) ? localPartyModule.isEmpty : true;

        // Proceed.

        tnMultiplayerState nextState = (localPartyModuleEmpty) ? tnMultiplayerState.LocalParty : tnMultiplayerState.Lobby;
        fsm.ChangeState(nextState);
    }

    private void On_Connection_FailedToConnectDialogCallback()
    {
        Return();
    }

    // Local party

    private void LocalParty_Enter()
    {
        Debug.Log("[LocalParty] Enter");

        // Send PlayMaker event.

        PlayMakerFSM.BroadcastEvent("MENU FLOW / LOCAL PARTY");

        // Open navbar.

        SwitchPanels(UIGroup.Group6, m_Navbar_Panel);

        // Register photon callbacks.

        PhotonCallbacks.onPhotonJoinRoomFailedMain += On_LocalParty_PhotonJoinRoomFailedEvent;
        PhotonCallbacks.onJoinedRoomMain += On_LocalParty_JoinedRoomEvent;

        // Setup input module.

        SetAllPlayersOnInputModule();

        // Open panel.

        SwitchPanels(UIGroup.Group0, m_LocalParty_Panel);

        if (m_LocalParty_Panel != null)
        {
            m_LocalParty_Panel.SetReceiveInput(true);

            m_LocalParty_Panel.backEvent += OnLocalPartyBack;
            m_LocalParty_Panel.proceedEvent += OnLocalPartyProceed;
        }
    }

    private void LocalParty_Update()
    {

    }

    private void LocalParty_Exit()
    {
        Debug.Log("[LocalParty] Exit");

        if (m_LocalParty_Panel != null)
        {
            m_LocalParty_Panel.SetReceiveInput(true);

            m_LocalParty_Panel.backEvent -= OnLocalPartyBack;
            m_LocalParty_Panel.proceedEvent -= OnLocalPartyProceed;
        }

        // Unregister photon callbacks.

        PhotonCallbacks.onPhotonJoinRoomFailedMain -= On_LocalParty_PhotonJoinRoomFailedEvent;
        PhotonCallbacks.onJoinedRoomMain -= On_LocalParty_JoinedRoomEvent;

        // Clear invite.

        m_Invite = null;
    }

    private void OnLocalPartyBack()
    {
        if (m_LocalParty_Panel != null)
        {
            m_LocalParty_Panel.SetReceiveInput(false);

            m_LocalParty_Panel.proceedEvent -= OnLocalPartyProceed;
            m_LocalParty_Panel.backEvent -= OnLocalPartyBack;
        }

        // Clear local party module.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        if (localPartyModule != null)
        {
            localPartyModule.Clear();
        }

        // Open waiting.

        OpenWaiting();

        // Disconnect.

        PhotonNetwork.Disconnect();
    }

    private void OnLocalPartyProceed()
    {
        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (m_LocalParty_Panel != null)
        {
            m_LocalParty_Panel.SetReceiveInput(false);

            // Unregister from events.

            m_LocalParty_Panel.proceedEvent -= OnLocalPartyProceed;
            m_LocalParty_Panel.backEvent -= OnLocalPartyBack;

            // Fill module and set player property.

            if (localPartyModule != null)
            {
                localPartyModule.Clear();

                for (int index = 0; index < m_LocalParty_Panel.slotCount; ++index)
                {
                    int playerId = m_LocalParty_Panel.GetPlayerId(index);

                    if (Hash.IsNullOrEmpty(playerId))
                        continue;

                    localPartyModule.AddPlayer(playerId);
                }
            }
        }

        // Set players properties.

        int localPartySize = (localPartyModule != null) ? localPartyModule.playersCount : 1;

        PhotonUtils.ClearPlayerCustomProperties();
        PhotonUtils.SetPlayerCustomProperty(PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, localPartySize);

        // Go to lobby or join room.

        if (m_Invite != null)
        {
            OpenWaiting();

            string args = m_Invite.args;
            m_Invite = null;

            PhotonNetwork.JoinRoom(args);
        }
        else
        {
            fsm.ChangeState(tnMultiplayerState.Lobby);
        }
    }

    private void On_LocalParty_PhotonJoinRoomFailedEvent(object[] i_CodeAndMsg)
    {
        CloseWaiting();

        Debug.Log("Room join failed.");
        fsm.ChangeState(tnMultiplayerState.Lobby);
    }

    private void On_LocalParty_JoinedRoomEvent()
    {
        CloseWaiting();

        Room room = PhotonNetwork.room;

        Debug.Log("Joined room..." + room.Name);
    }

    // Lobby

    private void Lobby_Enter()
    {
        Debug.Log("[Lobby] Enter");

        // Send PlayMaker event.

        PlayMakerFSM.BroadcastEvent("MENU FLOW / LOBBY");

        // Clear variables.

        m_Lobby_JoinRoomRequested = false;

        // Setup joined photon property.

        PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_Joined, false);

        // Setup ready photon property.

        PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, false);

        // Register photon callbacks.

        PhotonCallbacks.onJoinedRoomMain += On_Lobby_JoinedRoomEvent;
        PhotonCallbacks.onPhotonJoinRoomFailedMain += On_Lobby_PhotonJoinRoomFailedEvent;

        // Set local captain on input module.

        SetLocalCaptainOnInputModule();

        // Open panel.

        UIBasePanel[] group0Panels = (m_ShowOnlinePlayersPanel) ? new UIBasePanel[] { m_Lobby_Panel, m_OnlinePlayers_Panel } : new UIBasePanel[] { m_Lobby_Panel };
        SwitchPanels(UIGroup.Group0, group0Panels);
        SwitchPanels(UIGroup.Group6, m_Navbar_Panel);

        // Register on panel events.

        if (m_Lobby_Panel != null)
        {
            m_Lobby_Panel.backEvent += On_Lobby_BackEvent;
            m_Lobby_Panel.createRoomRequestedEvent += On_Lobby_CreateRoomRequestedEvent;
            m_Lobby_Panel.joinRoomRequestedEvent += On_Lobby_JoinRequestedEvent;
        }
    }

    private void Lobby_Update()
    {
        if (m_Lobby_JoinRoomRequested)
            return;

    }

    private void Lobby_Exit()
    {
        Debug.Log("[Lobby] Exit");

        // Unregister photon callbacks.

        PhotonCallbacks.onJoinedRoomMain -= On_Lobby_JoinedRoomEvent;
        PhotonCallbacks.onPhotonJoinRoomFailedMain -= On_Lobby_PhotonJoinRoomFailedEvent;

        // Unregister from panel events.

        if (m_Lobby_Panel != null)
        {
            m_Lobby_Panel.backEvent -= On_Lobby_BackEvent;
            m_Lobby_Panel.createRoomRequestedEvent -= On_Lobby_CreateRoomRequestedEvent;
            m_Lobby_Panel.joinRoomRequestedEvent -= On_Lobby_JoinRequestedEvent;
        }
    }

    private void On_Lobby_BackEvent()
    {
        if (m_Lobby_JoinRoomRequested)
            return;

        fsm.ChangeState(tnMultiplayerState.LocalParty);
    }

    private void On_Lobby_CreateRoomRequestedEvent()
    {
        fsm.ChangeState(tnMultiplayerState.CreatingRoom);
    }

    private void On_Lobby_JoinRequestedEvent(string i_RoomName)
    {
        if (m_Lobby_JoinRoomRequested)
            return;

        if (i_RoomName == "")
            return;

        m_Lobby_JoinRoomRequested = true;

        // Close panel on group 0 (Lobby).

        ClearGroup(UIGroup.Group0);

        // Open waiting.

        OpenWaiting();

        // Join room.

        PhotonNetwork.JoinRoom(i_RoomName);
    }

    private void On_Lobby_JoinedRoomEvent()
    {
        Debug.Log("Joined room.");

        // Proceed.

        fsm.ChangeState(tnMultiplayerState.TryToJoin);
    }

    private void On_Lobby_PhotonJoinRoomFailedEvent(object[] i_CodeAndMsg)
    {
        // Close waiting.

        CloseWaiting();

        // Show dialog.

        ShowDialog("OUCH", "Cannot join room.", OnRoomJoinFailedDialogCallback);
    }

    private void OnRoomJoinFailedDialogCallback()
    {
        m_Lobby_JoinRoomRequested = false;

        SwitchPanels(UIGroup.Group0, m_Lobby_Panel);
    }

    // Creating room

    private void CreatingRoom_Enter()
    {
        Debug.Log("[CreatingRoom] Enter");

        // Send PlayMaker event.

        PlayMakerFSM.BroadcastEvent("MENU FLOW / CREATING ROOM");

        // Clear variables.

        m_CreatingRoom_ProceedRequested = false;

        // Set local captain on input module.

        SetLocalCaptainOnInputModule();

        // Register photon callbacks.

        PhotonCallbacks.onCreatedRoomMain += On_CreatingRoom_CreatedRoomEvent;
        PhotonCallbacks.onJoinedRoomMain += On_CreatingRoom_JoinedRoomEvent;
        PhotonCallbacks.onPhotonCreateRoomFailedMain += On_CreatingRoom_PhotonCreateRoomFailedEvent;
        PhotonCallbacks.onPhotonJoinRoomFailedMain += On_CreatingRoom_PhotonJoinRoomFailedEvent;

        // Open panels.

        SwitchPanels(UIGroup.Group0, m_CreateRoom_Panel);
        SwitchPanels(UIGroup.Group6, m_Navbar_Panel);

        // Register on panel events.

        if (m_CreateRoom_Panel != null)
        {
            m_CreateRoom_Panel.proceedEvent += On_CreatingRoom_ProceedEvent;
            m_CreateRoom_Panel.backEvent += On_CreatingRoom_BackEvent;
        }
    }

    private void CreatingRoom_Update()
    {
        if (m_CreatingRoom_ProceedRequested)
            return;
    }

    private void CreatingRoom_Exit()
    {
        Debug.Log("[CreatingRoom] Exit");

        // Unregister photon callbacks.

        PhotonCallbacks.onCreatedRoomMain -= On_CreatingRoom_CreatedRoomEvent;
        PhotonCallbacks.onJoinedRoomMain -= On_CreatingRoom_JoinedRoomEvent;
        PhotonCallbacks.onPhotonCreateRoomFailedMain -= On_CreatingRoom_PhotonCreateRoomFailedEvent;
        PhotonCallbacks.onPhotonJoinRoomFailedMain -= On_CreatingRoom_PhotonJoinRoomFailedEvent;

        // Unregister from panel events.

        if (m_CreateRoom_Panel != null)
        {
            m_CreateRoom_Panel.proceedEvent -= On_CreatingRoom_ProceedEvent;
            m_CreateRoom_Panel.backEvent -= On_CreatingRoom_BackEvent;
        }
    }

    private void On_CreatingRoom_ProceedEvent()
    {
        if (m_CreatingRoom_ProceedRequested)
            return;

        if (m_CreateRoom_Panel == null)
            return;

        m_CreatingRoom_ProceedRequested = true;

        // Close create room panel.

        ClearGroup(UIGroup.Group0);

        // Open waiting.

        OpenWaiting();

        // Get selected properties.

        int selectedGameModeId = m_CreateRoom_Panel.selectedGameModeId;
        int selectedMatchDurationOptionId = m_CreateRoom_Panel.selectedMatchDurationOptionId;
        int selectedGoldenGoalOptionId = m_CreateRoom_Panel.selectedGoldenGoalOptionId;
        int selectedRefereeOptionId = m_CreateRoom_Panel.selectedRefereeOptionId;
        int selectedBallId = m_CreateRoom_Panel.selectedBallId;
        int selectedStadiumId = m_CreateRoom_Panel.selectedStadiumId;
        int selectedMaxPlayers = m_CreateRoom_Panel.selectedMaxPlayers;

        // Get room size.

        //tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(selectedStadiumId);
        int roomSize = selectedMaxPlayers; // (stadiumData != null) ? stadiumData.onlineTeamSize.max * 2 : 4;

        // Get local party size.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        int localPartySize = (localPartyModule != null) ? localPartyModule.playersCount : 1;

        // Create room properties.

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = Convert.ToByte(roomSize);

        roomOptions.CustomRoomPropertiesForLobby = new string[9];
        roomOptions.CustomRoomPropertiesForLobby[0] = PhotonPropertyKey.s_RoomCustomPropertyKey_GameMode;
        roomOptions.CustomRoomPropertiesForLobby[1] = PhotonPropertyKey.s_RoomCustomPropertyKey_MatchDuration;
        roomOptions.CustomRoomPropertiesForLobby[2] = PhotonPropertyKey.s_RoomCustomPropertyKey_GoldenGoal;
        roomOptions.CustomRoomPropertiesForLobby[3] = PhotonPropertyKey.s_RoomCustomPropertyKey_Referee;
        roomOptions.CustomRoomPropertiesForLobby[4] = PhotonPropertyKey.s_RoomCustomPropertyKey_Ball;
        roomOptions.CustomRoomPropertiesForLobby[5] = PhotonPropertyKey.s_RoomCustomPropertyKey_Stadium;
        roomOptions.CustomRoomPropertiesForLobby[6] = PhotonPropertyKey.s_RoomCustomPropertyKey_HostName;
        roomOptions.CustomRoomPropertiesForLobby[7] = PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerCount;
        roomOptions.CustomRoomPropertiesForLobby[8] = PhotonPropertyKey.s_RoomCustomPropertyKey_AvgPing;

        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_GameMode, selectedGameModeId);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_MatchDuration, selectedMatchDurationOptionId);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_GoldenGoal, selectedGoldenGoalOptionId);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_Referee, selectedRefereeOptionId);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_Ball, selectedBallId);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_Stadium, selectedStadiumId);

        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_MatchStatus, "Waiting");
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_HostName, PhotonNetwork.playerName);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerCount, localPartySize);

        // Index assignment table.

        tnMultiplayerIndexTable assignedIndices = new tnMultiplayerIndexTable(roomSize);
        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, assignedIndices);

        // Current sides.

        int[] sides = new int[roomSize];
        for (int index = 0; index < sides.Length; ++index)
        {
            sides[index] = 0;
        }

        roomOptions.CustomRoomProperties.Add(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, sides);

        // Create room.

        PhotonNetwork.CreateRoom("", roomOptions, TypedLobby.Default);
    }

    private void On_CreatingRoom_BackEvent()
    {
        if (m_CreatingRoom_ProceedRequested)
            return;

        fsm.ChangeState(tnMultiplayerState.Lobby);
    }

    private void On_CreatingRoom_CreatedRoomEvent()
    {
        Debug.Log("Room created.");
    }

    private void On_CreatingRoom_JoinedRoomEvent()
    {
        Debug.Log("Joined room.");

        // Set mine assigned indices. I'm sure that I'm Master Client.

        Debug.Assert(PhotonNetwork.isMasterClient);

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        int localPartySize = (localPartyModule != null) ? localPartyModule.playersCount : 1;

        int photonPlayerId = PhotonNetwork.player.ID;
        tnMultiplayerIndexTable indexTable;
        if (PhotonUtils.TryGetRoomCustomProperty<tnMultiplayerIndexTable>(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTable))
        {
            // Get first #localPartySize indices.

            List<int> indices = new List<int>();
            for (int index = 0; index < localPartySize; ++index)
            {
                indices.Add(index);
            }

            // Set indices on local party module.

            if (localPartyModule != null)
            {
                for (int index = 0; index < localPartyModule.playersCount; ++index)
                {
                    localPartyModule.SetOnlinePlayerIndexByIndex(index, -1);

                    if (index < indices.Count)
                    {
                        int onlinePlayerIndex = indices[index];
                        localPartyModule.SetOnlinePlayerIndexByIndex(index, onlinePlayerIndex);
                    }
                }
            }

            // Set indices on custom room properties.

            indexTable.AssignIndicesTo(photonPlayerId, indices);
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, indexTable);
        }

        // Proceed.

        fsm.ChangeState(tnMultiplayerState.TryToJoin);
    }

    private void On_CreatingRoom_PhotonCreateRoomFailedEvent(object[] i_CodeAndMsg)
    {
        On_CreatingRoom_PhotonJoinRoomFailedEvent(i_CodeAndMsg);
    }

    private void On_CreatingRoom_PhotonJoinRoomFailedEvent(object[] i_CodeAndMsg)
    {
        // Close waiting.

        CloseWaiting();

        // Show dialog.

        ShowDialog("OUCH", "Error creating room.", OnRoomCreationFailedDialogCallback);
    }

    private void OnRoomCreationFailedDialogCallback()
    {
        fsm.ChangeState(tnMultiplayerState.Lobby);
    }

    // Try to join

    private void TryToJoin_Enter()
    {
        Debug.Log("[TryToJoin] Enter");

        // Register on photon callbacks.

        PhotonCallbacks.onLeftRoomMain += On_TryToJoin_LeftRoomEvent;

        // Get local party size.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        int localPartySize = (localPartyModule != null) ? localPartyModule.playersCount : 1;

        // Get room players count.

        int otherPlayersCount = 0;

        PhotonPlayer[] photonPlayers = PhotonNetwork.otherPlayers;
        if (photonPlayers != null)
        {
            for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
            {
                PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];

                if (photonPlayer == null)
                    continue;

                int localPlayersCount;
                if (PhotonUtils.TryGetPlayerCustomProperty<int>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, out localPlayersCount))
                {
                    otherPlayersCount += localPlayersCount;
                }
            }
        }

        // Check if I can join room.

        Room room = PhotonNetwork.room;
        bool canJoin = (room != null) ? (otherPlayersCount + localPartySize <= room.MaxPlayers) : false;

        string status;
        PhotonUtils.TryGetRoomCustomProperty<string>(PhotonPropertyKey.s_RoomCustomPropertyKey_MatchStatus, out status);
        canJoin &= (status == "Waiting");

        // Join room if you can.

        if (canJoin)
        {
            // Close waiting.

            CloseWaiting();

            // Go to online party. 

            fsm.ChangeState(tnMultiplayerState.SideSelection);
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    private void TryToJoin_Update()
    {

    }

    private void TryToJoin_Exit()
    {
        Debug.Log("[TryToJoin] Exit");

        // Register on photon callbacks.

        PhotonCallbacks.onLeftRoomMain -= On_TryToJoin_LeftRoomEvent;
    }

    private void On_TryToJoin_LeftRoomEvent()
    {
        // Close waiting.

        CloseWaiting();

        // Show dialog.

        ShowDialog("OUCH", "Room is full.", On_TryToJoin_JoinFailedDialogCallback);
    }

    private void On_TryToJoin_JoinFailedDialogCallback()
    {
        // Open waiting.

        OpenWaiting();

        // Go to connection.

        ChangeState(tnMultiplayerState.Connection);
    }

    // Side selection

    private IEnumerator SideSelection_Enter()
    {
        Debug.Log("[SideSelection] Enter");

        // Write Match settings module.

        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
        if (matchSettingsModule != null)
        {
            matchSettingsModule.Clear();

            int gameModeId;
            PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_GameMode, out gameModeId);

            int matchDurationOptionId;
            PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_MatchDuration, out matchDurationOptionId);

            int goldenGoalOptionId;
            PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_GoldenGoal, out goldenGoalOptionId);

            int refereeOptonId;
            PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_Referee, out refereeOptonId);

            int ballId;
            PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_Ball, out ballId);

            int stadiumId;
            PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_Stadium, out stadiumId);

            matchSettingsModule.SetGameModeId(gameModeId);
            matchSettingsModule.SetMatchDurationOption(matchDurationOptionId);
            matchSettingsModule.SetGoldenGoalOption(goldenGoalOptionId);
            matchSettingsModule.SetRefereeOption(refereeOptonId);
            matchSettingsModule.SetBallId(ballId);
            matchSettingsModule.SetStadiumId(stadiumId);
        }

        // Send PlayMaker event.

        PlayMakerFSM.BroadcastEvent("MENU FLOW / ONLINE SIDE SELECTION");

        // Clear variables.

        m_PlayerDisconnectedDuringMatchSetup = false;
        m_SideSelection_ProceedRequested = false;
        m_SideSelection_BackRequested = false;

        // Setup input module.

        SetLocalCaptainOnInputModule();

        // Clear panel.

        if (m_SideSelection_Panel != null)
        {
            m_SideSelection_Panel.ClearDevices();
        }

        // Set photon player property.

        PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_Joined, true);

        // Register on photon callbacks.

        PhotonCallbacks.onPhotonPlayerConnectedMain += On_SideSelection_PhotonPlayerConnectedEvent;
        PhotonCallbacks.onPhotonPlayerDisconnectedMain += On_SideSelection_PhotonPlayerDisconnectedEvent;
        PhotonCallbacks.onMasterClientSwitchedMain += On_SideSelection_MasterClientSwitchedEvent;
        PhotonCallbacks.onPhotonCustomRoomPropertiesChangedMain += On_SideSelection_PhotonCustomRoomPropertiesChangedEvent;
        PhotonCallbacks.onPhotonPlayerPropertiesChangedMain += On_SideSelection_PhotonPlayerCustomPropertiesChangedEvent;
        PhotonCallbacks.onLeftRoomMain += On_SideSelection_LeftRoomEvent;

        // Open panels.

        SwitchPanels(UIGroup.Group6, m_Navbar_Panel);

        bool switchCompleted = false;
        Action switchCallback = () => { switchCompleted = true; };
        SequentialSwitchPanels(UIGroup.Group0, m_SideSelection_Panel, m_MatchInfo_Panel, switchCallback);
        yield return new WaitUntil(() => (switchCompleted == true));

        // Refresh panel.

        Refresh_SideSelection_Panel(true);
    }

    private void SideSelection_Update()
    {
        if (m_SideSelection_BackRequested || m_SideSelection_ProceedRequested)
            return;

        bool isReady;
        PhotonUtils.TryGetPlayerCustomProperty<bool>(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, out isReady);

        // Read input.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        if (localPartyModule != null)
        {
            for (int localPlayerIndex = 0; localPlayerIndex < localPartyModule.playersCount; ++localPlayerIndex)
            {
                int localPlayerId = localPartyModule.GetPlayerId(localPlayerIndex);

                if (Hash.IsNullOrEmpty(localPlayerId))
                    continue;

                bool isLocalCaptain = (localPartyModule.captainId == localPlayerId);

                int onlinePlayerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(localPlayerIndex);

                if (onlinePlayerIndex < 0)
                    continue;

                bool left = tnInputUtils.GetNegativeButtonDown(localPlayerId, InputButtons.s_HorizontalLeft, WifiInputButtons.s_HorizontalLeft);
                bool right = tnInputUtils.GetPositiveButtonDown(localPlayerId, InputButtons.s_HorizontalRight, WifiInputButtons.s_HorizontalRight);
                bool confirm = tnInputUtils.GetButtonDown(localPlayerId, InputButtons.s_Submit, WifiInputButtons.s_Submit);
                bool cancel = tnInputUtils.GetButtonDown(localPlayerId, InputButtons.s_Cancel, WifiInputButtons.s_Cancel);
                bool start = tnInputUtils.GetButtonDown(localPlayerId, InputButtons.s_Start, WifiInputButtons.s_Start);

                if (isLocalCaptain)
                {
                    if (!isReady)
                    {
                        if (cancel)
                        {
                            if (!m_SideSelection_BackRequested && !m_SideSelection_ProceedRequested)
                            {
                                m_SideSelection_BackRequested = true;

                                SfxPlayer.PlayMain(m_SideSelection_Leave_Sfx);

                                OpenWaiting();
                                PhotonNetwork.LeaveRoom();
                                return;
                            }
                        }
                    }
                }

                if (!PhotonNetwork.isMasterClient && isLocalCaptain)
                {
                    if (isReady)
                    {
                        if (cancel)
                        {
                            PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, false);
                            return;
                        }
                    }
                    else
                    {
                        if (confirm)
                        {
                            bool canConfirm = true;

                            int[] sides;
                            PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides);
                            if (sides != null)
                            {
                                for (int index = 0; index < localPartyModule.playersCount; ++index)
                                {
                                    int playerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(index);

                                    if (playerIndex < 0 || playerIndex >= sides.Length)
                                        continue;

                                    int side = sides[playerIndex];
                                    canConfirm &= (side != 0);
                                }
                            }

                            if (canConfirm)
                            {
                                PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, true);
                                return;
                            }
                        }
                    }
                }

                if (!isReady)
                {
                    if (left != right)
                    {
                        int direction = (left) ? -1 : 1;
                        m_PhotonView.RPC("RPC_SideSelection_Move", PhotonTargets.MasterClient, onlinePlayerIndex, direction);
                    }
                }

                if (PhotonNetwork.isMasterClient && isLocalCaptain)
                {
                    if (start)
                    {
                        // Check if all are ready.

                        bool allReady = true;
                        if (localPartyModule != null)
                        {
                            int[] sides;
                            PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides);
                            if (sides != null)
                            {
                                for (int index = 0; index < localPartyModule.playersCount; ++index)
                                {
                                    int playerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(index);

                                    if (playerIndex < 0 || playerIndex >= sides.Length)
                                        continue;

                                    int side = sides[playerIndex];
                                    allReady &= (side != 0);
                                }
                            }
                        }

                        PhotonPlayer[] photonPlayers = PhotonNetwork.otherPlayers;
                        if (photonPlayers != null)
                        {
                            if (photonPlayers.Length > 0)
                            {
                                for (int index = 0; index < photonPlayers.Length; ++index)
                                {
                                    PhotonPlayer photonPlayer = photonPlayers[index];

                                    if (photonPlayer == null)
                                        continue;

                                    bool joined;
                                    PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_Joined, out joined);

                                    if (joined)
                                    {
                                        bool ready;
                                        PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, out ready);
                                        allReady &= ready;
                                    }
                                }
                            }
                            else
                            {
                                allReady = false;
                            }
                        }

                        // Check if can proceed (team size).

                        bool canProceed = true;

                        int leftTeamSize, centerTeamSize, rightTeamSize;
                        EvaluateSides(out leftTeamSize, out centerTeamSize, out rightTeamSize);
                        int differenceLeftRight = Mathf.Abs(leftTeamSize - rightTeamSize);

                        canProceed &= (centerTeamSize == 0 && (differenceLeftRight <= 1));

                        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
                        if (matchSettingsModule != null)
                        {
                            int stadiumId = matchSettingsModule.stadiumId;
                            tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);
                            if (stadiumData != null)
                            {
                                IntRange teamSizeRange = stadiumData.onlineTeamSize;
                                bool leftValid = teamSizeRange.IsValueValid(leftTeamSize);
                                bool rightValid = teamSizeRange.IsValueValid(rightTeamSize);

                                canProceed &= (leftValid && rightValid);
                            }
                        }

                        if (allReady && canProceed)
                        {
                            if (!m_SideSelection_ProceedRequested && !m_SideSelection_BackRequested)
                            {
                                OpenWaiting();

                                m_SideSelection_ProceedRequested = true;
                                m_PhotonView.RPC("RPC_SideSelection_Proceed", PhotonTargets.AllViaServer, null);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SideSelection_Exit()
    {
        Debug.Log("[SideSelection] Exit");

        // Unregister photon callbacks.

        PhotonCallbacks.onPhotonPlayerConnectedMain -= On_SideSelection_PhotonPlayerConnectedEvent;
        PhotonCallbacks.onPhotonPlayerDisconnectedMain -= On_SideSelection_PhotonPlayerDisconnectedEvent;
        PhotonCallbacks.onMasterClientSwitchedMain -= On_SideSelection_MasterClientSwitchedEvent;
        PhotonCallbacks.onPhotonCustomRoomPropertiesChangedMain -= On_SideSelection_PhotonCustomRoomPropertiesChangedEvent;
        PhotonCallbacks.onPhotonPlayerPropertiesChangedMain -= On_SideSelection_PhotonPlayerCustomPropertiesChangedEvent;
        PhotonCallbacks.onLeftRoomMain -= On_SideSelection_LeftRoomEvent;
    }

    private void On_SideSelection_PhotonPlayerConnectedEvent(PhotonPlayer i_Player)
    {
        // Nothing to do here. We are waiting for join confirmation.
    }

    private void On_SideSelection_PhotonPlayerDisconnectedEvent(PhotonPlayer i_Player)
    {
        // NOTE: Here I have to refresh the entire table. If master client is disconnected before room properties updated are sent.

        if (PhotonNetwork.isMasterClient)
        {
            PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;

            // Check if player is joined.

            tnMultiplayerIndexTable indexTable;
            if (PhotonUtils.TryGetRoomCustomProperty<tnMultiplayerIndexTable>(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTable))
            {
                if (indexTable != null)
                {
                    tnMultiplayerIndexTable newIndexTable = new tnMultiplayerIndexTable(indexTable.maxPlayers);

                    // Evaluate indices.

                    if (photonPlayers != null)
                    {
                        for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
                        {
                            PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];

                            if (photonPlayer == null)
                                continue;

                            int photonPlayerId = photonPlayer.ID;

                            List<int> playerIndices = GetAssignedIndicesFor(photonPlayerId, indexTable);
                            if (playerIndices.Count > 0)
                            {
                                AssignIndicesTo(photonPlayerId, playerIndices, newIndexTable);
                            }
                        }
                    }

                    // Clear sides.

                    int[] sides;
                    if (PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides))
                    {
                        if (sides != null)
                        {
                            for (int playerIndex = 0; playerIndex < sides.Length; ++playerIndex)
                            {
                                int indexOwnerId;
                                if (!IsIndexAssigned(playerIndex, out indexOwnerId, newIndexTable))
                                {
                                    sides[playerIndex] = 0;
                                }
                            }
                        }
                    }

                    PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, newIndexTable);
                    PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, sides);
                }
            }

            // Update room player count.

            int playerCount = 0;

            if (photonPlayers != null)
            {
                for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
                {
                    PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];

                    if (photonPlayer == null)
                        continue;

                    int localPartySize;
                    PhotonUtils.TryGetPlayerCustomProperty<int>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, out localPartySize);

                    playerCount += localPartySize;
                }
            }

            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerCount, playerCount);
        }

        // Refresh panel.

        Refresh_SideSelection_Panel();
    }

    private void On_SideSelection_MasterClientSwitchedEvent(PhotonPlayer i_NewMasterClient)
    {
        // Update room host name.

        if (PhotonNetwork.isMasterClient)
        {
            PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, false);
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_HostName, PhotonNetwork.playerName);
        }

        // Refresh panel.

        Refresh_SideSelection_Panel();
    }

    private void On_SideSelection_PhotonCustomRoomPropertiesChangedEvent(Hashtable i_Properties)
    {
        // Update assigned indices in local party module.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        if (localPartyModule != null)
        {
            object indexTableObject;
            if (i_Properties.TryGetValue(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTableObject))
            {
                tnMultiplayerIndexTable indexTable = (tnMultiplayerIndexTable)indexTableObject;
                if (indexTable != null)
                {
                    int myPhotonPlayerId = PhotonNetwork.player.ID;
                    List<int> assignedIndices = GetAssignedIndicesFor(myPhotonPlayerId, indexTable);
                    for (int index = 0; index < localPartyModule.playersCount; ++index)
                    {
                        localPartyModule.SetOnlinePlayerIndexByIndex(index, -1);

                        if (index < assignedIndices.Count)
                        {
                            int onlinePlayerIndex = assignedIndices[index];
                            localPartyModule.SetOnlinePlayerIndexByIndex(index, onlinePlayerIndex);
                        }
                    }
                }
            }

            // Security check. (Anyone can't be ready and be in the center state).

            object sidesObject;
            if (i_Properties.TryGetValue(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sidesObject))
            {
                int[] sides = (int[])sidesObject;
                if (sides != null)
                {
                    for (int index = 0; index < sides.Length; ++index)
                    {
                        int side = sides[index];

                        if (side == 0)
                        {
                            bool isLocalPlayer = tnGameModulesUtils.IsLocalPlayer(index);
                            if (isLocalPlayer)
                            {
                                bool ready;
                                PhotonUtils.TryGetPlayerCustomProperty<bool>(PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, out ready);

                                if (ready)
                                {
                                    PhotonUtils.SetPlayerCustomProperty(PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Refresh panel.

        Refresh_SideSelection_Panel();
    }

    private void On_SideSelection_PhotonPlayerCustomPropertiesChangedEvent(PhotonPlayer i_Player, Hashtable i_Properties)
    {
        if (PhotonNetwork.isMasterClient)
        {
            // Check if player has joined.

            object joinedObject;
            if (i_Properties.TryGetValue(PhotonPropertyKey.s_PlayerCustomPropertyKey_Joined, out joinedObject))
            {
                bool joined = (bool)joinedObject;
                if (joined)
                {
                    int localPartySize;
                    if (PhotonUtils.TryGetPlayerCustomProperty<int>(i_Player, PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, out localPartySize))
                    {
                        tnMultiplayerIndexTable indexTable;
                        if (PhotonUtils.TryGetRoomCustomProperty<tnMultiplayerIndexTable>(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTable))
                        {
                            // Get Available indices.

                            List<int> availableIndices = GetAvailableIndices(indexTable);

                            // If there are enough available indices, then assign it to new player.

                            if (availableIndices.Count >= localPartySize)
                            {
                                // Assign indices.

                                List<int> playerIndices = new List<int>(availableIndices.GetRange(0, localPartySize));
                                AssignIndicesTo(i_Player.ID, playerIndices, indexTable);
                                PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, indexTable);

                                // Update player count room property.

                                int currentPlayerCount;
                                PhotonUtils.TryGetRoomCustomProperty<int>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerCount, out currentPlayerCount);

                                int playerCount = currentPlayerCount + localPartySize;
                                PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerCount, playerCount);
                            }
                            else
                            {
                                // There are no enough available indices.

                                m_PhotonView.RPC("RPC_KickPlayer", i_Player, null);

                                Debug.Assert(false, "Player has joined but there is no available indices.");
                            }
                        }
                    }
                }
            }
        }

        // Check if ready state is changed.

        if (i_Properties.ContainsKey(PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready))
        {
            bool ready = (bool)i_Properties[PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready];

            if (m_SideSelection_Panel != null)
            {
                m_SideSelection_Panel.NotifyPlayerReady(ready);
            }

            Refresh_SideSelection_Panel();
        }
    }

    private void On_SideSelection_LeftRoomEvent()
    {
        // Change state.

        ChangeState(tnMultiplayerState.Connection);
    }

    private void Refresh_SideSelection_Panel(bool i_SetDeviceSideImmediatly = false)
    {
        if (m_SideSelection_Panel == null)
            return;

        Room room = PhotonNetwork.room;

        if (room == null)
        {
            m_SideSelection_Panel.ClearDevices();
            return;
        }

        int maxPlayers = room.MaxPlayers;

        tnMultiplayerIndexTable indexTable;
        if (PhotonUtils.TryGetRoomCustomProperty<tnMultiplayerIndexTable>(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTable))
        {
            for (int playerIndex = 0; playerIndex < maxPlayers; ++playerIndex)
            {
                int indexOwnerId = GetIndexOwnerId(playerIndex, indexTable);

                if (indexOwnerId != -1)
                {
                    tnOnlinePlayerData onlinePlayerData = tnGameData.GetOnlinePlayerDataByIndexMain(playerIndex);
                    Color playerColor = (onlinePlayerData != null) ? onlinePlayerData.color : Color.white;

                    PhotonPlayer photonPlayer = PhotonPlayer.Find(indexOwnerId);
                    if (photonPlayer != null)
                    {
                        int guestIndex = GetGuestIndex(playerIndex, indexTable);

                        string photonPlayerName = BuildPhotonPlayerName(photonPlayer, guestIndex);

                        m_SideSelection_Panel.SetDeviceColor(playerIndex, playerColor);
                        m_SideSelection_Panel.SetDeviceName(playerIndex, photonPlayerName);

                        bool ready;
                        PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, out ready);

                        m_SideSelection_Panel.SetDeviceReady(playerIndex, ready);
                        m_SideSelection_Panel.SetDeviceCaptain(playerIndex, photonPlayer.IsMasterClient);

                        int[] sides;
                        if (PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides))
                        {
                            int side = sides[playerIndex];
                            tnUI_MP_DeviceSide deviceSide = (tnUI_MP_DeviceSide)side;
                            m_SideSelection_Panel.SetDeviceSide(playerIndex, deviceSide, i_SetDeviceSideImmediatly);
                        }
                        else
                        {
                            m_SideSelection_Panel.SetDeviceSide(playerIndex, tnUI_MP_DeviceSide.Center, i_SetDeviceSideImmediatly);
                        }

                        m_SideSelection_Panel.SetDeviceState(playerIndex, tnUI_MP_DeviceState.Occupied);
                    }
                    else
                    {
                        m_SideSelection_Panel.SetDeviceSide(playerIndex, tnUI_MP_DeviceSide.Center, true);
                        m_SideSelection_Panel.SetDeviceState(playerIndex, tnUI_MP_DeviceState.Open);
                    }
                }
                else
                {
                    m_SideSelection_Panel.SetDeviceSide(playerIndex, tnUI_MP_DeviceSide.Center, true);
                    m_SideSelection_Panel.SetDeviceState(playerIndex, tnUI_MP_DeviceState.Open);
                }
            }
        }

        // Triggers.

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (PhotonNetwork.isMasterClient)
        {
            // Check if all are ready.

            bool allReady = true;
            if (localPartyModule != null)
            {
                int[] sides;
                PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides);
                if (sides != null)
                {
                    for (int index = 0; index < localPartyModule.playersCount; ++index)
                    {
                        int playerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(index);

                        if (playerIndex < 0 || playerIndex >= sides.Length)
                            continue;

                        int side = sides[playerIndex];
                        allReady &= (side != 0);
                    }
                }
            }

            PhotonPlayer[] photonPlayers = PhotonNetwork.otherPlayers;
            if (photonPlayers != null)
            {
                if (photonPlayers.Length > 0)
                {
                    for (int index = 0; index < photonPlayers.Length; ++index)
                    {
                        PhotonPlayer photonPlayer = photonPlayers[index];

                        if (photonPlayer == null)
                            continue;

                        bool joined;
                        PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_Joined, out joined);

                        if (joined)
                        {
                            bool ready;
                            PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, out ready);
                            allReady &= ready;
                        }
                    }
                }
                else
                {
                    allReady = false;
                }
            }

            // Check if can proceed (team size).

            bool canProceed = true;

            int leftTeamSize, centerTeamSize, rightTeamSize;
            EvaluateSides(out leftTeamSize, out centerTeamSize, out rightTeamSize);
            int differenceLeftRight = Mathf.Abs(leftTeamSize - rightTeamSize);

            canProceed &= (centerTeamSize == 0 && (differenceLeftRight <= 1));

            tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
            if (matchSettingsModule != null)
            {
                int stadiumId = matchSettingsModule.stadiumId;
                tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);
                if (stadiumData != null)
                {
                    IntRange teamSizeRange = stadiumData.onlineTeamSize;
                    bool leftValid = teamSizeRange.IsValueValid(leftTeamSize);
                    bool rightValid = teamSizeRange.IsValueValid(rightTeamSize);

                    canProceed &= (leftValid && rightValid);
                }
            }

            // Start.

            m_SideSelection_Panel.SetStartTriggerActive(true);
            m_SideSelection_Panel.SetStartTriggerCanSend(allReady && canProceed);

            // Confirm.

            m_SideSelection_Panel.SetConfirmTriggerActive(false);
            m_SideSelection_Panel.SetConfirmTriggerCanSend(false);

            // Cancel.

            m_SideSelection_Panel.SetCancelTriggerActive(false);
            m_SideSelection_Panel.SetCancelTriggerCanSend(false);

            // Back.

            m_SideSelection_Panel.SetBackTriggerActive(true);
            m_SideSelection_Panel.SetBackTriggerCanSend(true);
        }
        else
        {

            bool ready;
            PhotonUtils.TryGetPlayerCustomProperty<bool>(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_SideSelection_Ready, out ready);

            bool canConfirm = true;
            if (localPartyModule != null)
            {
                int[] sides;
                PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides);
                if (sides != null)
                {
                    for (int index = 0; index < localPartyModule.playersCount; ++index)
                    {
                        int playerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(index);

                        if (playerIndex < 0 || playerIndex >= sides.Length)
                            continue;

                        int side = sides[playerIndex];
                        canConfirm &= (side != 0);
                    }
                }
            }

            // Start.

            m_SideSelection_Panel.SetStartTriggerActive(false);
            m_SideSelection_Panel.SetStartTriggerCanSend(false);

            // Confirm.

            m_SideSelection_Panel.SetConfirmTriggerActive(true);
            m_SideSelection_Panel.SetConfirmTriggerCanSend(!ready && canConfirm);

            // Cancel.

            m_SideSelection_Panel.SetCancelTriggerActive(ready);
            m_SideSelection_Panel.SetCancelTriggerCanSend(ready);

            // Back.

            m_SideSelection_Panel.SetBackTriggerActive(!ready);
            m_SideSelection_Panel.SetBackTriggerCanSend(!ready);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // tnMultiplayerIndex Table utils.

    private void ClearIndicesFor(int i_Id, tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        if (i_MultiplayerIndexTable == null)
            return;

        i_MultiplayerIndexTable.ClearIndicesFor(i_Id);
    }

    private List<int> GetAssignedIndicesFor(int i_Id, tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        if (i_MultiplayerIndexTable == null)
        {
            return new List<int>();
        }

        return i_MultiplayerIndexTable.GetAssignedIndicesFor(i_Id);
    }

    private List<int> GetAvailableIndices(tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        if (i_MultiplayerIndexTable == null)
        {
            return new List<int>();
        }

        return i_MultiplayerIndexTable.GetAvailableIndices();
    }

    private int GetIndexOwnerId(int i_PlayerIndex, tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        if (i_MultiplayerIndexTable == null)
        {
            return -1;
        }

        return i_MultiplayerIndexTable.GetIndexOwnerId(i_PlayerIndex);
    }

    private int GetGuestIndex(int i_PlayerIndex, tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        if (i_MultiplayerIndexTable == null)
        {
            return -1;
        }

        return i_MultiplayerIndexTable.GetGuestIndex(i_PlayerIndex);
    }

    private void AssignIndicesTo(int i_PlayerId, List<int> i_Indices, tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        if (i_MultiplayerIndexTable == null)
            return;

        i_MultiplayerIndexTable.AssignIndicesTo(i_PlayerId, i_Indices);
    }

    private bool IsIndexAssigned(int i_PlayerIndex, out int o_PlayerId, tnMultiplayerIndexTable i_MultiplayerIndexTable)
    {
        o_PlayerId = -1;

        if (i_MultiplayerIndexTable == null)
        {
            return false;
        }

        return i_MultiplayerIndexTable.IsIndexAssigned(i_PlayerIndex, out o_PlayerId);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Other utils.

    private string BuildPhotonPlayerName(PhotonPlayer i_Player, int i_GuestIndex)
    {
        if (i_Player == null)
        {
            return "";
        }

        string photonPlayerName = i_Player.NickName;
        if (i_GuestIndex > 0)
        {
            photonPlayerName += " (" + i_GuestIndex + ")";
        }

        return photonPlayerName;
    }

    private tnUI_MP_DeviceSide GetNextSide(tnUI_MP_DeviceSide i_Current, bool i_Left, bool i_Right)
    {
        if (i_Left == i_Right || (i_Left == false && i_Right == false))
        {
            return i_Current;
        }

        tnUI_MP_DeviceSide targetState = tnUI_MP_DeviceSide.Center;

        switch (i_Current)
        {
            case tnUI_MP_DeviceSide.Center:
                targetState = (i_Left) ? tnUI_MP_DeviceSide.Left : tnUI_MP_DeviceSide.Right;
                break;

            case tnUI_MP_DeviceSide.Left:
                targetState = (i_Left) ? tnUI_MP_DeviceSide.Left : tnUI_MP_DeviceSide.Center;
                break;

            case tnUI_MP_DeviceSide.Right:
                targetState = (i_Left) ? tnUI_MP_DeviceSide.Center : tnUI_MP_DeviceSide.Right;
                break;
        }

        return targetState;
    }

    private void EvaluateSides(out int o_Left, out int o_Center, out int o_Right)
    {
        int left = 0;
        int center = 0;
        int right = 0;

        tnMultiplayerIndexTable indexTable;
        if (PhotonUtils.TryGetRoomCustomProperty<tnMultiplayerIndexTable>(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTable))
        {
            int[] sides;
            if (PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides))
            {
                if (sides != null)
                {
                    PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
                    if (photonPlayers != null)
                    {
                        for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
                        {
                            PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];

                            if (photonPlayer == null)
                                continue;

                            int photonPlayerId = photonPlayer.ID;

                            List<int> indices = GetAssignedIndicesFor(photonPlayerId, indexTable);
                            for (int index = 0; index < indices.Count; ++index)
                            {
                                int playerIndex = indices[index];

                                if (playerIndex < 0 || playerIndex >= sides.Length)
                                    continue;

                                int side = sides[playerIndex];
                                tnUI_MP_DeviceSide deviceSide = (tnUI_MP_DeviceSide)side;

                                left += (deviceSide == tnUI_MP_DeviceSide.Left) ? 1 : 0;
                                center += (deviceSide == tnUI_MP_DeviceSide.Center) ? 1 : 0;
                                right += (deviceSide == tnUI_MP_DeviceSide.Right) ? 1 : 0;
                            }
                        }
                    }
                }
            }
        }

        o_Left = left;
        o_Center = center;
        o_Right = right;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [PunRPC]
    private void RPC_KickPlayer()
    {
        m_SideSelection_BackRequested = true;

        OpenWaiting();
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    private void RPC_SideSelection_Proceed()
    {
        if (m_SideSelection_BackRequested)
            return;

        SfxPlayer.PlayMain(m_SideSelection_Proceed_Sfx);

        m_SideSelection_ProceedRequested = true;

        if (PhotonNetwork.isMasterClient)
        {
            PhotonUtils.SetRoomOpened(false);
            PhotonUtils.SetRoomVisible(false);

            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_MatchStatus, "MatchSetup");
        }

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule != null)
        {
            teamsModule.Clear();

            int[] sides;
            if (PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides))
            {
                if (sides != null)
                {
                    tnTeamDescription team0 = new tnTeamDescription();
                    tnTeamDescription team1 = new tnTeamDescription();

                    for (int index = 0; index < sides.Length; ++index)
                    {
                        int side = sides[index];
                        if (side != 0)
                        {
                            tnCharacterDescription characterDescription = new tnCharacterDescription();
                            characterDescription.SetOnlinePlayerIndex(index);

                            tnTeamDescription team = (side == 1) ? team1 : team0;
                            team.AddCharacterDescription(characterDescription);
                        }
                    }

                    teamsModule.AddTeamDescription(team0);
                    teamsModule.AddTeamDescription(team1);
                }
            }
        }

        ChangeState(tnMultiplayerState.TeamSelection);
    }

    [PunRPC]
    private void RPC_SideSelection_Move(int i_PlayerIndex, int i_Direction)
    {
        if (m_SideSelection_ProceedRequested)
            return;

        if (PhotonNetwork.isMasterClient)
        {
            int[] sides;
            if (PhotonUtils.TryGetRoomCustomProperty<int[]>(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, out sides))
            {
                if (sides != null)
                {
                    if (i_PlayerIndex >= 0 && i_PlayerIndex < sides.Length)
                    {
                        int newSide = Mathf.Clamp(sides[i_PlayerIndex] + i_Direction, -1, 1);
                        sides[i_PlayerIndex] = newSide;

                        PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerSides, sides);
                    }
                }
            }
        }
    }

    // TeamSelection

    private IEnumerator TeamSelection_Enter()
    {
        Debug.Log("[TeamSelection] Enter");

        // Send PlayMaker event.

        PlayMakerFSM.BroadcastEvent("MENU FLOW / TEAM SELECTION");

        // Close waiting.

        CloseWaiting();

        // Clear Input module.

        ClearInputModule();

        // Clear variables.

        m_TeamSelection_ProceedRequested = false;
        m_TeamSelection_PlayersReady = 0;

        m_TeamSelectionTimeSynced = false;
        m_TeamSelectionTimePropertyAlreadySet = false;

        // Register photon callbacks.

        PhotonCallbacks.onPhotonPlayerDisconnectedMain += On_TeamSelection_PhotonPlayerDisconnectedEvent;

        // Open panels.

        bool switchComplete = false;
        Action switchCallback = () => { switchComplete = true; };
        SequentialSwitchPanels(UIGroup.Group0, m_TeamSelection_Panel, switchCallback);
        yield return new WaitUntil(() => (switchComplete == true));

        // Setup panel.

        if (m_TeamSelection_Panel != null)
        {
            // Clear all.

            m_TeamSelection_Panel.ClearAll();

            // Setup timer.

            m_TeamSelection_Panel.SetTimer(m_TeamSelectionTime);

            // Setup teams.

            List<int> teamKeys = tnGameData.GetTeamsKeysMain();
            if (teamKeys != null)
            {
                for (int teamKeyIndex = 0; teamKeyIndex < teamKeys.Count; ++teamKeyIndex)
                {
                    int teamKey = teamKeys[teamKeyIndex];

                    if (Hash.IsNullOrEmpty(teamKey))
                        continue;

                    m_TeamSelection_Panel.AddTeam(teamKey);
                }
            }

            // Setup colors.

            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
            if (teamsModule != null)
            {
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                    if (teamDescription == null)
                        continue;

                    tnCharacterDescription captain = teamDescription.captain;

                    if (captain == null)
                        continue;

                    int captainOnlinePlayerIndex = captain.onlinePlayerIndex;
                    List<int> onlinePlayersKeys = tnGameData.GetOnlinePlayersKeysMain();

                    if (onlinePlayersKeys == null)
                        continue;

                    if (captainOnlinePlayerIndex < 0 || captainOnlinePlayerIndex >= onlinePlayersKeys.Count)
                        continue;

                    int onlinePlayerKey = onlinePlayersKeys[captainOnlinePlayerIndex];
                    tnOnlinePlayerData onlinePlayerData = tnGameData.GetOnlinePlayerDataMain(onlinePlayerKey);
                    if (onlinePlayerData != null)
                    {
                        Color playerColor = onlinePlayerData.color;
                        m_TeamSelection_Panel.SetPlayerColor(teamIndex, playerColor);
                    }
                }
            }

            // Force initial selection.

            m_TeamSelection_Panel.ForceTeamSelection(0, 0);
            m_TeamSelection_Panel.ForceTeamSelection(1, 1);

            // Set triggers states.

            m_TeamSelection_Panel.SetConfirmTriggerCanSend(false);
        }

        // Notify I'm ready.

        m_PhotonView.RPC("RPC_TeamSelection_PlayerReady", PhotonTargets.AllViaServer, null);

        // Wait all.

        yield return new WaitUntil(() => (m_TeamSelection_PlayersReady == ((PhotonNetwork.room == null) ? 1 : PhotonNetwork.room.PlayerCount)));
    }

    private void TeamSelection_Update()
    {
        if (m_TeamSelection_ProceedRequested || m_PlayerDisconnectedDuringMatchSetup)
            return;

        // Sync time.

        if (!m_TeamSelectionTimeSynced)
        {
            UpdateTeamSelectionStartTime();
        }

        // Update timer.

        double time = m_TeamSelectionTime;

        double startTime;
        if (PhotonUtils.TryGetRoomCustomProperty<double>(PhotonPropertyKey.s_RoomCustomPropertyKey_TeamSelectionStartTime, out startTime))
        {
            m_TeamSelectionTimeSynced = true;
            double elapsedTime = PhotonNetwork.time - startTime;
            time = m_TeamSelectionTime - elapsedTime;
        }

        time = Math.Max(time, 0.0);

        if (m_TeamSelection_Panel != null)
        {
            m_TeamSelection_Panel.SetTimer(time);
        }

        // Read input.

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (teamsModule != null && localPartyModule != null)
        {
            for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
            {
                bool alreadyConfirmed = false;
                if (m_TeamSelection_Panel != null)
                {
                    alreadyConfirmed = m_TeamSelection_Panel.GetTeamConfirm(teamIndex);
                }

                if (m_TeamSelection_Panel != null)
                {
                    m_TeamSelection_Panel.SetConfirmTriggerCanSend(!alreadyConfirmed);
                }

                if (alreadyConfirmed)
                    continue;

                tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                if (teamDescription == null)
                    continue;

                tnCharacterDescription captain = teamDescription.captain;

                if (captain == null)
                    continue;

                int captainOnlinePlayerIndex = captain.onlinePlayerIndex;
                int playerId = localPartyModule.GetPlayerIdByOnlineIndex(captainOnlinePlayerIndex);

                if (Hash.IsValid(playerId))
                {
                    bool left = tnInputUtils.GetNegativeButtonDown(playerId, InputButtons.s_HorizontalLeft, WifiInputButtons.s_HorizontalLeft);
                    bool right = tnInputUtils.GetPositiveButtonDown(playerId, InputButtons.s_HorizontalRight, WifiInputButtons.s_HorizontalRight);
                    bool down = tnInputUtils.GetNegativeButtonDown(playerId, InputButtons.s_VerticalDown, WifiInputButtons.s_VerticalDown);
                    bool up = tnInputUtils.GetPositiveButtonDown(playerId, InputButtons.s_VerticalUp, WifiInputButtons.s_VerticalUp);

                    bool confirm = tnInputUtils.GetButtonDown(playerId, InputButtons.s_Submit, WifiInputButtons.s_Submit);
                    confirm |= (time <= 0.0);

                    if ((left || right) && !(left && right))
                    {
                        int directionIndex = (left) ? ((int)UINavigationDirection.Left) : ((int)UINavigationDirection.Right);
                        m_PhotonView.RPC("RPC_TeamSelection_Move", PhotonTargets.AllViaServer, teamIndex, directionIndex);
                    }

                    if ((down || up) && !(down && up))
                    {
                        int directionIndex = (down) ? ((int)UINavigationDirection.Down) : ((int)UINavigationDirection.Up);
                        m_PhotonView.RPC("RPC_TeamSelection_Move", PhotonTargets.AllViaServer, teamIndex, directionIndex);
                    }

                    if (confirm)
                    {
                        m_PhotonView.RPC("RPC_TeamSelection_Confirm", PhotonTargets.AllViaServer, teamIndex);
                    }
                }
            }
        }

        // Check if all teams has confirmed.

        if (PhotonNetwork.isMasterClient)
        {
            if (m_TeamSelection_Panel != null && teamsModule != null)
            {
                bool allConfirmed = true;
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    allConfirmed &= m_TeamSelection_Panel.GetTeamConfirm(teamIndex);
                }

                if (allConfirmed)
                {
                    OpenWaiting();

                    m_TeamSelection_ProceedRequested = true;

                    int[] teamIds = new int[teamsModule.teamsCount];
                    for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                    {
                        int selectedTeamId = m_TeamSelection_Panel.GetSelectedTeamId(teamIndex);
                        teamIds[teamIndex] = selectedTeamId;
                    }

                    m_PhotonView.RPC("RPC_TeamSelection_Proceed", PhotonTargets.AllViaServer, (int[])teamIds);
                }
            }
        }
    }

    private IEnumerator TeamSelection_Exit()
    {
        Debug.Log("[TeamSelection] Exit");

        // Unregister photon callbacks.

        PhotonCallbacks.onPhotonPlayerDisconnectedMain -= On_TeamSelection_PhotonPlayerDisconnectedEvent;

        // Wait for a small amount of time and proceed.

        yield return new WaitForSeconds(m_TeamSelectionProceedDelay);
    }

    private void UpdateTeamSelectionStartTime()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        if (PhotonNetwork.time < 0.0001f)
        {
            m_TeamSelectionTimeSynced = false;
            return;
        }

        if (!m_TeamSelectionTimePropertyAlreadySet)
        {
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_TeamSelectionStartTime, PhotonNetwork.time);
            m_TeamSelectionTimePropertyAlreadySet = true;
        }
    }

    private void On_TeamSelection_PhotonPlayerDisconnectedEvent(PhotonPlayer i_Player)
    {
        if (m_TeamSelection_Panel != null)
        {
            m_TeamSelection_Panel.SetConfirmTriggerCanSend(false);
        }
    }

    [PunRPC]
    private void RPC_TeamSelection_PlayerReady()
    {
        ++m_TeamSelection_PlayersReady;
    }

    [PunRPC]
    private void RPC_TeamSelection_Move(int i_TeamIndex, int i_DirectionIndex)
    {
        if (m_PlayerDisconnectedDuringMatchSetup || m_TeamSelection_ProceedRequested)
            return;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        if (i_TeamIndex < 0 || i_TeamIndex >= teamsModule.teamsCount)
            return;

        UINavigationDirection direction = (UINavigationDirection)i_DirectionIndex;

        if (m_TeamSelection_Panel != null)
        {
            m_TeamSelection_Panel.MoveSelection(i_TeamIndex, direction);
        }
    }

    [PunRPC]
    private void RPC_TeamSelection_Confirm(int i_TeamIndex)
    {
        if (m_PlayerDisconnectedDuringMatchSetup || m_TeamSelection_ProceedRequested)
            return;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        if (i_TeamIndex < 0 || i_TeamIndex >= teamsModule.teamsCount)
            return;

        if (m_TeamSelection_Panel != null)
        {
            m_TeamSelection_Panel.ConfirmTeam(i_TeamIndex);
        }
    }

    [PunRPC]
    private void RPC_TeamSelection_Proceed(int[] i_SelectedTeamsIds)
    {
        if (m_PlayerDisconnectedDuringMatchSetup)
            return;

        // Open waiting.

        OpenWaiting();

        // Fill module.

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule != null)
        {
            if (i_SelectedTeamsIds != null)
            {
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                    if (teamDescription == null)
                        continue;

                    if (teamIndex >= i_SelectedTeamsIds.Length)
                        continue;

                    int selectedTeamId = i_SelectedTeamsIds[teamIndex];
                    teamDescription.SetTeamId(selectedTeamId);
                }

                int[] teamIds = new int[teamsModule.teamsCount];
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                    if (teamDescription == null)
                        continue;

                    int teamId = teamDescription.teamId;
                    teamIds[teamIndex] = teamId;
                }

                Color[] colors = Utils.ComputeTeamColors(teamIds);
                if (colors != null)
                {
                    for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                    {
                        tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                        if (teamDescription == null)
                            continue;

                        if (teamIndex >= colors.Length)
                            continue;

                        Color teamColor = colors[teamIndex];
                        teamDescription.SetTeamColor(teamColor);
                    }
                }
            }
        }

        // Proceed

        ChangeState(tnMultiplayerState.CharacterSelection);
    }

    // Characters selection.

    private IEnumerator CharacterSelection_Enter()
    {
        Debug.Log("[CharacterSelection] Enter");

        // Send PlayMaker event.

        PlayMakerFSM.BroadcastEvent("MENU FLOW / CHARACTER SELECTION");

        // Close waiting.

        CloseWaiting();

        // Clear input module.

        ClearInputModule();

        // Clear variables.

        m_CharacterSelection_ProceedRequested = false;
        m_CharacterSelection_PlayerReady = 0;

        m_CharacterSelectionTimeSynced = false;
        m_CharacterSelectionTimePropertyAlreadySet = false;

        // Register photon callbacks.

        PhotonCallbacks.onPhotonPlayerDisconnectedMain += On_CharacterSelection_PhotonPlayerDisconnectedEvent;

        // Open panels.

        bool switchComplete = false;
        Action switchCallback = () => { switchComplete = true; };
        SequentialSwitchPanels(UIGroup.Group0, m_CharacterSelection_Panel, switchCallback);
        yield return new WaitUntil(() => (switchComplete == true));

        // Setup panel.

        if (m_CharacterSelection_Panel != null)
        {
            // Clear all.

            m_CharacterSelection_Panel.ClearAll();

            // Set timer.

            m_CharacterSelection_Panel.SetTimer(m_CharacterSelectionTime);

            // Setup.

            m_CharacterSelection_Panel.Setup();

            // Setup colors.

            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
            if (teamsModule != null)
            {
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                    if (teamDescription == null)
                        continue;

                    tnCharacterDescription captain = teamDescription.captain;

                    if (captain == null)
                        continue;

                    int captainOnlinePlayerIndex = captain.onlinePlayerIndex;
                    List<int> onlinePlayersKeys = tnGameData.GetOnlinePlayersKeysMain();

                    if (onlinePlayersKeys == null)
                        continue;

                    if (captainOnlinePlayerIndex < 0 || captainOnlinePlayerIndex >= onlinePlayersKeys.Count)
                        continue;

                    int onlinePlayerKey = onlinePlayersKeys[captainOnlinePlayerIndex];
                    tnOnlinePlayerData onlinePlayerData = tnGameData.GetOnlinePlayerDataMain(onlinePlayerKey);
                    if (onlinePlayerData != null)
                    {

                    }
                }
            }

            // Force initial selection.

            m_CharacterSelection_Panel.ForceSelection(0, 0);
            m_CharacterSelection_Panel.ForceSelection(1, 0);

            // Set trigger can send.

            m_CharacterSelection_Panel.SetProceedTriggerCanSend(false);
            m_CharacterSelection_Panel.SetBackTriggerCanSend(false);
        }

        // Notify I'm ready.

        m_PhotonView.RPC("RPC_CharacterSelection_PlayerReady", PhotonTargets.AllViaServer, null);

        // Wait all.

        yield return new WaitUntil(() => (m_CharacterSelection_PlayerReady == ((PhotonNetwork.room == null) ? 1 : PhotonNetwork.room.PlayerCount)));
    }

    private void CharacterSelection_Update()
    {
        if (m_CharacterSelection_ProceedRequested || m_PlayerDisconnectedDuringMatchSetup)
            return;

        // Sync timer.

        if (!m_CharacterSelectionTimeSynced)
        {
            UpdateCharacterSelectionStartTime();
        }

        // Update timer.

        double time = m_CharacterSelectionTime;

        double startTime;
        if (PhotonUtils.TryGetRoomCustomProperty<double>(PhotonPropertyKey.s_RoomCustomPropertyKey_CharacterSelectionStartTime, out startTime))
        {
            m_CharacterSelectionTimeSynced = true;
            double elapsedTime = PhotonNetwork.time - startTime;
            time = m_CharacterSelectionTime - elapsedTime;
        }

        time = Math.Max(time, 0.0);

        if (m_CharacterSelection_Panel != null)
        {
            m_CharacterSelection_Panel.SetTimer(time);
        }

        // Read input.

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (teamsModule != null && localPartyModule != null)
        {
            for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
            {
                tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                if (teamDescription == null)
                    continue;

                tnCharacterDescription captain = teamDescription.captain;

                if (captain == null)
                    continue;

                int captainOnlinePlayerIndex = captain.onlinePlayerIndex;
                int playerId = localPartyModule.GetPlayerIdByOnlineIndex(captainOnlinePlayerIndex);

                if (Hash.IsValid(playerId))
                {
                    bool alreadyConfirmed = (m_CharacterSelection_Panel != null) ? m_CharacterSelection_Panel.GetTeamReady(teamIndex) : false;
                    if (alreadyConfirmed)
                    {
                        if (m_CharacterSelection_Panel != null)
                        {
                            m_CharacterSelection_Panel.SetProceedTriggerCanSend(false);
                            m_CharacterSelection_Panel.SetBackTriggerCanSend(false);
                        }

                        continue;
                    }
                    
                    if (m_CharacterSelection_Panel != null)
                    {
                        m_CharacterSelection_Panel.SetProceedTriggerCanSend(true);

                        bool hasCharacterSelected = m_CharacterSelection_Panel.HasCharacterSelected(teamIndex);
                        m_CharacterSelection_Panel.SetBackTriggerCanSend(hasCharacterSelected);
                    }

                    bool left = tnInputUtils.GetNegativeButtonDown(playerId, InputButtons.s_HorizontalLeft, WifiInputButtons.s_HorizontalLeft);
                    bool right = tnInputUtils.GetPositiveButtonDown(playerId, InputButtons.s_HorizontalRight, WifiInputButtons.s_HorizontalRight);
                    bool down = tnInputUtils.GetNegativeButtonDown(playerId, InputButtons.s_VerticalDown, WifiInputButtons.s_VerticalDown);
                    bool up = tnInputUtils.GetPositiveButtonDown(playerId, InputButtons.s_VerticalUp, WifiInputButtons.s_VerticalUp);
                    bool confirm = tnInputUtils.GetButtonDown(playerId, InputButtons.s_Submit, WifiInputButtons.s_Submit);
                    bool cancel = tnInputUtils.GetButtonDown(playerId, InputButtons.s_Cancel, WifiInputButtons.s_Cancel);
                    bool ready = tnInputUtils.GetButtonDown(playerId, InputButtons.s_Start, WifiInputButtons.s_Start);

                    ready |= (time <= 0.0);

                    if ((left || right) && !(left && right))
                    {
                        int directionIndex = (left) ? ((int)UINavigationDirection.Left) : ((int)UINavigationDirection.Right);
                        m_PhotonView.RPC("RPC_CharacterSelection_Move", PhotonTargets.AllViaServer, teamIndex, directionIndex);
                    }

                    if ((down || up) && !(down && up))
                    {
                        int directionIndex = (down) ? ((int)UINavigationDirection.Down) : ((int)UINavigationDirection.Up);
                        m_PhotonView.RPC("RPC_CharacterSelection_Move", PhotonTargets.AllViaServer, teamIndex, directionIndex);
                    }

                    if ((confirm || cancel) && (confirm != cancel))
                    {
                        if (confirm)
                        {
                            m_PhotonView.RPC("RPC_CharacterSelection_Confirm", PhotonTargets.AllViaServer, teamIndex);
                        }
                        else
                        {
                            m_PhotonView.RPC("RPC_CharacterSelection_Cancel", PhotonTargets.AllViaServer, teamIndex);
                        }
                    }
                    else
                    {
                        if (ready)
                        {
                            m_PhotonView.RPC("RPC_CharacterSelection_Ready", PhotonTargets.AllViaServer, teamIndex);
                        }
                    }
                }
            }
        }

        // Check if all teams are ready.

        if (PhotonNetwork.isMasterClient)
        {
            if (m_CharacterSelection_Panel != null && teamsModule != null)
            {
                bool allConfirmed = true;
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    allConfirmed &= m_CharacterSelection_Panel.GetTeamReady(teamIndex);
                }

                if (allConfirmed)
                {
                    OpenWaiting();

                    m_CharacterSelection_ProceedRequested = true;

                    m_PhotonView.RPC("RPC_CharacterSelection_Proceed", PhotonTargets.AllViaServer, null);
                }
            }
        }
    }

    private IEnumerator CharacterSelection_Exit()
    {
        Debug.Log("[CharacterSelection] Exit");

        // Unregister photon callbacks.

        PhotonCallbacks.onPhotonPlayerDisconnectedMain -= On_CharacterSelection_PhotonPlayerDisconnectedEvent;

        // Wait for a small amount of time and proceed.

        yield return new WaitForSeconds(m_CharacterSelectionProceedDelay);
    }

    private void UpdateCharacterSelectionStartTime()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        if (PhotonNetwork.time < 0.0001f)
        {
            m_CharacterSelectionTimeSynced = false;
            return;
        }

        if (!m_CharacterSelectionTimePropertyAlreadySet)
        {
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_CharacterSelectionStartTime, PhotonNetwork.time);
            m_CharacterSelectionTimePropertyAlreadySet = true;
        }
    }

    private void On_CharacterSelection_PhotonPlayerDisconnectedEvent(PhotonPlayer i_PhotonPlayer)
    {

    }

    [PunRPC]
    private void RPC_CharacterSelection_PlayerReady()
    {
        ++m_CharacterSelection_PlayerReady;
    }

    [PunRPC]
    private void RPC_CharacterSelection_Move(int i_TeamIndex, int i_DirectionIndex)
    {
        if (m_PlayerDisconnectedDuringMatchSetup || m_CharacterSelection_ProceedRequested)
            return;

        UINavigationDirection navigationDirection = (UINavigationDirection)i_DirectionIndex;

        if (m_CharacterSelection_Panel != null)
        {
            m_CharacterSelection_Panel.Move(i_TeamIndex, navigationDirection);
        }
    }

    [PunRPC]
    private void RPC_CharacterSelection_Confirm(int i_TeamIndex)
    {
        if (m_PlayerDisconnectedDuringMatchSetup || m_CharacterSelection_ProceedRequested)
            return;

        if (m_CharacterSelection_Panel != null)
        {
            m_CharacterSelection_Panel.Confirm(i_TeamIndex);
        }
    }

    [PunRPC]
    private void RPC_CharacterSelection_Cancel(int i_TeamIndex)
    {
        if (m_PlayerDisconnectedDuringMatchSetup || m_CharacterSelection_ProceedRequested)
            return;

        if (m_CharacterSelection_Panel != null)
        {
            m_CharacterSelection_Panel.Cancel(i_TeamIndex);
        }
    }

    [PunRPC]
    private void RPC_CharacterSelection_Ready(int i_TeamIndex)
    {
        if (m_PlayerDisconnectedDuringMatchSetup || m_CharacterSelection_ProceedRequested)
            return;

        if (m_CharacterSelection_Panel != null)
        {
            m_CharacterSelection_Panel.Ready(i_TeamIndex);
        }
    }

    [PunRPC]
    private void RPC_CharacterSelection_Proceed()
    {
        if (m_PlayerDisconnectedDuringMatchSetup)
            return;

        // Open waiting.

        OpenWaiting();

        // Fill module.

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule != null)
        {
            if (m_CharacterSelection_Panel != null)
            {
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                    if (teamDescription == null)
                        continue;

                    List<int> charactersIds = m_CharacterSelection_Panel.GetLineUpIds(teamIndex);

                    if (charactersIds == null)
                        continue;

                    int max = Mathf.Min(charactersIds.Count, teamDescription.charactersCount);
                    for (int index = 0; index < max; ++index)
                    {
                        tnCharacterDescription characterDescription = teamDescription.GetCharacterDescription(index);

                        if (characterDescription == null)
                            continue;

                        int characterId = charactersIds[index];

                        characterDescription.SetCharacterId(characterId);
                    }
                }
            }
        }

        // Proceed.

        ChangeState(tnMultiplayerState.LoadGame);
    }

    // LoadGame

    private void LoadGame_Enter()
    {
        Debug.Log("[LoadGame] Enter");

        ClearAllGroups();

        // Open loading.

        if (m_LoadingPanel != null)
        {
            m_LoadingPanel.Present();
        }

        // Prepare Photon.

        UnregisterPhotonCommonEvents();

        PhotonNetwork.isMessageQueueRunning = false;

        // Stop Music

        MusicPlayer.StopMain();
        MusicPlayer.SetPlaylistMain(null);

        // Proceed to game scene.

        StaticCoroutine.RunMain(LoadGame());
    }

    private void LoadGame_Update()
    {
        bool b = Input.GetKeyDown(KeyCode.B);
        bool m = Input.GetKeyDown(KeyCode.M);

        if (b && m)
        {
            OpenWaiting();
            PhotonNetwork.Disconnect();
        }
    }

    private void LoadGame_Exit()
    {
        Debug.Log("[LoadGame] Exit");
    }

    private IEnumerator LoadGame()
    {
        if (PhotonNetwork.room != null)
        {
            // Load game.

            AsyncOperation loadGame = SceneManager.LoadSceneAsync(s_MultiplayerGame_SceneName, LoadSceneMode.Single);
            while (!loadGame.isDone)
            {
                yield return null;
            }
        }
    }

    // PHOTON CALLBACKS

    private void OnPhotonPlayerDisconnectedEvent(PhotonPlayer i_PhotonPlayer)
    {
        if (!m_PlayerDisconnectedDuringMatchSetup)
        {
            bool joined;
            PhotonUtils.TryGetPlayerCustomProperty<bool>(i_PhotonPlayer, PhotonPropertyKey.s_PlayerCustomPropertyKey_Joined, out joined);

            if (joined)
            {
                if (isInMatchSetup)
                {
                    m_PlayerDisconnectedDuringMatchSetup = true;

                    OpenWaiting();
                    PhotonNetwork.Disconnect();
                }
            }
        }
    }

    private void OnDisconnectedFromPhotonEvent()
    {
        if (fsm.currentState == tnMultiplayerState.Login || fsm.currentState == tnMultiplayerState.Connection)
            return;

        // Clear UI.

        ClearAllGroups();
        CloseWaiting();

        // If I'm in local party, so return.

        if (fsm.currentState == tnMultiplayerState.LocalParty)
        {
            Return();
            return;
        }

        // Show dialog.

        string title = "OUCH";
        string detail = (m_PlayerDisconnectedDuringMatchSetup) ? "A player has left the room. You will be returned to the lobby." : "Something went wrong. You have been disconnected.";

        ShowDialog(title, detail, OnDisconnectedFromPhotonDialogCallback);
    }

    private void OnDisconnectedFromPhotonDialogCallback()
    {
        // Open waiting.

        OpenWaiting();

        // Go to connection.

        ChangeState(tnMultiplayerState.Connection);
    }

    // INTERNALS

    private void RegisterPhotonCommonEvents()
    {
        PhotonCallbacks.onPhotonPlayerDisconnectedMain += OnPhotonPlayerDisconnectedEvent;
        PhotonCallbacks.onDisconnectedFromPhotonMain += OnDisconnectedFromPhotonEvent;
    }

    private void UnregisterPhotonCommonEvents()
    {
        PhotonCallbacks.onPhotonPlayerDisconnectedMain -= OnPhotonPlayerDisconnectedEvent;
        PhotonCallbacks.onDisconnectedFromPhotonMain -= OnDisconnectedFromPhotonEvent;
    }

    private void OpenWaiting()
    {
        SwitchPanels(UIGroup.Group7, m_Waiting_Panel);
    }

    private void CloseWaiting()
    {
        ClearGroup(UIGroup.Group7);
    }

    private void SetLocalCaptainOnInputModule()
    {
        InputModule inputModule = UIEventSystem.inputModuleMain;
        if (inputModule != null)
        {
            inputModule.Clear();

            tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
            if (localPartyModule != null)
            {
                int captainId = localPartyModule.captainId;
                if (Hash.IsValid(captainId))
                {
                    tnPlayerData playerData = tnGameData.GetPlayerDataMain(captainId);
                    if (playerData != null)
                    {
                        string playerInputName = playerData.playerInputName;
                        string wifiPlayerInputName = playerData.wifiPlayerInputName;

                        if (!StringUtils.IsNullOrEmpty(playerInputName))
                        {
                            inputModule.AddPlayer(playerInputName);
                        }
                        else
                        {
                            if (!StringUtils.IsNullOrEmpty(wifiPlayerInputName))
                            {
                                inputModule.AddWifiPlayer(wifiPlayerInputName);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetLocalPartyOnInputModule()
    {
        InputModule inputModule = UIEventSystem.inputModuleMain;
        if (inputModule != null)
        {
            inputModule.Clear();

            tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
            if (localPartyModule != null)
            {
                for (int index = 0; index < localPartyModule.playersCount; ++index)
                {
                    int playerId = localPartyModule.GetPlayerId(index);

                    if (Hash.IsNullOrEmpty(playerId))
                        continue;

                    tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

                    if (playerData == null)
                        continue;

                    string playerInputName = playerData.playerInputName;
                    string wifiPlayerInputName = playerData.wifiPlayerInputName;

                    if (!StringUtils.IsNullOrEmpty(playerInputName))
                    {
                        inputModule.AddPlayer(playerInputName);
                    }
                    else
                    {
                        if (!StringUtils.IsNullOrEmpty(wifiPlayerInputName))
                        {
                            inputModule.AddWifiPlayer(wifiPlayerInputName);
                        }
                    }
                }
            }
        }
    }

    private void SetAllPlayersOnInputModule()
    {
        InputModule inputModule = UIEventSystem.inputModuleMain;

        if (inputModule == null)
            return;

        inputModule.Clear();

        List<int> playersIds = tnGameData.GetPlayersKeysMain();

        if (playersIds != null)
        {
            for (int index = 0; index < playersIds.Count; ++index)
            {
                int playerId = playersIds[index];

                if (Hash.IsNullOrEmpty(playerId))
                    continue;

                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

                if (playerData == null)
                    continue;

                string playerInputName = playerData.playerInputName;
                string wifiPlayerInputName = playerData.wifiPlayerInputName;

                PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerInputName);
                WiFiPlayerInput wifiPlayerInput = WiFiInputSystem.GetPlayerByNameMain(wifiPlayerInputName);

                if (playerInput != null)
                {
                    inputModule.AddPlayer(playerInput);
                }
                else
                {
                    if (wifiPlayerInput != null)
                    {
                        inputModule.AddWifiPlayer(wifiPlayerInput);
                    }
                }
            }
        }
    }

    private void SetMasterClientOnInputModule()
    {
        if (PhotonNetwork.isMasterClient)
        {
            SetLocalCaptainOnInputModule();
        }
    }

    private void ClearInputModule()
    {
        InputModule inputModule = UIEventSystem.inputModuleMain;

        if (inputModule == null)
            return;

        inputModule.Clear();
    }

    private void OnDisconnectedDialogCallback()
    {
        ChangeState(tnMultiplayerState.Connection);
    }

    // UTILS

    private void ChangeState(tnMultiplayerState i_TargetState)
    {
        fsm.ChangeState(i_TargetState);
    }

    private void ShowDialog(string i_Title, string i_DetailText, Action i_Callback = null)
    {
        if (m_Dialog_Panel != null)
        {
            // Set players that will be able to respond to dialog.

            tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
            if (localPartyModule != null && !localPartyModule.isEmpty)
            {
                SetLocalCaptainOnInputModule();
            }
            else
            {
                SetAllPlayersOnInputModule();
            }

            // Open panel.

            SwitchPanels(UIGroup.Group5, m_Dialog_Panel);

            // Configure and run dialog.

            Action callback = () => { ClearGroup(UIGroup.Group5); if (i_Callback != null) i_Callback(); };
            m_Dialog_Panel.SetTitle(i_Title);
            m_Dialog_Panel.SetDeatilText(i_DetailText);
            m_Dialog_Panel.ShowDialog(callback);
        }
        else
        {
            if (i_Callback != null)
            {
                i_Callback();
            }
        }
    }

    private string GeneratePlayerName()
    {
        string playerName = "PLAYER_";

        int randomId = UnityEngine.Random.Range(0, 1000);
        playerName += randomId.ToString();

        return playerName;
    }
}