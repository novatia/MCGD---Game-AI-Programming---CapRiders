using UnityEngine;
using UnityEngine.SceneManagement;

using TrueSync;

public class tnAssetTuner : MonoBehaviour
{
    // Serializable fields
    
    [Header("Assets")]

    [SerializeField]
    private TrueSyncConfig m_OnlineConfig = null;
    [SerializeField]
    private TrueSyncConfig m_OfflineConfig = null;
    [SerializeField]
    private GameObject m_CharacterStandardPrefab = null;
    [SerializeField]
    private tnAIDatabase m_AIDatabase = null;
    [SerializeField]
    private tnMatchController m_StandardMatchController = null;
    [SerializeField]
    private tnMatchController m_SubbuteoMatchController = null;

    // Fields

    private tnKick m_KickComponent = null;
    private tnCharacterController m_CharacterControllerComponent = null;

    private bool m_CanChangeValue = true;

    private bool m_OnGui = false;

    // MonoBehaviour's interface

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (m_CharacterStandardPrefab != null)
        {
            m_CharacterControllerComponent = m_CharacterStandardPrefab.GetComponent<tnCharacterController>();
            m_KickComponent = m_CharacterStandardPrefab.GetComponent<tnKick>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            m_OnGui = !m_OnGui;
        }

        if (!m_CanChangeValue)
            return;

        // Online config.

        if (m_OnlineConfig != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_OnlineConfig.syncWindow += 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_OnlineConfig.syncWindow = Mathf.Max(m_OnlineConfig.syncWindow - 1, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                m_OnlineConfig.rollbackWindow += 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_OnlineConfig.rollbackWindow = Mathf.Max(m_OnlineConfig.rollbackWindow - 1, 0);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                m_OnlineConfig.showStats = !m_OnlineConfig.showStats;
            }
        }

        // Offline config.

        if (m_OfflineConfig != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                m_OfflineConfig.syncWindow += 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                m_OfflineConfig.syncWindow = Mathf.Max(m_OfflineConfig.syncWindow - 1, 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                m_OfflineConfig.rollbackWindow += 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                m_OfflineConfig.rollbackWindow = Mathf.Max(m_OfflineConfig.rollbackWindow - 1, 0);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                m_OfflineConfig.showStats = !m_OfflineConfig.showStats;
            }
        }

        // Character

