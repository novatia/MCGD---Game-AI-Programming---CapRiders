using UnityEngine;

using System;
using System.Collections.Generic;

using WiFiInput.Server;

using GoUI;

using PlayerInputEvents;

public class tnPanel_LocalParty : UIPanel<tnView_LocalParty>
{
    [SerializeField]
    [DisallowEditInPlayMode]
    [Greater(0)]
    private int m_SlotCount = 4;

    private int[] m_JoinedPlayerIds = null;
    private bool[] m_ChangeFlags = null;

    private bool m_ReceiveInput = false;

    public event Action proceedEvent = null;
    public event Action backEvent = null;

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        m_JoinedPlayerIds = new int[m_SlotCount];
        m_ChangeFlags = new bool[m_SlotCount];
    }

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (viewInstance != null)
        {
            viewInstance.proceedEvent += OnViewProceedEvent;
            viewInstance.backEvent += OnViewBackEvent;

            // Triggers

            viewInstance.SetProceedTriggerCanSend(false);
            viewInstance.SetCancelTriggerActive(false);
            viewInstance.SetCancelTriggerCanSend(false);
            viewInstance.SetBackTriggerActive(true);
            viewInstance.SetBackTriggerCanSend(true);
        }

        // Register on input system events.

        InputSystem.onControllerConnectedEventMain += OnControllerConnectedEvent;
        InputSystem.onControllerDisconnectedEventMain += OnControllerDisconnectedEvent;

        // Clear

        ClearAll();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);

        if (viewInstance == null || !viewInstance.isOpen)
            return;

        // Clear change flags.

        for (int index = 0; index < m_ChangeFlags.Length; ++index)
        {
            m_ChangeFlags[index] = false;
        }

        int freeSlot = GetFreeSlotCount();

        // Listen for new join.

        if (m_ReceiveInput)
        {
            if (freeSlot > 0)
            {
                List<int> playersIds = tnGameData.GetPlayersKeysMain();

                if (playersIds != null)
                {
                    for (int index = 0; index < playersIds.Count; ++index)
                    {
                        int playerId = playersIds[index];

                        if (IsAlreadyJoined(playerId))
                            continue;

                        bool startPressed = tnInputUtils.GetButtonDown(playerId, "Start");

                        if (startPressed)
                        {
                            int targetIndex = GetFirstFreeSlotIndex();
                            if (targetIndex >= 0)
                            {
                                m_JoinedPlayerIds[targetIndex] = playerId;
                                m_ChangeFlags[targetIndex] = true;

                                --freeSlot;
                            }
                        }
                    }
                }
            }

            // Check if someone leave the group.

            for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
            {
                int playerId = m_JoinedPlayerIds[index];

                if (Hash.IsNullOrEmpty(playerId))
                    continue;

                bool cancelPressed = tnInputUtils.GetButtonDown(playerId, "Cancel");

                if (cancelPressed)
                {
                    m_JoinedPlayerIds[index] = Hash.s_NULL;
                    m_ChangeFlags[index] = true;

                    ++freeSlot;
                }
            }
        }

        // Cache if something has changed.

        bool somethingChanged = false;
        for (int index = 0; index < m_ChangeFlags.Length; ++index)
        {
            somethingChanged |= m_ChangeFlags[index];
        }

        // Set players on input module.

        if (somethingChanged)
        {
            if (IsAllFree())
            {
                SetAllPlayersOnInputModule();
            }
            else
            {
                InputModule inputModule = UIEventSystem.inputModuleMain;
                if (inputModule != null)
                {
                    inputModule.Clear();

                    for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
                    {
                        int playerId = m_JoinedPlayerIds[index];

                        if (Hash.IsNullOrEmpty(playerId))
                            continue;

                        PlayerInput playerInput;
                        WiFiPlayerInput wifiPlayerInput;
                        tnInputUtils.GetPlayersInputs(playerId, out playerInput, out wifiPlayerInput);

                        if (playerInput != null)
                        {
                            inputModule.AddPlayer(playerInput);
                        }
                        else
                        {
                            if (wifiPlayerInput != null)
                            {
                                inputModule.AddWifiPlayer(wifiPlayerInput);
                            }
                        }
                    }
                }
            }
        }

        // Update view.

        if (somethingChanged)
        {
            // Update slots.

            int joinedIndex = 0;
            string photonPlayerName = PhotonNetwork.playerName;

            for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
            {
                bool changed = m_ChangeFlags[index];
                int playerId = m_JoinedPlayerIds[index];

                bool isFree = Hash.IsNullOrEmpty(playerId);

                if (!isFree)
                {
                    SetPlayerName(index, photonPlayerName, joinedIndex);
                    ++joinedIndex;
                }

                if (!changed)
                    continue;

                if (isFree)
                {
                    // Free slot.

                    FreePlayerSlot(index);
                }
                else
                {
                    // Set slot.

                    SetPlayerSlot(index, playerId);
                }
            }

            // Update triggers.

            if (IsAllFree())
            {
                viewInstance.SetProceedTriggerCanSend(false);

                viewInstance.SetCancelTriggerActive(false);
                viewInstance.SetCancelTriggerCanSend(false);

                viewInstance.SetBackTriggerActive(true);
                viewInstance.SetBackTriggerCanSend(true);
            }
            else
            {
                viewInstance.SetProceedTriggerCanSend(true);

                viewInstance.SetCancelTriggerActive(true);
                viewInstance.SetCancelTriggerCanSend(true);

                viewInstance.SetBackTriggerActive(false);
                viewInstance.SetBackTriggerCanSend(false);
            }
        }
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (viewInstance != null)
        {
            viewInstance.proceedEvent -= OnViewProceedEvent;
            viewInstance.backEvent -= OnViewBackEvent;
        }

        // Unregister on input system events.

        InputSystem.onControllerConnectedEventMain -= OnControllerConnectedEvent;
        InputSystem.onControllerDisconnectedEventMain -= OnControllerDisconnectedEvent;
    }

    // LOGIC

    public int joinedPlayerCount
    {
        get
        {
            return (m_SlotCount - GetFreeSlotCount());
        }
    }

    public int slotCount
    {
        get
        {
            return m_SlotCount;
        }
    }

    public int GetJoinedPlayerId(int i_Index)
    {
        int joinedIndex = 0;
        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];

            if (Hash.IsNullOrEmpty(playerId))
                continue;

            if (i_Index == joinedIndex)
            {
                return playerId;
            }
            else
            {
                ++joinedIndex;
            }
        }

        return Hash.s_NULL;
    }

    public int GetPlayerId(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_JoinedPlayerIds.Length)
        {
            return Hash.s_NULL;
        }

        return m_JoinedPlayerIds[i_Index];
    }

    public void SetReceiveInput(bool i_Receive)
    {
        m_ReceiveInput = i_Receive;
    }

    // INTERNALS

    private void SetPlayerSlot(int i_Index, int i_PlayerId)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetPlayerSlot(i_Index, i_PlayerId);
    }

    private void SetPlayerName(int i_Index, string i_PlayerName, int i_GuestIndex)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetPlayerName(i_Index, i_PlayerName, i_GuestIndex);
    }

    private void FreePlayerSlot(int i_Index)
    {
        if (viewInstance == null)
            return;

        viewInstance.FreePlayerSlot(i_Index);
    }

    private void ClearAll()
    {
        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            m_JoinedPlayerIds[index] = Hash.s_NULL;
        }

        if (viewInstance == null)
            return;

        viewInstance.ClearAll();
    }

    // Triggers

    private void SetProceedTriggerActive(bool i_Active)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetProceedTriggerActive(i_Active);
    }

    private void SetProceedTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetProceedTriggerCanSend(i_CanSend);
    }

    private void SetBackTriggerActive(bool i_Active)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetBackTriggerActive(i_Active);
    }

    private void SetBackTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetBackTriggerCanSend(i_CanSend);
    }

    private void SetCancelTriggerActive(bool i_Active)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetCancelTriggerActive(i_Active);
    }

    private void SetCancelTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetCancelTriggerCanSend(i_CanSend);
    }

    // UTILS

    private int GetFreeSlotCount()
    {
        int count = 0;

        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];
            count += (playerId == Hash.s_NULL) ? 1 : 0;
        }

        return count;
    }

    private int GetFirstFreeSlotIndex()
    {
        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];

            if (Hash.IsNullOrEmpty(playerId))
            {
                return index;
            }
        }

        return -1;
    }

    private bool IsAllFree()
    {
        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];

            if (Hash.IsNullOrEmpty(playerId))
                continue;

            return false;
        }

        return true;
    }

    private bool IsAlreadyJoined(int i_PlayerId)
    {
        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];

            if (Hash.IsNullOrEmpty(playerId))
                continue;

            if (playerId == i_PlayerId)
            {
                return true;
            }
        }

        return false;
    }

    private int GetFirstOccupiedSlotIndex()
    {
        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];

            if (Hash.IsNullOrEmpty(playerId))
                continue;

            return index;
        }

        return -1;
    }

    private void SetAllPlayersOnInputModule()
    {
        InputModule inputModule = UIEventSystem.inputModuleMain;

        if (inputModule == null)
            return;

        inputModule.Clear();

        List<int> playersIds = tnGameData.GetPlayersKeysMain();

        if (playersIds != null)
        {
            for (int index = 0; index < playersIds.Count; ++index)
            {
                int playerId = playersIds[index];

                if (Hash.IsNullOrEmpty(playerId))
                    continue;

                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

                if (playerData == null)
                    continue;

                string playerInputName = playerData.playerInputName;
                string wifiPlayerInputName = playerData.wifiPlayerInputName;

                PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerInputName);
                WiFiPlayerInput wifiPlayerInput = WiFiInputSystem.GetPlayerByNameMain(wifiPlayerInputName);

                if (playerInput != null)
                {
                    inputModule.AddPlayer(playerInput);
                }
                else
                {
                    if (wifiPlayerInput != null)
                    {
                        inputModule.AddWifiPlayer(wifiPlayerInput);
                    }
                }
            }
        }
    }

    // EVENTS

    private void OnViewProceedEvent()
    {
        if (proceedEvent != null)
        {
            proceedEvent();
        }
    }

    private void OnViewBackEvent()
    {
        if (backEvent != null)
        {
            backEvent();
        }
    }

    private void OnControllerConnectedEvent(ControllerEventParams i_Params)
    {
        OnControllerChangedEvent();
    }

    private void OnControllerDisconnectedEvent(ControllerEventParams i_Params)
    {
        OnControllerChangedEvent();
    }
    
    private void OnControllerChangedEvent()
    {
        bool somethingChanged = false;

        bool[] changes = new bool[m_JoinedPlayerIds.Length];
        for (int index = 0; index < changes.Length; ++index)
        {
            changes[index] = false;
        }

        // Check for consistency.

        bool keyboardAssigned = false;

        for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
        {
            int playerId = m_JoinedPlayerIds[index];

            if (Hash.IsNullOrEmpty(playerId))
                continue;

            PlayerInput playerInput;
            WiFiPlayerInput wifiPlayerInput;
            tnInputUtils.GetPlayersInputs(playerId, out playerInput, out wifiPlayerInput);

            if (playerInput != null)
            {
                if (keyboardAssigned)
                {
                    if (playerInput.JoystickCount == 0)
                    {
                        m_JoinedPlayerIds[index] = Hash.s_NULL;
                        changes[index] = true;
                        somethingChanged = true;
                    }
                }
                else
                {
                    if (playerInput.JoystickCount == 0)
                    {
                        keyboardAssigned = true;
                    }
                }
            }
        }

        // Update view.

        if (viewInstance != null)
        {
            if (somethingChanged)
            {
                string photonPlayerName = PhotonNetwork.playerName;
                int joinedIndex = 0;

                for (int index = 0; index < m_JoinedPlayerIds.Length; ++index)
                {
                    int playerId = m_JoinedPlayerIds[index];
                    bool changed = changes[index];

                    if (Hash.IsNullOrEmpty(playerId))
                    {
                        if (changed)
                        {
                            viewInstance.FreePlayerSlot(index);
                        }
                    }
                    else
                    {
                        viewInstance.SetPlayerName(index, photonPlayerName, joinedIndex);
                        ++joinedIndex;
                    }
                }
            }
        }
    }
}