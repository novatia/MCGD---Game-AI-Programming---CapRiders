using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using WiFiInput.Server;

using GoUI;

using CharacterList = System.Collections.Generic.List<tnUICharacter>;
using SlotList = System.Collections.Generic.List<tnUICharacterSlot>;
using PlayerIndexMap = System.Collections.Generic.Dictionary<int, int>;
using SelectionCache = System.Collections.Generic.List<UnityEngine.GameObject>;

public enum SwapType
{
    Character,
    Color,
}

public class tnCharacterSelectionController : UIViewController
{
    // STATIC

    private static int s_MaxPlayers = 2;
    private static int s_TeamSize = 11;

    private static float s_ProceedDelay = 0.5f;

    private static string s_TriggerProceed = "TRIGGER_PROCEED";
    private static string s_TriggerCancel = "TRIGGER_CANCEL";

    private static string s_PlayerInput_Left = "HorizontalLeft";
    private static string s_PlayerInput_Right = "HorizontalRight";
    private static string s_PlayerInput_Up = "VerticalUp";
    private static string s_PlayerInput_Down = "VerticalDown";

    private static string s_PlayerInput_Submit = "Submit";
    private static string s_PlayerInput_Cancel = "Cancel";
    private static string s_PlayerInput_Start = "Start";

    private static string s_WiFiPlayerInput_Horizontal = "Horizontal";
    private static string s_WiFiPlayerInput_Vertical = "Vertical";

    private static string s_WiFiPlayerInput_Submit = "Submit";
    private static string s_WiFiPlayerInput_Cancel = "Cancel";
    private static string s_WiFiPlayerInput_Start = "Start";

    // Types

    private enum MoveDir
    {
        None,
        Left,
        Right,
        Up,
        Down,
    }

    private enum FacingDir
    {
        FacingRight,
        FacingLeft,
    }

    // Fields

    [SerializeField]
    private tnUICharacter m_CharacterPrefab = null;
    [SerializeField]
    private tnUICharacterSlot m_CharacterSlotPrefab = null;

    [SerializeField]
    private RectTransform m_CharactersRoot = null;
    [SerializeField]
    private RectTransform m_SlotsRoot = null;

    //[SerializeField]
    //private tnUISwapMenu[] m_SwapMenus = new tnUISwapMenu[s_MaxPlayers];

    [SerializeField]
    private tnUITeamInfo[] m_TeamInfo = new tnUITeamInfo[s_MaxPlayers];
    [SerializeField]
    private tnUITeamAnchors[] m_TeamAnchorsSets = new tnUITeamAnchors[s_MaxPlayers];
    [SerializeField]
    private tnUIBench[] m_TeamAnchorsBench = new tnUIBench[s_MaxPlayers];
    [SerializeField]
    private tnCharacterPortrait[] m_Portraits = new tnCharacterPortrait[s_MaxPlayers];
    [SerializeField]
    private FacingDir[] m_Facing = new FacingDir[s_MaxPlayers];
    [SerializeField]
    private GameObject[] m_Overlays = new GameObject[s_MaxPlayers];

    [Serializable]
    public class NavigationEvent : UnityEvent { }

    [Serializable]
    public class SelectionEvent : UnityEvent { }

    [Serializable]
    public class ReadyEvent : UnityEvent { }
    [Serializable]
    public class CancelEvent : UnityEvent { }

    [SerializeField]
    private NavigationEvent m_OnNavigate = new NavigationEvent();
    public NavigationEvent onNavigate { get { return m_OnNavigate; } set { m_OnNavigate = value; } }

    [SerializeField]
    private SelectionEvent m_OnSelect = new SelectionEvent();
    public SelectionEvent onSelect { get { return m_OnSelect; } set { m_OnSelect = value; } }

    [SerializeField]
    private ReadyEvent m_OnReady = new ReadyEvent();
    public ReadyEvent onReady { get { return m_OnReady; } set { m_OnReady = value; } }

    [SerializeField]
    private CancelEvent m_OnCancel = new CancelEvent();
    public CancelEvent onCancel { get { return m_OnCancel; } set { m_OnCancel = value; } }

    private tnUIAnchorsSet[] m_TeamAnchors = new tnUIAnchorsSet[s_MaxPlayers];

    private int[] m_Ids = new int[s_MaxPlayers];

    private PlayerInput[] m_Players = new PlayerInput[s_MaxPlayers];
    private WiFiPlayerInput[] m_WiFiPlayers = new WiFiPlayerInput[s_MaxPlayers];

    private bool[] m_Confirmations = new bool[s_MaxPlayers];
    //private bool[] m_IsSwapping = new bool[s_MaxPlayers];

    private SelectionCache[] m_SelectionsCache = new SelectionCache[s_MaxPlayers];
    private GameObject[] m_CurrentSlots = new GameObject[s_MaxPlayers];

    private CharacterList m_CharactersPool = new CharacterList();
    private SlotList m_CharactersSlotsPool = new SlotList();

    private SlotList[] m_LineUp = new SlotList[s_MaxPlayers];
    private SlotList[] m_Bench = new SlotList[s_MaxPlayers];

