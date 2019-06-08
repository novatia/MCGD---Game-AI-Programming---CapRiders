using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using FullInspector;

using BaseMatchEvents;

using TrueSync;

[fiInspectorOnly]
public class tnBaseMatchController : tnMatchController
{
    // STATIC

    public static string[] s_GoalSpawnPoints = new string[]
    {
        "Spawn_Goal_0",
        "Spawn_Goal_1",
    };

    private static string s_PreFieldReset_MessengerEvent = "PreFieldReset";
    private static string s_FieldReset_MessengerEvent = "FieldReset";
    private static string s_KickOff_MessengerEvent = "KickOff";
    private static string s_ValidatedGoal_MessengerEvent = "ValidatedGoal";
    private static string s_Goal_MessengerEvent = "Goal";

    private static string s_BallSpawnPoint = "Spawn_Ball";
    private static string s_RefereeSpawnPoint = "Spawn_Referee";

    private static string s_ResourcePath_Referee = "Characters/Common/p_Referee";

    private static string s_TopLeft_Corner = "NULL_CornerTopLeft";
    private static string s_BottomRight_Corner = "NULL_CornerBottomRight";
    private static string s_Midfield = "NULL_Midfield";

    private static string s_GoalTop = "NULL_GoalTop";
    private static string s_GoalBottom = "NULL_GoalBottom";

    private static string s_AreaTop = "NULL_AreaTop";
    private static string s_AreaBottom = "NULL_AreaBottom";

    private static string s_TopLeft_Bound = "NULL_BoundTopLeft";
    private static string s_BottomRight_Bound = "NULL_BoundBottomRight";

    // Serializable fields

    [InspectorHeader("Game")]

    [SerializeField]
    [InspectorCategory("GAME")]
    [DisallowEditInPlayMode]
    private bool m_InfiniteTime = false;

    [SerializeField]
    [InspectorCategory("GAME")]
    private float m_CelebrationDuration = 4f;
    [SerializeField]
    [InspectorCategory("GAME")]
    private float m_WaitBetweenResetAndGo = 2f;

    // Serializable fields

    [InspectorHeader("Graphics")]

    [SerializeField]
    [InspectorCategory("GRAPHICS")]
    private bool m_CreateSupporters = true;

    // Fields

    private FP m_MatchDuration = 0f;

    [AddTracking]
    protected FP m_MatchTime = 0f;

    [AddTracking]
    protected bool m_Celebrating = false;
    [AddTracking]
    protected FP m_CelebrationTimer = 0f;

    [AddTracking]
    protected bool m_WaitingForGo = false;
    [AddTracking]
    protected FP m_WaitForGoTimer = 0f;

    [AddTracking]
    protected bool m_Waiting = true;

    private bool m_GoldenGoalEnabled = false;

    [AddTracking]
    protected bool m_GoldenGoal = false;

    private tnBall m_Ball = null;
    private List<tnGoal> m_Goals = new List<tnGoal>();

    private GameObject m_RefereeInstance = null;
    private GameObject m_MidfieldGo = null;

    private tnGoalEventParams m_GoalParamsCache;

    [AddTracking]
    protected int m_GoalTick = 0;
    [AddTracking]
    protected int m_EndGameRequestTick = 0;
    [AddTracking]
    protected int m_KickOffRequestTick = 0;
    [AddTracking]
    protected int m_ResetFieldRequestTick = 0;

    private Coroutine m_EndGameSequenceCoroutine = null;

    // Match config cache.

    private int m_BallId = Hash.s_NULL;
    private int m_StadiumId = Hash.s_NULL;
    private bool m_HasReferee = false;

    // Components

    private tnSlowMotionController m_SlowMotionController = null;
    
    // ACCESSORS
    
    public bool goldenGoalEnabled
    {
        get
        {
            return m_GoldenGoalEnabled;
        }
    }

    public bool goldenGoal
    {
        get
        {
            return m_GoldenGoal;
        }
    }

    public bool hasReferee
    {
        get
        {
            return m_HasReferee;
        }
    }

    public int ballId
    {
        get
        {
            return m_BallId;
        }
    }

    public int stadiumId
    {
        get
        {
            return m_StadiumId;
        }
    }

    public tnBall ball
    {
        get
        {
            return m_Ball;
        }
    }

    public GameObject referee
    {
        get
        {
            return m_RefereeInstance;
        }
    }

    public int goalsCount
    {
        get
        {
            return m_Goals.Count;
        }
    }

