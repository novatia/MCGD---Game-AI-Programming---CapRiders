using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using System;
using System.Collections;

using WiFiInput.Server;

using GoUI;

using TrueSync;

using FullInspector;

using Random = UnityEngine.Random;

public enum tnGameState
{
    None = 0,

    Setup = 1,
    Initialize = 2,

    ReadyToStart = 3,

    Game = 4,

    GameFinished = 5,

    RestartGame = 6,
    ReturnToMainMenu = 7,
}

[fiInspectorOnly]
public class tnGameFsm : tnGameFSM<tnGameState>
{
    // STATIC

    private static string s_Loading_Tag = "Loading";

    private static string s_Camera_SpawnPoint_Name = "Spawn_Camera";

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
    private bool m_ShowMatchTutorial = true;

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
    private tnPanel_FlatBackground m_FlatBackgroundPanel = null;
    private tnPanel_PauseMenu m_PauseMenuPanel = null;
    private tnPanel_InGameOptions m_InGameOptionsPanel = null;
    private UIController m_LoadingPanel = null;

    private int m_Seed = 0;

    private GameObject m_GameCameraGO = null;

    private TrueSyncManager m_TrueSyncManager = null;
    private tnMatchController m_MatchController = null;

    private ObjectPoolController m_ObjectPoolController = null;

    private bool m_Paused = false;
    private bool m_InGameOptions = false;

    // Game

    private FP m_TimeScale_Cached = FP.One;
    private bool m_MatchJustUnpaused = false;

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Get Component.

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

    private void Start()
    {
        StartFSM();
    }

    // tnFSM's interface

    protected override tnGameState startingState
    {
        get
        {
            return tnGameState.Setup;
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

    // Setup

    private IEnumerator Setup_Enter()
    {
        Debug.Log("[Setup] Enter");

        m_Seed = (m_ForcedSeed >= 0) ? m_ForcedSeed : Random.Range(0, int.MaxValue);

        yield return StartCoroutine(UnloadUnusedAssets());
        yield return StartCoroutine(LoadMap());

        CreateCamera();

        LoadObjectPools();

        SetupTrueSyncManager();
        CreateMatchController();

        SetupInputModule();

        SetupAudio();

        fsm.ChangeState(tnGameState.Initialize);
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
            matchControllerInstance.gameObject.name = "MatchController";
            matchControllerInstance.SetSeed(m_Seed);
            matchControllerInstance.SetShowTutorial(m_ShowMatchTutorial);

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

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule != null)
        {
            for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
            {
                tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                if (teamDescription == null)
                    continue;

                for (int characterIndex = 0; characterIndex < teamDescription.charactersCount; ++characterIndex)
                {
                    tnCharacterDescription characterDescription = teamDescription.GetCharacterDescription(characterIndex);

                    if (characterDescription == null)
                        continue;

                    int playerId = characterDescription.playerId;

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

        fsm.ChangeState(tnGameState.ReadyToStart);
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
        fsm.ChangeState(tnGameState.Game);
    }

    // Game

    private IEnumerator Game_Enter()
    {
        Debug.Log("[Game] Enter");

        // Open panels.

        SwitchPanels(UIGroup.Group6, m_CountdownPanel);

        // Register on match controller events.

        if (m_MatchController != null)
        {
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
        // Check pause.

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
        fsm.ChangeState(tnGameState.GameFinished);
    }

    private void OnMatchPaused()
    {
        if (m_Paused)
            return;

        m_Paused = true;

        // Set time scale.

        if (m_TrueSyncManager != null)
        {
            m_TimeScale_Cached = m_TrueSyncManager.timeScale;
            m_TrueSyncManager.ForceTimeScale(FP.Zero);
        }

        // Open panels.

        SwitchPanels(UIGroup.Group3, m_FlatBackgroundPanel, m_PauseMenuPanel);

        // Set audio mixer snapshot.

        AudioMixerManager.SetSnapshotMain(m_SnapshotPause, 0f, -10);

        // Setup panel.

        if (m_PauseMenuPanel != null)
        {
            m_PauseMenuPanel.onResumeEvent += OnPauseResumeSelected;
            m_PauseMenuPanel.onOptionsEvent += OnPauseOptionsSelected;
            m_PauseMenuPanel.onRestartEvent += OnPauseRestartSelected;
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

        // Reset time scale.

        if (m_TrueSyncManager != null)
        {
            m_TrueSyncManager.ForceTimeScale(m_TimeScale_Cached);
        }

        // Clear audio mixer snapshot.

        AudioMixerManager.RemoveMain(m_SnapshotPause);

        // Unregister from events.

        if (m_PauseMenuPanel != null)
        {
            m_PauseMenuPanel.onResumeEvent -= OnPauseResumeSelected;
            m_PauseMenuPanel.onOptionsEvent -= OnPauseOptionsSelected;
            m_PauseMenuPanel.onRestartEvent -= OnPauseRestartSelected;
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
        if (m_MatchController != null)
        {
            m_MatchController.Unpause();
        }
        else
        {
            OnMatchUnpaused();
        }

        fsm.ChangeState(tnGameState.RestartGame);
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

        fsm.ChangeState(tnGameState.ReturnToMainMenu);
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

    // GameFinished

    private void GameFinished_Enter()
    {
        Debug.Log("[GameFinished] Enter");

        // Clear all groups.

        ClearAllGroups();

        // Open End Game menu.

        SwitchPanels(UIGroup.Group1, m_EndGamePanel);

        // Register on end game menu events.

        if (m_EndGamePanel != null)
        {
            m_EndGamePanel.SetState(tnEndGamePanelState.OfflineSelection);

            m_EndGamePanel.mainMenuRequestedEvent += On_EndGame_MainMenuSelected;
            m_EndGamePanel.rematchRequestedEvent += On_EndGame_RematchSelected;
        }
    }

    private void GameFinished_Update()
    {

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

    private void On_EndGame_MainMenuSelected()
    {
        // Go to main menu.

        fsm.ChangeState(tnGameState.ReturnToMainMenu);
    }

    private void On_EndGame_RematchSelected()
    {
        // Go to restart game.

        fsm.ChangeState(tnGameState.RestartGame);
    }

    // RestartGame

    private void RestartGame_Enter()
    {
        Debug.Log("[RestartGame] Enter");

        // Clear all groups.

        ClearAllGroups();

        // Open loading.

        if (m_LoadingPanel != null)
        {
            m_LoadingPanel.Present();
        }

        // Stop music.

        MusicPlayer.StopMain();
        MusicPlayer.SetPlaylistMain(null);

        // Proceed to game scene.

        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    private void RestartGame_Update()
    {

    }

    private void RestartGame_Exit()
    {
        Debug.Log("[RestartGame] Exit");
    }

    // ReturnToMainMenu

    private void ReturnToMainMenu_Enter()
    {
        Debug.Log("[ReturnToMainMenu] Enter");

        // Clear All Groups.

        ClearAllGroups();

        // Open loading.

        if (m_LoadingPanel != null)
        {
            m_LoadingPanel.Present();
        }

        // Load scene.

        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    private void ReturnToMainMenu_Update()
    {

    }

    private void ReturnToMainMenu_Exit()
    {
        Debug.Log("[ReturnToMainMenu] Exit");
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
}