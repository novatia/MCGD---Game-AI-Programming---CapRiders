using UnityEngine;
using UnityEngine.UI;

using System;

using GoUI;

public class tnView_SideSelection : GoUI.UIView
{
    [SerializeField]
    private int m_SlotCount = 8;
    [SerializeField]
    private int m_Spacing = 20;

    [SerializeField]
    private tnUI_MP_Device m_DevicePrefab = null;
    [SerializeField]
    private RectTransform m_SlotsRoot = null;

    [SerializeField]
    private UIEventTrigger m_StartTrigger = null;
    [SerializeField]
    private UIEventTrigger m_ConfirmTrigger = null;
    [SerializeField]
    private UIEventTrigger m_CancelTrigger = null;
    [SerializeField]
    private UIEventTrigger m_BackTrigger = null;

    private tnUI_MP_Device[] m_Devices = null;

    private float deviceHeight
    {
        get
        {
            if (m_DevicePrefab != null)
            {
                RectTransform rectTransform = m_DevicePrefab.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    return rectTransform.rect.height;
                }
            }

            return 0f;
        }
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        // Create slots.

        int slotCount = Mathf.Max(1, m_SlotCount);
        m_Devices = new tnUI_MP_Device[slotCount];

        if (m_SlotsRoot != null && m_DevicePrefab != null && slotCount > 0)
        {
            //float slotHeight = deviceHeight;
            //float contentHeight = m_SlotsRoot.rect.height;
            //float spacing = (contentHeight - slotHeight * slotCount) / (slotCount - 1);

            for (int slotIndex = 0; slotIndex < slotCount; ++slotIndex)
            {
                tnUI_MP_Device device = Instantiate<tnUI_MP_Device>(m_DevicePrefab);
                device.gameObject.name = "Device_" + slotIndex;
                device.transform.SetParent(m_SlotsRoot, false);

                RectTransform deviceRectTransform = device.GetComponent<RectTransform>();
                if (deviceRectTransform != null)
                {
                    deviceRectTransform.pivot = UIPivot.s_TopCenter;
                    deviceRectTransform.SetAnchor(UIAnchor.s_TopCenter);

                    //float y = slotIndex * (slotHeight + spacing);
                    float y = slotIndex * (deviceHeight + m_Spacing);

                    deviceRectTransform.anchoredPosition = new Vector2(0f, -y);
                }

                m_Devices[slotIndex] = device;
            }
        }
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // LOGIC 

    public bool IsDeviceInTransition(int i_Index)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            return device.isInTransition;
        }

        return false;
    }

    public void SetDeviceSide(int i_Index, tnUI_MP_DeviceSide i_Side, bool i_Immediatly = false)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.SetDeviceSide(i_Side, i_Immediatly);
        }
    }

    public void SetDeviceColor(int i_Index, Color i_Color)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.SetDeviceColor(i_Color);
        }
    }

    public void SetDeviceName(int i_Index, string i_PlayerName)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.SetPlayerName(i_PlayerName);
        }
    }

    public void SetDeviceState(int i_Index, tnUI_MP_DeviceState i_State)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.SetState(i_State);
        }
    }

    public void SetDeviceReady(int i_Index, bool i_Ready)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.SetReady(i_Ready);
        }
    }

    public void SetDeviceCaptain(int i_Index, bool i_Captain)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.SetCaptain(i_Captain);
        }
    }

    public void ClearDevice(int i_Index)
    {
        tnUI_MP_Device device = GetDevice(i_Index);
        if (device != null)
        {
            device.Clear();
        }
    }

    public void ClearDevices()
    {
        if (m_Devices == null)
            return;

        for (int index = 0; index < m_Devices.Length; ++index)
        {
            ClearDevice(index);
        }
    }

    // Trigger

    public void SetStartTriggerActive(bool i_Active)
    {
        if (m_StartTrigger != null)
        {
            m_StartTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetStartTriggerCanSend(bool i_CanSend)
    {
        if (m_StartTrigger != null)
        {
            m_StartTrigger.canSend = i_CanSend;
        }
    }

    public void SetConfirmTriggerActive(bool i_Active)
    {
        if (m_ConfirmTrigger != null)
        {
            m_ConfirmTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetConfirmTriggerCanSend(bool i_CanSend)
    {
        if (m_ConfirmTrigger != null)
        {
            m_ConfirmTrigger.canSend = i_CanSend;
        }
    }

    public void SetCancelTriggerActive(bool i_Active)
    {
        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetCancelTriggerCanSend(bool i_CanSend)
    {
        if (m_CancelTrigger != null)
        {
            m_CancelTrigger.canSend = i_CanSend;
        }
    }

    public void SetBackTriggerActive(bool i_Active)
    {
        if (m_BackTrigger != null)
        {
            m_BackTrigger.gameObject.SetActive(i_Active);
        }
    }

    public void SetBackTriggerCanSend(bool i_CanSend)
    {
        if (m_BackTrigger != null)
        {
            m_BackTrigger.canSend = i_CanSend;
        }
    }

    // INTERNALS

    private tnUI_MP_Device GetDevice(int i_Index)
    {
        if (m_Devices == null)
        {
            return null;
        }

        if (i_Index < 0 || i_Index >= m_Devices.Length)
        {
            return null;
        }

        return m_Devices[i_Index];
    }
}