    public tnGoal GetGoal(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Goals.Count)
        {
            return null;
        }

        return m_Goals[i_Index];
    }

    public Vector3 midfieldPosition
    {
        get
        {
            if (m_MidfieldGo != null)
            {
                return m_MidfieldGo.transform.position;
            }

            return Vector3.zero;
        }
    }

    public FP matchTime
    {
        get
        {
            return m_MatchTime;
        }
    }

    public FP remainingTime
    {
        get
        {
            return MathFP.Max(0, m_MatchDuration - m_MatchTime);
        }
    }

    public bool infiniteTime
    {
        get { return m_InfiniteTime; }
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Get references.

        m_MidfieldGo = GameObject.Find(s_Midfield);
        m_SlowMotionController = GetComponent<tnSlowMotionController>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Messenger.AddListener<tnGoalEventParams>(s_Goal_MessengerEvent, OnGoalEvent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        StopAllCoroutines();

        Messenger.RemoveListener<tnGoalEventParams>(s_Goal_MessengerEvent, OnGoalEvent);
    }

    // tnMultiplayerMatchController's interface

    protected override void OnPreInit()
    {
        base.OnPreInit();

        CreateSupporters();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        ConfigureMatch();

        SpawnGoals();
        SpawnBall();

        SpawnReferee();

        ConfigureHoles();
    }

    protected override void OnStartMatch()
    {
        base.OnStartMatch();
    }

    protected override void OnEndMatch()
    {
        base.OnEndMatch();

        // Disable input.

        DisableInput();

        // Set waiting.

        m_Waiting = true;

        // Force slow motion off.

        SetSlowMotionEnabled(false);

        // Run end game sequence.

        if (m_EndGameSequenceCoroutine == null)
        {
            IEnumerator endSequence = RunEndMatchSequence();
            m_EndGameSequenceCoroutine = StartCoroutine(endSequence);
        }
    }

    protected override void SyncedInput()
    {
        base.SyncedInput();
    }

    protected override void SyncedStart()
    {
        base.SyncedStart();

        KickOff();
    }

    protected override void LateSyncedUpdate()
    {
        base.LateSyncedUpdate();

        // Get delta time.

        FP deltaTime = TrueSyncManager.deltaTimeMain;
        int rollbackWindow = Mathf.Max(1, TrueSyncManager.rollbackWindowMain); // Max handle cases with zero rollback.
        int currentTick = TrueSyncManager.ticksMain;

        // Check for end game request.

        if (m_EndGameRequestTick > 0)
        {
            if (currentTick == m_EndGameRequestTick + rollbackWindow)
            {
                m_EndGameRequestTick = 0;
                RequestEndGame();
            }

            return;
        }

        // Check for kick off.

        if (m_Waiting)
        {
            if (m_KickOffRequestTick > 0)
            {
                if (currentTick == m_KickOffRequestTick + rollbackWindow)
                {
                    Debug.Log("Kick off: " + TrueSyncManager.ticksMain + " (" + m_KickOffRequestTick + ")");

                    m_KickOffRequestTick = 0;

                    // Kick off.

                    KickOff();

                    // Enable player behaviours

                    SetPlayerBehavioursEnabled(true);
                }
            }

            // Check waiting for go.

            if (m_WaitingForGo)
            {
                m_WaitForGoTimer -= deltaTime;
                if (m_WaitForGoTimer < FP.Zero)
                {
                    m_WaitForGoTimer = FP.Zero;
                    m_WaitingForGo = false;

                    // Set ready for kick off.

                    m_KickOffRequestTick = TrueSyncManager.ticksMain;
                }
            }

            if (m_ResetFieldRequestTick > 0)
            {
                if (currentTick == m_ResetFieldRequestTick + rollbackWindow)
                {
                    Debug.Log("Reset field: " + TrueSyncManager.ticksMain + " (" + m_ResetFieldRequestTick + ")");

                    m_ResetFieldRequestTick = 0;

                    // Disable input.

                    DisableInput();

                    // Reset field.

                    ResetField();

                    // Disable player behaviours

                    SetPlayerBehavioursEnabled(false);

                    // Reset camera position.

                    tnGameCamera gameCamera = (cameraGo != null) ? cameraGo.GetComponent<tnGameCamera>() : null;
                    if (gameCamera != null)
                    {
                        gameCamera.ResetPosition();
                    }

                    // Set waiting for go.

                    m_WaitingForGo = true;
                    m_WaitForGoTimer = m_WaitBetweenResetAndGo;
                }
            }

            // Check celbration.

            if (m_Celebrating)
            {
                // Check timer.

                m_CelebrationTimer -= deltaTime;
                if (m_CelebrationTimer < FP.Zero)
                {
                    m_CelebrationTimer = FP.Zero;
                    m_Celebrating = false;

                    // Check if game is ended or it should be continue.

                    if (m_GoldenGoal)
                    {
                        // Game is ended.

                        m_EndGameRequestTick = TrueSyncManager.ticksMain;
                    }
                    else
                    {
                        // Request reset field.

                        m_ResetFieldRequestTick = TrueSyncManager.ticksMain;
                    }
                }
            }

            return;
        }

        // Check for goal.

        if (m_GoalTick > 0)
        {
            if (currentTick == m_GoalTick + rollbackWindow)
            {
                // Reset goal tick.

                m_GoalTick = 0;

                // Disable slow motion controller.

                SetSlowMotionEnabled(false);

                // Stop.

                m_Waiting = true;

                m_Celebrating = true;
                m_CelebrationTimer = m_CelebrationDuration;

                // Disable AI input (local).

                DisableAIInput();

                // Callback.

                OnGoal(m_GoalParamsCache);

                // Raise event.

                Messenger.Broadcast<tnGoalEventParams>(s_ValidatedGoal_MessengerEvent, m_GoalParamsCache);

                return;
            }
        }

        if (m_InfiniteTime)
            return;

        // Update match timer.

        m_MatchTime += deltaTime;

        // Check match time for ending.

        if (m_MatchTime > m_MatchDuration)
        {
            if (!Draw())
            {
                m_EndGameRequestTick = TrueSyncManager.ticksMain;
            }
            else
            {
                if (!m_GoldenGoalEnabled)
                {
                    m_EndGameRequestTick = TrueSyncManager.ticksMain;
                }
                else
                {
                    if (!m_GoldenGoal)
                    {
                        m_GoldenGoal = true;

                        OnGoldenGoalStart();
                    }
                }
            }
        }
    }

    protected override void OnMatchBecomeInvalid()
    {
        base.OnMatchBecomeInvalid();

        // Disable input.

        DisableInput();

        // Set waiting.

        m_Waiting = true;
    }

    protected override bool CanPauseMatch()
    {
        return !m_Waiting;
    }

    protected override void OnPauseMatch()
    {
        base.OnPauseMatch();

        // Disable input.

        if (PhotonNetwork.offlineMode)
        {
            DisableInput();
        }
        else
        {
            DisableHumanInput();
        }
    }

    protected override void OnUnpauseMatch()
    {
        base.OnUnpauseMatch();

        // Enable input.

        if (!m_Waiting && !m_MatchEnded)
        {
            EnableInput();
        }
    }

    protected override void OnConfigureAI()
    {
        base.OnConfigureAI();

        GameObject topLeft = GameObject.Find(s_TopLeft_Corner);
        GameObject bottomRight = GameObject.Find(s_BottomRight_Corner);

        GameObject midfield = GameObject.Find(s_Midfield);

        GameObject goalTop = GameObject.Find(s_GoalTop);
        GameObject goalBottom = GameObject.Find(s_GoalBottom);

        GameObject areaTop = GameObject.Find(s_AreaTop);
        GameObject areaBottom = GameObject.Find(s_AreaBottom);

        for (int aiTeamIndex = 0; aiTeamIndex < teamsCount; ++aiTeamIndex)
        {
            int aiCount = GetAICount(aiTeamIndex);

            for (int aiIndex = 0; aiIndex < aiCount; ++aiIndex)
            {
                tnBaseAIInputFiller aiInputFiller = (tnBaseAIInputFiller)GetAI(aiTeamIndex, aiIndex);

                if (aiInputFiller != null)
                {
                    tnBaseAIData aiData = new tnBaseAIData();

                    // Fill teammates and opponents.

                    {
                        for (int teamIndex = 0; teamIndex < teamsCount; ++teamIndex)
                        {
                            int teamSize = GetTeamSize(teamIndex);

                            for (int characterIndex = 0; characterIndex < teamSize; ++characterIndex)
                            {
                                GameObject characterInstance = GetCharacter(teamIndex, characterIndex);
                                if (characterInstance != null)
                                {
                                    if (teamIndex == aiTeamIndex) // Teammate.
                                    {
                                        aiData.AddMyTeamCharacter(characterInstance.transform);
                                    }
                                    else // Opponent.
                                    {
                                        aiData.AddOpponentTeamCharacter(characterInstance.transform);
                                    }
                                }
                            }
                        }
                    }

                    // Fill ball.

                    if (m_Ball != null)
                    {
                        aiData.SetBall(m_Ball.transform);
                    }

                    // Fill goal.

                    if (m_Goals != null)
                    {
                        if (aiTeamIndex < m_Goals.Count)
                        {
                            tnGoal goal = m_Goals[aiTeamIndex];
                            aiData.SetGoal(goal.transform);
                        }
                    }

                    // Fill opponent goal.

                    if (m_Goals != null)
                    {
                        for (int teamIndex = 0; teamIndex < teamsCount; ++teamIndex)
                        {
                            if (teamIndex == aiTeamIndex)
                                continue;

                            if (teamIndex < m_Goals.Count)
                            {
                                tnGoal goal = m_Goals[teamIndex];
                                aiData.SetOpponentGoal(goal.transform);

                                break; // NOTE: Only the first opponent goal will be linked to the AI.
                            }
                        }
                    }

                    // Fill reference position.

                    {
                        Vector3 selfPosition = aiInputFiller.self.transform.position;
                        aiData.SetReferencePosition(selfPosition);
                    }

                    // Anchors.

                    {
                        if (topLeft != null)
                        {
                            aiData.SetTopLeftAnchor(topLeft.transform);
                        }

                        if (bottomRight != null)
                        {
                            aiData.SetBottomRightAnchor(bottomRight.transform);
                        }

                        if (midfield != null)
                        {
                            aiData.SetMidfieldAnchor(midfield.transform);
                        }

                        if (goalTop != null)
                        {
                            aiData.SetGoalTop(goalTop.transform);
                        }

                        if (goalBottom != null)
                        {
                            aiData.SetGoalBottom(goalBottom.transform);
                        }

                        if (areaTop != null)
                        {
                            aiData.SetAreaTop(areaTop.transform);
                        }

                        if (areaBottom != null)
                        {
                            aiData.SetAreaBottom(areaBottom.transform);
                        }
                    }

                    aiInputFiller.Setup(aiData);
                }
            }
        }
    }

    protected override tnAIInputFiller CreateAIInputFiller(int i_TeamIndex, int i_Index, GameObject i_Character)
    {
        return CreateBaseAIInputFiller(i_TeamIndex, i_Index, i_Character);
    }

    // INTERNALS

    private void ConfigureMatch()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null)
            return;

        // Match duration

        int matchDurationOptionId = matchSettingsModule.matchDurationOption;

        float matchDuration = 180f; // Default value.

        float time;
        if (tnGameData.TryGetMatchDurationValueMain(matchDurationOptionId, out time))
        {
            matchDuration = time;
        }

        m_MatchDuration = matchDuration;

        // Golden goal

        int goldenGoalOptionId = matchSettingsModule.goldenGoalOption;

        bool goldenGoal = false; // Defualt value.

        string goldenGoalValue;
        if (tnGameData.TryGetGoldenGoalValueMain(goldenGoalOptionId, out goldenGoalValue))
        {
            goldenGoal = (goldenGoalValue == "ON");
        }

        m_GoldenGoalEnabled = goldenGoal;
    }

    private void CreateSupporters()
    {
        if (!m_CreateSupporters)
            return;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        System.Random random = new System.Random(m_Seed);

        string[] animatedResourceNames = new string[]
        {
            "Characters/Common/p_Supporter01_Animated",
            "Characters/Common/p_Supporter02_Animated",
            "Characters/Common/p_Supporter03_Animated",
        };

        string[] staticResourceNames = new string[]
        {
            "Characters/Common/p_Supporter01",
            "Characters/Common/p_Supporter02",
            "Characters/Common/p_Supporter03",
        };

        // Load animated resources.

        List<GameObject> animatedResources = new List<GameObject>();

        for (int resourceIndex = 0; resourceIndex < animatedResourceNames.Length; ++resourceIndex)
        {
            GameObject resourcePrefab = (GameObject)Resources.Load<GameObject>(animatedResourceNames[resourceIndex]);

            if (resourcePrefab == null)
                continue;

            animatedResources.Add(resourcePrefab);
        }

        // Load static resources.

        List<GameObject> staticResources = new List<GameObject>();

        for (int resourceIndex = 0; resourceIndex < staticResourceNames.Length; ++resourceIndex)
        {
            GameObject resourcePrefab = (GameObject)Resources.Load<GameObject>(staticResourceNames[resourceIndex]);

            if (resourcePrefab == null)
                continue;

            staticResources.Add(resourcePrefab);
        }

        // Iterate each supporter area.

        tnSupporterArea[] supportersAreas = FindObjectsOfType<tnSupporterArea>();

        for (int supporterAreaIndex = 0; supporterAreaIndex < supportersAreas.Length; ++supporterAreaIndex)
        {
            tnSupporterArea supporterArea = supportersAreas[supporterAreaIndex];

            if (supporterArea == null)
                continue;

            // Compute team colors for this area.

            List<Color> teamColors = new List<Color>();

            Vector3 boundsMin = supporterArea.boundsMin;
            Vector3 boundsMax = supporterArea.boundsMax;

            if (boundsMin.x < 0f || boundsMax.x < 0f)
            {
                // Assign to Team 0.

                tnTeamDescription teamDescription = teamsModule.GetTeamDescription(0);
                if (teamDescription != null)
                {
                    Color[] colorsToAdd = GetTeamColors(teamDescription.teamId);
                    teamColors.AddRange(colorsToAdd);
                }
            }

            if (boundsMin.x > 0f || boundsMax.x > 0f)
            {
                // Assign to Team 1.

                tnTeamDescription teamDescription = teamsModule.GetTeamDescription(1);
                if (teamDescription != null)
                {
                    Color[] colorsToAdd = GetTeamColors(teamDescription.teamId);
                    teamColors.AddRange(colorsToAdd);
                }
            }

            Color[] spawnColors = teamColors.ToArray();

            if (spawnColors.Length == 0)
                continue;

            // Spawn supporters.

            for (int supporterIndex = 0; supporterIndex < supporterArea.numPoints; ++supporterIndex)
            {
                GameObject supporterPrefab = null;

                if (supporterIndex < supporterArea.maxAnimators)
                {
                    // Select an animated resource.

                    int resourceIndex = random.Next(0, animatedResources.Count);
                    supporterPrefab = animatedResources[resourceIndex];
                }
                else
                {
                    // Select a static resource.

                    int resourceIndex = random.Next(0, staticResources.Count);
                    supporterPrefab = staticResources[resourceIndex];
                }

                if (supporterPrefab == null)
                    continue;

                Vector2 spawnPoint = supporterArea.GetPoint(supporterIndex);
                Vector3 spawnPosition = new Vector3(spawnPoint.x, spawnPoint.y, 0f);

                GameObject supporterInstance = (GameObject)Instantiate(supporterPrefab, spawnPosition, Quaternion.identity);

                if (supporterInstance.transform.position.x > 0f)
                {
                    Vector3 flipScale = supporterInstance.transform.localScale;
                    flipScale.x *= -1f;

                    supporterInstance.transform.localScale = flipScale;
                }

                supporterInstance.transform.SetParent(supporterArea.transform, true);

                tnSupporter supporterComponent = supporterInstance.GetComponent<tnSupporter>();
                if (supporterComponent != null)
                {
                    int randomIndex = random.Next(0, teamColors.Count);
                    Color randomColor = teamColors[randomIndex];
                    supporterComponent.SetColor(randomColor);
                }

                tnDepth2d depth2d = supporterInstance.GetComponent<tnDepth2d>();
                if (depth2d != null)
                {
                    float scale = 0.5f;
                    depth2d.SetScale(scale);

                    float depthLevel = supporterArea.transform.position.z;
                    depth2d.SetOffset(depthLevel);
                }
            }
        }
    }

    private void SpawnGoals()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null)
            return;

        int stadiumId = matchSettingsModule.stadiumId;
        m_StadiumId = stadiumId;

        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

        if (stadiumData == null)
            return;

        tnGoal goalPrefab = stadiumData.LoadAndGetGoalPrefab();

        if (goalPrefab == null)
            return;

        for (int teamIndex = 0; teamIndex < teamsCount; ++teamIndex)
        {
            if (teamIndex >= s_GoalSpawnPoints.Length)
                break;

            int teamId = GetTeamId(teamIndex);

            string goalSpawnPointName = s_GoalSpawnPoints[teamIndex];
            GameObject goalSpawnPointGo = GameObject.Find(goalSpawnPointName);

            if (goalSpawnPointGo == null)
                continue;

            TSTransform2D goalSpawnPointTransform = goalSpawnPointGo.GetComponent<TSTransform2D>();

            if (goalSpawnPointTransform == null)
                continue;

            Vector3 spawnPosition = goalSpawnPointGo.transform.position;
            Quaternion spawnRotation = goalSpawnPointGo.transform.rotation;

            tnGoal goalInstance = Instantiate<tnGoal>(goalPrefab);
            goalInstance.gameObject.name = "Goal_" + teamIndex;

            goalInstance.transform.position = spawnPosition;
            goalInstance.transform.rotation = spawnRotation;

            // Configure TSTransform

            TSTransform2D tsTransform = goalInstance.GetComponent<TSTransform2D>();
            if (tsTransform != null)
            {
                tsTransform.position = goalSpawnPointTransform.position;
                tsTransform.rotation = goalSpawnPointTransform.rotation;
            }

            // Rotate.

            if (goalInstance.transform.position.x > 0f)
            {
                goalInstance.Rotate();
            }

            // Setup goal.

            goalInstance.SetTeamId(teamId);

            // Register on TrueSyncController.

            TrueSyncManager.RegisterTrueSyncObjectMain(goalInstance.gameObject);

            // Set data on slow motion controller.

            if (m_SlowMotionController != null)
            {
                m_SlowMotionController.AddSegment(goalInstance.slowMotionPivotA, goalInstance.slowMotionPivotB);
            }
            
            // Add to goal list.

            m_Goals.Add(goalInstance);
        }
    }

    private void SpawnBall()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null)
            return;

        int ballId = matchSettingsModule.ballId;
        m_BallId = ballId;

        tnBallData ballData = tnGameData.GetBallDataMain(m_BallId);

        if (ballData == null)
            return;

        tnBall ballPrefab = tnGameData.LoadAndGetBallPrefabMain();

        if (ballPrefab == null)
            return;

        GameObject ballSpawnPointGo = GameObject.Find(s_BallSpawnPoint);

        if (ballSpawnPointGo == null)
            return;

        TSTransform2D ballSpawnPointTransform = ballSpawnPointGo.GetComponent<TSTransform2D>();

        if (ballSpawnPointTransform == null)
            return;

        Vector3 spawnPosition = ballSpawnPointTransform.position.ToVector();
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, ballSpawnPointTransform.rotation.AsFloat());

        // Spawn ball.

        tnBall ballInstance = Instantiate<tnBall>(ballPrefab);
        ballInstance.gameObject.name = "Ball";

        ballInstance.transform.position = spawnPosition;
        ballInstance.transform.rotation = spawnRotation;

        // Can rotate?

        ballInstance.SetCanRotate(ballData.canRotate);

        // Configure TSTransform

        TSTransform2D tsTransform = ballInstance.GetComponent<TSTransform2D>();
        if (tsTransform != null)
        {
            tsTransform.position = ballSpawnPointTransform.position;
            tsTransform.rotation = ballSpawnPointTransform.rotation;
        }

        // Set ball texture and material.

        tnBallView ballView = ballInstance.GetComponent<tnBallView>();
        if (ballView != null)
        {
            Texture ballTexture = ballData.texture;
            ballView.SetTexture(ballTexture);

            ballView.SetTrailMaterial(ballData.trailMaterial);
            ballView.SetParticleEffect(ballData.particleEffect);
        }

        // Set depth level

        tnDepth2d depth2d = ballInstance.GetComponent<tnDepth2d>();
        if (depth2d != null)
        {
            depth2d.SetOffset(ballSpawnPointGo.transform.position.z);
        }

        // Register True Sync Obejct.

        TrueSyncManager.RegisterTrueSyncObjectMain(ballInstance.gameObject);

        // Bind to camera.

        if (cameraGo != null)
        {
            tnGameCamera gameCamera = cameraGo.GetComponent<tnGameCamera>();
            if (gameCamera != null)
            {
                gameCamera.SetTarget(ballInstance.transform);
            }
        }

        // Bind to slow motion controller.

        if (m_SlowMotionController != null)
        {
            TSRigidBody2D ballRigidbody = ballInstance.GetComponent<TSRigidBody2D>();
            if (ballRigidbody != null)
            {
                m_SlowMotionController.SetTarget(ballRigidbody);
            }
        }

        // Set Bounds of field for respown if ball go out of the field.

        GameObject topLeft = GameObject.Find(s_TopLeft_Bound);
        GameObject bottomRight = GameObject.Find(s_BottomRight_Bound);

        if (topLeft != null && bottomRight != null)
        {
            TSTransform2D tsTransformTopLeft = topLeft.GetComponent<TSTransform2D>();
            TSTransform2D tsTransformBottomRight = bottomRight.GetComponent<TSTransform2D>();

            if (tsTransformTopLeft != null && tsTransformBottomRight != null)
            {
                FP minX = tsTransformTopLeft.position.x;
                FP minY = tsTransformBottomRight.position.y;
                FP maxX = tsTransformBottomRight.position.x;
                FP maxY = tsTransformTopLeft.position.y;

                TSVector2 min = new TSVector2(minX, minY);
                TSVector2 max = new TSVector2(maxX, maxY);

                ballInstance.SetBoundLimits(min, max);
                ballInstance.SetSafeRespawnOutField(true);
            }
        }

        // Save instance.

        m_Ball = ballInstance;
    }

    private void SpawnReferee()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null)
            return;

        int refereeOptionId = matchSettingsModule.refereeOption;
        m_HasReferee = false;

        string refereeOption;
        if (tnGameData.TryGetRefereeValueMain(refereeOptionId, out refereeOption))
        {
            m_HasReferee = (refereeOption == "ON");
            if (m_HasReferee)
            {
                GameObject refereeSpawnPointGo = GameObject.Find(s_RefereeSpawnPoint);

                if (refereeSpawnPointGo == null)
                    return;

                TSTransform2D refereeSpawnPointTransform = refereeSpawnPointGo.GetComponent<TSTransform2D>();

                if (refereeSpawnPointTransform == null)
                    return;

                GameObject refereePrefab = Resources.Load<GameObject>(s_ResourcePath_Referee);
                if (refereePrefab != null)
                {
                    Vector3 spawnPosition = refereeSpawnPointTransform.position.ToVector();
                    Quaternion spawnRotation = Quaternion.Euler(0f, 0f, refereeSpawnPointTransform.rotation.AsFloat());

                    GameObject refereeInstance = Instantiate<GameObject>(refereePrefab);
                    refereeInstance.gameObject.name = "Referee";

                    refereeInstance.transform.position = spawnPosition;
                    refereeInstance.transform.rotation = spawnRotation;

                    // Configure TSTransform

                    TSTransform2D tsTransform = refereeInstance.GetComponent<TSTransform2D>();
                    if (tsTransform != null)
                    {
                        tsTransform.position = refereeSpawnPointTransform.position;
                        tsTransform.rotation = refereeSpawnPointTransform.rotation;
                    }

                    // Configure input filler and tnCharacterInput.

                    if (PhotonNetwork.isMasterClient)
                    {
                        tnRefereeInputFiller inputFiller = new tnRefereeInputFiller(refereeInstance);
                        inputFiller.SetBall(m_Ball);

                        tnInputController inputController = new tnInputController(inputFiller);
                        AddInputController(inputController);

                        tnCharacterInput characterInput = refereeInstance.GetComponent<tnCharacterInput>();
                        if (characterInput != null)
                        {
                            characterInput.Bind(inputController);
                        }

                        RegisterObjectOnInputCollector(refereeInstance, 0);
                    }

                    // Configure depth2d.

                    tnDepth2d depth2d = refereeInstance.GetComponent<tnDepth2d>();
                    if (depth2d != null)
                    {
                        depth2d.SetOffset(refereeSpawnPointGo.transform.position.z);
                    }

                    // Setup true sync object.

                    TrueSyncObject trueSyncObject = refereeInstance.GetComponent<TrueSyncObject>();
                    if (trueSyncObject != null)
                    {
                        int ownerId = 0;
                        if (!PhotonNetwork.offlineMode && (PhotonNetwork.masterClient != null))
                        {
                            ownerId = PhotonNetwork.masterClient.ID;
                        }

                        trueSyncObject.SetOwnerId(ownerId);

                        TrueSyncManager.RegisterTrueSyncObjectMain(trueSyncObject);
                    }

                    m_RefereeInstance = refereeInstance;
                }
            }
        }
    }

    private void ConfigureHoles()
    {
        tnHole[] holes = FindObjectsOfType<tnHole>();
        tnHoleTarget[] holeTargets = FindObjectsOfType<tnHoleTarget>();

        for (int targetIndex = 0; targetIndex < holeTargets.Length; ++targetIndex)
        {
            tnHoleTarget holeTarget = holeTargets[targetIndex];

            for (int holeIndex = 0; holeIndex < holes.Length; ++holeIndex)
            {
                tnHole hole = holes[holeIndex];
                hole.RegisterTarget(holeTarget);
            }
        }
    }

    private void KickOff()
    {
        if (!m_Waiting)
            return;

        // Launch ball.

        LaunchBall();

        // Enable input.

        if (!paused)
        {
            EnableInput();
        }
        else
        {
            EnableAIInput();
        }

        // Kick-off.

        m_Waiting = false;

        // Enable slow motion.

        SetSlowMotionEnabled(true);

        // Callback.

        OnKickOff();

        // Raise event.

        Messenger.Broadcast(s_KickOff_MessengerEvent);
    }

    private void ResetField()
    {
        // Raise event.

        Messenger.Broadcast(s_PreFieldReset_MessengerEvent);

        // Respawn players.

        RespawnPlayers();

        // Respawn referee.

        RespawnReferee();

        // Respawn ball.

        RespawnBall();

        // Callback.

        OnResetField();

        // Raise event.

        Messenger.Broadcast(s_FieldReset_MessengerEvent);
    }

    private IEnumerator RunEndMatchSequence()
    {
        yield return StartCoroutine(EndMatchSequence());
        FinishGame();
    }

    // PROTECTED VIRTUAL

    protected virtual bool Draw()
    {
        return false;
    }

    protected virtual void OnResetField()
    {

    }

    protected virtual void OnKickOff()
    {

    }

    protected virtual void OnGoldenGoalStart()
    {

    }

    protected virtual void OnGoal(tnGoalEventParams i_Params)
    {

    }

    protected virtual IEnumerator EndMatchSequence()
    {
        yield return null;
    }

    protected virtual tnBaseAIInputFiller CreateBaseAIInputFiller(int i_TeamIndex, int i_Index, GameObject i_Character)
    {
        return new tnNullBaseAIInputFiller(i_Character);
    }

    // UTILS

    private void LaunchBall()
    {

    }

    private void RespawnPlayers()
    {
        for (int characterIndex = 0; characterIndex < charactersCount; ++characterIndex)
        {
            GameObject characterGo = GetCharacterByIndex(characterIndex);

            if (characterGo == null)
                continue;

            tnRespawn respawn = characterGo.GetComponent<tnRespawn>();

            if (respawn == null)
                continue;

            respawn.Respawn();
        }
    }

    private void RespawnReferee()
    {
        if (m_RefereeInstance == null)
            return;

        tnRespawn respawn = m_RefereeInstance.GetComponent<tnRespawn>();

        if (respawn == null)
            return;

        respawn.Respawn();
    }

    private void RespawnBall()
    {
        if (m_Ball == null)
            return;

        tnRespawn respawn = m_Ball.GetComponent<tnRespawn>();

        if (respawn == null)
            return;

        respawn.Respawn();
    }

    private void SetPlayerBehavioursEnabled(bool i_Enabled)
    {
        for (int index = 0; index < charactersCount; ++index)
        {
            GameObject character = GetCharacterByIndex(index);

            if (character == null)
                continue;

            tnCharacterController characterController = character.GetComponent<tnCharacterController>();
            if (characterController != null)
            {
                characterController.runSyncedUpdate = i_Enabled;
            }

            tnKick kick = character.GetComponent<tnKick>();
            if (kick != null)
            {
                kick.runSyncedUpdate = i_Enabled;
            }

            tnAttract attract = character.GetComponent<tnAttract>();
            if (attract != null)
            {
                attract.runSyncedUpdate = i_Enabled;
            }

            tnTaunt taunt = character.GetComponent<tnTaunt>();
            if (taunt != null)
            {
                taunt.runSyncedUpdate = i_Enabled;
            }

            tnSubbuteoController subbuteoController = character.GetComponent<tnSubbuteoController>();
            if (subbuteoController != null)
            {
                subbuteoController.runSyncedUpdate = i_Enabled;
            }
        }
    }

    private void SetSlowMotionEnabled(bool i_Enabled)
    {
        if (m_SlowMotionController != null)
        {
            m_SlowMotionController.SetEnabled(i_Enabled);
        }
    }

    // EVENTS

    private void OnGoalEvent(tnGoalEventParams i_Params)
    {
        if (m_Waiting)
            return;

        if (m_GoalTick != 0)
            return;

        int currentTick = TrueSyncManager.ticksMain;

        m_GoalTick = currentTick;
        m_GoalParamsCache = i_Params;
    }
}