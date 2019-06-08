using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using GoUI;

using WiFiInput.Server;

using TuesdayNights;

public delegate void JoinRoomRequestedEvent(string i_Room);

public class tnPanel_Lobby : UIPanel<tnView_Lobby>
{
    // Serializeable fields

    [Header("Slots")]

    [SerializeField]
    private int m_SlotsCount = 6;

    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_RefreshSfx = null;
    [SerializeField]
    private SfxDescriptor m_CreateRoomSfx = null;

    // Fields

    private List<RoomInfo> m_Rooms = new List<RoomInfo>();

    private string m_CurrentSelectedRoomName = "";
    private int m_CurrentIndexRoom = -1;

    private int m_MinShowedIndex = -1;

    private event JoinRoomRequestedEvent m_JoinRoomRequestedEvent = null;
    private event Action m_CreateRoomRequestedEvent = null;
    private event Action m_BackEvent = null;

    private int minShowedIndex
    {
        get
        {
            return m_MinShowedIndex;
        }
        set
        {
            m_MinShowedIndex = value;
        }
    }

    private int maxShowedIndex
    {
        get
        {
            int a = m_MinShowedIndex + m_SlotsCount - 1;
            int b = (m_Rooms.Count > 0) ? m_Rooms.Count - 1 : -1;

            return Math.Min(a, b);
        }
    }

    // ACCESSORS

    public event JoinRoomRequestedEvent joinRoomRequestedEvent
    {
        add
        {
            m_JoinRoomRequestedEvent += value;
        }
        remove
        {
            m_JoinRoomRequestedEvent -= value;
        }
    }

    public event Action createRoomRequestedEvent
    {
        add
        {
            m_CreateRoomRequestedEvent += value;
        }

        remove
        {
            m_CreateRoomRequestedEvent -= value;
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

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        Internal_RegisterEvents();

        Refresh();
        ForceSelection(0);
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();
        if (localPartyModule != null)
        {
            int playerId = localPartyModule.captainId;

            bool up = tnInputUtils.GetPositiveButtonDown(playerId, "VerticalUp", "Vertical");
            bool down = tnInputUtils.GetNegativeButtonDown(playerId, "VerticalDown", "Vertical");

            bool refresh = tnInputUtils.GetButtonDown(playerId, "Action1");
            bool createRoom = tnInputUtils.GetButtonDown(playerId, "Action2", "Action2");

            if (m_Rooms.Count > 0)
            {
                if (refresh)
                {
                    Refresh();
                    ForceSelection(0);

                    SfxPlayer.PlayMain(m_RefreshSfx);
                }
                else
                {
                    if ((up || down) && !(up && down))
                    {
                        if (up)
                        {
                            MoveUp();
                        }
                        else
                        {
                            MoveDown();
                        }
                    }
                }
            }

            if (createRoom)
            {
                SfxPlayer.PlayMain(m_CreateRoomSfx);

                if (m_CreateRoomRequestedEvent != null)
                {
                    m_CreateRoomRequestedEvent();
                }
            }
        }
    }

    protected override void OnExit()
    {
        base.OnExit();

        Internal_UnregisterEvents();
    }

    // INTERNALS

    private void Refresh()
    {
        m_Rooms.Clear();

        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        if (rooms != null)
        {
            for (int index = 0; index < rooms.Length; ++index)
            {
                RoomInfo room = rooms[index];

                if (room == null)
                    continue;

                m_Rooms.Add(room);
            }
        }

        minShowedIndex = (m_Rooms.Count > 0) ? 0 : -1;

        UpdateView();
    }

    private void SetRoomData(int i_Index, Sprite i_StadiumThumbnail, string i_StadiumName, string i_GameMode, string i_Rules, string i_HostName, int i_PlayersCount, int i_TotalPlayers, int i_Ping)
    {
        if (viewInstance != null)
        {
            viewInstance.SetRoomData(i_Index, i_StadiumThumbnail, i_StadiumName, i_GameMode, i_Rules, i_HostName, i_PlayersCount, i_TotalPlayers, i_Ping);
        }
    }

    private void ClearView()
    {
        if(viewInstance != null)
        {
            viewInstance.Clear();
        }
    }

    private void MoveUp()
    {
        if ((m_CurrentIndexRoom - 1) >= 0  && m_Rooms.Count > 0)
        {
            int newIndex = m_CurrentIndexRoom - 1;

            if (newIndex < minShowedIndex)
            {
                minShowedIndex = newIndex;
                UpdateView();
            }

            SelectByIndex(newIndex);
        }
    }

    private void MoveDown()
    {
        if ((m_CurrentIndexRoom + 1) < m_Rooms.Count)
        {
            int newIndex = m_CurrentIndexRoom + 1;

            if (newIndex > maxShowedIndex)
            {
                ++minShowedIndex;
                UpdateView();
            }

            SelectByIndex(newIndex);
        }
    }

    private void ForceSelection(int i_Index)
    {
        if (i_Index >= 0 && i_Index < m_Rooms.Count)
        {
            RoomInfo roomInfo = m_Rooms[i_Index];

            if (roomInfo != null)
            {
                m_CurrentIndexRoom = i_Index;
                m_CurrentSelectedRoomName = roomInfo.Name;

                if (viewInstance != null)
                {
                    int viewIndex = i_Index - minShowedIndex;
                    viewInstance.ForceSelection(viewIndex);
                    return;
                }
            }
        }

        m_CurrentSelectedRoomName = "";
        m_CurrentIndexRoom = -1;
    }

    private void SelectByIndex(int i_Index)
    {
        if (i_Index >= 0 && i_Index < m_Rooms.Count)
        {
            RoomInfo roomInfo = m_Rooms[i_Index];

            if (roomInfo != null)
            {
                m_CurrentIndexRoom = i_Index;
                m_CurrentSelectedRoomName = roomInfo.Name;

                if (viewInstance != null)
                {
                    int viewIndex = i_Index - minShowedIndex;
                    viewInstance.SelectByIndex(viewIndex);
                    return;
                }
            }
        }

        m_CurrentSelectedRoomName = "";
        m_CurrentIndexRoom = -1;
    }

    private void SelectByName(string i_RoomName)
    {
        for (int index = 0; index < m_Rooms.Count; ++index)
        {
            RoomInfo room = m_Rooms[index];

            if (room == null)
                continue;

            if (i_RoomName == room.Name)
            {
                SelectByIndex(index);
                return;
            }
        }

        m_CurrentSelectedRoomName = "";
        m_CurrentIndexRoom = -1;
    }

    private void UpdateView()
    {
        if (viewInstance == null)
            return;

        ClearView();

        int showedRooms = 0;
        for (int index = minShowedIndex; index >= 0 && index <= maxShowedIndex; ++index)
        {
            RoomInfo roomInfo = m_Rooms[index];

            if (roomInfo == null)
                continue;

            int stadiumId;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_Stadium, out stadiumId);

            int gameModeId;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_GameMode, out gameModeId);

            int matchDurationOptionId;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_MatchDuration, out matchDurationOptionId);

