using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System;
using System.Collections.Generic;

using GoUI;

using WiFiInput.Server;

using TuesdayNights;

using IndexList = System.Collections.Generic.List<int>;

public class tnTeamSelectionController : UIViewController
{
    private static int s_MaxPlayers = 2;

    private static float s_ProceedDelay = 0.5f;

    private enum MoveDir
    {
        None,
        Left,
        Right,
        Up,
        Down,
    }

    [SerializeField]
    private GridLayoutGroup m_LayoutGroup = null;
    [SerializeField]
    private GameObject m_SlotPrefab = null;

    [SerializeField]
    private Image[] m_Images = null;
    [SerializeField]
    private Text[] m_Labels = null;

    [Serializable]
    public class NavigationEvent : UnityEvent { }
    [Serializable]
    public class SelectionEvent : UnityEvent { }

    [SerializeField]
    private NavigationEvent m_OnNavigate = new NavigationEvent();
    public NavigationEvent onNavigate { get { return m_OnNavigate; } set { m_OnNavigate = value; } }

    [SerializeField]
    private NavigationEvent m_OnSelect = new NavigationEvent();
    public NavigationEvent onSelect { get { return m_OnSelect; } set { m_OnSelect = value; } }

    [SerializeField]
    private NavigationEvent m_OnDeselect = new NavigationEvent();
    public NavigationEvent onDeselect { get { return m_OnDeselect; } set { m_OnDeselect = value; } }

    private List<GameObject> m_Slots = new List<GameObject>();

    private UIEventTrigger m_TriggerProceed = null;
    private UIEventTrigger m_TriggerCancel = null;

    private int[] m_Ids = new int[s_MaxPlayers];

    private PlayerInput[] m_Players = new PlayerInput[s_MaxPlayers];
    private WiFiPlayerInput[] m_WifiPlayers = new WiFiPlayerInput[s_MaxPlayers];

    private List<IndexList> m_ControllingMap = new List<IndexList>();

    private GameObject[] m_Selections = new GameObject[s_MaxPlayers];
    private bool[] m_Confirmations = new bool[s_MaxPlayers];

    private bool m_HasFocus = false;

    private bool m_BackRequested = false;
    private bool m_BackInvoked = false;

    private bool m_ProceedRequested = false;
    private bool m_ProceedInvoked = false;

    private float m_ProceedTimer = 0f;

    private static string s_PlayerInput_Left = "HorizontalLeft";
    private static string s_PlayerInput_Right = "HorizontalRight";
    private static string s_PlayerInput_Up = "VerticalUp";
    private static string s_PlayerInput_Down = "VerticalDown";

    private static string s_PlayerInput_Submit = "Submit";
    private static string s_PlayerInput_Cancel = "Cancel";

    private static string s_WiFiPlayerInput_Horizontal = "Horizontal";
    private static string s_WiFiPlayerInput_Vertical = "Vertical";

    private static string s_WiFiPlayerInput_Submit = "Submit";
    private static string s_WiFiPlayerInput_Cancel = "Cancel";

    private static string s_WidgetId_ProceedTrigger = "WIDGET_TRIGGER_PROCEED";
    private static string s_WidgetId_CancelTrigger = "WIDGET_TRIGGER_CANCEL";

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        // Initialize Controlling map.

        for (int index = 0; index < s_MaxPlayers; ++index)
        {
            IndexList indexList = new IndexList();
            m_ControllingMap.Add(indexList);
        }

        // Get widgets.

        UIPageDescriptor pageDescriptor = GetComponentInChildren<UIPageDescriptor>();
        if (pageDescriptor != null)
        {
            m_TriggerProceed = pageDescriptor.GetWidget<UIEventTrigger>(s_WidgetId_ProceedTrigger);
            m_TriggerCancel = pageDescriptor.GetWidget<UIEventTrigger>(s_WidgetId_CancelTrigger);
        }

        // Create and setup slots.

        if (m_LayoutGroup == null || m_SlotPrefab == null)
            return;

        // Configure grid layout.

        RectTransform slotPrefabTransform = m_SlotPrefab.GetComponent<RectTransform>();
        if (slotPrefabTransform != null)
        {
            m_LayoutGroup.cellSize = new Vector2(slotPrefabTransform.rect.width, slotPrefabTransform.rect.height);
        }

        // Spawn slots.

        List<int> teamKeys = tnGameData.GetTeamsKeysMain();

