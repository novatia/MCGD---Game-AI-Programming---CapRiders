using UnityEngine;

using System;
using System.Collections.Generic;

using WiFiInput.Server;

using TuesdayNights;

using FullInspector;

using TrueSync;

[fiInspectorOnly]
public class tnMatchController : TrueSyncBehaviour
{
    // STATIC

    private static string s_RumbleParams_ResourcePath = "Data/Game/RumbleParams";

    // Serializable fields

    [SerializeField]
    [InspectorCategory("GAME")]
    private int m_OfflinePlayerInputDelay = 0;

    // Fields

    [AddTracking]
    protected bool m_GameStarted = false;
    [AddTracking]
    protected bool m_GameEnded = false;
    [AddTracking]
    protected bool m_GameFinished = false;

    [AddTracking]
    protected bool m_MatchStarted = false;
    [AddTracking]
    protected bool m_MatchEnded = false;

    private bool m_SimulationStarted = false;
    private bool m_SimulationEnded = false;

    private bool m_Paused = false;

    [AddTracking]
    protected bool m_MatchValid = true;

    private tnInputManager m_InputManager = null;
    private tnPlayerInputCollector m_PlayerInputCollector = null;

    private GameObject m_CharacterPrefab = null;

    [AddTracking]
    protected int m_Seed = 0;

    protected bool m_ShowTutorial = false;

    private List<PlayerInput> m_LocalPlayersInput = new List<PlayerInput>();
    private List<WiFiPlayerInput> m_LocalWifiPlayersInput = new List<WiFiPlayerInput>();
    private List<List<tnAIInputFiller>> m_LocalAI = new List<List<tnAIInputFiller>>();

    private List<List<GameObject>> m_Teams = new List<List<GameObject>>();
    private List<int> m_TeamsIds = new List<int>();
    private List<GameObject> m_Characters = new List<GameObject>();

    private List<GameObject> m_LocalCharacters = new List<GameObject>();

    private SortableList<tnTeamResults> m_TeamsResults = new SortableList<tnTeamResults>();
    private SortableList<tnCharacterResults> m_CharactersResults = new SortableList<tnCharacterResults>();

    private GameObject m_CameraGo = null;

    private int m_GameModeId = Hash.s_NULL;

    private event Action m_StartGameRequestedEvent = null;
    private event Action m_EndGameRequestedEvent = null;
    private event Action m_FinishGameEvent = null;
    private event Action m_MatchBecomeInvalidEvent = null;
    private event Action m_MatchPausedEvent = null;
    private event Action m_MatchUnpausedEvent = null;

    // ACCESSORS

    public int offlinePlayerInputDelay
    {
        get
        {
            return m_OfflinePlayerInputDelay;
        }
        set
        {
            m_OfflinePlayerInputDelay = value;
        }
    }

    public event Action startGameRequestedEvent
    {
        add { m_StartGameRequestedEvent += value; }
        remove { m_StartGameRequestedEvent -= value; }
    }

    public event Action endGameRequestedEvent
    {
        add { m_EndGameRequestedEvent += value; }
        remove { m_EndGameRequestedEvent -= value; }
    }

    public event Action finishGameEvent
    {
        add { m_FinishGameEvent += value; }
        remove { m_FinishGameEvent -= value; }
    }

    public event Action matchBecomeInvalidEvent
    {
        add
        {
            m_MatchBecomeInvalidEvent += value;
        }

        remove
        {
            m_MatchBecomeInvalidEvent -= value;
        }
    }

    public event Action matchPausedEvent
    {
        add { m_MatchPausedEvent += value; }
        remove { m_MatchUnpausedEvent -= value; }
    }

    public event Action matchUnpausedEvent
    {
        add { m_MatchUnpausedEvent += value; }
        remove { m_MatchUnpausedEvent -= value; }
    }

    public bool paused
    {
        get { return m_Paused; }
    }

    public GameObject cameraGo
    {
        get
        {
            return m_CameraGo;
        }
    }

    public int gameModeId
    {
        get { return m_GameModeId; }
    }

    public bool simulationStarted
    {
        get
        {
            return m_SimulationStarted;
        }
    }

    public bool simulationEnded
    {
        get
        {
            return m_SimulationEnded;
        }
    }