    private PlayerIndexMap[] m_PlayerIndexMaps = new PlayerIndexMap[s_MaxPlayers];

    private bool[] m_SkipFrame = new bool[s_MaxPlayers];

    private UIEventTrigger m_TriggerProceed = null;
    private UIEventTrigger m_TriggerCancel = null;

    private bool m_HasFocus = false;

    private bool m_BackRequested = false;
    private bool m_BackInvoked = false;

    private bool m_ProceedRequested = false;
    private bool m_ProceedInvoked = false;

    private float m_ProceedTimer = 0f;

    // MonoBehaviour's interface

    void Awake()
    {
        // Get widgets references.

        UIPageDescriptor pageDescriptor = GetComponentInChildren<UIPageDescriptor>();
        if (pageDescriptor != null)
        {
            m_TriggerProceed = pageDescriptor.GetWidget<UIEventTrigger>(s_TriggerProceed);
            m_TriggerCancel = pageDescriptor.GetWidget<UIEventTrigger>(s_TriggerCancel);
        }

        // Create data structures.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_LineUp[playerIndex] = new SlotList();
            m_Bench[playerIndex] = new SlotList();

            m_PlayerIndexMaps[playerIndex] = new PlayerIndexMap();

            m_SelectionsCache[playerIndex] = new SelectionCache();
        }

        // Initialize pools.