        foreach (int key in teamKeys)
        {
            GameObject slotInstance = (GameObject)Instantiate(m_SlotPrefab);
            slotInstance.SetParent(m_LayoutGroup.gameObject, true);

            tnTeamFlag teamFlag = slotInstance.GetComponent<tnTeamFlag>();
            if (teamFlag != null)
            {
                // Set Team Id.

                teamFlag.SetTeamId(key);

                // Set flag image and team name.

                tnTeamData teamData = tnGameData.GetTeamDataMain(key);
                teamFlag.SetImage(teamData.flag);
                teamFlag.SetLabel(teamData.name);
            }

            m_Slots.Add(slotInstance);
        }
    }

    void OnEnable()
    {
        // Clear slots.

        for (int slotIndex = 0; slotIndex < m_Slots.Count; ++slotIndex)
        {
            ClearSlot(slotIndex);
        }

        // Reset player ids.

        for (int playerIndex = 0; playerIndex < m_Players.Length; ++playerIndex)
        {
            m_Ids[playerIndex] = Hash.s_NULL;
        }

        // Clear players.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_Players[playerIndex] = null;
        }

        // Clear Wifi players.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_WifiPlayers[playerIndex] = null;
        }

        // Clear index lists.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            IndexList indexList = m_ControllingMap[playerIndex];
            indexList.Clear();
        }

        // Clear selection cache.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_Selections[playerIndex] = null;
        }

        // Clear confirmations.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_Confirmations[playerIndex] = false;
        }

        // Reset flags and timers.

        m_HasFocus = false;

        m_BackRequested = false;
        m_BackInvoked = false;

        m_ProceedRequested = false;
        m_ProceedInvoked = false;

        m_ProceedTimer = 0f;

        // Select controlling players.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            SelectPlayer(playerIndex);
        }
    }

    void OnDisable()
    {

    }

    void Update()
    {
        if (!m_HasFocus)
            return;

        // Process proceed requests.

        if (ProcessProceedRequests())
        {
            return;
        }

        // Process back requests.

        if (ProcessBackRequests())
        {
            return;
        }

        // Process user requests.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            if (!IsHuman(playerIndex))
                continue;

            if (!HasConfirmed(playerIndex))
            {
                CheckForPreviousControl(playerIndex);

                UpdateSelection(playerIndex);
                CheckForConfirmation(playerIndex);
            }
            else
            {
                CheckForCancelation(playerIndex);
            }

            CheckForControlSwitch(playerIndex);
        }
    }

    // UIViewController's interface

    public override void OnEnter()
    {
        base.OnEnter();

        // Configure selections.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            if (IsHuman(playerIndex))
            {
                ForceSelection(playerIndex);
            }
        }

        m_HasFocus = true;
    }

    public override void OnExit()
    {
        base.OnExit();

        m_HasFocus = false;
    }

    // BUSINESS LOGIC

    public void UpdateModule()
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule == null)
            return;

        // Evaluate teams colors.

        Color[] teamColors = null;

        {
            int[] teamIds = new int[s_MaxPlayers];

            for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
            {
                teamIds[teamIndex] = GetTeamId(teamIndex);
            }

            teamColors = Utils.ComputeTeamColors(teamIds);
        }

        // Fill team descriptors.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

            if (teamDescription == null)
                continue;

            // Set team id.

            int teamId = GetTeamId(teamIndex);

            teamDescription.SetTeamId(teamId);

            // Set team color.

            Color teamColor;

            if (teamColors != null)
            {
                teamColor = teamColors[teamIndex];
            }
            else
            {
                teamColor = Color.white;
            }

            teamDescription.SetTeamColor(teamColor);

            LogManager.Log(this, LogContexts.FSM, "Team " + teamIndex + " : " + teamId + " " + "[" + teamColor + "]");
        }
    }

    // INTERNALS

    private void SelectPlayer(int i_TeamIndex)
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        tnTeamDescription teamDescription = teamsModule.GetTeamDescription(i_TeamIndex);
        if (teamDescription != null)
        {
            int playerId = teamDescription.captainPlayerId;
            m_Ids[i_TeamIndex] = playerId;

            // Cache PlayerInput.

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);
            if (playerData != null)
            {
                if (!StringUtils.IsNullOrEmpty(playerData.playerInputName))
                {
                    m_Players[i_TeamIndex] = InputSystem.GetPlayerByNameMain(playerData.playerInputName);

                    PushControllingMap(i_TeamIndex, i_TeamIndex);
                }
                else
                {
                    if (!StringUtils.IsNullOrEmpty(playerData.wifiPlayerInputName))
                    {
                        m_WifiPlayers[i_TeamIndex] = WiFiInputSystem.GetPlayerByNameMain(playerData.wifiPlayerInputName);

                        PushControllingMap(i_TeamIndex, i_TeamIndex);
                    }
                }
            }
        }

        // Update image and label.

        UpdateImageColor(i_TeamIndex);
        UpdateLabel(i_TeamIndex);
    }

    private void ForceSelection(int i_PlayerIndex)
    {
        Select(i_PlayerIndex, GetFirstAvailableSlot());
    }

    private void UpdateSelection(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        // Check for selection changes.

        MoveDir moveDirection = MoveDir.None;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            moveDirection = GetMoveDirection(playerInput);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WifiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                moveDirection = GetMoveDirection(wifiPlayerInput);
            }
        }

        if (moveDirection != MoveDir.None)
        {
            IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];

            int controlledPlayerIndex = controlledPlayers.GetLast();

            if (!IsValidIndex(controlledPlayerIndex))
                return;

            GameObject currentSelection = m_Selections[controlledPlayerIndex];
            Select(controlledPlayerIndex, GetNearest(currentSelection, moveDirection));
        }
    }

    private void CheckForConfirmation(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];

        int controlledPlayerIndex = controlledPlayers.GetLast();

        if (!IsValidIndex(controlledPlayerIndex))
            return;

        bool alreadyConfirmed = m_Confirmations[controlledPlayerIndex];

        if (alreadyConfirmed)
            return;

        bool confirmPressed = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            confirmPressed = playerInput.GetButtonDown(s_PlayerInput_Submit);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WifiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                confirmPressed = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Submit);
            }
        }

        if (confirmPressed)
        {
            GameObject currentSelection = m_Selections[controlledPlayerIndex];
            if (currentSelection != null)
            {
                tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    teamFlag.SetSelected(GetPlayerColor(controlledPlayerIndex));
                }

                // Raise event.

                if (m_OnSelect != null)
                {
                    m_OnSelect.Invoke();
                }

                m_Confirmations[controlledPlayerIndex] = true;
            }
        }
    }

    private void CheckForCancelation(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];

        int controlledPlayerIndex = controlledPlayers.GetLast();

        if (!IsValidIndex(controlledPlayerIndex))
            return;

        bool cancelPressed = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            cancelPressed = playerInput.GetButtonDown(s_PlayerInput_Cancel);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WifiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                cancelPressed = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Cancel);
            }
        }

        if (cancelPressed)
        {
            GameObject currentSelection = m_Selections[controlledPlayerIndex];
            if (currentSelection != null)
            {
                tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    teamFlag.SetHighlighted(GetPlayerColor(controlledPlayerIndex));
                }

                // Raise event.

                if (m_OnDeselect != null)
                {
                    m_OnDeselect.Invoke();
                }

                m_Confirmations[controlledPlayerIndex] = false;
            }
        }
    }

    private void CheckForPreviousControl(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        bool cancelPressed = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            cancelPressed = playerInput.GetButtonDown(s_PlayerInput_Cancel);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WifiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                cancelPressed = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Cancel);
            }
        }

        if (cancelPressed)
        {
            PopControllingMap(i_PlayerIndex);
        }
    }

    private bool ProcessProceedRequests()
    {
        if (m_ProceedInvoked)
        {
            return true;
        }

        if (!m_ProceedRequested)
        {
            bool allConfirmed = true;

            for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
            {
                allConfirmed &= m_Confirmations[playerIndex];
            }

            if (allConfirmed)
            {
                m_ProceedRequested = true;
            }
        }

        if (m_ProceedRequested && !m_ProceedInvoked)
        {
            m_ProceedTimer += Time.deltaTime;

            if (m_ProceedTimer > s_ProceedDelay)
            {
                if (m_TriggerProceed != null && m_TriggerProceed.enabled)
                {
                    m_TriggerProceed.Invoke();
                }

                m_ProceedInvoked = true;
            }
        }

        return m_ProceedRequested;
    }

    private bool ProcessBackRequests()
    {
        if (m_BackInvoked)
        {
            return true;
        }

        if (!m_BackRequested)
        {
            bool backRequest = false;

            for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
            {
                bool backPressed = false;

                PlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    backPressed |= playerInput.GetButtonDown(s_PlayerInput_Cancel);
                }
                else
                {
                    WiFiPlayerInput wifiPlayerInput = m_WifiPlayers[playerIndex];
                    if (wifiPlayerInput != null)
                    {
                        backPressed |= wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Cancel);
                    }
                }

                backRequest |= backPressed;
                backRequest &= !m_Confirmations[playerIndex];
            }

            if (backRequest)
            {
                m_BackRequested = true;
            }
        }

        if (m_BackRequested && !m_BackInvoked)
        {
            if (m_TriggerCancel != null && m_TriggerCancel.enabled)
            {
                m_TriggerCancel.Invoke();
            }

            m_BackInvoked = true;
        }

        return m_BackRequested;
    }

    private bool HasConfirmed(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
        {
            return false;
        }

        IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];

        int controlledPlayerIndex = controlledPlayers.GetLast();

        if (!IsValidIndex(controlledPlayerIndex))
        {
            return false;
        }

        return m_Confirmations[controlledPlayerIndex];
    }

    private void CheckForControlSwitch(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];

        int controlledPlayerIndex = controlledPlayers.GetLast();

        if (!IsValidIndex(controlledPlayerIndex))
            return;

        bool alreadyConfirmed = m_Confirmations[controlledPlayerIndex];

        if (!alreadyConfirmed)
            return;

        for (int aiIndex = 0; aiIndex < s_MaxPlayers; ++aiIndex)
        {
            if (aiIndex == i_PlayerIndex || IsHuman(aiIndex))
                continue;

            if (m_Confirmations[aiIndex])
                continue;
               
            PushControllingMap(i_PlayerIndex, aiIndex);
            ForceSelection(aiIndex);

            break;
        }
    }

    private GameObject GetNearest(GameObject i_Source, MoveDir i_Direction)
    {
        if (i_Source == null)
        {
            return null;
        }

        GameObject nextSelection = null;

        switch (i_Direction)
        {
            case MoveDir.Left:
                nextSelection = GetNearestOnLeft(i_Source);
                break;
            case MoveDir.Right:
                nextSelection = GetNearestOnRight(i_Source);
                break;
            case MoveDir.Up:
                nextSelection = GetNearestOnUp(i_Source);
                break;
            case MoveDir.Down:
                nextSelection = GetNearestOnDown(i_Source);
                break;
        }

        return nextSelection;
    }

    private GameObject GetNearestOnLeft(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnLeft();

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnLeft();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnRight(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnRight();

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnRight();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnUp(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnUp();

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnUp();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnDown(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnDown();

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnDown();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private bool IsAvailable(GameObject i_Slot)
    {
        if (i_Slot == null)
        {
            return false;
        }

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            GameObject currentSelection = m_Selections[playerIndex];
            if (i_Slot == currentSelection)
            {
                return false;
            }
        }

        return true;
    }

    private GameObject GetFirstAvailableSlot()
    {
        for (int slotIndex = 0; slotIndex < m_Slots.Count; ++slotIndex)
        {
            GameObject slotInstance = m_Slots[slotIndex];

            if (IsAvailable(slotInstance))
            {
                return slotInstance;
            }
        }

        return null;
    }

    private MoveDir GetMoveDirection(PlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
        {
            return MoveDir.None;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Left))
        {
            return MoveDir.Left;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Right))
        {
            return MoveDir.Right;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Up))
        {
            return MoveDir.Up;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Down))
        {
            return MoveDir.Down;
        }

        return MoveDir.None; // Something went wrong.
    }

    private MoveDir GetMoveDirection(WiFiPlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
        {
            return MoveDir.None;
        }
        else if (i_PlayerInput.GetNegativeButtonDown(s_WiFiPlayerInput_Horizontal))
        {
            return MoveDir.Left;
        }
        else if (i_PlayerInput.GetPositiveButtonDown(s_WiFiPlayerInput_Horizontal))
        {
            return MoveDir.Right;
        }
        else if (i_PlayerInput.GetPositiveButtonDown(s_WiFiPlayerInput_Vertical))
        {
            return MoveDir.Up;
        }
        else if (i_PlayerInput.GetNegativeButtonDown(s_WiFiPlayerInput_Vertical))
        {
            return MoveDir.Down;
        }

        return MoveDir.None; // Something went wrong.
    }

    private void Select(int i_TeamIndex, GameObject i_Slot)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        if (i_Slot == null)
            return;

        // Release previous selection.

        {
            GameObject currentSelection = m_Selections[i_TeamIndex];

            if (currentSelection != null)
            {
                tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    teamFlag.SetAvailable();
                }

                // Raise event.

                if (m_OnNavigate != null)
                {
                    m_OnNavigate.Invoke();
                }
            }
        }

        m_Selections[i_TeamIndex] = i_Slot;

        // Update new selection.

        {
            tnTeamFlag teamFlag = i_Slot.GetComponent<tnTeamFlag>();
            if (teamFlag != null)
            {
                teamFlag.SetHighlighted(GetPlayerColor(i_TeamIndex));
            }
        }
    }

    private void Select(int i_TeamIndex, int i_SlotIndex)
    {
        if (i_SlotIndex < 0 || i_SlotIndex >= m_Slots.Count)
        {
            return;
        }

        GameObject slotInstance = m_Slots[i_SlotIndex];
        Select(i_TeamIndex, slotInstance);
    }

    private bool IsHuman(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
        {
            return false;
        }

        return (m_Players[i_PlayerIndex] != null || m_WifiPlayers[i_PlayerIndex] != null);
    }

    private void ClearSlot(int i_SlotIndex)
    {
        if (i_SlotIndex < 0 || i_SlotIndex >= m_Slots.Count)
        {
            return;
        }

        GameObject slotInstance = m_Slots[i_SlotIndex];

        tnTeamFlag teamFlag = slotInstance.GetComponent<tnTeamFlag>();
        if (teamFlag != null)
        {
            teamFlag.SetAvailable();
        }
    }

    private void UpdatePlayerInputCache(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        int playerId = m_Ids[i_PlayerIndex];

        if (playerId == Hash.s_NULL)
            return;

        tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);
        if (playerData != null)
        {
            if (!StringUtils.IsNullOrEmpty(playerData.playerInputName))
            {
                m_Players[i_PlayerIndex] = InputSystem.GetPlayerByNameMain(playerData.playerInputName);
            }
            else
            {
                if (!StringUtils.IsNullOrEmpty(playerData.wifiPlayerInputName))
                {
                    m_WifiPlayers[i_PlayerIndex] = WiFiInputSystem.GetPlayerByNameMain(playerData.wifiPlayerInputName);
                }
            }
        }
    }

    private void UpdateImageColor(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= m_Images.Length)
            return;

        Image image = m_Images[i_PlayerIndex];

        if (image != null)
        {
            image.color = GetPlayerColor(i_PlayerIndex);
        }
    }

    private void UpdateLabel(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= m_Labels.Length)
            return;

        Text label = m_Labels[i_PlayerIndex];

        if (label != null)
        {
            label.text = GetPlayerName(i_PlayerIndex);
        }
    }

    private Color GetPlayerColor(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
        {
            return Color.white;
        }

        int playerId = m_Ids[i_PlayerIndex];
        return GetPlayerColorById(playerId);
    }

    private Color GetPlayerColorById(int i_PlayerId)
    {
        if (i_PlayerId != Hash.s_NULL)
        {
            tnPlayerData playerData = tnGameData.GetPlayerDataMain(i_PlayerId);
            if (playerData != null)
            {
                return playerData.color;
            }
        }

        return Color.white; // Invalid player.
    }

    private string GetPlayerName(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
        {
            return StringUtils.s_NULL;
        }

        int playerId = m_Ids[i_PlayerIndex];
        return GetPlayerNameById(playerId);
    }

    private string GetPlayerNameById(int i_PlayerId)
    {
        if (i_PlayerId != Hash.s_NULL)
        {
            tnPlayerData playerData = tnGameData.GetPlayerDataMain(i_PlayerId);
            if (playerData != null)
            {
                return playerData.name;
            }
        }

        return StringUtils.s_NULL; // Invalid player.
    }

    private int GetTeamId(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
        {
            return Hash.s_NULL;
        }

        GameObject currentSelection = m_Selections[i_PlayerIndex];

        if (currentSelection != null)
        {
            tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
            if (teamFlag != null)
            {
                return teamFlag.teamId;
            }
        }

        return Hash.s_NULL; // Invalid team.
    }

    private bool IsValidIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_MaxPlayers)
        {
            return false;
        }

        return true;
    }

    private void PopControllingMap(int i_PlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];

        if (controlledPlayers.Count <= 1)
            return;

        {
            int controlledPlayerIndex = controlledPlayers.GetLast();

            GameObject currentSelection = m_Selections[controlledPlayerIndex];
            if (currentSelection != null)
            {
                tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    teamFlag.SetAvailable();
                }
            }

            m_Selections[controlledPlayerIndex] = null;
        }

        controlledPlayers.Pop();

        {
            int newControlledPlayerIndex = controlledPlayers.GetLast();

            if (!IsValidIndex(newControlledPlayerIndex))
                return;

            GameObject currentSelection = m_Selections[newControlledPlayerIndex];
            if (currentSelection != null)
            {
                tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    teamFlag.SetHighlighted(GetPlayerColor(newControlledPlayerIndex));
                }
            }

            m_Confirmations[newControlledPlayerIndex] = false;
        }
    }

    private void PushControllingMap(int i_PlayerIndex, int i_ControlledPlayerIndex)
    {
        if (!IsValidIndex(i_PlayerIndex))
            return;

        IndexList controlledPlayers = m_ControllingMap[i_PlayerIndex];
        controlledPlayers.Add(i_ControlledPlayerIndex);
    }
}