    public bool simulationRunning
    {
        get
        {
            return m_SimulationStarted && !m_SimulationEnded;
        }
    }

    // MonoBehaviour's interface

    protected virtual void Awake()
    {
        m_InputManager = new tnInputManager();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_MatchController;
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (!m_MatchStarted || m_MatchEnded || !m_MatchValid)
            return;

        if (m_Paused)
            return;

        // Check if match is already valid.

        {
            bool matchValid = true;

            for (int teamIndex = 0; teamIndex < teamsCount; ++teamIndex)
            {
                List<GameObject> team = m_Teams[teamIndex];

                if (team == null)
                    continue;

                bool teamValid = false;
                for (int characterIndex = 0; characterIndex < team.Count; ++characterIndex)
                {
                    GameObject character = team[characterIndex];
                    if (character != null && character.activeSelf)
                    {
                        teamValid = true;
                    }
                }

                matchValid &= teamValid;
            }

            if (!matchValid)
            {
                MatchBecomeInvalid();
            }
        }

        if (!m_MatchValid)
            return;

        // Update input manager.

        m_InputManager.Update();

        // TODO: Sort results.
    }

    protected virtual void OnDestroy()
    {

    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedInput()
    {
        base.OnSyncedInput();
    }

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        m_SimulationStarted = true;

        SyncedStart();
    }

    public override void OnLateSyncedUpdate()
    {
        base.OnSyncedUpdate();

        if (!m_MatchStarted || m_MatchEnded || !m_MatchValid)
            return;

        LateSyncedUpdate();
    }

    public override void OnPlayerDisconnection(int i_PlayerId)
    {
        base.OnPlayerDisconnection(i_PlayerId);

        if (simulationRunning)
        {
            // Callback.

            OnPlayerDisconnected(i_PlayerId);

            // Remove player.

            TrueSyncManager.RemovePlayerMain(i_PlayerId);
        }
    }

    // LOGIC

    public void SetSeed(int i_Seed)
    {
        TSRandom tsRandom = TSRandom.New(i_Seed);
        TSRandom.instance = tsRandom;

        m_Seed = i_Seed;
    }

    public void SetShowTutorial(bool i_ShowTutorial)
    {
        m_ShowTutorial = i_ShowTutorial;
    }

    public void SetCamera(GameObject i_CameraGo)
    {
        m_CameraGo = i_CameraGo;
    }

    public void Initialize()
    {
        // Create player input collector.

        CreatePlayerInputCollector();

        // Callback.

        OnPreInit();

        // Create characters.

        CreateCharacters();

        // Callbacks.

        OnInitialize();

        OnPostInit();

        OnConfigureAI();

        OnConfigureStats();

        // Disable input.

        DisableInput();
    }

    public void OnGameReady()
    {
        SetupMatch();
    }

    public void OnStartGame()
    {
        if (m_GameStarted || m_GameFinished)
            return;

        m_GameStarted = true;

        StartMatch();
    }

    public void OnEndGame()
    {
        if (!m_GameStarted || m_GameEnded)
            return;

        m_GameEnded = true;

        EndMatch();
    }

    public void MatchBecomeInvalid()
    {
        if (!m_MatchValid)
            return;

        m_MatchValid = false;

        // End TrueSync simulation.

        TrueSyncManager.EndSimulationMain();
        m_SimulationEnded = true;

        // Callback.

        OnMatchBecomeInvalid();

        // Raise event.

        if (m_MatchBecomeInvalidEvent != null)
        {
            m_MatchBecomeInvalidEvent();
        }
    }

    public void ClearUI()
    {
        OnClearUI();
    }

    // Pause LOGIC

    public bool canPause
    {
        get { return CanPauseMatch(); }
    }

    public bool Pause()
    {
        if (!canPause)
        {
            return false;
        }

        PauseMatch();

        return true;
    }

    public void Unpause()
    {
        UnpauseMatch();
    }

    // Input LOGIC

    public void InhibitInput()
    {
        m_InputManager.InhibitAll();
    }

    public void InhibitAIInput()
    {
        m_InputManager.InhibitAllAIs();
    }

    public void InhibitHumanInput()
    {
        m_InputManager.InhibitAllHumans();
    }

    public void EnableInput()
    {
        m_InputManager.ActivateAll();
    }

    public void EnableAIInput()
    {
        m_InputManager.ActivateAllAIs();
    }