            int goldenGoalOptionId;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_GoldenGoal, out goldenGoalOptionId);

            int refereeOptionId;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_Referee, out refereeOptionId);

            string host;
            PhotonUtils.TryGetRoomCustomProperty<string>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_HostName, out host);

            int playersCount;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_PlayerCount, out playersCount);

            int ping;
            PhotonUtils.TryGetRoomCustomProperty<int>(roomInfo, PhotonPropertyKey.s_RoomCustomPropertyKey_AvgPing, out ping);

            int maxPlayer = roomInfo.MaxPlayers;

            Sprite stadiumThumbnail = null;
            string stadiumName = "";
            string gameModeName = "";
            string rules = "";

            tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);
            if (stadiumData != null)
            {
                stadiumThumbnail = stadiumData.icon;
                stadiumName = stadiumData.name;
            }

            tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);
            if (gameModeData != null)
            {
                gameModeName = gameModeData.name;
            }

            float matchDuration;
            tnGameData.TryGetMatchDurationValueMain(matchDurationOptionId, out matchDuration);
            string matchDurationRule = TimeUtils.TimeToString(matchDuration, true, true);

            string goldenGoal;
            tnGameData.TryGetGoldenGoalValueMain(goldenGoalOptionId, out goldenGoal);
            string goldenGoalRule = (goldenGoal == "ON") ? "GOLDEN GOAL" : "NO GOLDEN GOAL";

            string referee;
            tnGameData.TryGetRefereeValueMain(refereeOptionId, out referee);
            string refereeRule = (referee == "ON") ? "REFEREE" : "";

            rules = goldenGoalRule + ", " + refereeRule + ((refereeRule == "") ? "" : ", ") + matchDurationRule;

            SetRoomData(showedRooms, stadiumThumbnail, stadiumName, gameModeName, rules, host, playersCount, maxPlayer, ping);
            ++showedRooms;
        }

        int roomsCount = m_Rooms.Count;
        float showedRoomsPercentage = (roomsCount > 0) ? (((float) showedRooms) / ((float)roomsCount)) : 1f;

        int slotCount = Mathf.Max(0, m_SlotsCount);
        int possibleStates = (roomsCount > slotCount) ? (roomsCount - slotCount) : 1;
        int currentStates = Mathf.Max(0, minShowedIndex);
        float positionPercentage = ((float)currentStates) / ((float)possibleStates);

        viewInstance.SetScrollbarHandleState(showedRoomsPercentage, positionPercentage);
        viewInstance.SetConfirmTriggerCanSend(roomsCount > 0);
        viewInstance.SetRefreshCommandActive(roomsCount > 0);
    }

    // UTILS

    private void Internal_RegisterEvents()
    {
        if (viewInstance != null)
        {
            viewInstance.confirmEvent += OnViewConfirmEvent;
            viewInstance.backEvent += OnViewBackEvent;
        }

        PhotonCallbacks.onReceivedRoomListUpdateMain += OnReceivedRoomListUpdateEvent;
    }

    private void Internal_UnregisterEvents()
    {
        if (viewInstance != null)
        {
            viewInstance.confirmEvent -= OnViewConfirmEvent;
            viewInstance.backEvent -= OnViewBackEvent;
        }

        PhotonCallbacks.onReceivedRoomListUpdateMain -= OnReceivedRoomListUpdateEvent;
    }

    // EVENTS

    private void OnViewConfirmEvent()
    {
        JoinRoomRequested();
    }

    private void OnViewBackEvent()
    {
        Back();
    }

    private void Back()
    {
        if (m_BackEvent != null)
        {
            m_BackEvent();
        }
    }

    private void JoinRoomRequested()
    {
        if (m_CurrentIndexRoom < 0 || m_CurrentSelectedRoomName == "")
            return;

        if (m_JoinRoomRequestedEvent != null)
        {
            m_JoinRoomRequestedEvent(m_CurrentSelectedRoomName);
        }
    }

    private void OnReceivedRoomListUpdateEvent()
    {
        if (m_Rooms.Count == 0)
        {
            Refresh();
            ForceSelection(0);
        }
    }
}