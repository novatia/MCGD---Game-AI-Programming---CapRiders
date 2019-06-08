using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using GoUI;

using WiFiInput.Server;

public class tnSideSelectionController : UIViewController
{
    // STATIC

    private static string s_ProceedTrigger = "TRIGGER_PROCEED";
    private static string s_CancelTrigger = "TRIGGER_CANCEL";

    private static string s_InfoMessage1 = "To connect more than 4 controllers \n disable XInput in the options menu.";
    private static string s_InfoMessage2 = "To use a Steam Controller \n enable XInput in the options menu.";

    // Fields

    [SerializeField]
    private tnUIControllersGrid m_ControllersGrid = null;
    [SerializeField]
    private tnUITeam m_TeamA = null;
    [SerializeField]
    private tnUITeam m_TeamB = null;

    [SerializeField]
    private RectTransform m_ControllersRoot = null;

    [SerializeField]
    private tnUIGamepad m_UIGamepadPrefab = null;
    [SerializeField]
    private tnUIPhone m_UIPhonePrefab = null;

    [SerializeField]
    private Text m_Info = null;

    private List<tnUIGamepad> m_Gamepads = null;
    private List<tnUIPhone> m_Phones = null;

    private UIEventTrigger m_TriggerProceed = null;
    private UIEventTrigger m_TriggerCancel = null;

    private bool m_HasFocus = false;

    // MonoBehaviour's interface

    void Awake()
    {
        m_Gamepads = new List<tnUIGamepad>();
        m_Phones = new List<tnUIPhone>();

        UIPageDescriptor pageDescriptor = GetComponentInChildren<UIPageDescriptor>();
        if (pageDescriptor != null)
        {
            m_TriggerProceed = pageDescriptor.GetWidget<UIEventTrigger>(s_ProceedTrigger);
            m_TriggerCancel = pageDescriptor.GetWidget<UIEventTrigger>(s_CancelTrigger);
        }

        InitializeGrids();
    }

    void OnEnable()
    {
        m_HasFocus = false;

        if (m_TeamA != null)
        {
            m_TeamA.Clear();
        }

        if (m_TeamB != null)
        {
            m_TeamB.Clear();
        }

        if (m_Info != null)
        {
            if (InputSystem.useXInputMain)
            {
                m_Info.text = s_InfoMessage1;
            }
            else
            {
                m_Info.text = s_InfoMessage2;
            }
        }
    }

    void OnDisable()
    {

    }

    void Start()
    {
        SpawnControllers();
    }

    void Update()
    {
        if (!m_HasFocus)
            return;

        UpdateProceedTrigger();
        UpdateCancelTrigger();

        CheckCancel();
        CheckProceed();
    }

    // UIViewController's interface

    public override void OnEnter()
    {
        base.OnEnter();

        for (int gamepadIndex = 0; gamepadIndex < m_Gamepads.Count; ++gamepadIndex)
        {
            tnUIGamepad gamepad = m_Gamepads[gamepadIndex];
            if (gamepad != null)
            {
                gamepad.hasFocus = true;
            }
        }

        for (int phoneIndex = 0; phoneIndex < m_Phones.Count; ++phoneIndex)
        {
            tnUIPhone phone = m_Phones[phoneIndex];
            if (phone != null)
            {
                phone.hasFocus = true;
            }
        }

        m_HasFocus = true;
    }

    public override void OnExit()
    {
        m_HasFocus = false;

        for (int phoneIndex = 0; phoneIndex < m_Phones.Count; ++phoneIndex)
        {
            tnUIPhone phone = m_Phones[phoneIndex];
            if (phone != null)
            {
                phone.hasFocus = false;
            }
        }

        for (int gamepadIndex = 0; gamepadIndex < m_Gamepads.Count; ++gamepadIndex)
        {
            tnUIGamepad gamepad = m_Gamepads[gamepadIndex];
            if (gamepad != null)
            {
                gamepad.hasFocus = false;
            }
        }

        base.OnExit();
    }

    // LOGIC

