using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using FullInspector;

using WiFiInput.Server;

using GoUI;

using TuesdayNights;

using TrueSync;

using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum tnMultiplayerGameState
{
    None = 0,

    Synchronization = 1,
    Setup = 2,
    Initialize = 3,

    ReadyToStart = 4,

    Game = 5,

    AbortMatch = 6,
    GameFinished = 7,

    WaitForRematch = 8,

    RestartGame = 9,
    ReturnToMainMenu = 10,

    LoadMainMenu = 11,
}

[fiInspectorOnly]
[RequireComponent(typeof(PhotonView))]
public class tnMultiplayerGameFsm : tnGameFSM<tnMultiplayerGameState>
{
    // STATIC

    private static string s_RematchVoted_PropertyKey = "rematch_voted";
    private static string s_RematchVote_PropertyKey = "rematch_vote";

    private static string s_Loading_Tag = "Loading";

    private static string s_Camera_SpawnPoint_Name = "Spawn_Camera";

    private static string s_PlayerJoined_PropertyKey = "PlayerJoined";

    // Serializeable fields.

    [InspectorHeader("Game")]

    [SerializeField]
    [GreaterOrEqual(-1)]
    [InspectorCategory("GAME")]
    private int m_ForcedSeed = -1;
    [SerializeField]
    [InspectorCategory("GAME")]
    private TrueSyncManager m_TrueSyncManagerPrefab = null;
    [SerializeField]
    [InspectorCategory("GAME")]
    private TrueSyncConfig m_TrueSyncConfig = null;
    [SerializeField]
    [InspectorCategory("GAME")]
    private double m_TimeForRematch = 10;

    [InspectorHeader("Audio")]

    [SerializeField]
    [InspectorCategory("AUDIO")]
    private AudioMixerSnapshot m_SnapshotGame = null;
    [SerializeField]
    [InspectorCategory("AUDIO")]
    private AudioMixerSnapshot m_SnapshotPause = null;
    [SerializeField]
    [InspectorCategory("AUDIO")]
    private MusicPlaylist m_MusicPlaylist = null;

    [InspectorHeader("UI")]

    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_Countdown m_CountdownPanelPrefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_EndGame m_EndGamePanelPrefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_Dialog m_DialogPanelPrefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_FlatBackground m_FlatBackgroundPanelPrefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_PauseMenu m_PauseMenuPanelPrefab = null;
    [SerializeField]
    [InspectorCategory("UI")]
    private tnPanel_InGameOptions m_InGameOptionsPanelPrefab = null;

    // FIELDS

    private tnPanel_Countdown m_CountdownPanel = null;
    private tnPanel_EndGame m_EndGamePanel = null;
    private tnPanel_Dialog m_DialogPanel = null;
    private tnPanel_FlatBackground m_FlatBackgroundPanel = null;
    private tnPanel_PauseMenu m_PauseMenuPanel = null;
    private tnPanel_InGameOptions m_InGameOptionsPanel = null;
    private UIController m_LoadingPanel = null;

    private int m_SharedSeed = 0;

    private GameObject m_GameCameraGO = null;

    private TrueSyncManager m_TrueSyncManager = null;
    private tnMatchController m_MatchController = null;

    private PhotonView m_PhotonView = null;
    private ObjectPoolController m_ObjectPoolController = null;

    private tnProceedRequestHandler m_ProceedRequestHandler = new tnProceedRequestHandler();
    private int m_ProceedRequestIndex = 0;

    private bool m_PlayerDisconnectedAfterGameFinished = false;

    private bool m_Paused = false;
    private bool m_InGameOptions = false;

    // Synchronization.

    private bool m_Synchronization_WaitingForPlayers = false;

    // Game.

    private bool m_MatchAborted = false;
    private bool m_MatchJustUnpaused = false;

    // GameFinished.

    private bool m_GameFinishedTimeSynced = false;
    private bool m_GameFinishedTimePropertyAlreadySet = false;
    private bool m_GameFinishedTimedOut = false;

    // Wait for rematch.

