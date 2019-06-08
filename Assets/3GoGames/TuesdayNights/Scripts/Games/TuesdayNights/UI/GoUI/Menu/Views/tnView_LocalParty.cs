using UnityEngine;
using UnityEngine.Events;

using System;
using System.Collections.Generic;

using GoUI;

using WiFiInput.Server;

public class tnView_LocalParty : GoUI.UIView
{
    [Header("Slots")]

    [SerializeField]
    private tnUILocalPlayerSlot m_SlotControllerEntry = null;
    [SerializeField]
    private RectTransform m_PanelSlots = null;

    [Header("Triggers")]

    [SerializeField]
    private UIEventTrigger m_ProceedTrigger = null;
    [SerializeField]
    private UIEventTrigger m_BackTrigger = null;
    [SerializeField]
    private UIEventTrigger m_CancelTrigger = null;

    [Header("Layout")]

    [SerializeField]
    [DisallowEditInPlayMode]
    [Greater(0)]
    private int m_Columns = 2;
    [SerializeField]
    [DisallowEditInPlayMode]
    [Greater(0)]
    private int m_SlotCount = 4;
    [SerializeField]
    private float m_CellSpacing = 25f;

    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_PlayerJoinedSfx = null;
    [SerializeField]
    private SfxDescriptor m_PlayerLeftSfx = null;

    private List<tnUILocalPlayerSlot> m_PlayerSlots = new List<tnUILocalPlayerSlot>();

    public event Action proceedEvent = null;
    public event Action backEvent = null;

    protected override void Awake()
    {
        base.Awake();

        SpawnSlots();
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (m_ProceedTrigger != null)
        {
            m_ProceedTrigger.onEvent.AddListener(OnProceedTriggerEvent);
        }

        if (m_BackTrigger != null)
        {
            m_BackTrigger.onEvent.AddListener(OnBackTriggerEvent);
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (m_ProceedTrigger != null)
        {
            m_ProceedTrigger.onEvent.RemoveListener(OnProceedTriggerEvent);
        }

        if (m_BackTrigger != null)
        {
            m_BackTrigger.onEvent.RemoveListener(OnBackTriggerEvent);
        }
    }

    // BUSINESS LOGIC

    public void SetPlayerSlot(int i_Index, int i_PlayerId)
    {
        tnUILocalPlayerSlot playerSlot = GetLocalPlayerSlot(i_Index);

        if (playerSlot == null)
            return;

        playerSlot.Bind(i_PlayerId);

        // Play sfx.

        SfxPlayer.PlayMain(m_PlayerJoinedSfx);
    }

    public void SetPlayerName(int i_Index, string i_PlayerName, int i_GuestIndex)
    {
        tnUILocalPlayerSlot playerSlot = GetLocalPlayerSlot(i_Index);

        if (playerSlot == null)
            return;

        playerSlot.SetPlayerName(i_PlayerName, i_GuestIndex);
    }

    public void FreePlayerSlot(int i_Index)
    {
        tnUILocalPlayerSlot playerSlot = GetLocalPlayerSlot(i_Index);

        if (playerSlot == null)
            return;

        playerSlot.Clear();

        // Play sfx.

        SfxPlayer.PlayMain(m_PlayerLeftSfx);
    }

    public void ClearAll()
    {
        for (int index = 0; index < m_PlayerSlots.Count; ++index)
        {
            tnUILocalPlayerSlot playerSlot = m_PlayerSlots[index];
            if (playerSlot != null)
            {
                playerSlot.Clear();
            }
        }
    }

    // Triggers.

    public void SetProceedTriggerActive(bool i_Active)
    {
        if (m_ProceedTrigger != null)
        {
            m_ProceedTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetProceedTriggerCanSend(bool i_CanSend)
    {
        if (m_ProceedTrigger != null)
        {
            m_ProceedTrigger.canSend = i_CanSend;
        }
    }

    public void SetBackTriggerActive(bool i_Active)
    {
        if (m_BackTrigger != null)
        {
            m_BackTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetCancelTriggerActive(bool i_Active)
    {
        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetBackTriggerCanSend(bool i_CanSend)
    {
        if (m_BackTrigger != null)
        {
            m_BackTrigger.canSend = i_CanSend;
        }
    }

    public void SetCancelTriggerCanSend(bool i_CanSend)
    {
        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.canSend = i_CanSend;
        }
    }

    // INTERNALS

    private void SpawnSlots()
    {
        if (m_PanelSlots == null || m_SlotControllerEntry == null)
            return;

        // Create new slots

        int rows = m_SlotCount / m_Columns + ((m_SlotCount % m_Columns != 0) ? 1 : 0);
        int columns = Mathf.Max(1, m_Columns);

        float slotWidth = 1f / columns;
        float slotHeight = 1f / rows;

        float contentWidth = m_PanelSlots.rect.width;
        float contentHeight = m_PanelSlots.rect.height;

        float spacingWidth = (contentWidth > 0f) ? (m_CellSpacing / contentWidth) : 0f;
        float spacingHeight = (contentHeight > 0f) ? (m_CellSpacing / contentHeight) : 0f;

        for (int rowIndex = 0; rowIndex < rows; ++rowIndex)
        {
            for (int columnIndex = 0; columnIndex < columns; ++columnIndex)
            {
                if (m_PlayerSlots.Count >= m_SlotCount)
                    continue;

                tnUILocalPlayerSlot slotEntry = tnUILocalPlayerSlot.Instantiate<tnUILocalPlayerSlot>(m_SlotControllerEntry);
                slotEntry.name = m_PlayerSlots.Count.ToString();
                slotEntry.Clear();
                slotEntry.transform.SetParent(m_PanelSlots, false);

                RectTransform slotRectTransform = slotEntry.GetComponent<RectTransform>();
                if (slotRectTransform != null)
                {
                    float anchorMinX = (columnIndex * slotWidth);
                    float anchorMaxX = anchorMinX + slotWidth;

                    float anchorMaxY = (1f - rowIndex * slotHeight);
                    float anchorMinY = (anchorMaxY - slotHeight);

                    anchorMinX += spacingWidth / 2f;
                    anchorMaxX -= spacingWidth / 2f;

                    anchorMinY += spacingHeight / 2f;
                    anchorMaxY -= spacingHeight / 2f;

                    if (rowIndex == 0)
                    {
                        anchorMaxY -= spacingHeight / 2f;
                    }

                    if (rowIndex == rows - 1)
                    {
                        anchorMinY += spacingHeight / 2f;
                    }

                    if (columnIndex == 0)
                    {
                        anchorMinX += spacingWidth / 2f;
                    }

                    if (columnIndex == columns - 1)
                    {
                        anchorMaxX -= spacingWidth / 2f;
                    }

                    slotRectTransform.SetAnchor(anchorMinX, anchorMinY, anchorMaxX, anchorMaxY);
                    slotRectTransform.sizeDelta = Vector2.zero;

                    m_PlayerSlots.Add(slotEntry);
                }
            }
        }
    }

    private tnUILocalPlayerSlot GetLocalPlayerSlot(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_PlayerSlots.Count)
        {
            return null;
        }

        return m_PlayerSlots[i_Index];
    }

    // EVENTS

    private void OnProceedTriggerEvent()
    {
        if (proceedEvent != null)
        {
            proceedEvent();
        }
    }

    private void OnBackTriggerEvent()
    {
        if (backEvent != null)
        {
            backEvent();
        }
    }
}
