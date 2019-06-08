using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace TrueSync
{
    public class TrueSyncManager : MonoBehaviour
    {
        // STATIC

        private static FP s_JitterTimeFactor = FP.FromFloat(0.001f);
        private static string s_TrueSyncConfigResourcePath = "Network/TrueSyncConfig";

        private static TrueSyncManager m_Instance = null;

        // STATIC ACCESSORS

        public static FP deltaTimeMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.deltaTime;
                }

                return 0;
            }
        }

        public static FP timeMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.time;
                }

                return 0;
            }
        }

        public static FP timeScaleMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.timeScale;
                }

                return FP.Zero;
            }
        }

        public static int ticksMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.ticks;
                }

                return 0;
            }
        }

        public static int lastSafeTickMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.lastSafeTick;
                }

                return 0;
            }
        }

        public static List<TSPlayerInfo> playersMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.players;
                }

                return null;
            }
        }

        public static TSPlayerInfo localPlayerMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.localPlayer;
                }

                return null;
            }
        }

        public static TrueSyncConfig configMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.config;
                }

                return null;
            }
        }

        public static int rollbackWindowMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.rollbackWindow;
                }

                return 0;
            }
        }

        public static int syncWindowMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.syncWindow;
                }

                return 0;
            }
        }

        public static int panicWindowMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.panicWindow;
                }

                return 0;
            }
        }

        public static bool isOnlineMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.isOnline;
                }

                return false;
            }
        }

        public static bool isOfflineMain
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.isOffline;
                }

                return false;
            }
        }

        // STATIC LOGIC

        public static void InitializeMain()
        {
            if (m_Instance != null)
            {
                m_Instance.Initialize();
            }
        }

        public static void StartSimulationMain()
        {
            if (m_Instance != null)
            {
                m_Instance.StartSimulation();
            }
        }

        public static void RunSimulationMain()
        {
            if (m_Instance != null)
            {
                m_Instance.RunSimulation();
            }
        }

        public static void PauseSimulationMain()
        {
            if (m_Instance != null)
            {
                m_Instance.PauseSimulation();
            }
        }

        public static void EndSimulationMain()
        {
            if (m_Instance != null)
            {
                m_Instance.EndSimulation();
            }
        }

        public static void RegisterTrueSyncObjectMain(GameObject i_Go)
        {
            if (m_Instance != null)
            {
                m_Instance.RegisterTrueSyncObject(i_Go);
            }
        }

        public static void RegisterTrueSyncObjectMain(TrueSyncObject i_TrueSyncObject)
        {
            if (m_Instance != null)
            {
                m_Instance.RegisterTrueSyncObject(i_TrueSyncObject);
            }
        }

        public static void RegisterIsReadyCallbackMain(TrueSyncIsReady i_Callback)
        {
            if (m_Instance != null)
            {
                m_Instance.RegisterIsReadyCallback(i_Callback);
            }
        }

        public static void RemovePlayerMain(int i_PlayerId)
        {
            if (m_Instance != null)
            {
                m_Instance.RemovePlayer(i_PlayerId);
            }
        }

        public static bool IsSafeTickMain(int i_Tick)
        {
            if (m_Instance != null)
            {
                return m_Instance.IsSafeTick(i_Tick);
            }

            return false;
        }

        public static bool IsSafeTickMain()
        {
            if (m_Instance != null)
            {
                return m_Instance.IsSafeTick();
            }

            return false;
        }

        public static bool IsTickOutOfRollbackMain(int i_Tick)
        {
            if (m_Instance != null)
            {
                return m_Instance.IsTickOutOfRollback(i_Tick);
            }

            return false;
        }

        public static void ForceTimeScaleMain(FP i_Value)
        {
            if (m_Instance != null)
            {
                m_Instance.ForceTimeScale(i_Value);
            }
        }

        // Fields

        private TSPlayerInfo m_NullPlayerInfo = null;

        private TrueSyncConfig m_Config = null;

        private AbstractLockstep m_Lockstep = null;
        private FP m_LockedTimeStep;

        private FP m_TsDeltaTime = FP.Zero;

        private TSTimeScaler m_TimeScaler = null;

        private List<TrueSyncManagedBehaviour> m_QueuedBehaviours = new List<TrueSyncManagedBehaviour>();
        private List<TrueSyncManagedBehaviour> m_GeneralBehaviours = new List<TrueSyncManagedBehaviour>();
        private Dictionary<byte, List<TrueSyncManagedBehaviour>> m_BehavioursPerPlayer = new Dictionary<byte, List<TrueSyncManagedBehaviour>>();
        private Dictionary<ITrueSyncBehaviour, TrueSyncManagedBehaviour> m_ManagedBehaviourMap = new Dictionary<ITrueSyncBehaviour, TrueSyncManagedBehaviour>();

        private List<TrueSyncManagedBehaviour> m_TsManagedBehaviourCache = new List<TrueSyncManagedBehaviour>();
        private List<TrueSyncBehaviour> m_TsBehaviourCache = new List<TrueSyncBehaviour>();

        // ACCESSORS

        public FP deltaTime
        {
            get
            {
                if (m_Lockstep != null)
                {
                    return m_Lockstep.deltaTime;
                }

                return 0;
            }
        }

        public FP time
        {
            get
            {
                if (m_Lockstep != null)
                {
                    return m_Lockstep.time;
                }

                return 0;
            }
        }

        public FP timeScale
        {
            get
            {
                if (m_TimeScaler != null)
                {
                    return m_TimeScaler.timeScale;
                }

                return FP.Zero;
            }
        }

        public int ticks
        {
            get
            {
                if (m_Lockstep != null)
                {
                    return m_Lockstep.Ticks;
                }

                return 0;
            }
        }

        public int lastSafeTick
        {
            get
            {
                if (m_Lockstep != null)
                {
                    return m_Lockstep.LastSafeTick;
                }

                return 0;
            }
        }

        public int rollbackWindow
        {
            get
            {
                if (m_Config != null)
                {
                    return m_Config.rollbackWindow;
                }

                return 0;
            }
        }

        public int syncWindow
        {
            get
            {
                if (m_Config != null)
                {
                    return m_Config.syncWindow;
                }

                return 0;
            }
        }

        public int panicWindow
        {
            get
            {
                if (m_Config != null)
                {
                    return m_Config.panicWindow;
                }

                return 0;
            }
        }

        public List<TSPlayerInfo> players
        {
            get
            {
                if (m_Lockstep == null)
                {
                    return null;
                }

                List<TSPlayerInfo> allPlayers = new List<TSPlayerInfo>();
                foreach (TSPlayer tsp in m_Lockstep.Players.Values)
                {
                    if (!tsp.dropped)
                    {
                        allPlayers.Add(tsp.playerInfo);
                    }
                }

                return allPlayers;
            }
        }

        public TSPlayerInfo localPlayer
        {
            get
            {
                if (m_Lockstep == null)
                {
                    return null;
                }

                return m_Lockstep.LocalPlayer.playerInfo;
            }
        }

        public TrueSyncConfig config
        {
            get
            {
                return m_Config;
            }
        }

        public bool isOnline
        {
            get
            {
                return PhotonNetwork.connected && PhotonNetwork.inRoom;
            }
        }

        public bool isOffline
        {
            get
            {
                return !isOnline;
            }
        }

        // MonoBehaviour's interface

        void Awake()
        {
            m_Instance = this;
            m_NullPlayerInfo = new TSPlayerInfo(0, "");
        }

        void FixedUpdate()
        {
            if (m_Lockstep != null)
            {
                m_TsDeltaTime += Time.fixedDeltaTime;

                if (m_TsDeltaTime >= (m_LockedTimeStep - s_JitterTimeFactor))
                {
                    m_TsDeltaTime = FP.Zero;

                    m_Lockstep.Update();
                }
            }
        }

        void OnDestroy()
        {
            ResourcePool.CleanUpAll();

            // Release StateTracker.

            StateTracker.CleanUp();

            // Release physics world.

            PhysicsManager.Cleanup();

            // Release self-reference.

            m_Instance = null;
        }

        void OnApplicationQuit()
        {
            EndSimulation();
        }

        // LOGIC

        public void Initialize(TrueSyncConfig i_Config = null)
        {
            TrueSyncConfig config = (i_Config != null) ? i_Config : (Resources.Load<TrueSyncConfig>(s_TrueSyncConfigResourcePath)) ;

            // Load config.

            m_Config = config;

            if (config == null)
                return;

            m_LockedTimeStep = config.lockedTimeStep;

            // Init state tracker.

            StateTracker.Init(config.rollbackWindow);

            // Init time.

            m_TimeScaler = new TSTimeScaler();
            m_TimeScaler.Init();

            // Create physics world.

            IPhysicsManager worldManager = new Physics2DWorldManager();
            worldManager.Gravity = new TSVector(config.gravity.x, config.gravity.y, 0);
            worldManager.SpeculativeContacts = config.speculativeContacts;
            worldManager.LockedTimeStep = config.lockedTimeStep;

            worldManager.Init();

            // Create communicator.

            ICommunicator communicator = null;
            if (isOnline)
            {
                communicator = new PhotonTrueSyncCommunicator(PhotonNetwork.networkingPeer);
            }
            else
            {
                Debug.Log("You are not connected to Photon. TrueSync will start in offline mode.");
            }

            // Create lockstep.

            m_Lockstep = AbstractLockstep.NewInstance(m_LockedTimeStep, communicator, worldManager, m_Config.syncWindow, m_Config.panicWindow, m_Config.rollbackWindow, OnGameStarted, OnGamePaused, OnGameUnPaused, OnGameEnded, OnPlayerDisconnection, OnStepUpdate, GetLocalData);

            Debug.Log("Lockstep initialized (Sync: " + m_Config.syncWindow + ", Rollback: " + m_Config.rollbackWindow + ")");

            // Stats

            if (m_Config.showStats)
            {
                TrueSyncStats statsComponent = gameObject.AddComponent<TrueSyncStats>();
                statsComponent.Lockstep = m_Lockstep;
            }

            // Add player on lockestep.

            if (isOnline)
            {
                List<PhotonPlayer> photonPlayers = new List<PhotonPlayer>(PhotonNetwork.playerList);
                photonPlayers.Sort(UnityUtils.playerComparer);

                for (int photonPlayerIndex = 0; photonPlayerIndex < photonPlayers.Count; ++photonPlayerIndex)
                {
                    PhotonPlayer photonPlayer = photonPlayers[photonPlayerIndex];
                    m_Lockstep.AddPlayer((byte)photonPlayer.ID, photonPlayer.NickName, photonPlayer.IsLocal);
                }
            }
            else
            {
                m_Lockstep.AddPlayer(0, "Local_Player", true);
            }

            // Fill behaviours per player dictionary.

            foreach (TSPlayer player in m_Lockstep.Players.Values)
            {
                List<TrueSyncManagedBehaviour> behaviours = new List<TrueSyncManagedBehaviour>();
                m_BehavioursPerPlayer.Add(player.ID, behaviours);
            }

            // Initialize Physics Manager.

            PhysicsManager.Initialize(worldManager);
            PhysicsManager.OnRemoveBody(OnRemovedRigidbody);
        }

        public void StartSimulation()
        {
            if (m_Lockstep != null)
            {
                m_Lockstep.RunSimulation(true);
            }
        }

        public void RunSimulation()
        {
            if (m_Lockstep != null)
            {
                m_Lockstep.RunSimulation(false);
            }
        }

        public void PauseSimulation()
        {
            if (m_Lockstep != null)
            {
                m_Lockstep.PauseSimulation();
            }
        }

        public void EndSimulation()
        {
            if (m_Lockstep != null)
            {
                m_Lockstep.EndSimulation();
            }
        }

        public void RegisterTrueSyncObject(GameObject i_Go)
        {
            if (i_Go == null)
                return;

            // Get TrueSyncObject

            TrueSyncObject trueSyncObject = i_Go.GetComponent<TrueSyncObject>();

            if (trueSyncObject == null)
                return;

            // Register true sync object.

            RegisterTrueSyncObject(trueSyncObject);
        }

        public void RegisterTrueSyncObject(TrueSyncObject i_TrueSyncObject)
        {
            if (i_TrueSyncObject == null)
                return;

            // Add True Sync Behaviours to QueuedBehaviours.

            for (int index = 0; index < i_TrueSyncObject.behaviourCount; ++index)
            {
                TrueSyncBehaviour trueSyncBehaviour = i_TrueSyncObject.GetTrueSyncBehaviourByIndex(index);

                if (trueSyncBehaviour == null)
                    continue;

                TrueSyncManagedBehaviour trueSyncManagedBehaviour = NewManagedBehavior(trueSyncBehaviour);
                m_QueuedBehaviours.Add(trueSyncManagedBehaviour);
            }

            // Initialize Object.

            InitializeObject(i_TrueSyncObject);

            // Callback

            i_TrueSyncObject.OnRegistration();
        }

        public void RegisterIsReadyCallback(TrueSyncIsReady i_Callback)
        {
            if (m_Lockstep != null)
            {
                m_Lockstep.GameIsReady += i_Callback;
            }
        }

        public void RemovePlayer(int i_PlayerId)
        {
            foreach (TrueSyncManagedBehaviour tsmb in m_BehavioursPerPlayer[(byte)i_PlayerId])
            {
                tsmb.disabled = true;

                TSCollider2D[] tsCollider2Ds = ((TrueSyncBehaviour)tsmb.trueSyncBehavior).gameObject.GetComponentsInChildren<TSCollider2D>();
                if (tsCollider2Ds != null)
                {
                    foreach (TSCollider2D tsCollider2D in tsCollider2Ds)
                    {
                        if (!tsCollider2D.body.TSDisabled)
                        {
                            DestroyTSRigidBody(tsCollider2D.gameObject, tsCollider2D.body);
                        }
                    }
                }
            }
        }

        public bool IsSafeTick(int i_Tick)
        {
            return (i_Tick <= lastSafeTick);
        }

        public bool IsSafeTick()
        {
            return (ticks <= lastSafeTick);
        }

        public bool IsTickOutOfRollback(int i_Tick)
        {
            return (i_Tick <= ticks - rollbackWindow);
        }

        public void ForceTimeScale(FP i_Value)
        {
            if (m_TimeScaler != null)
            {
                m_TimeScaler.SetTimeScale(i_Value);
            }
        }

        // INTERNALS

        private void ProcessQueuedBehaviours()
        {
            if (m_QueuedBehaviours.Count > 0)
            {
                // Add all queued to general.

                m_GeneralBehaviours.AddRange(m_QueuedBehaviours);

                SortList(m_GeneralBehaviours);

                // Assign player queued behaviours to player and remove from general.

                AssignQueuedToPlayers();

                // Setup data on new behaviours.

                for (int index = 0; index < m_QueuedBehaviours.Count; ++index)
                {
                    TrueSyncManagedBehaviour tsmb = m_QueuedBehaviours[index];

                    if (tsmb == null)
                        continue;

                    tsmb.SetGameInfo(m_Lockstep.LocalPlayer.playerInfo, m_Lockstep.Players.Count);
                    tsmb.OnSyncedStart();
                }

                // Clear queue.

                m_QueuedBehaviours.Clear();
            }
        }

        private void AssignQueuedToPlayers()
        {
            m_TsManagedBehaviourCache.Clear();

            for (int index = 0; index < m_QueuedBehaviours.Count; ++index)
            {
                TrueSyncManagedBehaviour tsmb = m_QueuedBehaviours[index];

                if (tsmb == null)
                    continue;

                TrueSyncBehaviour bh = (TrueSyncBehaviour)tsmb.trueSyncBehavior;

                List<TrueSyncManagedBehaviour> tsmbList = null;
                if (m_BehavioursPerPlayer.TryGetValue((byte)bh.ownerIndex, out tsmbList))
                {
                    bh.owner = m_Lockstep.Players[(byte)bh.ownerIndex].playerInfo;

                    m_TsManagedBehaviourCache.Add(tsmb);
                    tsmbList.Add(tsmb);

                    SortList(tsmbList);
                }
                else
                {
                    bh.owner = m_NullPlayerInfo;
                    bh.ownerIndex = -1;
                }

                bh.localOwner = m_Lockstep.LocalPlayer.playerInfo;
                bh.numberOfPlayers = m_Lockstep.Players.Count;

                tsmb.owner = bh.owner;
                tsmb.localOwner = bh.localOwner;
            }

            for (int index = 0; index < m_TsManagedBehaviourCache.Count; ++index)
            {
                TrueSyncManagedBehaviour item = m_TsManagedBehaviourCache[index];

                if (item == null)
                    continue;

                m_GeneralBehaviours.Remove(item);
            }

            m_TsManagedBehaviourCache.Clear();
        }

        private void DestroyTSRigidBody(GameObject i_TsColliderGO, IBody i_Body)
        {
            i_TsColliderGO.gameObject.SetActive(false);
            m_Lockstep.Destroy(i_Body);
        }

        private void InitializeObject(TrueSyncObject i_TrueSyncObject)
        {
            if (i_TrueSyncObject == null)
                return;

            // Register colliders on physics manager.

            for (int index = 0; index < i_TrueSyncObject.colliderCount; ++index)
            {
                TSCollider2D collider = i_TrueSyncObject.GetColliderByIndex(index);

                if (collider == null)
                    continue;

                PhysicsManager.AddBody(collider);
            }

            // Init transforms.

            for (int index = 0; index < i_TrueSyncObject.transformCount; ++index)
            {
                TSTransform2D t = i_TrueSyncObject.GetTransformByIndex(index);

                if (t == null)
                    continue;

                t.Initialize();
            }

            // Init rigidbody, if any.

            for (int index = 0; index < i_TrueSyncObject.rigidbodyCount; ++index)
            {
                TSRigidBody2D rigidbody = i_TrueSyncObject.GetRigidBodyByIndex(index);

                if (rigidbody == null)
                    continue;

                rigidbody.Initialize();
            }
        }

        // CALLBACK

        private void OnRemovedRigidbody(IBody i_Body)
        {
            GameObject go = PhysicsManager.GetGameObject(i_Body);
            if (go != null)
            {
                TrueSyncObject trueSyncObject = go.GetComponent<TrueSyncObject>();
                if (trueSyncObject != null)
                {
                    m_TsBehaviourCache.Clear();
                    for (int index = 0; index < trueSyncObject.behaviourCount; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncObject.GetTrueSyncBehaviourByIndex(index);
                        if (behaviour != null)
                        {
                            m_TsBehaviourCache.Add(behaviour);
                        }
                    }

                    RemoveFromTSMBList(m_QueuedBehaviours, m_TsBehaviourCache);
                    RemoveFromTSMBList(m_GeneralBehaviours, m_TsBehaviourCache);

                    foreach (List<TrueSyncManagedBehaviour> list in m_BehavioursPerPlayer.Values)
                    {
                        RemoveFromTSMBList(list, m_TsBehaviourCache);
                    }

                    m_TsBehaviourCache.Clear();
                }
            }
        }

        // Lockstep callbacks.

        private void OnGameStarted()
        {
            ProcessQueuedBehaviours();
        }

        private void OnGamePaused()
        {
            if (m_GeneralBehaviours != null)
            {
                for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = m_GeneralBehaviours[index];
                    bh.OnGamePaused();
                }
            }

            foreach (List<TrueSyncManagedBehaviour> behaviors in m_BehavioursPerPlayer.Values)
            {
                for (int index = 0; index < behaviors.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = behaviors[index];
                    bh.OnGamePaused();
                }
            }
        }

        private void OnGameUnPaused()
        {
            if (m_GeneralBehaviours != null)
            {
                for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = m_GeneralBehaviours[index];
                    bh.OnGameUnPaused();
                }
            }

            foreach (List<TrueSyncManagedBehaviour> behaviors in m_BehavioursPerPlayer.Values)
            {
                for (int index = 0; index < behaviors.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = behaviors[index];
                    bh.OnGameUnPaused();
                }
            }
        }

        private void OnGameEnded()
        {
            if (m_GeneralBehaviours != null)
            {
                for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = m_GeneralBehaviours[index];
                    bh.OnGameEnded();
                }
            }

            foreach (List<TrueSyncManagedBehaviour> behaviors in m_BehavioursPerPlayer.Values)
            {
                for (int index = 0; index < behaviors.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = behaviors[index];
                    bh.OnGameEnded();
                }
            }
        }

        private void OnPlayerDisconnection(byte i_PlayerId)
        {
            if (m_GeneralBehaviours != null)
            {
                for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = m_GeneralBehaviours[index];
                    bh.OnPlayerDisconnection((int)i_PlayerId);
                }
            }

            foreach (List<TrueSyncManagedBehaviour> behaviors in m_BehavioursPerPlayer.Values)
            {
                for (int index = 0; index < behaviors.Count; ++index)
                {
                    TrueSyncManagedBehaviour bh = behaviors[index];
                    bh.OnPlayerDisconnection((int)i_PlayerId);
                }
            }
        }

        private void OnStepUpdate(List<InputData> i_AllInputData)
        {
            // Sync time.

            if (m_TimeScaler != null)
            {
                m_TimeScaler.Sync();
            }

            // PRE SYNC UPDATE

            // Clear input.

            TrueSyncInput.SetAllInputs(null);

            // Synced pre update on general behaviours.

            for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
            {
                TrueSyncManagedBehaviour tsmb = m_GeneralBehaviours[index];

                if (tsmb == null || tsmb.disabled)
                    continue;

                tsmb.OnPreSyncedUpdate();
            }

            // Synced pre update on player behaviours.

            if (i_AllInputData != null)
            {
                for (int index = 0; index < i_AllInputData.Count; ++index)
                {
                    InputData inputData = i_AllInputData[index];

                    if (inputData == null)
                        continue;

                    List<TrueSyncManagedBehaviour> tsmbList = null;
                    if (m_BehavioursPerPlayer.TryGetValue(inputData.ownerID, out tsmbList))
                    {
                        for (int tsmbIndex = 0; tsmbIndex < tsmbList.Count; ++tsmbIndex)
                        {
                            TrueSyncManagedBehaviour tsmb = tsmbList[tsmbIndex];

                            if (tsmb == null || tsmb.disabled)
                                continue;

                            tsmb.OnPreSyncedUpdate();
                        }
                    }
                }
            }

            // UPDATE

            // Set input.

            TrueSyncInput.SetAllInputs(i_AllInputData);

            TrueSyncInput.CurrentSimulationData = null;

            // Synced update on general behaviours.

            for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
            {
                TrueSyncManagedBehaviour tsmb = m_GeneralBehaviours[index];

                if (tsmb == null || tsmb.disabled)
                    continue;

                tsmb.OnSyncedUpdate();
            }

            // Synced update on player behaviours.

            if (i_AllInputData != null)
            {
                for (int index = 0; index < i_AllInputData.Count; ++index)
                {
                    InputData inputData = i_AllInputData[index];

                    if (inputData == null)
                        continue;

                    List<TrueSyncManagedBehaviour> tsmbList = null;
                    if (m_BehavioursPerPlayer.TryGetValue(inputData.ownerID, out tsmbList))
                    {
                        TrueSyncInput.CurrentSimulationData = inputData;

                        for (int tsmbIndex = 0; tsmbIndex < tsmbList.Count; ++tsmbIndex)
                        {
                            TrueSyncManagedBehaviour tsmb = tsmbList[tsmbIndex];

                            if (tsmb == null || tsmb.disabled)
                                continue;

                            tsmb.OnSyncedUpdate();
                        }
                    }
                }

                TrueSyncInput.CurrentSimulationData = null;
            }

            // LATE UPDATE

            // Clear input.

            TrueSyncInput.SetAllInputs(null);

            // Synced late update on general behaviours.

            for (int index = 0; index < m_GeneralBehaviours.Count; ++index)
            {
                TrueSyncManagedBehaviour tsmb = m_GeneralBehaviours[index];

                if (tsmb == null || tsmb.disabled)
                    continue;

                ITrueSyncBehaviour iTsb = tsmb.trueSyncBehavior;
                if (iTsb != null)
                {
                    if (iTsb is TrueSyncBehaviour)
                    {
                        TrueSyncBehaviour tsb = (TrueSyncBehaviour)iTsb;
                        tsb.OnLateSyncedUpdate();
                    }
                }
            }

            // Synced late update on player behaviours.

            if (i_AllInputData != null)
            {
                for (int index = 0; index < i_AllInputData.Count; ++index)
                {
                    InputData inputData = i_AllInputData[index];

                    if (inputData == null)
                        continue;

                    List<TrueSyncManagedBehaviour> tsmbList = null;
                    if (m_BehavioursPerPlayer.TryGetValue(inputData.ownerID, out tsmbList))
                    {
                        for (int tsmbIndex = 0; tsmbIndex < tsmbList.Count; ++tsmbIndex)
                        {
                            TrueSyncManagedBehaviour tsmb = tsmbList[tsmbIndex];

                            if (tsmb == null || tsmb.disabled)
                                continue;

                            ITrueSyncBehaviour iTsb = tsmb.trueSyncBehavior;
                            if (iTsb != null)
                            {
                                if (iTsb is TrueSyncBehaviour)
                                {
                                    TrueSyncBehaviour tsb = (TrueSyncBehaviour)iTsb;
                                    tsb.OnLateSyncedUpdate();
                                }
                            }
                        }
                    }
                }
            }

            // Process queued behaviours.

            ProcessQueuedBehaviours();
        }

        private int m_LastGetLocalDataTick = int.MinValue;
        private void GetLocalData(InputData i_PlayerInputData)
        {
            /////////////////////////////////////////////////////////////////////////////
            // This is to skip the second unnecessary call when synced windows is 0.
            // Bug already reported to Jeff.
            // This condition should never trigger when sync window is greater than 0.
            /////////////////////////////////////////////////////////////////////////////

            if (m_LastGetLocalDataTick == ticks)
                return;

            // Cahce current tick.

            m_LastGetLocalDataTick = ticks;

            /////////////////////////////////////////////////////////////////////////////

            TrueSyncInput.CurrentInputData = i_PlayerInputData;

            // Synced input on player behaviours.

            List<TrueSyncManagedBehaviour> tsmbList = null;
            if (m_BehavioursPerPlayer.TryGetValue(i_PlayerInputData.ownerID, out tsmbList))
            {
                for (int index = 0; index < tsmbList.Count; ++index)
                {
                    TrueSyncManagedBehaviour tsmb = tsmbList[index];

                    if (tsmb == null || tsmb.disabled)
                        continue;

                    tsmb.OnSyncedInput();
                }
            }

            TrueSyncInput.CurrentInputData = null;
        }

        // UTILS

        private TrueSyncManagedBehaviour NewManagedBehavior(ITrueSyncBehaviour i_TrueSyncBehavior)
        {
            TrueSyncManagedBehaviour result = new TrueSyncManagedBehaviour(i_TrueSyncBehavior);
            m_ManagedBehaviourMap[i_TrueSyncBehavior] = result;

            return result;
        }

        private void RemoveFromTSMBList(List<TrueSyncManagedBehaviour> i_TsmbList, List<TrueSyncBehaviour> i_Behaviours)
        {
            m_TsManagedBehaviourCache.Clear();

            for (int index = 0; index < i_TsmbList.Count; ++index)
            {
                TrueSyncManagedBehaviour tsmb = i_TsmbList[index];

                if (tsmb == null)
                    continue;

                TrueSyncBehaviour bh = (TrueSyncBehaviour)tsmb.trueSyncBehavior;
                if (i_Behaviours.Contains(bh))
                {
                    m_TsManagedBehaviourCache.Add(tsmb);
                }
            }

            for (int index = 0; index < m_TsManagedBehaviourCache.Count; ++index)
            {
                TrueSyncManagedBehaviour tsmb = m_TsManagedBehaviourCache[index];

                if (tsmb == null)
                    continue;

                i_TsmbList.Remove(tsmb);
            }

            m_TsManagedBehaviourCache.Clear();
        }

        // STATIC UTILS

        private static void SortList(List<TrueSyncManagedBehaviour> io_List)
        {
            if (io_List == null)
                return;

            io_List.Sort(UnityUtils.trueSyncBehaviourComparer);
        }
    }
}