    public void UpdateModule()
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule == null)
            return;

        teamsModule.Clear();

        SetupTeamA(teamsModule);
        SetupTeamB(teamsModule);
    }

    // INTERNALS

    private void InitializeGrids()
    {
        if (m_TeamA != null)
        {
            m_TeamA.CreateGrid();
        }

        if (m_ControllersGrid != null)
        {
            m_ControllersGrid.CreateGrid();
        }

        if (m_TeamB != null)
        {
            m_TeamB.CreateGrid();
        }
    }

    private void SpawnControllers()
    {
        SpawnGamepads();
        SpawnPhones();
    }

    private void SpawnGamepads()
    {
        if (m_UIGamepadPrefab == null)
            return;

        if (m_ControllersGrid == null)
            return;

        if (m_ControllersRoot == null)
            return;

        List<int> playersKeys = tnGameData.GetPlayersKeysMain();

        if (playersKeys == null)
            return;

        int playersCount = InputSystem.numPlayersMain;
        int max = Mathf.Min(playersCount, playersKeys.Count);

        for (int index = 0; index < max; ++index)
        {
            ControllerAnchor anchor = m_ControllersGrid.GetAnchorByIndex(index);
            if (anchor != null)
            {
                int playerId = playersKeys[index];
                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);
                if (playerData != null)
                {
                    tnUIGamepad gamepad = GameObject.Instantiate<tnUIGamepad>(m_UIGamepadPrefab);
                    gamepad.transform.SetParent(m_ControllersRoot, false);

                    gamepad.SetPlayerId(playerId);
                    gamepad.SetPlayerName(playerData.playerInputName);

                    gamepad.SetDefaultAnchor(anchor);

                    gamepad.gameObject.name = playerData.name + "_Pad";
                    Color color = playerData.color;
                    gamepad.SetColor(color, true);

                    gamepad.SetTeamsManagers(m_TeamA, m_TeamB);

                    gamepad.hasFocus = false;

                    m_Gamepads.Add(gamepad);
                }
            }
        }
    }

    private void SpawnPhones()
    {
        if (m_UIPhonePrefab == null)
            return;

        if (m_ControllersGrid == null)
            return;

        if (m_ControllersRoot == null)
            return;

        List<int> playersKeys = tnGameData.GetPlayersKeysMain();

        if (playersKeys == null)
            return;

        int playersCount = WiFiInputSystem.playersCountMain;
        int max = Mathf.Min(playersCount, playersKeys.Count);

        for (int index = 0; index < max; ++index)
        {
            ControllerAnchor anchor = m_ControllersGrid.GetAnchorByIndex(index + m_Gamepads.Count);
            if (anchor != null)
            {
                int playerIndex = index + m_Gamepads.Count;

                if (playerIndex >= playersKeys.Count)
                    continue;

                int playerId = playersKeys[playerIndex];
                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);
                if (playerData != null)
                {
                    tnUIPhone phone = GameObject.Instantiate<tnUIPhone>(m_UIPhonePrefab);
                    phone.transform.SetParent(m_ControllersRoot, false);

                    phone.SetPlayerId(playerId);
                    phone.SetPlayerName(playerData.wifiPlayerInputName);

                    phone.SetDefaultAnchor(anchor);

                    phone.gameObject.name = playerData.name + "_Phone";
                    Color color = playerData.color;
                    phone.SetColor(color, true);

                    phone.SetTeamsManagers(m_TeamA, m_TeamB);

                    phone.hasFocus = false;

                    m_Phones.Add(phone);
                }
            }
        }
    }

    private void SetupTeamA(tnTeamsModule i_Module)
    {
        if (m_TeamA == null)
            return;

        tnTeamDescription teamDescription = new tnTeamDescription();

        // Add real players.

        for (int index = 0; index < m_TeamA.entriesCount; ++index)
        {
            GridEntry gridEntry = m_TeamA.GetEntryByIndex(index);

            if (gridEntry == null)
                continue;

            bool present = (gridEntry.device != null);
            if (present)
            {
                int playerId = gridEntry.device.playerId;

                tnCharacterDescription characterDescription = new tnCharacterDescription();
                characterDescription.SetPlayerId(playerId);

                teamDescription.AddCharacterDescription(characterDescription);

                LogManager.Log(this, LogContexts.FSM, "Added character to Team A [Real player]");
            }
        }

        // Add bots.

        for (int index = 0; index < m_TeamA.entriesCount; ++index)
        {
            GridEntry gridEntry = m_TeamA.GetEntryByIndex(index);

            if (gridEntry == null)
                continue;

            bool present = (gridEntry.isBot);
            if (present)
            {
                tnCharacterDescription characterDescription = new tnCharacterDescription();
                characterDescription.SetPlayerId(Hash.s_NULL);

                teamDescription.AddCharacterDescription(characterDescription);

                LogManager.Log(this, LogContexts.FSM, "Added character to Team A [BOT]");
            }
        }

        // Add team description.

        i_Module.AddTeamDescription(teamDescription);
    }

    private void SetupTeamB(tnTeamsModule i_Module)
    {
        if (m_TeamB == null)
            return;

        tnTeamDescription teamDescription = new tnTeamDescription();

        // Add real players.

        for (int index = 0; index < m_TeamB.entriesCount; ++index)
        {
            GridEntry gridEntry = m_TeamB.GetEntryByIndex(index);

            if (gridEntry == null)
                continue;

            bool present = (gridEntry.device != null);
            if (present)
            {
                int playerId = gridEntry.device.playerId;

                tnCharacterDescription characterDescription = new tnCharacterDescription();
                characterDescription.SetPlayerId(playerId);

                teamDescription.AddCharacterDescription(characterDescription);

                LogManager.Log(this, LogContexts.FSM, "Added character to Team A [Real player]");
            }
        }

        // Add bots.

        for (int index = 0; index < m_TeamB.entriesCount; ++index)
        {
            GridEntry gridEntry = m_TeamB.GetEntryByIndex(index);

            if (gridEntry == null)
                continue;

            bool present = (gridEntry.isBot);
            if (present)
            {
                tnCharacterDescription characterDescription = new tnCharacterDescription();
                characterDescription.SetPlayerId(Hash.s_NULL);

                teamDescription.AddCharacterDescription(characterDescription);

                LogManager.Log(this, LogContexts.FSM, "Added character to Team A [BOT]");
            }
        }

        // Add team description.

        i_Module.AddTeamDescription(teamDescription);
    }

    private void UpdateProceedTrigger()
    {
        if (m_TriggerProceed == null || m_TeamA == null || m_TeamB == null)
            return;

        bool teamAPresent = false;
        bool teamBPresent = false;

        bool teamAHasRealPlayer = false;
        bool teamBHasRealPlayer = false;

        // Check Team A

        for (int index = 0; index < m_TeamA.entriesCount; ++index)
        {
            GridEntry gridEntry = m_TeamA.GetEntryByIndex(index);
            if (gridEntry != null)
            {
                if (!gridEntry.isFree)
                {
                    teamAPresent = true;
                    teamAHasRealPlayer |= !gridEntry.isBot;
                }
            }
        }

        // Check Team B

        for (int index = 0; index < m_TeamB.entriesCount; ++index)
        {
            GridEntry gridEntry = m_TeamB.GetEntryByIndex(index);
            if (gridEntry != null)
            {
                if (!gridEntry.isFree)
                {
                    teamBPresent = true;
                    teamBHasRealPlayer |= !gridEntry.isBot;
                }
            }
        }

        // Update trigger state

        m_TriggerProceed.canSend = (teamAPresent && teamBPresent) && (teamAHasRealPlayer || teamBHasRealPlayer);
    }

    private void UpdateCancelTrigger()
    {
        if (m_TriggerCancel == null)
            return;

        // Nothing to do.
    }

    private void CheckProceed()
    {
        if (m_Gamepads == null || m_Phones == null || m_TriggerProceed == null)
            return;

        bool confirm = false;

        // Check gamepads

        for (int gamepadIndex = 0; gamepadIndex < m_Gamepads.Count; ++gamepadIndex)
        {
            tnUIGamepad gamepad = m_Gamepads[gamepadIndex];
            if (gamepad != null)
            {
                if (gamepad.deviceState == DeviceState.Left || gamepad.deviceState == DeviceState.Right)
                {
                    confirm |= gamepad.GetProceedButton();
                }
            }
        }

        // Check phones

        for (int phoneIndex = 0; phoneIndex < m_Phones.Count; ++phoneIndex)
        {
            tnUIPhone phone = m_Phones[phoneIndex];
            if (phone != null)
            {
                if (phone.deviceState == DeviceState.Left || phone.deviceState == DeviceState.Right)
                {
                    confirm |= phone.GetProceedButton();
                }
            }
        }

        if (confirm)
        {
            m_TriggerProceed.Invoke();
        }
    }

    private void CheckCancel()
    {
        if (m_Gamepads == null || m_Phones == null || m_TriggerCancel == null)
            return;

        bool cancel = false;

        // Check gamepads

        for (int gamepadIndex = 0; gamepadIndex < m_Gamepads.Count; ++gamepadIndex)
        {
            tnUIGamepad gamepad = m_Gamepads[gamepadIndex];
            if (gamepad != null)
            {
                cancel |= gamepad.GetCancelButton();
            }
        }

        // Check phones

        for (int phoneIndex = 0; phoneIndex < m_Phones.Count; ++phoneIndex)
        {
            tnUIPhone phone = m_Phones[phoneIndex];
            if (phone != null)
            {
                cancel |= phone.GetCancelButton();
            }
        }

        if (cancel)
        {
            m_TriggerCancel.Invoke();
        }
    }
}
