using UnityEngine;
using UnityEngine.Events;

using System;

using GoUI;

public class tnView_Lobby : GoUI.UIView
{
    [Serializable]
    public class SelectionChanged : UnityEvent { }

    // Serializeable fields

    [Header("Slots")]

    [SerializeField]
    private tnUIRoomEntry m_RoomEntryPrefab = null;
    [SerializeField]
    private RectTransform m_Content = null;
    [SerializeField]
    private int m_SlotCount = 6;

    [SerializeField]
    private RectTransform m_ScrollbarHandle = null;
    [SerializeField]
    private Vector2 m_ScrollbarHandleSizeDelta = Vector2.zero;

    [Header("Triggers")]

    [SerializeField]
    private UIEventTrigger m_ConfirmTrigger = null;
    [SerializeField]
    private UIEventTrigger m_BackTrigger = null;

    [Header("Navbar")]

    [SerializeField]
    private CanvasGroup m_RefreshCommand = null;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_RefreshCommandDisabledAlpha = 0.5f;

    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_SelectionChangedSfx = null;

    // Fields

    private tnUIRoomEntry[] m_RoomEntries = null;
    private tnUIRoomEntry m_SelectedEntry = null;

    private event Action m_BackEvent = null;
    private event Action m_ConfirmEvent = null;

    // ACCESSORS

