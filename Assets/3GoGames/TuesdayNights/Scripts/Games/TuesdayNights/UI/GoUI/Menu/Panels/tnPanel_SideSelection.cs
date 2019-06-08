using UnityEngine;

using System;

using GoUI;

public class tnPanel_SideSelection : UIPanel<tnView_SideSelection>
{
    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_PlayerReadySfx = null;
    [SerializeField]
    private SfxDescriptor m_PlayerNotReadySfx = null;

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        Internal_RegisterEvent();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        Internal_UnregisterEvent();
    }

    // LOGIC

    public bool IsDeviceInTransition(int i_Index)
    {
        if (viewInstance == null)
        {
            return false;
        }

        return viewInstance.IsDeviceInTransition(i_Index);
    }

    public void SetDeviceSide(int i_Index, tnUI_MP_DeviceSide i_Side, bool i_Immediatly = false)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeviceSide(i_Index, i_Side, i_Immediatly);
        }
    }

    public void SetDeviceColor(int i_Index, Color i_Color)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeviceColor(i_Index, i_Color);
        }
    }

    public void SetDeviceName(int i_Index, string i_PlayerName)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeviceName(i_Index, i_PlayerName);
        }
    }

    public void SetDeviceState(int i_Index, tnUI_MP_DeviceState i_State)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeviceState(i_Index, i_State);
        }
    }

    public void SetDeviceReady(int i_Index, bool i_Ready)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeviceReady(i_Index, i_Ready);
        }
    }

    public void NotifyPlayerReady(bool i_Ready)
    {
        SfxDescriptor sfx = (i_Ready) ? m_PlayerReadySfx : m_PlayerNotReadySfx;
        SfxPlayer.PlayMain(sfx);
    }

    public void SetDeviceCaptain(int i_Index, bool i_Captain)
    {
        if (viewInstance != null)
        {
            viewInstance.SetDeviceCaptain(i_Index, i_Captain);
        }
    }

    public void ClearDevice(int i_Index)
    {
        if (viewInstance != null)
        {
            viewInstance.ClearDevice(i_Index);
        }
    }

    public void ClearDevices()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearDevices();
        }
    }

    // Triggers

    public void SetStartTriggerActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetStartTriggerActive(i_Active);
        }
    }

    public void SetStartTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance != null)
        {
            viewInstance.SetStartTriggerCanSend(i_CanSend);
        }
    }

    public void SetConfirmTriggerActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetConfirmTriggerActive(i_Active);
        }
    }

    public void SetConfirmTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance != null)
        {
            viewInstance.SetConfirmTriggerCanSend(i_CanSend);
        }
    }

    public void SetCancelTriggerActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetCancelTriggerActive(i_Active);
        }
    }

    public void SetCancelTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance != null)
        {
            viewInstance.SetCancelTriggerCanSend(i_CanSend);
        }
    }

    public void SetBackTriggerActive(bool i_Active)
    {
        if (viewInstance != null)
        {
            viewInstance.SetBackTriggerActive(i_Active);
        }
    }

    public void SetBackTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance != null)
        {
            viewInstance.SetBackTriggerCanSend(i_CanSend);
        }
    }

    // INTERNAL

    private void Internal_RegisterEvent()
    {

    }

    private void Internal_UnregisterEvent()
    {

    }
}