        InitializePool();
    }

    void OnEnable()
    {
        // Clear player index map

        for (int playerIndex = 0; playerIndex < m_Players.Length; ++playerIndex)
        {
            m_PlayerIndexMaps[playerIndex].Clear();
        }

        // Reset player ids.

        for (int playerIndex = 0; playerIndex < m_Players.Length; ++playerIndex)
        {
            m_Ids[playerIndex] = Hash.s_NULL;
        }

        // Clear just close menu

        for (int playerIndex = 0; playerIndex < m_Players.Length; ++playerIndex)
        {
            m_SkipFrame[playerIndex] = false;
        }

        // Clear players.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_Players[playerIndex] = null;
        }

        // Clear Wifi players.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_WiFiPlayers[playerIndex] = null;
        }

        // Clear selection cache.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_SelectionsCache[playerIndex].Clear();
        }

        // Clear current slots.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_CurrentSlots[playerIndex] = null;
        }

        // Clear confirmations.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_Confirmations[playerIndex] = false;
        }

        // Clear swapping flag.

        /*

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_IsSwapping[playerIndex] = false;
        }

        */

        // Clear anchors.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_TeamAnchors[playerIndex] = null;
        }

        // Clear line-up.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            ClearLineUp(playerIndex);
        }

        // Clear bench.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            ClearBench(playerIndex);
        }

        // Disable overlays.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            ClearOverlay(playerIndex);
        }

        // Clear potraits.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            ClearPortrait(playerIndex);
            SetPortraitEnable(playerIndex, false);
        }

        // Disable SwapMenues

        /*
        
        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            tnUISwapMenu swapMenu = m_SwapMenus[playerIndex];
            if (swapMenu != null)
            {
                swapMenu.transform.position = Vector3.zero;
                swapMenu.gameObject.SetActive(false);
            }
        }

        */

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

        // Setup teams.

        SetupTeams();
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

        // Process users requests.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            if (IsReady(playerIndex))
                continue;

            if (m_SkipFrame[playerIndex])
            {
                m_SkipFrame[playerIndex] = false;
                continue;
            }

            //if (!IsSwapping(playerIndex))
            {
                UpdateSelection(playerIndex);
                UpdatePortrait(playerIndex);
                CheckForConfirmSelection(playerIndex);
            }
            //else
            //{
            //    UpdateMenu(playerIndex);
            //}

            CheckForCancelSelection(playerIndex);
        }

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            if (m_SkipFrame[playerIndex])
            {
                m_SkipFrame[playerIndex] = false;
                continue;
            }

            //if (!IsSwapping(playerIndex))
            {
                if (!IsReady(playerIndex))
                {
                    CheckForReady(playerIndex);
                }
                else
                {
                    CheckForCancelation(playerIndex);
                }
            }

            UpdateOverlay(playerIndex);
        }
    }

    // UIViewController's interface

    public override void OnEnter()
    {
        base.OnEnter();

        // Configure selections.

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            SelectFirst(playerIndex);
            UpdatePortrait(playerIndex);
            SetPortraitEnable(playerIndex, true);
        }

        m_HasFocus = true;
    }

    public override void OnExit()
    {
        base.OnExit();

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            ClearPortrait(playerIndex);
            SetPortraitEnable(playerIndex, false);
        }

        m_HasFocus = false;
    }

    // LOGIC

    public void UpdateModule()
    {
        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            ConfigureTeam(playerIndex);
        }
    }

    // INTERNALS

    // Check flag

    private bool IsReady(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
        {
            return false;
        }

        return m_Confirmations[i_PlayerIndex];
    }

    /*

    private bool IsSwapping(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
        {
            return false;
        }

        return m_IsSwapping[i_PlayerIndex];
    }

    */

    //////////////////////////////////////

    // Selection check

    private void CheckForConfirmSelection(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        bool confirmPressed = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            confirmPressed = playerInput.GetButtonDown(s_PlayerInput_Submit);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                confirmPressed = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Submit);
            }
        }

        if (confirmPressed)
        {
            SelectionCache cache = m_SelectionsCache[i_PlayerIndex];

            GameObject currentSelection = m_CurrentSlots[i_PlayerIndex];

            if (currentSelection != null)
            {
                if (cache.IsEmpty())
                {
                    cache.Add(currentSelection);

                    tnUICharacterSlot slot = currentSelection.GetComponent<tnUICharacterSlot>();
                    if (slot != null)
                    {
                        slot.Select();

                        if (m_OnSelect != null)
                        {
                            m_OnSelect.Invoke();
                        }
                    }
                }
                else
                {
                    // Open Menu

                    cache.Add(currentSelection);

                    // bool needMenu = true;

                    GameObject first = cache[0];
                    GameObject second = cache[1];

                    if (first == null || second == null)
                        return;

                    tnUICharacterSlot firstSlot = first.GetComponent<tnUICharacterSlot>();
                    tnUICharacterSlot secondSlot = second.GetComponent<tnUICharacterSlot>();

                    if (firstSlot == null || secondSlot == null)
                        return;

                    /*

                    if (firstSlot == secondSlot)
                    {
                        needMenu = false;
                    }

                    SlotList bench = m_Bench[i_PlayerIndex];
                    if (bench.Contains(firstSlot) || bench.Contains(secondSlot))
                    {
                        needMenu = false;
                    }

                    if (!firstSlot.hasPlayerColor && !secondSlot.hasPlayerColor)
                    {
                        needMenu = false;
                    }

                    if (needMenu)
                    {
                        OpenMenu(i_PlayerIndex, secondSlot.transform.position);
                    }
                    else

                    */
                    
                    {
                        if (cache.Count == 2)
                        {
                            Swap(i_PlayerIndex, firstSlot, secondSlot, SwapType.Character);

                            firstSlot.Deselect();
                            secondSlot.Deselect();

                            cache.Clear();
                        }
                    }
                }
            }
        }
    }

    private void CheckForCancelSelection(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        bool hasCanceled = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            hasCanceled = playerInput.GetButtonDown(s_PlayerInput_Cancel);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                hasCanceled = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Cancel);
            }
        }

        if (hasCanceled)
        {
            /*
            
            if (IsSwapping(i_PlayerIndex))
            {
                // Close Menu and deselect all.

                // CloseMenu(i_PlayerIndex);

                SelectionCache cache = m_SelectionsCache[i_PlayerIndex];

                if (cache.Count == 2)
                {
                    GameObject lastSelection = cache[1];
                    if (lastSelection != null)
                    {
                        tnUICharacterSlot slot = lastSelection.GetComponent<tnUICharacterSlot>();
                        if (slot != null)
                        {
                            slot.Deselect();
                        }
                    }

                    cache.RemoveAt(1);
                }
            }
            else

            */
            {
                SelectionCache cache = m_SelectionsCache[i_PlayerIndex];

                if (!cache.IsEmpty())
                {
                    GameObject lastSelection = cache[0];
                    if (lastSelection != null)
                    {
                        tnUICharacterSlot slot = lastSelection.GetComponent<tnUICharacterSlot>();
                        if (slot != null)
                        {
                            slot.Deselect();

                            if (m_OnCancel != null)
                            {
                                m_OnCancel.Invoke();
                            }
                        }
                    }

                    cache.Clear();
                }
            }
        }
    }

    private void UpdateSelection(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        MoveDir moveDirection = MoveDir.None;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            moveDirection = GetMoveDirection(playerInput);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                moveDirection = GetMoveDirection(wifiPlayerInput);
            }
        }

        if (moveDirection != MoveDir.None)
        {
            GameObject currentSelection = m_CurrentSlots[i_PlayerIndex];
            Select(i_PlayerIndex, GetNearest(currentSelection, moveDirection, i_PlayerIndex));
        }
    }

    //////////////////////////////////////

    // Ready Check

    private void CheckForReady(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        bool isReady = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            isReady = playerInput.GetButtonDown(s_PlayerInput_Start);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                isReady = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Start);
            }
            else
            {
                isReady = true;
            }
        }

        if (isReady)
        {
            {
                // Cancel current selection, if any.

                SelectionCache cache = m_SelectionsCache[i_PlayerIndex];

                if (!cache.IsEmpty())
                {
                    GameObject lastSelection = cache[0];
                    if (lastSelection != null)
                    {
                        tnUICharacterSlot slot = lastSelection.GetComponent<tnUICharacterSlot>();
                        if (slot != null)
                        {
                            slot.Deselect();
                        }
                    }

                    cache.Clear();
                }
            }

            m_Confirmations[i_PlayerIndex] = true;

            if (m_OnReady != null)
            {
                m_OnReady.Invoke();
            }
        }
    }

    private void CheckForCancelation(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        bool hasCanceled = false;

        PlayerInput playerInput = m_Players[i_PlayerIndex];
        if (playerInput != null)
        {
            hasCanceled = playerInput.GetButtonDown(s_PlayerInput_Cancel);
        }
        else
        {
            WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[i_PlayerIndex];
            if (wifiPlayerInput != null)
            {
                hasCanceled = wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Cancel);
            }
        }

        if (hasCanceled)
        {
            m_Confirmations[i_PlayerIndex] = false;

            if (m_OnCancel != null)
            {
                m_OnCancel.Invoke();
            }
        }
    }

    /////////////////////////////////////

    // Update Overlay

    private void UpdateOverlay(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        GameObject panelGo = m_Overlays[i_PlayerIndex];

        if (panelGo != null)
        {
            bool isActive = m_Confirmations[i_PlayerIndex];
            panelGo.SetActive(isActive);
        }
    }

    private void ClearOverlay(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        GameObject panelGo = m_Overlays[i_PlayerIndex];

        if (panelGo != null)
        {
            panelGo.SetActive(false);
        }
    }

    /////////////////////////////////////

    // Update Portrait

    private void UpdatePortrait(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        tnCharacterPortrait portrait = m_Portraits[i_PlayerIndex];
        GameObject currentSelection = m_CurrentSlots[i_PlayerIndex];

        if (portrait != null && currentSelection != null)
        {
            tnUICharacterSlot slot = currentSelection.GetComponent<tnUICharacterSlot>();
            if (slot != null)
            {
                int characterId = slot.characterId;
                if (characterId != Hash.s_NULL)
                {
                    tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);
                    if (characterData != null)
                    {
                        FacingDir facing = m_Facing[i_PlayerIndex];

                        Sprite characterSprite = (facing == FacingDir.FacingLeft) ? characterData.uiIconFacingLeft : characterData.uiIconFacingRight;

                        portrait.SetCharacterPortrait(characterSprite);
                        portrait.SetName(characterData.displayName);
                    }
                }
            }
        }
    }

    private void ClearPortrait(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        tnCharacterPortrait portrait = m_Portraits[i_PlayerIndex];

        if (portrait != null)
        {
            portrait.Clear();
        }
    }

    private void SetPortraitEnable(int i_PlayerIndex, bool i_Enable)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        tnCharacterPortrait portrait = m_Portraits[i_PlayerIndex];

        if (portrait != null)
        {
            portrait.gameObject.SetActive(i_Enable);
        }
    }

    ///////////////////////////////////////

    // Process Triggers Requests

    private bool ProcessProceedRequests()
    {
        if (m_ProceedInvoked)
        {
            return true;
        }

        if (!m_ProceedRequested)
        {
            bool allReady = true;

            for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
            {
                allReady &= m_Confirmations[playerIndex];
            }

            if (allReady)
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

                    m_ProceedInvoked = true;
                }
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
                SelectionCache cache = m_SelectionsCache[playerIndex];

                if (m_Confirmations[playerIndex] /*|| m_IsSwapping[playerIndex]*/ || !cache.IsEmpty())
                    continue;

                bool backPressed = false;

                PlayerInput playerInput = m_Players[playerIndex];
                if (playerInput != null)
                {
                    backPressed |= playerInput.GetButtonDown(s_PlayerInput_Cancel);
                }
                else
                {
                    WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[playerIndex];
                    if (wifiPlayerInput != null)
                    {
                        backPressed |= wifiPlayerInput.GetButtonDown(s_WiFiPlayerInput_Cancel);
                    }
                }

                backRequest |= backPressed;
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

                m_BackInvoked = true;
            }
        }

        return m_BackRequested;
    }

    ////////////////////////////////////////

    // Handle Menu

    /*

    private void OpenMenu(int i_PlayerIndex, Vector2 i_Position)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= m_SwapMenus.Length)
            return;

        tnUISwapMenu swapMenu = m_SwapMenus[i_PlayerIndex];

        if (swapMenu == null)
            return;

        swapMenu.gameObject.SetActive(true);

        swapMenu.transform.position = i_Position;

        // Configure callbacks.

        {
            SwapButtonClicked onSwapCharacterEvent = () =>
            {
                SelectionCache cache = m_SelectionsCache[i_PlayerIndex];

                if (cache.Count != 2)
                    return;

                GameObject first = cache[0];
                GameObject second = cache[1];

                if (first != null && second != null)
                {
                    tnUICharacterSlot firstSlot = first.GetComponent<tnUICharacterSlot>();
                    tnUICharacterSlot secondSlot = second.GetComponent<tnUICharacterSlot>();

                    if (firstSlot != null && secondSlot != null)
                    {
                        Swap(i_PlayerIndex, firstSlot, secondSlot, SwapType.Character);

                        firstSlot.Deselect();
                        secondSlot.Deselect();
                    }
                }

                cache.Clear();

                CloseMenu(i_PlayerIndex);

                m_SkipFrame[i_PlayerIndex] = true;
            };

            SwapButtonClicked onSwapColorEvent = () =>
            {
                SelectionCache cache = m_SelectionsCache[i_PlayerIndex];

                if (cache.Count != 2)
                    return;

                GameObject first = cache[0];
                GameObject second = cache[1];

                if (first != null && second != null)
                {
                    tnUICharacterSlot firstSlot = first.GetComponent<tnUICharacterSlot>();
                    tnUICharacterSlot secondSlot = second.GetComponent<tnUICharacterSlot>();

                    if (firstSlot != null && secondSlot != null)
                    {
                        Swap(i_PlayerIndex, firstSlot, secondSlot, SwapType.Color);

                        firstSlot.Deselect();
                        secondSlot.Deselect();
                    }
                }

                cache.Clear();

                CloseMenu(i_PlayerIndex);

                m_SkipFrame[i_PlayerIndex] = true;
            };

            swapMenu.onSwapCharacterEvent += onSwapCharacterEvent;
            swapMenu.onSwapColorEvent += onSwapColorEvent;
        }

        swapMenu.SetFocus();

        m_IsSwapping[i_PlayerIndex] = true;
    }

    private void UpdateMenu(int i_PlayerIndex)
    {
        tnUISwapMenu swapMenu = m_SwapMenus[i_PlayerIndex];
        if (swapMenu != null)
        {
            PlayerInput playerInput = m_Players[i_PlayerIndex];
            if (playerInput != null)
            {
                swapMenu.Frame(playerInput);
            }
            else
            {
                WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[i_PlayerIndex];
                if (wifiPlayerInput != null)
                {
                    swapMenu.Frame(wifiPlayerInput);
                }
            }
        }
    }

    private void CloseMenu(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= m_SwapMenus.Length)
            return;

        tnUISwapMenu swapMenu = m_SwapMenus[i_PlayerIndex];

        if (swapMenu == null)
            return;

        swapMenu.ClearFocus();

        // Clear callbacks.

        {
            SwapButtonClicked eventHandler = swapMenu.onSwapCharacterEvent;
            if (eventHandler != null)
            {
                foreach (Delegate del in eventHandler.GetInvocationList())
                {
                    eventHandler -= (SwapButtonClicked)del;
                }
            }
        }

        {
            SwapButtonClicked eventHandler = swapMenu.onSwapColorEvent;
            if (eventHandler != null)
            {
                foreach (Delegate del in eventHandler.GetInvocationList())
                {
                    eventHandler -= (SwapButtonClicked)del;
                }
            }
        }

        swapMenu.transform.position = Vector2.zero;

        swapMenu.gameObject.SetActive(false);

        m_IsSwapping[i_PlayerIndex] = false;

        GameObject currentSelection = m_CurrentSlots[i_PlayerIndex];
        UIEventSystem.SetFocusMain(currentSelection);
    }

    */

    ////////////////////////////////////////

    private void SelectPlayer(int i_TeamIndex)
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        tnTeamDescription teamDescription = teamsModule.GetTeamDescription(i_TeamIndex);
        if (teamDescription != null)
        {
            tnCharacterDescription captain = teamDescription.captain;

            if (captain != null)
            {
                int playerId = captain.playerId;
                m_Ids[i_TeamIndex] = playerId;

                // Cache PlayerInput.

                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);
                if (playerData != null)
                {
                    if (!StringUtils.IsNullOrEmpty(playerData.playerInputName))
                    {
                        m_Players[i_TeamIndex] = InputSystem.GetPlayerByNameMain(playerData.playerInputName);
                    }
                    else
                    {
                        if (!StringUtils.IsNullOrEmpty(playerData.wifiPlayerInputName))
                        {
                            m_WiFiPlayers[i_TeamIndex] = WiFiInputSystem.GetPlayerByNameMain(playerData.wifiPlayerInputName);
                        }
                        else
                        {
                            m_Confirmations[i_TeamIndex] = true;
                        }
                    }
                }
            }
        }
    }

    private void SetupTeams()
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            tnTeamDescription teamDescription = teamsModule.GetTeamDescription(playerIndex);
            SetupTeam(playerIndex, teamDescription);
        }
    }

    private void SetupTeam(int i_PlayerIndex, tnTeamDescription i_TeamDescription)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        if (i_TeamDescription == null)
            return;

        int teamId = i_TeamDescription.teamId;
        tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);

        if (teamData == null)
            return;

        List<int> lineUp = teamData.GetDefaultLineUp(i_TeamDescription.charactersCount);

        if (lineUp == null)
            return;

        SetTeamInfo(i_PlayerIndex, teamData);

        // Lineup.

        {
            tnUITeamAnchors teamAnchors = m_TeamAnchorsSets[i_PlayerIndex];

            tnUIAnchorsSet anchorsSet = null;

            if (teamAnchors != null)
            {
                anchorsSet = teamAnchors.GetAnchorsSetBySize(i_TeamDescription.charactersCount);
            }

            if (anchorsSet != null)
            {
                for (int index = 0; index < anchorsSet.anchorsCount && index < lineUp.Count; ++index)
                {
                    RectTransform anchor = anchorsSet.GetAnchorByIndex(index);
                    if (anchor != null)
                    {
                        int characterId = lineUp[index];

                        if (!teamData.Contains(characterId))
                            continue;

                        tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);
                        tnCharacterDescription characterDescription = i_TeamDescription.GetCharacterDescription(index);

                        if (characterData == null || characterDescription == null)
                            continue;

                        FacingDir facingDir = m_Facing[i_PlayerIndex];

                        Color teamColor = i_TeamDescription.teamColor;
                        Sprite flag = teamData.baseSprite;

                        tnUICharacter character = SpawnCharacter(characterData, facingDir, teamColor, flag);

                        tnUICharacterSlot slot = SpawnCharacterSlot();
                        slot.transform.position = anchor.position;
                        slot.character = character;
                        slot.characterId = characterId;

                        Color playerColor;
                        if (GetPlayerColorById(characterDescription.playerId, out playerColor))
                        {
                            slot.SetPlayerColor(playerColor);
                        }
                        else
                        {
                            slot.ClearPlayerColor();
                        }

                        PlayerIndexMap playerIndexMap = m_PlayerIndexMaps[i_PlayerIndex];
                        playerIndexMap.Add(index, index);

                        SlotList slotList = m_LineUp[i_PlayerIndex];
                        slotList.Add(slot);
                    }
                }
            }

            m_TeamAnchors[i_PlayerIndex] = anchorsSet;
        }

        // Bench.

        {
            tnUIBench bench = m_TeamAnchorsBench[i_PlayerIndex];

            int lastBenchIndexUsed = -1;
            for (int index = 0; index < bench.entriesCount && index < teamData.charactersCount; ++index)
            {
                tnUIBenchEntry benchEntry = bench.GetEntryByIndex(index);
                if (benchEntry != null && benchEntry.anchor != null)
                {
                    int characterId = Hash.s_NULL;

                    for (int characterIndex = lastBenchIndexUsed + 1; characterIndex < teamData.charactersCount; ++characterIndex)
                    {
                        int id = teamData.GetCharacterKey(characterIndex);
                        if (!lineUp.Contains(id))
                        {
                            characterId = id;
                            lastBenchIndexUsed = characterIndex;
                            break;
                        }
                    }

                    tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);

                    if (characterData == null)
                        continue;

                    FacingDir facingDir = m_Facing[i_PlayerIndex];

                    Color teamColor = i_TeamDescription.teamColor;
                    Sprite flag = teamData.baseSprite;

                    tnUICharacter character = SpawnCharacter(characterData, facingDir, teamColor, flag);

                    tnUICharacterSlot slot = SpawnCharacterSlot();
                    slot.transform.position = benchEntry.anchor.position;
                    slot.character = character;
                    slot.characterId = characterId;
                    slot.ClearPlayerColor();

                    SlotList slotList = m_Bench[i_PlayerIndex];
                    slotList.Add(slot);
                }
            }
        }
    }

    private void SetTeamInfo(int i_PlayerIndex, tnTeamData i_TeamData)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        if (i_TeamData == null)
            return;

        tnUITeamInfo teamInfo = m_TeamInfo[i_PlayerIndex];

        if (teamInfo != null)
        {
            teamInfo.SetFlag(i_TeamData.flag);
            teamInfo.SetName(i_TeamData.name);
        }
    }

    private void ConfigureTeam(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        SlotList slotList = m_LineUp[i_PlayerIndex];

        tnTeamDescription teamDescription = teamsModule.GetTeamDescription(i_PlayerIndex);
        if (teamDescription != null)
        {
            for (int characterIndex = 0; characterIndex < teamDescription.charactersCount; ++characterIndex)
            {
                tnCharacterDescription characterDescription = teamDescription.GetCharacterDescription(characterIndex);
                if (characterDescription != null)
                {
                    PlayerIndexMap playerIndexMap = m_PlayerIndexMaps[i_PlayerIndex];
                    int spawnOrder = -1;

                    for (int index = 0; index < playerIndexMap.Count; ++index)
                    {
                        int startSpawnOrder;
                        if (playerIndexMap.TryGetValue(index, out startSpawnOrder))
                        {
                            if (startSpawnOrder == characterIndex)
                            {
                                spawnOrder = index;
                            }
                        }
                    }

                    characterDescription.SetSpawnOrder(spawnOrder);

                    if (spawnOrder >= 0 && spawnOrder < slotList.Count)
                    {
                        tnUICharacterSlot slot = slotList[spawnOrder];
                        if (slot != null)
                        {
                            int characterId = slot.characterId;
                            characterDescription.SetCharacterId(characterId);
                        }
                    }
                }
            }
        }
    }

    private void Swap(int i_PlayerIndex, tnUICharacterSlot i_A, tnUICharacterSlot i_B, SwapType i_SwapType)
    {
        if (i_A == null || i_B == null || i_A == i_B)
            return;

        if (i_SwapType == SwapType.Character)
        {
            SwapCharacter(i_A, i_B);
        }
        else
        {
            if (i_SwapType == SwapType.Color)
            {
                Color firstColor = i_A.playerColor;
                Color secondColor = i_B.playerColor;

                bool isFirstHuman = i_A.hasPlayerColor;
                bool isSecondHuman = i_B.hasPlayerColor;

                if (isSecondHuman)
                {
                    i_A.SetPlayerColor(secondColor);
                }
                else
                {
                    i_A.ClearPlayerColor();
                }

                if (isFirstHuman)
                {
                    i_B.SetPlayerColor(firstColor);
                }
                else
                {
                    i_B.ClearPlayerColor();
                }

                SlotList lineUp = m_LineUp[i_PlayerIndex];
                PlayerIndexMap playerIndexMap = m_PlayerIndexMaps[i_PlayerIndex];

                for (int i = 0; i < lineUp.Count; ++i)
                {
                    tnUICharacterSlot first = lineUp[i];
                    for (int j = 0; j < lineUp.Count; ++j)
                    {
                        tnUICharacterSlot second = lineUp[j];
                        if (first == i_A && second == i_B)
                        {
                            int tempIndex = playerIndexMap[i];
                            playerIndexMap[i] = playerIndexMap[j];
                            playerIndexMap[j] = tempIndex;
                        }
                    }
                }
            }
        }
    }

    private void SwapCharacter(tnUICharacterSlot i_A, tnUICharacterSlot i_B)
    {
        if (i_A == null || i_B == null)
            return;

        int tempCharacterId = i_A.characterId;
        i_A.characterId = i_B.characterId;
        i_B.characterId = tempCharacterId;

        tnUICharacter tempCharacter = i_A.character;
        i_A.character = i_B.character;
        i_B.character = tempCharacter;
    }

    private void ClearLineUp(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        SlotList slotList = m_LineUp[i_PlayerIndex];

        for (int index = 0; index < slotList.Count; ++index)
        {
            tnUICharacterSlot slot = slotList[index];
            if (slot != null)
            {
                tnUICharacter character = slot.character;
                RecycleCharacter(character);

                slot.Clear();
            }

            RecycleCharacterSlot(slot);
        }

        slotList.Clear();
    }

    private void ClearBench(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        SlotList slotList = m_Bench[i_PlayerIndex];

        for (int index = 0; index < slotList.Count; ++index)
        {
            tnUICharacterSlot slot = slotList[index];
            if (slot != null)
            {
                tnUICharacter character = slot.character;
                RecycleCharacter(character);

                slot.Clear();
            }

            RecycleCharacterSlot(slot);
        }

        slotList.Clear();
    }

    private void SelectFirst(int i_PlayerIndex)
    {
        Select(i_PlayerIndex, GetFirstSlot(i_PlayerIndex));
    }

    private void Select(int i_PlayerIndex, int i_SlotIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        SlotList lineUp = m_LineUp[i_PlayerIndex];
        SlotList bench = m_Bench[i_PlayerIndex];

        if (i_SlotIndex < 0 || i_SlotIndex >= lineUp.Count + bench.Count)
        {
            return;
        }
        else
        {
            if (i_SlotIndex < lineUp.Count)
            {
                tnUICharacterSlot slot = lineUp[i_SlotIndex];
                if (slot != null)
                {
                    Select(i_PlayerIndex, slot.gameObject);
                }
            }
            else
            {
                i_SlotIndex = i_SlotIndex % lineUp.Count;
                tnUICharacterSlot slot = bench[i_SlotIndex];
                if (slot != null)
                {
                    Select(i_PlayerIndex, slot.gameObject);
                }
            }
        }
    }

    private void Select(int i_PlayerIndex, GameObject i_Slot)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
            return;

        if (i_Slot == null)
            return;

        // Release previous selection.

        {
            GameObject currentSlot = m_CurrentSlots[i_PlayerIndex];

            if (currentSlot != null)
            {
                tnUICharacterSlot characterSlot = currentSlot.GetComponent<tnUICharacterSlot>();
                if (characterSlot != null)
                {
                    characterSlot.SetAvailable();

                    // Play SFX.

                    if (m_OnNavigate != null)
                    {
                        m_OnNavigate.Invoke();
                    }
                }
            }
        }

        m_CurrentSlots[i_PlayerIndex] = i_Slot;

        // Update new selection.

        {
            tnUICharacterSlot characterSlot = i_Slot.GetComponent<tnUICharacterSlot>();
            if (characterSlot != null)
            {
                Color color;
                GetPlayerColorByIndex(i_PlayerIndex, out color);
                characterSlot.SetHighlighted(color);
            }
        }
    }

    private GameObject GetFirstSlot(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
        {
            return null;
        }

        SlotList slotList = m_LineUp[i_PlayerIndex];

        if (slotList.Count > 0)
        {
            tnUICharacterSlot characterSlot = slotList[0];

            if (characterSlot != null)
            {
                return characterSlot.gameObject;
            }
        }

        return null;
    }

    // UTILITIES

    private void InitializePool()
    {
        int poolSize = 2 * s_TeamSize;

        if (m_CharacterPrefab != null && m_CharactersRoot != null)
        {
            for (int index = 0; index < poolSize; ++index)
            {
                tnUICharacter characterInstance = GameObject.Instantiate<tnUICharacter>(m_CharacterPrefab);
                characterInstance.transform.SetParent(m_CharactersRoot, false);
                characterInstance.gameObject.SetActive(false);

                m_CharactersPool.Add(characterInstance);
            }
        }

        if (m_CharacterSlotPrefab != null && m_SlotsRoot != null)
        {
            for (int index = 0; index < poolSize; ++index)
            {
                tnUICharacterSlot characterSlot = GameObject.Instantiate<tnUICharacterSlot>(m_CharacterSlotPrefab);
                characterSlot.transform.SetParent(m_SlotsRoot, false);
                characterSlot.gameObject.SetActive(false);

                m_CharactersSlotsPool.Add(characterSlot);
            }
        }
    }

    private tnUICharacter SpawnCharacter(tnCharacterData i_CharacterData, FacingDir i_Facing, Color i_TeamColor, Sprite i_Flag)
    {
        tnUICharacter character = null;
        if (m_CharactersPool.Count > 0)
        {
            character = m_CharactersPool[m_CharactersPool.Count - 1];
            m_CharactersPool.RemoveAt(m_CharactersPool.Count - 1);
            character.gameObject.SetActive(true);
        }
        else
        {
            if (m_CharacterPrefab == null)
            {
                return null;
            }

            character = GameObject.Instantiate<tnUICharacter>(m_CharacterPrefab);
            character.transform.SetParent(transform, false);
        }

        character.SetBaseColor(i_TeamColor);
        character.SetFlagSprite(i_Flag);

        if (i_Facing == FacingDir.FacingRight)
        {
            character.SetCharacterSprite(i_CharacterData.uiIconFacingRight);
        }
        else // Facing Left
        {
            character.SetCharacterSprite(i_CharacterData.uiIconFacingLeft);
        }

        character.SetName(i_CharacterData.displayName);

        character.SetAvailable();
        character.Deselect();

        return character;
    }

    private void RecycleCharacter(tnUICharacter i_Character)
    {
        if (i_Character == null)
            return;

        i_Character.Clear();

        i_Character.gameObject.SetActive(false);
        m_CharactersPool.Add(i_Character);
    }

    private tnUICharacterSlot SpawnCharacterSlot()
    {
        if (m_CharactersSlotsPool.Count > 0)
        {
            tnUICharacterSlot slot = m_CharactersSlotsPool[m_CharactersSlotsPool.Count - 1];
            m_CharactersSlotsPool.RemoveAt(m_CharactersSlotsPool.Count - 1);
            slot.gameObject.SetActive(true);
            return slot;
        }
        else
        {
            if (m_CharacterSlotPrefab == null)
            {
                return null;
            }

            tnUICharacterSlot slot = GameObject.Instantiate<tnUICharacterSlot>(m_CharacterSlotPrefab);
            slot.transform.SetParent(transform, false);
            return slot;
        }
    }

    private void RecycleCharacterSlot(tnUICharacterSlot i_Slot)
    {
        if (i_Slot == null)
            return;

        i_Slot.Clear();

        i_Slot.gameObject.SetActive(false);
        m_CharactersSlotsPool.Add(i_Slot);
    }

    private GameObject GetNearest(GameObject i_Source, MoveDir i_Direction, int i_PlayerIndex)
    {
        if (i_Source == null)
        {
            return null;
        }

        GameObject nextSelection = null;

        switch (i_Direction)
        {
            case MoveDir.Left:
                nextSelection = GetNearestOnLeft(i_Source, i_PlayerIndex);
                break;
            case MoveDir.Right:
                nextSelection = GetNearestOnRight(i_Source, i_PlayerIndex);
                break;
            case MoveDir.Up:
                nextSelection = GetNearestOnUp(i_Source, i_PlayerIndex);
                break;
            case MoveDir.Down:
                nextSelection = GetNearestOnDown(i_Source, i_PlayerIndex);
                break;
        }

        return nextSelection;
    }

    private GameObject GetNearestOnLeft(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnLeft();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnRight(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnRight();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnUp(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnUp();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnDown(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnDown();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private bool IsAvailable(GameObject i_Slot, int i_PlayerIndex)
    {
        if (i_Slot == null)
        {
            return false;
        }

        tnUICharacterSlot slot = i_Slot.GetComponent<tnUICharacterSlot>();
        if (slot != null)
        {
            SlotList lineUp = m_LineUp[i_PlayerIndex];
            SlotList bench = m_Bench[i_PlayerIndex];

            if (lineUp.Contains(slot) || bench.Contains(slot))
            {
                if (slot.character != null)
                {
                    return true;
                }
            }
        }

        return false;
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

    private bool GetPlayerColorByIndex(int i_PlayerIndex, out Color o_Color)
    {
        o_Color = Color.white;

        if (i_PlayerIndex < 0 || i_PlayerIndex >= s_MaxPlayers)
        {
            return false;
        }

        int playerId = m_Ids[i_PlayerIndex];
        return GetPlayerColorById(playerId, out o_Color);
    }

    private bool GetPlayerColorById(int i_PlayerId, out Color o_Color)
    {
        o_Color = Color.white;

        if (i_PlayerId != Hash.s_NULL)
        {
            tnPlayerData playerData = tnGameData.GetPlayerDataMain(i_PlayerId);
            if (playerData != null)
            {
                o_Color = playerData.color;
                return true;
            }
        }

        return false;
    }
}