    private bool m_WaitForRematch_QuitDialogOpened = false;
    private bool m_WaitForRematch_Success = false;
    private bool m_WaitForRematch_Failed = false;

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Add players to process request handler.

        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
        {
            PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];
            int photonPlayerId = photonPlayer.ID;
            m_ProceedRequestHandler.AddPlayer(photonPlayerId);
        }

        // Get component.

        m_PhotonView = GetComponent<PhotonView>();
        m_ObjectPoolController = GetComponent<ObjectPoolController>();

        // UI

        if (m_CountdownPanelPrefab != null)
        {
            tnPanel_Countdown countdownPanel = Instantiate<tnPanel_Countdown>(m_CountdownPanelPrefab);
            countdownPanel.transform.SetParent(transform, true);

            m_CountdownPanel = countdownPanel;
        }

        if (m_EndGamePanelPrefab != null)
        {
            tnPanel_EndGame endGamePanel = Instantiate<tnPanel_EndGame>(m_EndGamePanelPrefab);
            endGamePanel.transform.SetParent(transform, true);

            m_EndGamePanel = endGamePanel;
        }

        if (m_DialogPanelPrefab != null)
        {
            tnPanel_Dialog dialogPanel = Instantiate<tnPanel_Dialog>(m_DialogPanelPrefab);
            dialogPanel.transform.SetParent(transform, true);

            m_DialogPanel = dialogPanel;
        }

        if (m_FlatBackgroundPanelPrefab != null)
        {
            tnPanel_FlatBackground flatBackgroundPanel = Instantiate<tnPanel_FlatBackground>(m_FlatBackgroundPanelPrefab);
            flatBackgroundPanel.transform.SetParent(transform, true);

            m_FlatBackgroundPanel = flatBackgroundPanel;
        }

        if (m_PauseMenuPanelPrefab != null)
        {
            tnPanel_PauseMenu pauseMenuPanel = Instantiate<tnPanel_PauseMenu>(m_PauseMenuPanelPrefab);
            pauseMenuPanel.transform.SetParent(transform, true);

            m_PauseMenuPanel = pauseMenuPanel;
        }

        if (m_InGameOptionsPanelPrefab != null)
        {
            tnPanel_InGameOptions inGameOptionsPanel = Instantiate<tnPanel_InGameOptions>(m_InGameOptionsPanelPrefab);
            inGameOptionsPanel.transform.SetParent(transform, true);

            m_InGameOptionsPanel = inGameOptionsPanel;
        }

        m_LoadingPanel = GameObjectUtils.FindObjectWithTag<UIController>(s_Loading_Tag);
    }

    void Start()
    {
        PhotonNetwork.isMessageQueueRunning = true;

        // Run FSM.

        StartFSM();
    }

    void OnEnable()
    {
        PhotonCallbacks.onPhotonPlayerDisconnectedMain += OnPhotonPlayerDisconnectedEvent;
        PhotonCallbacks.onLeftRoomMain += OnLeftRoomEvent;
    }

    void OnDisable()
    {
        PhotonCallbacks.onPhotonPlayerDisconnectedMain -= OnPhotonPlayerDisconnectedEvent;
        PhotonCallbacks.onLeftRoomMain -= OnLeftRoomEvent;
    }

    // LOGIC

    // tnFSM's interface

    protected override tnMultiplayerGameState startingState
    {
        get
        {
            return tnMultiplayerGameState.Synchronization;
        }
    }

    protected override void OnFSMStarted()
    {
        base.OnFSMStarted();
    }

    protected override void OnFSMReturn()
    {
        base.OnFSMReturn();
    }

    // STATES

    // Synchronization

    private void Synchronization_Enter()
    {
        Debug.Log("[Synchronization] Enter");

        // Clear variables.

        m_Synchronization_WaitingForPlayers = true;

        // Notify.

        NotifyJoin();
    }

    private void Synchronization_Update()
    {
        if (PhotonNetwork.room == null)
        {
            RPC_Setup(0);
            return;
        }

        if (!PhotonNetwork.isMasterClient)
            return;

        int totalPlayers = PhotonNetwork.room.PlayerCount;

        // Wait for players.

        if (m_Synchronization_WaitingForPlayers)
        {
            int playersJoined = 0;

            PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
            if (photonPlayers != null)
            {
                for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
                {
                    PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];

                    if (photonPlayer == null)
                        continue;

                    bool playerJoined;
                    if (PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, s_PlayerJoined_PropertyKey, out playerJoined))
                    {
                        if (playerJoined)
                        {
                            ++playersJoined;
                        }
                    }
                }
            }

            if (playersJoined == totalPlayers)
            {
                m_Synchronization_WaitingForPlayers = false;
                int seed = (m_ForcedSeed >= 0) ? m_ForcedSeed : Random.Range(0, int.MaxValue);
                m_PhotonView.RPC("RPC_Setup", PhotonTargets.AllViaServer, (int)seed);
            }
        }
    }

    private void Synchronization_Exit()
    {
        Debug.Log("[Synchronization] Exit");
    }

    private void NotifyJoin()
    {
        PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, s_PlayerJoined_PropertyKey, true);
    }

    // Setup

    private IEnumerator Setup_Enter()
    {
        Debug.Log("[Setup] Enter");

        PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, s_PlayerJoined_PropertyKey, false);

        yield return StartCoroutine(UnloadUnusedAssets());
        yield return StartCoroutine(LoadMap());

        SetupPlayerProperty();

        CreateCamera();

        LoadObjectPools();

        SetupTrueSyncManager();
        CreateMatchController();

        SetupInputModule();

        SetupAudio();

        ProceedTo(tnMultiplayerGameState.Initialize);
    }

    private void Setup_Update()
    {

    }

    private void Setup_Exit()
    {
        Debug.Log("[Setup] Exit");
    }

    private IEnumerator LoadMap()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
        if (matchSettingsModule != null)
        {
            int stadiumId = matchSettingsModule.stadiumId;
            tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);
            if (stadiumData != null)
            {
                string sceneName = stadiumData.sceneName;
                IEnumerator loadScene = LoadSceneAdditiveAsync(sceneName);
                yield return StartCoroutine(loadScene);
            }
        }
    }

    private void SetupPlayerProperty()
    {
        PhotonUtils.SetPlayerCustomProperty(PhotonNetwork.player, s_RematchVoted_PropertyKey, false);
    }

    private void CreateCamera()
    {
        GameObject spawnPointGo = GameObject.Find(s_Camera_SpawnPoint_Name);

        Vector3 spawnPointPosition = (spawnPointGo != null) ? spawnPointGo.transform.position : Vector3.zero;
        spawnPointPosition.z = -15f;

        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null)
            return;

        int gameModeId = matchSettingsModule.gameModeId;
        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);

        if (gameModeData == null)
            return;

        int cameraSetId = gameModeData.camerasSetId;
        tnCamerasSet cameraSet = tnGameData.GetCameraSetMain(cameraSetId);

        if (cameraSet == null)
            return;

        int stadiumId = matchSettingsModule.stadiumId;
        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

        if (stadiumData == null)
            return;

        GameObject cameraPrefab = cameraSet.GetCamera(stadiumData.cameraId);

        if (cameraPrefab == null)
            return;

        m_GameCameraGO = Instantiate<GameObject>(cameraPrefab);
        m_GameCameraGO.name = "GameCamera";
        m_GameCameraGO.tag = "GameCamera";

        tnGameCamera gameCamera = m_GameCameraGO.GetComponent<tnGameCamera>();
        if (gameCamera != null)
        {
            gameCamera.SetPosition(spawnPointPosition);
        }
        else
        {
            m_GameCameraGO.transform.position = spawnPointPosition;
        }

        m_GameCameraGO.transform.rotation = Quaternion.identity;
    }

    private void LoadObjectPools()
    {
        if (m_ObjectPoolController == null)
            return;

        m_ObjectPoolController.LoadAll();
    }

    private void SetupTrueSyncManager()
    {
        if (m_TrueSyncManagerPrefab == null)
            return;

        m_TrueSyncManager = Instantiate<TrueSyncManager>(m_TrueSyncManagerPrefab);
        m_TrueSyncManager.Initialize(m_TrueSyncConfig);

        TrueSyncObject[] trueSyncObjects = FindObjectsOfType<TrueSyncObject>();
        if (trueSyncObjects != null)
        {
            for (int index = 0; index < trueSyncObjects.Length; ++index)
            {
                TrueSyncObject trueSyncObject = trueSyncObjects[index];
                m_TrueSyncManager.RegisterTrueSyncObject(trueSyncObject);
            }
        }
    }

    private void CreateMatchController()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null)
            return;

        int gameModeId = matchSettingsModule.gameModeId;
        tnMatchController matchControllerInstance = tnGameModeFactory.CreateMatchController(gameModeId);
        if (matchControllerInstance != null)
        {
            matchControllerInstance.gameObject.name = "MultiplayerMatchController";
            matchControllerInstance.SetSeed(m_SharedSeed);

            matchControllerInstance.SetCamera(m_GameCameraGO);
        }

        m_MatchController = matchControllerInstance;

        if (m_TrueSyncManager != null)
        {
            m_TrueSyncManager.RegisterTrueSyncObject(m_MatchController.gameObject);
        }
    }

    private void SetupInputModule()
    {
        InputModule inputModule = UIEventSystem.inputModuleMain;

        if (inputModule == null)
            return;

        inputModule.Clear();

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        if (localPartyModule != null)
        {
            for (int playerIndex = 0; playerIndex < localPartyModule.playersCount; ++playerIndex)
            {
                int playerId = localPartyModule.GetPlayerId(playerIndex);

                if (Hash.IsNullOrEmpty(playerId))
                    continue;

                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

                if (playerData == null)
                    continue;

                string playerInputName = playerData.playerInputName;
                string wifiPlayerInputName = playerData.wifiPlayerInputName;

                PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerInputName);
                WiFiPlayerInput wifiPlyerInput = WiFiInputSystem.GetPlayerByNameMain(wifiPlayerInputName);

                if (playerInput != null)
                {
                    inputModule.AddPlayer(playerInput);
                }
                else
                {
                    if (wifiPlyerInput != null)
                    {
                        inputModule.AddWifiPlayer(wifiPlyerInput);
                    }
                }
            }
        }
    }

    private void SetupAudio()
    {
        SetupAudioMixer();

        MusicPlayer.SetPlaylistMain(m_MusicPlaylist);
    }

    private void SetupAudioMixer()
    {
        AudioMixerManager.ClearMain();
        AudioMixerManager.SetSnapshotMain(m_SnapshotGame, 0f, 0);
    }

    // Initialize

    private void Initialize_Enter()
    {
        Debug.Log("[Initialize] Enter");

        if (m_MatchController != null)
        {
            m_MatchController.Initialize();
        }

        ProceedTo(tnMultiplayerGameState.ReadyToStart);
    }

    private void Initialize_Update()
    {

    }

    private void Initialize_Exit()
    {
        Debug.Log("[Initialize] Exit");
    }

    // Ready to start

    private void ReadyToStart_Enter()
    {
        Debug.Log("[ReadyToStart] Enter");

        // Hide loading.

        if (m_LoadingPanel != null)
        {
            m_LoadingPanel.Dismiss();
        }

        // Register callback on match controller and set it as ready.

        if (m_MatchController != null)
        {
            m_MatchController.startGameRequestedEvent += On_MatchController_StartGameRequestedEvent;
            m_MatchController.OnGameReady();
        }
        else
        {
            On_MatchController_StartGameRequestedEvent();
        }
    }

    private void ReadyToStart_Update()
    {

    }

    private void ReadyToStart_Exit()
    {
        Debug.Log("[ReadyToStart] Exit");

        // Unregister match controller callbacks.

        if (m_MatchController != null)
        {
            m_MatchController.startGameRequestedEvent -= On_MatchController_StartGameRequestedEvent;
        }
    }

    private void On_MatchController_StartGameRequestedEvent()
    {
        ProceedTo(tnMultiplayerGameState.Game);
    }

    // Game

    private IEnumerator Game_Enter()
    {
        Debug.Log("[Game] Enter");

        // Clear variables.

        m_MatchAborted = false;

        // Open panels.

        SwitchPanels(UIGroup.Group6, m_CountdownPanel);

        // Register on match controller events.

        if (m_MatchController != null)
        {
            m_MatchController.matchBecomeInvalidEvent += OnMatchBecomeInvalid;
            m_MatchController.endGameRequestedEvent += OnEndGameRequested;
            m_MatchController.finishGameEvent += OnFinishGame;

            m_MatchController.matchPausedEvent += OnMatchPaused;
            m_MatchController.matchUnpausedEvent += OnMatchUnpaused;
        }

        // Define countdown completed callback.

        bool countdownCompleted = false;
        Action countdownCompletedCallback = () => { countdownCompleted = true; };

        // Start countdown and register on panel events.

        if (m_CountdownPanel != null)
        {
            m_CountdownPanel.countdownCompletedEvent += countdownCompletedCallback;
            m_CountdownPanel.StartCountdown();
        }
        else
        {
            countdownCompletedCallback();
        }

        // Wait for countdown.

        yield return new WaitUntil(() => (countdownCompleted == true));

        // Unregister from panel events.

        if (m_CountdownPanel != null)
        {
            m_CountdownPanel.countdownCompletedEvent -= countdownCompletedCallback;
        }

        // Start music.

        MusicPlayer.PlayMain();

        // Start game on match controller.

        if (m_MatchController != null)
        {
            m_MatchController.OnStartGame();
        }
    }

    private void Game_Update()
    {
        if (PhotonNetwork.room == null)
        {
            if (!m_MatchAborted)
            {
                m_MatchAborted = true;

                if (m_MatchController != null)
                {
                    m_MatchController.MatchBecomeInvalid();
                }
                else
                {
                    OnMatchBecomeInvalid();
                }
            }

            return;
        }

        if (!m_Paused)
        {
            if (!m_MatchJustUnpaused)
            {
                bool pausePressed = false;

                InputModule inputModule = UIEventSystem.inputModuleMain;
                if (inputModule != null)
                {
                    for (int index = 0; index < inputModule.playersCount; ++index)
                    {
                        PlayerInput playerInput = inputModule.GetPlayerInput(index);

                        if (playerInput == null)
                            continue;

                        pausePressed |= playerInput.GetButtonDown("Pause");
                    }

                    for (int index = 0; index < inputModule.wifiPlayersCount; ++index)
                    {
                        WiFiPlayerInput playerInput = inputModule.GetWifiPlayerInput(index);

                        if (playerInput == null)
                            continue;

                        pausePressed |= playerInput.GetButtonDown("Pause");
                    }
                }

                if (pausePressed)
                {
                    if (m_MatchController != null)
                    {
                        bool canPause = m_MatchController.canPause;
                        if (canPause)
                        {
                            m_MatchController.Pause();
                        }
                    }
                }
            }

            m_MatchJustUnpaused = false;
        }
    }

    private void Game_Exit()
    {
        Debug.Log("[Game] Exit");

        // Register on match controller events.

        if (m_MatchController != null)
        {
            m_MatchController.matchBecomeInvalidEvent -= OnMatchBecomeInvalid;
            m_MatchController.endGameRequestedEvent -= OnEndGameRequested;
            m_MatchController.finishGameEvent -= OnFinishGame;

            m_MatchController.matchPausedEvent -= OnMatchPaused;
            m_MatchController.matchUnpausedEvent -= OnMatchUnpaused;
        }
    }

    private void OnEndGameRequested()
    {
        if (m_MatchController != null)
        {
            m_MatchController.OnEndGame();
        }
    }

    private void OnFinishGame()
    {
        fsm.ChangeState(tnMultiplayerGameState.GameFinished);
    }

    private void OnMatchBecomeInvalid()
    {
        fsm.ChangeState(tnMultiplayerGameState.AbortMatch);
    }

    private void OnMatchPaused()
    {
        if (m_Paused)
            return;

        m_Paused = true;

        // Open panels.

        SwitchPanels(UIGroup.Group3, m_FlatBackgroundPanel, m_PauseMenuPanel);

        // Set audio mixer snapshot.

        AudioMixerManager.SetSnapshotMain(m_SnapshotPause, 0f, -10);

        // Setup panel.

        if (m_PauseMenuPanel != null)
        {
            m_PauseMenuPanel.SetResumeButtonActive(true);
            m_PauseMenuPanel.SetRestartButtonActive(false);
            m_PauseMenuPanel.SetOptionsButtonActive(true);
            m_PauseMenuPanel.SetExitButtonActive(true);

            m_PauseMenuPanel.onResumeEvent += OnPauseResumeSelected;
            m_PauseMenuPanel.onOptionsEvent += OnPauseOptionsSelected;
            //m_PauseMenuPanel.onRestartEvent += OnPauseRestartSelected;
            m_PauseMenuPanel.onExitEvent += OnPauseExitSelected;
        }
    }

    private void OnMatchUnpaused()
    {
        if (!m_Paused)
            return;

        if (m_InGameOptions)
        {
            m_InGameOptions = false;

            // Unregister from events.

            if (m_InGameOptionsPanel != null)
            {
                m_InGameOptionsPanel.onBackEvent -= OnOptionsBack;
            }

            // Save game settings.

            GameSettings.SaveMain();
        }

        m_Paused = false;
        m_MatchJustUnpaused = true;

        // Inhibit input.

        if (m_MatchController != null)
        {
            m_MatchController.InhibitInput();
        }

        // Clear audio mixer snapshot.

        AudioMixerManager.RemoveMain(m_SnapshotPause);

        // Unregister from events.

        if (m_PauseMenuPanel != null)
        {
            m_PauseMenuPanel.onResumeEvent -= OnPauseResumeSelected;
            m_PauseMenuPanel.onOptionsEvent -= OnPauseOptionsSelected;
            //m_PauseMenuPanel.onRestartEvent -= OnPauseRestartSelected;
            m_PauseMenuPanel.onExitEvent -= OnPauseExitSelected;
        }

        // Close panels.

        ClearGroup(UIGroup.Group3);
    }

    private void OnPauseResumeSelected()
    {
        if (m_MatchController != null)
        {
            m_MatchController.Unpause();
        }
        else
        {
            OnMatchUnpaused();
        }
    }

    private void OnPauseOptionsSelected()
    {
        if (m_InGameOptions)
            return;

        m_InGameOptions = true;

        // Open panels.

        SwitchPanels(UIGroup.Group3, m_FlatBackgroundPanel, m_InGameOptionsPanel);

        // Register events.

        if (m_InGameOptionsPanel != null)
        {
            m_InGameOptionsPanel.onBackEvent += OnOptionsBack;
        }
    }

    private void OnPauseRestartSelected()
    {
        // It can't occurs here.
    }

    private void OnPauseExitSelected()
    {
        if (m_MatchController != null)
        {
            m_MatchController.Unpause();
        }
        else
        {
            OnMatchUnpaused();
        }

        fsm.ChangeState(tnMultiplayerGameState.ReturnToMainMenu);
    }

    private void OnOptionsBack()
    {
        if (!m_InGameOptions)
            return;

        m_InGameOptions = false;

        // Unregister from events.

        if (m_InGameOptionsPanel != null)
        {
            m_InGameOptionsPanel.onBackEvent -= OnOptionsBack;
        }

        // Switch panel.

        SwitchPanels(UIGroup.Group3, m_FlatBackgroundPanel, m_PauseMenuPanel);

        // Save game settings.

        GameSettings.SaveMain();
    }

    // AbortMatch

    private void AbortMatch_Enter()
    {
        Debug.Log("[AbortMatch] Enter");

        // Clear all groups.

        ClearAllGroups();

        if (m_MatchController != null)
        {
            m_MatchController.ClearUI();
        }

        // Show dialog.

        ShowDialog("MATCH OVER", "This match is no longer valid. You will be returned to main menu.", On_AbortMatch_DialogCallback);
    }

    private void AbortMatch_Update()
    {

    }

    private void AbortMatch_Exit()
    {
        Debug.Log("[AbortMatch] Exit");
    }

    private void On_AbortMatch_DialogCallback()
    {
        fsm.ChangeState(tnMultiplayerGameState.ReturnToMainMenu);
    }

    // Game Finished

    private void GameFinished_Enter()
    {
        Debug.Log("[GameFinished] Enter");

        // Clear variables.

        m_GameFinishedTimeSynced = false;
        m_GameFinishedTimePropertyAlreadySet = false;
        m_GameFinishedTimedOut = false;

        // Clear all groups.

        ClearAllGroups();

        // Open End Game menu.

        SwitchPanels(UIGroup.Group1, m_EndGamePanel);

        // Register on end game menu events.

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetState(tnEndGamePanelState.OnlineSelection);
            m_EndGamePanel.SetTimer((float)m_TimeForRematch);

            m_EndGamePanel.mainMenuRequestedEvent += On_EndGame_MainMenuSelected;
            m_EndGamePanel.rematchRequestedEvent += On_EndGame_RematchSelected;
        }
    }

    private void GameFinished_Update()
    {
        if (m_GameFinishedTimedOut)
            return;

        // Sync time.

        if (!m_GameFinishedTimeSynced)
        {
            UpdateGameFinishedStartTime();
        }

        // Update timer.

        double time = m_TimeForRematch;
        double startTime;
        bool startTimeAlreadySet = false;

        if (PhotonUtils.TryGetRoomCustomProperty<bool>(PhotonPropertyKey.s_RoomCustomPropertyKey_GameFinishedStartTimeAlreadySet, out startTimeAlreadySet))
        {
            if (startTimeAlreadySet)
            {
                if (PhotonUtils.TryGetRoomCustomProperty<double>(PhotonPropertyKey.s_RoomCustomPropertyKey_GameFinishedStartTime, out startTime))
                {
                    m_GameFinishedTimeSynced = true;
                    double elapsedTime = PhotonNetwork.time - startTime;
                    time = m_TimeForRematch - elapsedTime;
                }
            }
        }

        time = Math.Max(time, 0.0);

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetTimer((float)time);
        }

        // Check time out.

        if (time <= 0.0)
        {
            OnTimedOut();
        }
    }

    private void GameFinished_Exit()
    {
        Debug.Log("[GameFinished] Exit");

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.mainMenuRequestedEvent -= On_EndGame_MainMenuSelected;
            m_EndGamePanel.rematchRequestedEvent -= On_EndGame_RematchSelected;
        }
    }

    private void UpdateGameFinishedStartTime()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        if (PhotonNetwork.time < 0.0001f)
        {
            m_GameFinishedTimeSynced = false;
            return;
        }

        if (!m_GameFinishedTimePropertyAlreadySet)
        {
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_GameFinishedStartTime, PhotonNetwork.time);
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_GameFinishedStartTimeAlreadySet, true);
            m_GameFinishedTimePropertyAlreadySet = true;
        }
    }

    private void OnTimedOut()
    {
        m_GameFinishedTimedOut = true;

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetState(tnEndGamePanelState.TimedOut);
        }

        PhotonNetwork.Disconnect();
    }

    private void On_EndGame_MainMenuSelected()
    {
        // Set photon player custom properties.

        Hashtable properties = new Hashtable();
        properties.Add(s_RematchVoted_PropertyKey, true);
        properties.Add(s_RematchVote_PropertyKey, false);

        PhotonNetwork.player.SetCustomProperties(properties);

        // Go to main menu.

        fsm.ChangeState(tnMultiplayerGameState.ReturnToMainMenu);
    }

    private void On_EndGame_RematchSelected()
    {
        if (m_GameFinishedTimedOut)
            return;

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetState(tnEndGamePanelState.WaitingForPlayers);
            m_EndGamePanel.SetReadyPlayers(0, 0);
        }

        // Set photon player custom properties.

        Hashtable properties = new Hashtable();
        properties.Add(s_RematchVoted_PropertyKey, true);
        properties.Add(s_RematchVote_PropertyKey, true);

        PhotonNetwork.player.SetCustomProperties(properties);

        // Go to wait for rematch.

        fsm.ChangeState(tnMultiplayerGameState.WaitForRematch);
    }

    private void SetEndGamePanelState(tnEndGamePanelState i_State)
    {
        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetState(i_State);
        }
    }

    // Wait For Rematch

    private void WaitForRematch_Enter()
    {
        Debug.Log("[WaitForRematch] Enter");

        // Clear variables.

        m_WaitForRematch_QuitDialogOpened = false;
        m_WaitForRematch_Success = false;
        m_WaitForRematch_Failed = false;
    }

    private void WaitForRematch_Update()
    {
        if (m_WaitForRematch_Success || m_WaitForRematch_Failed)
            return;

        if (m_PlayerDisconnectedAfterGameFinished || m_WaitForRematch_Failed)
        {
            if (m_WaitForRematch_QuitDialogOpened)
                return;

            // Set flag.

            m_WaitForRematch_QuitDialogOpened = true;

            // Clear all groups.

            ClearAllGroups();

            // Clear Match controller UI.

            if (m_MatchController != null)
            {
                m_MatchController.ClearUI();
            }

            // Open dialog.

            ShowDialog("MATCH OVER", "You will be returned to the main menu.", On_WaitForRematch_QuitDialogCallback);

            return;
        }

        // Sync time.

        if (!m_GameFinishedTimeSynced)
        {
            UpdateGameFinishedStartTime();
        }

        // Update timer.

        double time = m_TimeForRematch;

        double startTime;
        if (PhotonUtils.TryGetRoomCustomProperty<double>(PhotonPropertyKey.s_RoomCustomPropertyKey_GameFinishedStartTime, out startTime))
        {
            m_GameFinishedTimeSynced = true;
            double elapsedTime = PhotonNetwork.time - startTime;
            time = m_TimeForRematch - elapsedTime;
        }

        time = Math.Max(time, 0.0);

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetTimer((float)time);
        }

        // Check rematch.

        int totalVotes = 0;
        int rematchVotes = 0;
        int playerCount = 0;

        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        if (photonPlayers != null)
        {
            playerCount = photonPlayers.Length;

            for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
            {
                PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];

                if (photonPlayer == null)
                    continue;

                bool voted;
                PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, s_RematchVoted_PropertyKey, out voted);
                if (voted)
                {
                    ++totalVotes;

                    bool vote;
                    PhotonUtils.TryGetPlayerCustomProperty<bool>(photonPlayer, s_RematchVote_PropertyKey, out vote);

                    rematchVotes += (vote) ? 1 : 0;
                }
            }
        }

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetReadyPlayers(rematchVotes, playerCount);
        }

        if (playerCount > 1)
        {
            if (rematchVotes == playerCount)
            {
                m_WaitForRematch_Success = true;

                ProceedTo(tnMultiplayerGameState.RestartGame);
            }
            else
            {
                if (totalVotes == playerCount)
                {
                    m_WaitForRematch_Failed = true;
                }
            }
        }
        else
        {
            m_WaitForRematch_Failed = true;
        }
    }

    private void WaitForRematch_Exit()
    {
        Debug.Log("[WaitForRematch] Exit");
    }

    private void On_WaitForRematch_QuitDialogCallback()
    {
        fsm.ChangeState(tnMultiplayerGameState.ReturnToMainMenu);
    }

    // Restart Game

    private void RestartGame_Enter()
    {
        Debug.Log("[RestartGame] Enter");

        // Clear all groups.

        ClearAllGroups();

        // Master client clear propriety for next match.

        if (PhotonNetwork.isMasterClient)
        {
            PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_GameFinishedStartTimeAlreadySet, false);
        }

        // Open loading.

        if (m_LoadingPanel != null)
        {
            m_LoadingPanel.Present();
        }

        // Stop photon queue.

        PhotonNetwork.isMessageQueueRunning = false;

        // Stop music.

        MusicPlayer.StopMain();
        MusicPlayer.SetPlaylistMain(null);

        // Proceed to game scene.

        SceneManager.LoadSceneAsync("MultiplayerGame", LoadSceneMode.Single);
    }

    private void RestartGame_Update()
    {

    }

    private void RestartGame_Exit()
    {
        Debug.Log("[RestartGame] Exit");
    }

    // Return to Main Menu

    private void ReturnToMainMenu_Enter()
    {
        Debug.Log("[ReturnToMainMenu] Enter");

        // Open loading.

        if (m_LoadingPanel != null)
        {
            m_LoadingPanel.Present();
        }

        // Register on photon callbacks.

        PhotonCallbacks.onDisconnectedFromPhotonMain += On_ReturnToMainMenu_PhotonDisconnectedEvent;

        // Disconnect from photon.

        ConnectionState connectionState = PhotonNetwork.connectionState;

        if (connectionState != ConnectionState.Disconnected && connectionState != ConnectionState.Disconnecting)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            On_ReturnToMainMenu_PhotonDisconnectedEvent();
        }
    }

    private void ReturnToMainMenu_Update()
    {

    }

    private void ReturnToMainMenu_Exit()
    {
        Debug.Log("[ReturnToMainMenu] Exit");

        // Unregister from photon callbacks.

        PhotonCallbacks.onDisconnectedFromPhotonMain -= On_ReturnToMainMenu_PhotonDisconnectedEvent;
    }

    private void On_ReturnToMainMenu_PhotonDisconnectedEvent()
    {
        fsm.ChangeState(tnMultiplayerGameState.LoadMainMenu);
    }

    // Load main menu.

    private void LoadMainMenu_Enter()
    {
        Debug.Log("[LoadMainMenu] Enter");

        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    private void LoadMainMenu_Update()
    {

    }

    private void LoadMainMenu_Exit()
    {
        Debug.Log("[LoadMainMenu] Exit");
    }

    // INTERNALS

    private IEnumerator UnloadUnusedAssets()
    {
        AsyncOperation op = Resources.UnloadUnusedAssets();
        if (op != null)
        {
            while (!op.isDone)
            {
                yield return null;
            }
        }
    }

    private IEnumerator LoadSceneAdditiveAsync(string i_SceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(i_SceneName, LoadSceneMode.Additive);
        if (op != null)
        {
            while (!op.isDone)
            {
                yield return null;
            }
        }
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

    // UTILS

    private void ShowDialog(string i_Title, string i_DetailText, Action i_Callback = null)
    {
        if (m_DialogPanel != null)
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

            SwitchPanels(UIGroup.Group5, m_DialogPanel);

            // Configure and run dialog.

            Action callback = () => { ClearGroup(UIGroup.Group5); if (i_Callback != null) i_Callback(); };
            m_DialogPanel.SetTitle(i_Title);
            m_DialogPanel.SetDeatilText(i_DetailText);
            m_DialogPanel.ShowDialog(callback);
        }
        else
        {
            if (i_Callback != null)
            {
                i_Callback();
            }
        }
    }

    private void ProceedTo(tnMultiplayerGameState i_TargetState)
    {
        int proceedRequestId = m_ProceedRequestIndex++;
        StartCoroutine(ProceedToState(proceedRequestId, i_TargetState));
    }

    private IEnumerator ProceedToState(int i_ProceedRequestIndex, tnMultiplayerGameState i_TargetState)
    {
        if (PhotonNetwork.room != null)
        {
            int photonPlayerId = PhotonNetwork.player.ID;
            m_PhotonView.RPC("RPC_NotifyReady", PhotonTargets.All, photonPlayerId, i_ProceedRequestIndex);
        }

        bool proceedSent = false;
        bool received = m_ProceedRequestHandler.IsReceivedProceedRequest(i_ProceedRequestIndex);

        while (!received)
        {
            if (PhotonNetwork.room == null)
            {
                proceedSent = true;
                RPC_Proceed((int)i_ProceedRequestIndex, (int)i_TargetState);
            }
            else
            {
                if (PhotonNetwork.isMasterClient)
                {
                    // Wait for all.

                    bool allReady = true;

                    PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
                    for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Length; ++photonPlayerIndex)
                    {
                        PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];
                        int photonPlayerId = photonPlayer.ID;
                        bool playerReady = IsPlayerReady(photonPlayerId, i_ProceedRequestIndex);
                        allReady &= playerReady;
                    }

                    if (allReady)
                    {
                        if (!proceedSent)
                        {
                            proceedSent = true;
                            m_PhotonView.RPC("RPC_Proceed", PhotonTargets.AllViaServer, (int)i_ProceedRequestIndex, (int)i_TargetState);
                        }
                    }
                }
            }

            yield return null;

            received = m_ProceedRequestHandler.IsReceivedProceedRequest(i_ProceedRequestIndex);
        }

        m_ProceedRequestHandler.RemoveReceivedProceedRequest(i_ProceedRequestIndex);
    }

    private bool IsPlayerReady(int i_PhotonPlayerId, int i_ProceedRequestId)
    {
        return m_ProceedRequestHandler.GetPlayerReady(i_PhotonPlayerId, i_ProceedRequestId);
    }

    // PHOTON CALLBACKS

    private void OnPhotonPlayerDisconnectedEvent(PhotonPlayer i_PhotonPlayer)
    {
        bool simulationRunning = (m_MatchController != null) ? m_MatchController.simulationRunning : false;

        //if (simulationRunning)
        //    return;

        bool simulationStarted = (m_MatchController != null) ? m_MatchController.simulationStarted : false;

        if (!simulationStarted || simulationRunning)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            tnMultiplayerGameState currentState = fsm.currentState;
            tnMultiplayerGameState gameFinished = tnMultiplayerGameState.GameFinished;

            int currentStateIndex = (int)currentState;
            int gameFinishedIndex = (int)gameFinished;

            m_PlayerDisconnectedAfterGameFinished = (currentStateIndex >= gameFinishedIndex);
        }
    }

    private void OnLeftRoomEvent()
    {

    }

    // RPC

    [PunRPC]
    private void RPC_NotifyReady(int i_PhotonPlayerId, int i_ProceedRequestId)
    {
        m_ProceedRequestHandler.SetPlayerReady(i_PhotonPlayerId, i_ProceedRequestId);
    }

    [PunRPC]
    private void RPC_Proceed(int i_ProceedRequestId, int i_TargetState)
    {
        m_ProceedRequestHandler.AddReceivedProceedRequest(i_ProceedRequestId);

        tnMultiplayerGameState targetState = (tnMultiplayerGameState)i_TargetState;
        fsm.ChangeState(targetState);
    }

    [PunRPC]
    private void RPC_Setup(int i_Seed)
    {
        Debug.Log("Shared seed: " + i_Seed);
        m_SharedSeed = i_Seed;

        fsm.ChangeState(tnMultiplayerGameState.Setup);
    }
}