        if (m_CharacterControllerComponent != null)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                m_CharacterControllerComponent.dashTickDelay += 1;
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                m_CharacterControllerComponent.dashTickDelay = Mathf.Max(m_CharacterControllerComponent.dashTickDelay - 1, 0);
            }
        }

        if (m_KickComponent != null)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                m_KickComponent.kickTickDelay += 1;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                m_KickComponent.kickTickDelay = Mathf.Max(m_KickComponent.kickTickDelay - 1, 0);
            }
        }

        // AI

        if (m_AIDatabase != null)
        {
            // Easy

            tnAILevelDescriptor easy = m_AIDatabase.GetAILevelDescriptor(0);
            if (easy != null)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    easy.inputDelay += 1;
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    easy.inputDelay = Mathf.Max(easy.inputDelay - 1, 0);
                }
            }

            // Normal

            tnAILevelDescriptor normal = m_AIDatabase.GetAILevelDescriptor(1);
            if (normal != null)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    normal.inputDelay += 1;
                }

                if (Input.GetKeyDown(KeyCode.H))
                {
                    normal.inputDelay = Mathf.Max(normal.inputDelay - 1, 0);
                }
            }

            // Hard

            tnAILevelDescriptor hard = m_AIDatabase.GetAILevelDescriptor(2);
            if (hard != null)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    hard.inputDelay += 1;
                }

                if (Input.GetKeyDown(KeyCode.N))
                {
                    hard.inputDelay = Mathf.Max(hard.inputDelay - 1, 0);
                }
            }
        }

        // Offline player delay

        if (m_StandardMatchController != null || m_SubbuteoMatchController != null)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (m_StandardMatchController != null)
                {
                    m_StandardMatchController.offlinePlayerInputDelay += 1;
                }

                if (m_SubbuteoMatchController != null)
                {
                    m_SubbuteoMatchController.offlinePlayerInputDelay += 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (m_StandardMatchController != null)
                {
                    m_StandardMatchController.offlinePlayerInputDelay = Mathf.Max(m_StandardMatchController.offlinePlayerInputDelay - 1, 0);
                }

                if (m_SubbuteoMatchController != null)
                {
                    m_SubbuteoMatchController.offlinePlayerInputDelay = Mathf.Max(m_SubbuteoMatchController.offlinePlayerInputDelay - 1, 0);
                }
            }
        }
    }

    private void OnGUI()
    {
        if (!m_OnGui)
            return;

        GUILayout.Label("Photon ping: " + PhotonNetwork.GetPing());

        if (m_OnlineConfig != null)
        {
            GUILayout.Label("ONLINE");
            string label = "Synced - Rollback: " + m_OnlineConfig.syncWindow + " - " + m_OnlineConfig.rollbackWindow + " (1 / 2) - (3 / 4)";
            GUILayout.Label(label);
            label = "Show stats: " + ((m_OnlineConfig.showStats) ? "ON" : "OFF") + " (X)";
            GUILayout.Label(label);
        }

        if (m_OfflineConfig != null)
        {
            GUILayout.Label("OFFLINE");
            string label = "Synced - Rollback: " + m_OfflineConfig.syncWindow + " - " + m_OfflineConfig.rollbackWindow + " (5 / 6) - (7 / 8)";
            GUILayout.Label(label);
            label = "Show stats: " + ((m_OfflineConfig.showStats) ? "ON" : "OFF") + " (C)"; 
            GUILayout.Label(label);
        }

        if (m_CharacterControllerComponent != null || m_KickComponent != null)
        {
            GUILayout.Label("CHARACTER");

            if (m_CharacterControllerComponent != null)
            {
                GUILayout.Label("Dash tick delay: " + m_CharacterControllerComponent.dashTickDelay + " (O/P)");
            }

            if (m_KickComponent != null)
            {
                GUILayout.Label("Kick tick delay: " + m_KickComponent.kickTickDelay + " (K/L)");
            }
        }

        if (m_AIDatabase != null)
        {
            GUILayout.Label("AI");

            tnAILevelDescriptor first = m_AIDatabase.GetAILevelDescriptor(0);
            tnAILevelDescriptor second = m_AIDatabase.GetAILevelDescriptor(1);
            tnAILevelDescriptor third = m_AIDatabase.GetAILevelDescriptor(2);

            if (first != null)
            {
                string label = first.label + ": " + first.inputDelay + " (U / I)";
                GUILayout.Label(label);
            }

            if (second != null)
            {
                string label = second.label + ": " + second.inputDelay + " (H / J)";
                GUILayout.Label(label);
            }

            if (third != null)
            {
                string label = third.label + ": " + third.inputDelay + " (N / M)";
                GUILayout.Label(label);
            }
        }

        if (m_StandardMatchController != null || m_SubbuteoMatchController != null)
        {
            GUILayout.Label("OFFLINE PLAYER DELAY");

            tnMatchController matchController = (m_StandardMatchController != null) ? m_StandardMatchController : m_SubbuteoMatchController;

            string label = "Delay: " + matchController.offlinePlayerInputDelay + " (V / B)";
            GUILayout.Label(label);
        }
    }   

    // EVENTS

    private void OnSceneLoaded(Scene i_Scene, LoadSceneMode i_LoadSceneMode)
    {
        if (i_Scene.name == "Menu")
        {
            m_CanChangeValue = true;
        }
        
        if (i_Scene.name == "Game" || i_Scene.name == "MultiplayerGame")
        {
            m_CanChangeValue = false;
        }
    }
}