    public void EnableHumanInput()
    {
        m_InputManager.ActivateAllHumans();
    }

    public void DisableInput()
    {
        m_InputManager.DeactivateAll();
    }

    public void DisableAIInput()
    {
        m_InputManager.DeactivateAllAIs();
    }

    public void DisableHumanInput()
    {
        m_InputManager.DeactivateAllHumans();
    }

    // Characters

    public int charactersCount
    {
        get
        {
            return m_Characters.Count;
        }
    }

    public int localCharactersCount
    {
        get
        {
            return m_LocalCharacters.Count;
        }
    }

    public GameObject GetCharacterByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Characters.Count)
        {
            return null;
        }

        return m_Characters[i_Index];
    }

    public GameObject GetCharacterById(int i_CharacterId)
    {
        for (int characterIndex = 0; characterIndex < m_Characters.Count; ++characterIndex)
        {
            GameObject character = m_Characters[characterIndex];

            if (character == null)
                continue;

            tnCharacterInfo characterInfo = character.GetComponent<tnCharacterInfo>();

            if (characterInfo == null)
                continue;

            int characterId = characterInfo.characterId;

            if (characterId == i_CharacterId)
            {
                return character;
            }
        }

        return null;
    }

    public GameObject GetLocalCharacterByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_LocalCharacters.Count)
        {
            return null;
        }

        return m_LocalCharacters[i_Index];
    }

    public GameObject GetLocalCharacterById(int i_CharacterId)
    {
        for (int characterIndex = 0; characterIndex < m_LocalCharacters.Count; ++characterIndex)
        {
            GameObject character = m_LocalCharacters[characterIndex];

            if (character == null)
                continue;

            tnCharacterInfo characterInfo = character.GetComponent<tnCharacterInfo>();

            if (characterInfo == null)
                continue;

            int characterId = characterInfo.characterId;

            if (characterId == i_CharacterId)
            {
                return character;
            }
        }

        return null;
    }

    // Results

    public tnCharacterResults GetCharacterResultsByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_CharactersResults.count)
        {
            return null;
        }

        return m_CharactersResults.GetByIndex(i_Index);
    }

    public tnCharacterResults GetCharacterResultsByPosition(int i_Position)
    {
        if (i_Position < 0 || i_Position >= m_CharactersResults.count)
        {
            return null;
        }

        return m_CharactersResults.GetByPosition(i_Position);
    }

    public tnCharacterResults GetCharacterResultsById(int i_Id)
    {
        for (int resultIndex = 0; resultIndex < m_CharactersResults.count; ++resultIndex)
        {
            tnCharacterResults characterResults = m_CharactersResults.GetByIndex(resultIndex);
            if (characterResults != null && characterResults.id == i_Id)
            {
                return characterResults;
            }
        }

        return null;
    }

    // AI

    public int GetAICount(int i_TeamIndex)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= teamsCount)
        {
            return 0;
        }

        List<tnAIInputFiller> aiList = m_LocalAI[i_TeamIndex];

        if (aiList == null)
        {
            return 0;
        }

        return aiList.Count;
    }

    public tnAIInputFiller GetAI(int i_TeamIndex, int i_Index)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= teamsCount)
        {
            return null;
        }

        List<tnAIInputFiller> aiList = m_LocalAI[i_TeamIndex];

        if (aiList == null || (i_Index < 0 || i_Index >= aiList.Count))
        {
            return null;
        }

        return aiList[i_Index];
    }

    // Teams

    public int teamsCount
    {
        get
        {
            return m_Teams.Count;
        }
    }

    public int GetTeamSize(int i_TeamIndex)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= m_Teams.Count)
        {
            return 0;
        }

        List<GameObject> team = m_Teams[i_TeamIndex];

        if (team == null)
        {
            return 0;
        }

        return team.Count;
    }

    public int GetTeamId(int i_TeamIndex)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= m_TeamsIds.Count)
        {
            return Hash.s_NULL;
        }

        return m_TeamsIds[i_TeamIndex];
    }

    public GameObject GetCharacter(int i_TeamIndex, int i_CharacterIndex)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= m_Teams.Count)
        {
            return null;
        }

        List<GameObject> team = m_Teams[i_TeamIndex];

        if (team == null)
        {
            return null;
        }

        if (i_CharacterIndex < 0 || i_CharacterIndex >= team.Count)
        {
            return null;
        }

        return team[i_CharacterIndex];
    }

    public tnTeamResults GetTeamResultsByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_TeamsResults.count)
        {
            return null;
        }

        return m_TeamsResults.GetByIndex(i_Index);
    }

    public tnTeamResults GetTeamResultsByPosition(int i_Position)
    {
        if (i_Position < 0 || i_Position >= m_TeamsResults.count)
        {
            return null;
        }

        return m_TeamsResults.GetByPosition(i_Position);
    }

    public tnTeamResults GetTeamResultsById(int i_Id)
    {
        for (int resultIndex = 0; resultIndex < m_TeamsResults.count; ++resultIndex)
        {
            tnTeamResults teamResults = m_TeamsResults.GetByIndex(resultIndex);
            if (teamResults != null && teamResults.id == i_Id)
            {
                return teamResults;
            }
        }

        return null;
    }

    // INTERNALS

    private void CreatePlayerInputCollector()
    {
        GameObject playerInputCollectorGo = new GameObject("PlayerInputCollector");
        tnPlayerInputCollector playerInputCollector = playerInputCollectorGo.AddComponent<tnPlayerInputCollector>();

        TrueSyncObject trueSyncObject = playerInputCollectorGo.AddComponent<TrueSyncObject>();
        TSPlayerInfo localPlayerInfo = TrueSyncManager.localPlayerMain;
        byte localPlayerId = (localPlayerInfo != null) ? localPlayerInfo.Id : (byte)0;
        trueSyncObject.SetOwnerId(localPlayerId);

        TrueSyncManager.RegisterTrueSyncObjectMain(trueSyncObject);

        m_PlayerInputCollector = playerInputCollector;
    }

    private void CreateCharacters()
    {
        // Cache character prefab path.

        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
        if (matchSettingsModule != null)
        {
            int gameModeId = matchSettingsModule.gameModeId;
            m_GameModeId = gameModeId;
            tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);

            if (gameModeData != null)
            {
                m_CharacterPrefab = gameModeData.LoadAndGetCharacterPrefabPath();
            }
        }

        // Create teams.

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
        {
            List<GameObject> team = new List<GameObject>();
            m_Teams.Add(team);
        }

        for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
        {
            List<tnAIInputFiller> aiList = new List<tnAIInputFiller>();
            m_LocalAI.Add(aiList);
        }

        for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
        {
            tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

            if (teamDescription == null)
                continue;

            ProceesTeam(teamIndex, teamDescription);
        }

        // Disable Input

        DisableInput();
    }

    private void ProceesTeam(int i_TeamIndex, tnTeamDescription i_TeamDescription)
    {
        if (i_TeamDescription == null)
            return;

        int teamId = i_TeamDescription.teamId;
        tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);

        if (teamData == null)
            return;

        m_TeamsIds.Add(teamId);

        // Get team size.

        int teamSize = i_TeamDescription.charactersCount;

        // Create team results.

        tnTeamResults teamResults = CreateTeamResults(teamId);
        m_TeamsResults.Add(teamResults);

        StateTracker.AddTracking(teamResults);

        // Callback.

        OnCreateTeam(i_TeamIndex, i_TeamDescription);

        for (int characterIndex = 0; characterIndex < teamSize; ++characterIndex)
        {
            tnCharacterDescription characterDescription = (PhotonNetwork.offlineMode) ? i_TeamDescription.GetCharacterDescriptionBySpawnOrder(characterIndex) : i_TeamDescription.GetCharacterDescription(characterIndex);

            if (characterDescription == null)
                continue;

            int disconnectedPlayerPhotonId = -1;
            int photonPlayerId;
            if (PhotonNetwork.offlineMode)
            {
                photonPlayerId = 0; // 0 is the default value related to a fake "Local_Player". See TrueSyncController::Initialize.
            }
            else
            {
                int onlinePlayerIndex = characterDescription.onlinePlayerIndex;
                tnGameModulesUtils.GetPhotonPlayerOwnerId(onlinePlayerIndex, out photonPlayerId);

                // Check if the owner of this character is still connected. If not, reset photon player id.

                PhotonPlayer owner = PhotonUtils.GetPhotonPlayer(photonPlayerId);
                disconnectedPlayerPhotonId = (owner == null) ? photonPlayerId : -1;
                photonPlayerId = (owner != null) ? photonPlayerId : -1;
            }

            // If photon player id is valid, spawn character.

            if (photonPlayerId >= 0)
            {
                SpawnCharacter(i_TeamIndex, teamSize, characterIndex, photonPlayerId, characterDescription);
            }
            else
            {
                Debug.LogWarning("[tnMatchController] Character skipped for photon player id " + disconnectedPlayerPhotonId + ".");
            }
        }
    }

    private void SpawnCharacter(int i_TeamIndex, int i_TeamSize, int i_SpawnIndex, int i_PhotonPlayerId, tnCharacterDescription i_CharacterDescription)
    {
        if (m_CharacterPrefab == null || i_CharacterDescription == null)
            return;

        int descriptorCharacterId = i_CharacterDescription.characterId;
        int descriptorOnlinePlayerIndex = i_CharacterDescription.onlinePlayerIndex;
        int descriptorPlayerId = i_CharacterDescription.playerId;

        string[] spawnPointsNames = SpawnPoints.GetSpawnPoints(i_TeamIndex, i_TeamSize);

        if (spawnPointsNames == null)
            return;

        if (i_SpawnIndex < 0 || i_SpawnIndex >= spawnPointsNames.Length)
            return;

        string spawnPointName = spawnPointsNames[i_SpawnIndex];
        GameObject spawnPointGo = GameObject.Find(spawnPointName);

        if (spawnPointGo == null)
            return;

        TSTransform2D spawnPoint = spawnPointGo.GetComponent<TSTransform2D>();

        if (spawnPoint == null)
            return;

        tnCharacterData characterData = tnGameData.GetCharacterDataMain(descriptorCharacterId);

        if (characterData == null)
            return;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        tnTeamDescription teamDescription = teamsModule.GetTeamDescription(i_TeamIndex);

        if (teamDescription == null)
            return;

        int teamId = teamDescription.teamId;
        Color teamColor = teamDescription.teamColor;

        tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);

        if (teamData == null)
            return;

        bool isLocal = (PhotonNetwork.offlineMode) ? true : tnGameModulesUtils.IsLocalPlayer(descriptorOnlinePlayerIndex);
        bool isHuman = (PhotonNetwork.offlineMode) ? (descriptorPlayerId != Hash.s_NULL) : (descriptorOnlinePlayerIndex >= 0);

        Vector3 spawnPosition = spawnPoint.position.ToVector();
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, spawnPoint.rotation.AsFloat());

        // Spawn character.

        GameObject characterInstance = Instantiate<GameObject>(m_CharacterPrefab);
        characterInstance.name = characterData.displayName;

        characterInstance.transform.position = spawnPosition;
        characterInstance.transform.rotation = spawnRotation;

        // Configure TSTransform

        TSTransform2D tsTransform = characterInstance.GetComponent<TSTransform2D>();
        if (tsTransform != null)
        {
            tsTransform.position = spawnPoint.position;
            tsTransform.rotation = spawnPoint.rotation;
        }

        // Configure depth2d.

        tnDepth2d depth2d = characterInstance.GetComponent<tnDepth2d>();
        if (depth2d != null)
        {
            depth2d.SetOffset(spawnPointGo.transform.position.z);
        }

        // Configure character stats database.

        tnStatsDatabase teamStats = teamData.teamStats;

        tnStatsContainer statsContainer = characterInstance.GetComponent<tnStatsContainer>();
        if (statsContainer != null)
        {
            statsContainer.SetStatsDatabase(teamStats);
        }

        // Configure character view.

        tnCharacterViewController characterViewController = characterInstance.GetComponent<tnCharacterViewController>();
        if (characterViewController != null)
        {
            // Base color.

            characterViewController.SetBaseColor(teamColor);

            // Charging force bar.

            characterViewController.SetChargingForceBarColor(teamColor);

            // Energy bar.

            characterViewController.SetEnergyBarColor(teamColor);

            // Flag.

            characterViewController.SetFlagSprite(teamData.baseSprite);

            // Animator

            characterViewController.SetAnimatorController(characterData.animatorController);

            // Set facing right.

            characterViewController.SetFacingRight((spawnPoint.position.x < 0f));

            // Player color.

            characterViewController.TurnOffColor();
            characterViewController.SetArrowVisible(false);
            characterViewController.SetArrowColor(Color.white);

            if (isLocal)
            {
                if (PhotonNetwork.offlineMode)
                {
                    if (isHuman)
                    {
                        tnPlayerData playerData = tnGameData.GetPlayerDataMain(descriptorPlayerId);
                        if (playerData != null)
                        {
                            Color playerColor = playerData.color;

                            characterViewController.SetPlayerColor(playerColor);

                            characterViewController.SetArrowVisible(true);
                            characterViewController.SetArrowColor(playerColor);
                        }
                    }
                }
                else
                {
                    List<int> onlinePlayersKeys = tnGameData.GetOnlinePlayersKeysMain();
                    if (onlinePlayersKeys != null)
                    {
                        if (descriptorOnlinePlayerIndex >= 0 && descriptorOnlinePlayerIndex < onlinePlayersKeys.Count)
                        {
                            int onlinePlayerKey = onlinePlayersKeys[descriptorOnlinePlayerIndex];
                            tnOnlinePlayerData onlinePlayerData = tnGameData.GetOnlinePlayerDataMain(onlinePlayerKey);
                            if (onlinePlayerData != null)
                            {
                                Color playerColor = onlinePlayerData.color;

                                characterViewController.SetPlayerColor(playerColor);

                                characterViewController.SetArrowVisible(true);
                                characterViewController.SetArrowColor(playerColor);
                            }
                        }
                    }
                }
            }
        }

        // Input: NOTE that current aiFacotry assumes that all AI are handled by the same client.
        // If you want to support AI in multiplayer you should change the ai factory implementation.
        // Now multiplayer isn't implemented, so now, if you're using AI, you are playing offline --> All AIs are yours.

        if (isLocal)
        {
            tnInputFiller inputFiller = null;
            tnRumbleController rumbleController = null;

            int localPlayerIndex;
            bool localPlayerIndexFound = tnGameModulesUtils.OnlineToLocalPlayerIndex(descriptorOnlinePlayerIndex, out localPlayerIndex);
            if (localPlayerIndexFound || PhotonNetwork.offlineMode)
            {
                tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

                int playerId = (PhotonNetwork.offlineMode) ? descriptorPlayerId : ((localPartyModule != null) ? localPartyModule.GetPlayerId(localPlayerIndex) : Hash.s_NULL);
                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

                if (playerData != null)
                {
                    string playerInputName = playerData.playerInputName;
                    string wifiPlayerInputName = playerData.wifiPlayerInputName;

                    PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerInputName);
                    WiFiPlayerInput wifiPlayerInput = WiFiInputSystem.GetPlayerByNameMain(wifiPlayerInputName);

                    if (playerInput != null)
                    {
                        inputFiller = new tnPlayerInputFiller(playerInput);
                        rumbleController = new tnRumbleController(playerInput);

                        m_LocalPlayersInput.Add(playerInput);
                    }
                    else
                    {
                        if (wifiPlayerInput != null)
                        {
                            inputFiller = new tnWiFiPlayerInputFiller(wifiPlayerInput);

                            m_LocalWifiPlayersInput.Add(wifiPlayerInput);
                        }
                    }
                }
                else
                {
                    tnAIInputFiller aiInputFiller = CreateAIInputFiller(i_TeamIndex, i_SpawnIndex, characterInstance);
                    inputFiller = aiInputFiller;

                    List<tnAIInputFiller> aiList = m_LocalAI[i_TeamIndex];
                    aiList.Add(aiInputFiller);
                }
            }

            // Bind input filler to character instance.

            if (inputFiller != null)
            {
                tnInputController inputController = new tnInputController(inputFiller);
                inputController.SetRumbleController(rumbleController);

                AddInputController(inputController);

                tnCharacterInput characterInput = characterInstance.GetComponent<tnCharacterInput>();
                if (characterInput != null)
                {
                    characterInput.Bind(inputController);
                }
            }

            // Add rumble component.

            if (isHuman)
            {
                tnRumbleParams rumbleParams = Resources.Load<tnRumbleParams>(s_RumbleParams_ResourcePath);

                if (rumbleParams != null)
                {
                    tnRumble rumbleComponent = characterInstance.GetComponent<tnRumble>();
                    if (rumbleComponent == null)
                    {
                        rumbleComponent = characterInstance.AddComponent<tnRumble>();
                    }

                    rumbleComponent.SetParams(rumbleParams);
                }
            }

            // Input Delay.

            int delay = (TrueSyncManager.isOfflineMain) ? m_OfflinePlayerInputDelay : 0;

            if (!isHuman)
            {
                tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
                int aiLevelIndex = (matchSettingsModule != null) ? matchSettingsModule.aiLevelIndex : (tnGameData.aiLevelCountMain / 2);
                tnAILevel aiLevel = tnGameData.GetAILevelMain(aiLevelIndex);
                if (aiLevel != null)
                {
                    delay = aiLevel.inputDelay;
                }
            }

            // Register on player input collector.

            RegisterObjectOnInputCollector(characterInstance, delay);
        }

        // Configure character info.

        tnCharacterInfo characterInfo = characterInstance.GetComponent<tnCharacterInfo>();
        if (characterInfo == null)
        {
            characterInfo = characterInstance.AddComponent<tnCharacterInfo>();
        }

        int characterIndex = m_Characters.Count;

        characterInfo.SetCharacterIndex(characterIndex);
        characterInfo.SetCharacterId(descriptorCharacterId);
        characterInfo.SetTeamIndex(i_TeamIndex);
        characterInfo.SetTeamId(teamId);
        characterInfo.SetTeamColor(teamColor);

        // Callback.

        OnCharacterSpawned(i_TeamIndex, characterIndex, characterInstance);

        // Add characters to lists.

        m_Characters.Add(characterInstance);
        if (isLocal)
        {
            m_LocalCharacters.Add(characterInstance);
        }

        List<GameObject> team = m_Teams[i_TeamIndex];
        team.Add(characterInstance);

        // Create character result.

        tnCharacterResults characterResults = CreateCharacterResults(descriptorCharacterId);
        characterResults.isHuman = isHuman;

        tnTeamResults teamResults = GetTeamResultsByIndex(i_TeamIndex);
        if (teamResults != null)
        {
            teamResults.AddCharacterResults(characterResults);
        }

        m_CharactersResults.Add(characterResults);

        // Track character result.

        StateTracker.AddTracking(characterResults);

        // Configure TrueSyncObject.

        TrueSyncObject trueSyncObject = characterInstance.GetComponent<TrueSyncObject>();
        if (trueSyncObject != null)
        {
            trueSyncObject.SetOwnerId(i_PhotonPlayerId);
            TrueSyncManager.RegisterTrueSyncObjectMain(trueSyncObject);
        }
    }

    private void SetupMatch()
    {
        Debug.Log("[tnMultiplayerMatchController] Setup Match");

        OnSetupMatch();
    }

    private void StartMatch()
    {
        if (m_MatchStarted || m_MatchEnded)
            return;

        Debug.Log("[tnMultiplayerMatchController] Start Match");

        // Set match as started.

        m_MatchStarted = true;

        // Start true sync simulation.

        TrueSyncManager.StartSimulationMain();

        // Callback.

        OnStartMatch();
    }

    private void EndMatch()
    {
        if (!m_MatchStarted || m_MatchEnded)
            return;

        Debug.Log("[tnMultiplayerMatchController] End Match.");

        // Set match as endend.

        m_MatchEnded = true;

        // Unpause.

        Unpause();

        // End true sync simulation.

        //TrueSyncManager.EndSimulationMain();
        m_SimulationEnded = true;

        // TODO: Sort results.

        // Callback.

        OnEndMatch();
    }

    private void PauseMatch()
    {
        if (m_Paused)
            return;

        Debug.Log("[MatchController] Pause.");

        m_Paused = true;

        // If offline, pause simulation.

        if (PhotonNetwork.offlineMode)
        {
            TrueSyncManager.PauseSimulationMain();
        }

        // Callback.

        OnPauseMatch();

        // Raise event.

        if (m_MatchPausedEvent != null)
        {
            m_MatchPausedEvent();
        }
    }

    private void UnpauseMatch()
    {
        if (!m_Paused)
            return;

        Debug.Log("[MatchController] Unpause.");

        m_Paused = false;

        // If offline, resume simulation.

        if (PhotonNetwork.offlineMode)
        {
            TrueSyncManager.RunSimulationMain();
        }

        // Callback.

        OnUnpauseMatch();

        // Raise event.

        if (m_MatchUnpausedEvent != null)
        {
            m_MatchUnpausedEvent();
        }
    }

    // PROTECTED

    protected void AddInputController(tnInputController i_InputController)
    {
        if (i_InputController == null)
            return;

        m_InputManager.AddController(i_InputController);
    }

    protected void RequestStartGame()
    {
        RaiseStartGameRequestEvent();
    }

    protected void RequestEndGame()
    {
        RaiseEndGameRequestedEvent();
    }

    protected void FinishGame()
    {
        RaiseFinishGameEvent();
    }

    protected void RegisterObjectOnInputCollector(GameObject i_Go, int i_Delay)
    {
        if (i_Go == null)
            return;

        int delay = Mathf.Max(0, i_Delay);
        m_PlayerInputCollector.RegisterGameObject(i_Go, delay);
    }

    // PROTECTED VIRTUAL

    protected virtual void OnPreInit()
    {

    }

    protected virtual void OnInitialize()
    {

    }

    protected virtual void OnPostInit()
    {

    }

    /*
     * On setup match should call the protected method RequestStartGame();
     */
    protected virtual void OnSetupMatch()
    {

    }

    protected virtual void OnStartMatch()
    {

    }

    /*
     * On end match should call the protected method FinishGame(); 
     */
    protected virtual void OnEndMatch()
    {

    }

    protected virtual void SyncedInput()
    {

    }

    protected virtual void SyncedStart()
    {

    }

    protected virtual void LateSyncedUpdate()
    {

    }

    protected virtual void OnPlayerDisconnected(int i_PlayerId)
    {

    }

    protected virtual void OnPauseMatch() { }

    protected virtual void OnUnpauseMatch() { }

    protected virtual bool CanPauseMatch() { return true; } // NOTE: You should override this. 

    protected virtual void OnMatchBecomeInvalid()
    {

    }

    protected virtual void OnCreateTeam(int i_TeamIndex, tnTeamDescription i_TeamDescription)
    {

    }

    protected virtual void OnConfigureAI()
    {

    }

    protected virtual void OnConfigureStats()
    {

    }

    protected virtual string[] GetSpawnPoints(int i_TeamIndex, int i_TeamSize)
    {
        return SpawnPoints.GetSpawnPoints(i_TeamIndex, i_TeamSize);
    }

    protected virtual tnAIInputFiller CreateAIInputFiller(int i_TeamIndex, int i_Index, GameObject i_Character)
    {
        return new tnAINullInputFiller(i_Character);
    }

    protected virtual tnCharacterResults CreateCharacterResults(int i_Id)
    {
        return new tnCharacterResults(i_Id); // NOTE: You should override this.
    }

    protected virtual tnTeamResults CreateTeamResults(int i_Id)
    {
        return new tnTeamResults(i_Id); // NOTE: You should ovrride this.
    }

    protected virtual Comparison<tnTeamResults> GetSortDelegate()
    {
        return NullSort;
    }

    protected virtual void OnCharacterSpawned(int i_TeamIndex, int i_CharacterIndex, GameObject i_Character)
    {

    }

    protected virtual void OnClearUI()
    {

    }

    // UTILS

    protected Color[] GetTeamColors(int i_TeamId)
    {
        tnTeamData teamData = tnGameData.GetTeamDataMain(i_TeamId);
        if (teamData != null)
        {
            Color[] teamColors = new Color[2]; // First/Second color.

            teamColors[0] = teamData.supportersFirstColor;
            teamColors[1] = teamData.supportersSecondColor;

            return teamColors;
        }

        return null;
    }

    // EVENTS

    private void RaiseStartGameRequestEvent()
    {
        if (m_StartGameRequestedEvent != null)
        {
            m_StartGameRequestedEvent();
        }
    }

    private void RaiseEndGameRequestedEvent()
    {
        if (m_EndGameRequestedEvent != null)
        {
            m_EndGameRequestedEvent();
        }
    }

    private void RaiseFinishGameEvent()
    {
        if (m_FinishGameEvent != null)
        {
            m_FinishGameEvent();
        }
    }

    // FUNCTORS

    private static int NullSort(tnTeamResults i_Res1, tnTeamResults i_Res2)
    {
        return 0;
    }
}