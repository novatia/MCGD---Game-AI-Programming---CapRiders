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

public enum tnMenuGameState
{
    None = 0,

    Setup = 1,
    Initialize = 2,

    ReadyToStart = 3,

    Game = 4,
}

[fiInspectorOnly]
public class tnMenuGameFsm : tnGameFSM<tnMenuGameState>
{
    // STATIC

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

    // FIELDS

    private int m_Seed = 0;

    private GameObject m_GameCameraGO = null;

    private TrueSyncManager m_TrueSyncManager = null;
    private tnMatchController m_MatchController = null;

    private ObjectPoolController m_ObjectPoolController = null;

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Get Component.

        m_ObjectPoolController = GetComponent<ObjectPoolController>();
    }

    // tnFSM's interface

    protected override tnMenuGameState startingState
    {
        get
        {
            return tnMenuGameState.Setup;
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

        fsm.ChangeState(tnMenuGameState.Initialize);
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

            matchControllerInstance.SetCamera(m_GameCameraGO);
        }

        m_MatchController = matchControllerInstance;

        if (m_TrueSyncManager != null)
        {
            m_TrueSyncManager.RegisterTrueSyncObject(m_MatchController.gameObject);
        }
    }

    // Initialize

    private void Initialize_Enter()
    {
        Debug.Log("[Initialize] Enter");

        if (m_MatchController != null)
        {
            m_MatchController.Initialize();
        }

        fsm.ChangeState(tnMenuGameState.ReadyToStart);
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
        fsm.ChangeState(tnMenuGameState.Game);
    }

    // Game

    private void Game_Enter()
    {
        Debug.Log("[Game] Enter");

        // Start game on match controller.

        if (m_MatchController != null)
        {
            m_MatchController.OnStartGame();
        }

        // Notify external FSMs.

        PlayMakerFSM.BroadcastEvent("GAME / START");
    }

    private void Game_Update()
    {
        // Nothing to do.
    }

    private void Game_Exit()
    {
        Debug.Log("[Game] Exit");
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