    private float roomHeight
    {
        get
        {
            if (m_RoomEntryPrefab != null)
            {
                RectTransform rectTransform = m_RoomEntryPrefab.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    return rectTransform.rect.height;
                }
            }

            return 0f;
        }
    }

    public event Action confirmEvent
    {
        add
        {
            m_ConfirmEvent += value;
        }
        remove
        {
            m_ConfirmEvent -= value;
        }
    }

    public event Action backEvent
    {
        add
        {
            m_BackEvent += value;
        }
        remove
        {
            m_BackEvent -= value;
        }
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Create slots.

        int slotCount = Mathf.Max(1, m_SlotCount);
        m_RoomEntries = new tnUIRoomEntry[slotCount];

        if (m_Content != null && m_RoomEntryPrefab != null && slotCount > 0)
        {
            float slotHeight = roomHeight;
            float contentHeight = m_Content.rect.height;
            float spacing = (contentHeight - slotHeight * slotCount) / (slotCount - 1);

            for (int slotIndex = 0; slotIndex < slotCount; ++slotIndex)
            {
                tnUIRoomEntry room = Instantiate<tnUIRoomEntry>(m_RoomEntryPrefab);
                room.gameObject.name = "Room_" + slotIndex;
                room.transform.SetParent(m_Content, false);

                RectTransform deviceRectTransform = room.GetComponent<RectTransform>();
                if (deviceRectTransform != null)
                {
                    deviceRectTransform.pivot = UIPivot.s_TopCenter;
                    deviceRectTransform.SetAnchor(UIAnchor.s_TopCenter);

                    float y = slotIndex * (slotHeight + spacing);

                    deviceRectTransform.anchoredPosition = new Vector2(0f, -y);
                }

                m_RoomEntries[slotIndex] = room;
            }
        }
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        Internal_RegisterEvents();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        Internal_UnregisterEvents();
    }

    // LOGIC 

    public void SetRoomData(int i_Index, Sprite i_StadiumThumbnail, string i_StadiumName, string i_GameMode, string i_Rules, string i_HostName, int i_PlayersCount, int i_TotalPlayers, int i_Ping)
    {
        Internal_SetRoomData(i_Index, i_StadiumThumbnail, i_StadiumName, i_GameMode, i_Rules, i_HostName, i_PlayersCount, i_TotalPlayers, i_Ping);
    }

    public void SetRoomData(int i_Index, tnUIRoomEntryData i_Data)
    {
        Internal_SetRoomData(i_Index, i_Data);
    }

    public void Clear()
    {
        for (int index = 0; index < m_RoomEntries.Length; ++index)
        {
            tnUIRoomEntry room = m_RoomEntries[index];

            if (room == null)
                continue;

            room.Clear();
        }

        m_SelectedEntry = null;
    }

    public void ForceSelection(int i_Index)
    {
        Internal_SelectByIndex(i_Index);
    }

    public void SelectByIndex(int i_Index)
    {
        Internal_SelectByIndex(i_Index);

        // Play sfx.

        SfxPlayer.PlayMain(m_SelectionChangedSfx);
    }

    public void SetConfirmTriggerCanSend(bool i_CanSend)
    {
        if (m_ConfirmTrigger != null)
        {
            m_ConfirmTrigger.canSend = i_CanSend;
        }
    }

    public void SetScrollbarHandleState(float i_ShowedRoomPercentage, float i_PositionPercentage)
    {
        if (m_ScrollbarHandle == null)
            return;

        float showedRoomPercentage = Mathf.Clamp01(i_ShowedRoomPercentage);
        float positionPercentage = Mathf.Clamp01(i_PositionPercentage);

        float anchorMinX = 0f;
        float anchorMaxX = 1f;

        float anchorMinY = 0f;
        float anchorMaxY = 1f;

        float min = showedRoomPercentage / 2f;
        float max = 1f - (showedRoomPercentage / 2f);

        float middle = Mathf.Lerp(min, max, 1f - positionPercentage);

        anchorMinY = middle - showedRoomPercentage / 2f;
        anchorMaxY = anchorMinY + showedRoomPercentage;

        m_ScrollbarHandle.SetAnchor(anchorMinX, anchorMinY, anchorMaxX, anchorMaxY);
        m_ScrollbarHandle.sizeDelta = m_ScrollbarHandleSizeDelta;
    }

    public void SetRefreshCommandActive(bool i_Active)
    {
        Internal_SetRefreshCommandActive(i_Active);
    }

    // INTERNALS

    private void Internal_SelectByIndex(int i_Index)
    {
        // Deselect.

        if (m_SelectedEntry != null)
        {
            m_SelectedEntry.Deselct();
            m_SelectedEntry = null;
        }

        // Select new.

        tnUIRoomEntry roomEntry = GetRoomEntry(i_Index);
        if (roomEntry != null)
        {
            roomEntry.Select();
            m_SelectedEntry = roomEntry;
        }
    }

    private void Internal_SetRoomData(int i_Index, Sprite i_StadiumThumbnail, string i_StadiumName, string i_GameMode, string i_Rules, string i_HostName, int i_PlayersCount, int i_TotalPlayers, int i_Ping)
    {
        tnUIRoomEntryData data = new tnUIRoomEntryData(i_StadiumThumbnail, i_StadiumName, i_GameMode, i_Rules, i_HostName, i_PlayersCount, i_TotalPlayers, i_Ping);
        Internal_SetRoomData(i_Index, data);
    }

    private void Internal_SetRoomData(int i_Index, tnUIRoomEntryData i_Data)
    {
        tnUIRoomEntry roomEntry = GetRoomEntry(i_Index);

        if (roomEntry == null)
            return;

        roomEntry.Enable();
        roomEntry.SetData(i_Data);
    }

    private tnUIRoomEntry GetRoomEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_RoomEntries.Length)
        {
            return null;
        }

        return m_RoomEntries[i_Index];
    }

    private void Internal_SetRefreshCommandActive(bool i_Active)
    {
        float alpha = (i_Active) ? 1f : m_RefreshCommandDisabledAlpha;
        if (m_RefreshCommand != null)
        {
            m_RefreshCommand.alpha = alpha;
        }
    }

    // UTILS

    private void Internal_RegisterEvents()
    {
        if (m_ConfirmTrigger != null)
        {
            m_ConfirmTrigger.onEvent.AddListener(OnConfirmTriggerEvent);
        }

        if (m_BackTrigger != null)
        {
            m_BackTrigger.onEvent.AddListener(OnBackTriggerEvent);
        }
    }

    private void Internal_UnregisterEvents()
    {
        if (m_ConfirmTrigger != null)
        {
            m_ConfirmTrigger.onEvent.RemoveListener(OnConfirmTriggerEvent);
        }

        if (m_BackTrigger != null)
        {
            m_BackTrigger.onEvent.RemoveListener(OnBackTriggerEvent);
        }
    }

    private void Back()
    {
        if (m_BackEvent != null)
        {
            m_BackEvent();
        }
    }

    private void Confirm()
    {
        if (m_ConfirmEvent != null)
        {
            m_ConfirmEvent();
        }
    }

    // EVENTS

    private void OnConfirmTriggerEvent()
    {
        Confirm();
    }

    private void OnBackTriggerEvent()
    {
        Back();
    }
}
