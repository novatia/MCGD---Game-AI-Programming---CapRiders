using UnityEngine;

using System;
using System.Collections.Generic;

using GoUI;

using TuesdayNights;

public class tnPanel_CreateRoom : UIPanel<tnView_CreateRoom>
{
    // Fields

    private event Action m_ProceedEvent = null;
    private event Action m_BackEvent = null;

    // ACCESSORS

    public int selectedGameModeId
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedGameModeId;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedMatchDurationOptionId
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedMatchDurationOptionId;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedGoldenGoalOptionId
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedGoldenGoalOptionId;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedRefereeOptionId
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedRefereeOptionId;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedBallId
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedBallId;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedStadiumId
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedStadiumId;
            }

            return Hash.s_NULL;
        }
    }

    public bool isSelectedStadiumLocked
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.isSelectedStadiumLocked;
            }

            return false;
        }
    }

    public int selectedMaxPlayers
    {
        get
        {
            if (viewInstance != null)
            {
                return viewInstance.selectedMaxPlayers;
            }

            return 0;
        }
    }

    public event Action proceedEvent
    {
        add
        {
            m_ProceedEvent += value;
        }
        remove
        {
            m_ProceedEvent -= value;
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

        if (viewInstance != null)
        {
            viewInstance.gameModeSelectedChangedEvent += OnSelectedGameModeChanged;
            viewInstance.stadiumSelectedChangedEvent += OnSelectedStadiumChanged;

            viewInstance.proceedEvent += OnViewProceedEvent;
            viewInstance.backEvent += OnViewBackEvent;
        }

        Setup();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (viewInstance != null)
        {
            viewInstance.gameModeSelectedChangedEvent -= OnSelectedGameModeChanged;
            viewInstance.stadiumSelectedChangedEvent -= OnSelectedStadiumChanged;

            viewInstance.proceedEvent -= OnViewProceedEvent;
            viewInstance.backEvent -= OnViewBackEvent;
        }
    }

    // INTERNALS

    private void Setup()
    {
        ClearAll();

        // Setup game mode selector.

        SetupGameModeSelector();

        // Setup match duration selector.

        SetupMatchDurationSelector();

        // Setup golden goal selector.

        SetupGoldenGoalSelector();

        // Setup referee selector.

        SetupRefereeSelector();

        // Setup ball selector.

        SetupBallSelector();

        // Select first game mode.

        SelectGameModeIndex(0);

        // Reset selectors to default value.

        int gameModeId = selectedGameModeId;

        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);
        if (gameModeData != null)
        {
            tnGameModeConfig gameModeConfig = tnGameData.GetConfigDataMain(gameModeData.optionsConfigId);
            if (gameModeConfig != null)
            {
                SelectMatchDuration(gameModeConfig.matchDurationOption);
                SelectReferee(gameModeConfig.refereeOption);
                SelectGoldenGoal(gameModeConfig.goldenGoalOption);
            }
        }

        // Select first ball.

        SelectBallIndex(0);

        // Setup stadium selector.

        SetupStadiumSelector(gameModeId);

        // Setup max players.

        SetupMaxPlayerSelector(selectedStadiumId);
    }

    private void ClearAll()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearAll();
        }
    }

    // Game mode

    private void SetupGameModeSelector()
    {
        SelectorData selectorData = new SelectorData();

        List<int> gameModesKeys = tnGameData.GetGameModesKeysMain();

        if (gameModesKeys != null)
        {
            for (int gameModeIndex = 0; gameModeIndex < gameModesKeys.Count; ++gameModeIndex)
            {
                int gameModeId = gameModesKeys[gameModeIndex];

                if (Hash.IsNullOrEmpty(gameModeId))
                    continue;

                tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);

                if (gameModeData == null)
                    continue;

                if (!gameModeData.hidden)
                {
                    IntRange teamSizeRange = gameModeData.onlinePlayersPerTeamRange;

                    int localPartySize;
                    PhotonUtils.TryGetPlayerCustomProperty<int>(PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, out localPartySize);

                    bool teamSizeInvalid = (localPartySize >= teamSizeRange.max * 2);

                    if (!teamSizeInvalid)
                    {
                        SelectorItem selectorItem = new SelectorItem(gameModeId, gameModeData.name, "", null);
                        selectorData.AddItem(selectorItem);
                    }
                }
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetGameModeSelectorData(selectorData);
        }
    }

    private void SelectGameModeIndex(int i_Index)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectGameModeIndex(i_Index);
        }
    }

    private void SelectGameMode(int i_Id)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectGameMode(i_Id);
        }
    }

    private void ClearGameModeSelector()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearGameModeSelector();
        }
    }

    // Duration

    private void SetupMatchDurationSelector()
    {
        SelectorData selectorData = new SelectorData();

        List<int> matchDurationKeys = tnGameData.GetMatchDurationOptionKeysMain();
        if (matchDurationKeys != null)
        {
            for (int index = 0; index < matchDurationKeys.Count; ++index)
            {
                int key = matchDurationKeys[index];

                float value;
                if (tnGameData.TryGetMatchDurationValueMain(key, out value))
                {
                    string time = TimeUtils.TimeToString(value, true, true);

                    SelectorItem selectorItem = new SelectorItem(key, time, "", null);
                    selectorData.AddItem(selectorItem);
                }
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetMatchDurationSelectorData(selectorData);
        }
    }

    private void SelectMatchDuration(int i_Id)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectMatchDuration(i_Id);
        }
    }

    private void ClearMatchDurationSelector()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearMatchDurationSelector();
        }
    }

    // Golden goal

    private void SetupGoldenGoalSelector()
    {
        SelectorData selectorData = new SelectorData();

        List<int> goldenGoalKeys = tnGameData.GetGoldenGoalOptionKeysMain();
        if (goldenGoalKeys != null)
        {
            for (int index = 0; index < goldenGoalKeys.Count; ++index)
            {
                int key = goldenGoalKeys[index];

                string value;
                if (tnGameData.TryGetGoldenGoalValueMain(key, out value))
                {
                    SelectorItem selectorItem = new SelectorItem(key, value, "", null);
                    selectorData.AddItem(selectorItem);
                }
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetGoldenGoalSelectorData(selectorData);
        }
    }

    private void SelectGoldenGoal(int i_Id)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectGoldenGoal(i_Id);
        }
    }

    private void ClearGoldenGoalSelector()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearGoldenGoalSelector();
        }
    }

    // Referee

    private void SetupRefereeSelector()
    {
        SelectorData selectorData = new SelectorData();

        List<int> refereeOptionKeys = tnGameData.GetRefereeOptionKeysMain();
        if (refereeOptionKeys != null)
        {
            for (int index = 0; index < refereeOptionKeys.Count; ++index)
            {
                int key = refereeOptionKeys[index];

                string value;
                if (tnGameData.TryGetRefereeValueMain(key, out value))
                {
                    SelectorItem selectorItem = new SelectorItem(key, value, "", null);
                    selectorData.AddItem(selectorItem);
                }
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetRefereeSelectorData(selectorData);
        }
    }

    private void SelectReferee(int i_Id)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectReferee(i_Id);
        }
    }

    private void ClearRefereeSelector()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearRefereeSelector();
        }
    }

    // Ball

    private void SetupBallSelector()
    {
        SelectorData selectorData = new SelectorData();

        List<int> ballKeys = tnGameData.GetBallsKeysMain();
        if (ballKeys != null)
        {
            for (int index = 0; index < ballKeys.Count; ++index)
            {
                int key = ballKeys[index];

                if (Hash.IsNullOrEmpty(key))
                    continue;

                tnBallData ballData = tnGameData.GetBallDataMain(key);

                if (ballData == null)
                    continue;

                SelectorItem selectorItem = new SelectorItem(key, ballData.name, "", ballData.icon);
                selectorData.AddItem(selectorItem);
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetBallSelectorData(selectorData);
        }
    }

    private void SelectBallIndex(int i_Index)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectBallIndex(i_Index);
        }
    }

    private void ClearBallSelector()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearBallSelector();
        }
    }

    // Stadium

    private void SetupStadiumSelector(int i_GameModeId)
    {
        SelectorData selectorData = new SelectorData();

        List<int> stadiumKeys = tnGameData.GetStadiumsKeysMain();

        if (stadiumKeys == null)
            return;

        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(i_GameModeId);

        for (int stadiumIndex = 0; stadiumIndex < stadiumKeys.Count; ++stadiumIndex)
        {
            int stadiumId = stadiumKeys[stadiumIndex];
            tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

            if (stadiumData == null)
                continue;

            if (stadiumData.hiddenOnline)
                continue;

            bool excludedByTag = false;

            if (gameModeData != null)
            {
                for (int excluderTagIndex = 0; excluderTagIndex < gameModeData.fieldsExcludersTagsCount; ++excluderTagIndex)
                {
                    int excluderTag = gameModeData.GetFieldExcluderTag(excluderTagIndex);

                    if (Hash.IsNullOrEmpty(excluderTag))
                        continue;

                    excludedByTag |= stadiumData.HasTag(excluderTag);
                }
            }

            IntRange teamSizeRange = stadiumData.onlineTeamSize;

            int localPartySize;
            PhotonUtils.TryGetPlayerCustomProperty<int>(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, out localPartySize);

            bool teamSizeInvalid = (localPartySize >= teamSizeRange.max * 2);

            bool locked = excludedByTag || teamSizeInvalid;

            SelectorItem selectorItem = new SelectorItem(stadiumId, stadiumData.name, stadiumData.description, stadiumData.icon, locked, String.Empty);
            selectorData.AddItem(selectorItem);
        }

        if (viewInstance != null)
        {
            viewInstance.SetStadiumSelectorData(selectorData);
        }

        SelectFirstUnlockedStadium();
    }

    private void SelectStadiumIndex(int i_Index)
    {
        if (viewInstance != null)
        {
            viewInstance.SelectStadiumIndex(i_Index);
        }
    }

    private void SelectFirstUnlockedStadium()
    {
        if (viewInstance != null)
        {
            viewInstance.SelectFirstUnlockedStadium();
        }
    }

    private void ClearStadiumSelector()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearStadiumSelector();
        }
    }

    // Max players

    private void SetupMaxPlayerSelector(int i_StadiumId)
    {
        SelectorData selectorData = new SelectorData();

        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(i_StadiumId);

        if (stadiumData != null)
        {
            int minPlayers = 2 * stadiumData.onlineTeamSize.min;
            int maxPlayers = 2 * stadiumData.onlineTeamSize.max;

            int localPartySize;
            PhotonUtils.TryGetPlayerCustomProperty<int>(PhotonNetwork.player, PhotonPropertyKey.s_PlayerCustomPropertyKey_LocalPartySize, out localPartySize);

            for (int numPlayers = minPlayers; numPlayers <= maxPlayers; numPlayers += 2)
            {
                if (numPlayers <= localPartySize)
                    continue;

                SelectorItem selectorItem = new SelectorItem(numPlayers, numPlayers.ToString(), "", null);
                selectorData.AddItem(selectorItem);
            }
        }

        if (viewInstance != null)
        {
            viewInstance.SetMaxPlayersSelectorData(selectorData);
        }

        RefreshMaxPlayers();
    }

    private void RefreshMaxPlayers()
    {
        if (viewInstance != null)
        {
            viewInstance.RefreshMaxPlayers();
        }
    }

    // UTILS

    private void Proceed()
    {
        if (m_ProceedEvent != null)
        {
            m_ProceedEvent();
        }
    }

    private void Back()
    {
        if (m_BackEvent != null)
        {
            m_BackEvent();
        }
    }

    // EVENTS

    private void OnSelectedGameModeChanged(SelectorItem i_SelectorItem)
    {
        if (i_SelectorItem == null)
            return;

        int gameModeId = i_SelectorItem.id;

        if (Hash.IsNullOrEmpty(gameModeId))
            return;

        //tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);
        //if (gameModeData != null)
        //{
        //    tnGameModeConfig gameModeConfig = tnGameData.GetConfigDataMain(gameModeData.optionsConfigId);
        //    if (gameModeConfig != null)
        //    {
        //        SelectMatchDuration(gameModeConfig.matchDurationOption);
        //        SelectReferee(gameModeConfig.refereeOption);
        //        SelectGoldenGoal(gameModeConfig.goldenGoalOption);
        //    }
        //}

        // Refresh stadium selector.

        SetupStadiumSelector(gameModeId);
    }

    private void OnSelectedStadiumChanged(SelectorItem i_SelectorItem)
    {
        if (i_SelectorItem == null)
            return;

        int stadiumId = i_SelectorItem.id;

        if (Hash.IsNullOrEmpty(stadiumId))
            return;

        // Refrsh max players selector.

        SetupMaxPlayerSelector(stadiumId);

        // Refresh proceed trigger status.

        bool locked = i_SelectorItem.locked;

        if (viewInstance != null)
        {
            viewInstance.SetProceedTriggerCanSend(!locked);
        }
    }

    private void OnViewProceedEvent()
    {
        Proceed();
    }

    private void OnViewBackEvent()
    {
        Back();
    }
}