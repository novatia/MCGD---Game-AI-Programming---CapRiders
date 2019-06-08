using UnityEngine;

using System;

using GoUI;

public class tnView_CreateRoom : GoUI.UIView
{
    [SerializeField]
    private UISelector m_GameModeSelector = null;
    [SerializeField]
    private UISelector m_MatchDurationSelector = null;
    [SerializeField]
    private UISelector m_GoldenGoalSelector = null;
    [SerializeField]
    private UISelector m_RefereeSelector = null;

    [SerializeField]
    private UISelector m_BallSelector = null;
    [SerializeField]
    private UISelector m_StadiumSelector = null;

    [SerializeField]
    private UISelector m_MaxPlayersSelector = null;

    [SerializeField]
    private UIEventTrigger m_ProceedTrigger = null;
    [SerializeField]
    private UIEventTrigger m_BackTrigger = null;

    private event Action m_ProceedEvent = null;
    private event Action m_BackEvent = null;

    public event Action<SelectorItem> gameModeSelectedChangedEvent = null;
    public event Action<SelectorItem> stadiumSelectedChangedEvent = null;

    // ACCESSORS

    public int selectedGameModeId
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_GameModeSelector);
            if (item != null)
            {
                return item.id;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedMatchDurationOptionId
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_MatchDurationSelector);
            if (item != null)
            {
                return item.id;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedGoldenGoalOptionId
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_GoldenGoalSelector);
            if (item != null)
            {
                return item.id;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedRefereeOptionId
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_RefereeSelector);
            if (item != null)
            {
                return item.id;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedBallId
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_BallSelector);
            if (item != null)
            {
                return item.id;
            }

            return Hash.s_NULL;
        }
    }

    public int selectedStadiumId
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_StadiumSelector);
            if (item != null)
            {
                return item.id;
            }

            return Hash.s_NULL;
        }
    }

    public bool isSelectedStadiumLocked
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_StadiumSelector);
            if (item != null)
            {
                return item.locked;
            }

            return false;
        }
    }

    public int selectedMaxPlayers
    {
        get
        {
            SelectorItem item = GetSelectedItem(m_MaxPlayersSelector);
            if (item != null)
            {
                return item.id; // Item id is the amount of players allowed.
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

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (m_GameModeSelector != null)
        {
            m_GameModeSelector.onChangeSelection.AddListener(OnGameModeSelectedChanged);
        }

        if (m_StadiumSelector != null)
        {
            m_StadiumSelector.onChangeSelection.AddListener(OnStadiumSelectedChanged);
        }

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

        if (m_GameModeSelector != null)
        {
            m_GameModeSelector.onChangeSelection.RemoveListener(OnGameModeSelectedChanged);
        }

        if (m_StadiumSelector != null)
        {
            m_StadiumSelector.onChangeSelection.RemoveListener(OnStadiumSelectedChanged);
        }

        if (m_ProceedTrigger != null)
        {
            m_ProceedTrigger.onEvent.RemoveListener(OnProceedTriggerEvent);
        }

        if (m_BackTrigger != null)
        {
            m_BackTrigger.onEvent.RemoveListener(OnBackTriggerEvent);
        }
    }

    // LOGIC

    public void ClearAll()
    {
        ClearGameModeSelector();
        ClearMatchDurationSelector();
        ClearGoldenGoalSelector();
        ClearRefereeSelector();

        ClearBallSelector();
        ClearStadiumSelector();

        ClearMaxPlayersSelector();
    }

    public void SetProceedTriggerCanSend(bool i_CanSend)
    {
        if (m_ProceedTrigger != null)
        {
            m_ProceedTrigger.canSend = i_CanSend;
        }
    }

    // Game Mode

    public void SetGameModeSelectorData(SelectorData i_SelectorData)
    {
        if (m_GameModeSelector != null)
        {
            m_GameModeSelector.SetData(i_SelectorData);
        }
    }

    public void SelectGameModeIndex(int i_Index)
    {
        if (m_GameModeSelector == null)
            return;

        m_GameModeSelector.SelectItemByIndex(i_Index);
    }

    public void SelectGameMode(int i_Id)
    {
        if (m_GameModeSelector == null)
            return;

        m_GameModeSelector.SelectItem(i_Id);
    }

    public void ClearGameModeSelector()
    {
        if (m_GameModeSelector != null)
        {
            m_GameModeSelector.Clear();
        }
    }

    // Duration

    public void SetMatchDurationSelectorData(SelectorData i_SelectorData)
    {
        if (m_MatchDurationSelector != null)
        {
            m_MatchDurationSelector.SetData(i_SelectorData);
        }
    }

    public void SelectMatchDuration(int i_Id)
    {
        if (m_MatchDurationSelector == null)
            return;

        m_MatchDurationSelector.SelectItem(i_Id);
    }

    public void ClearMatchDurationSelector()
    {
        if (m_MatchDurationSelector != null)
        {
            m_MatchDurationSelector.Clear();
        }
    }

    // Golden goal

    public void SetGoldenGoalSelectorData(SelectorData i_SelectorData)
    {
        if (m_GoldenGoalSelector != null)
        {
            m_GoldenGoalSelector.SetData(i_SelectorData);
        }
    }

    public void SelectGoldenGoal(int i_Id)
    {
        if (m_GoldenGoalSelector == null)
            return;

        m_GoldenGoalSelector.SelectItem(i_Id);
    }

    public void ClearGoldenGoalSelector()
    {
        if (m_GoldenGoalSelector != null)
        {
            m_GoldenGoalSelector.Clear();
        }
    }

    // Referee

    public void SetRefereeSelectorData(SelectorData i_SelectorData)
    {
        if (m_RefereeSelector != null)
        {
            m_RefereeSelector.SetData(i_SelectorData);
        }
    }

    public void SelectReferee(int i_Id)
    {
        if (m_RefereeSelector == null)
            return;

        m_RefereeSelector.SelectItem(i_Id);
    }

    public void ClearRefereeSelector()
    {
        if (m_RefereeSelector != null)
        {
            m_RefereeSelector.Clear();
        }
    }

    // Ball

    public void SetBallSelectorData(SelectorData i_SelectorData)
    {
        if (m_BallSelector != null)
        {
            m_BallSelector.SetData(i_SelectorData);
        }
    }

    public void SelectBallIndex(int i_Index)
    {
        if (m_BallSelector == null)
            return;

        m_BallSelector.SelectItemByIndex(i_Index);
    }

    public void ClearBallSelector()
    {
        if (m_BallSelector != null)
        {
            m_BallSelector.Clear();
        }
    }

    // Stadium

    public void SetStadiumSelectorData(SelectorData i_SelectorData)
    {
        if (m_StadiumSelector != null)
        {
            m_StadiumSelector.SetData(i_SelectorData);
        }
    }

    public void SelectStadiumIndex(int i_Index)
    {
        if (m_StadiumSelector == null)
            return;

        m_StadiumSelector.SelectItemByIndex(i_Index);
    }

    public void SelectFirstUnlockedStadium()
    {
        if (m_StadiumSelector == null)
            return;

        m_StadiumSelector.SelectFirstUnlockedItem();
    }

    public void ClearStadiumSelector()
    {
        if (m_StadiumSelector != null)
        {
            m_StadiumSelector.Clear();
        }
    }

    // Max players

    public void SetMaxPlayersSelectorData(SelectorData i_SelectorData)
    {
        if (m_MaxPlayersSelector != null)
        {
            m_MaxPlayersSelector.SetData(i_SelectorData);
        }
    }

    public void RefreshMaxPlayers()
    {
        if (m_MaxPlayersSelector == null)
            return;

        if (m_MaxPlayersSelector.itemsCount > 0)
        {
            m_MaxPlayersSelector.SelectItemByIndex(m_MaxPlayersSelector.itemsCount - 1);
        }
        else
        {
            m_MaxPlayersSelector.Clear();
        }
    }

    public void ClearMaxPlayersSelector()
    {
        if (m_MaxPlayersSelector != null)
        {
            m_MaxPlayersSelector.Clear();
        }
    }

    // INTERNALS

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

    private void OnGameModeSelectedChanged(SelectorItem i_SelectorItem)
    {
        if (gameModeSelectedChangedEvent != null)
        {
            gameModeSelectedChangedEvent(i_SelectorItem);
        }
    }

    private void OnStadiumSelectedChanged(SelectorItem i_SelectorItem)
    {
        if (stadiumSelectedChangedEvent != null)
        {
            stadiumSelectedChangedEvent(i_SelectorItem);
        }
    }

    private void OnProceedTriggerEvent()
    {
        Proceed();
    }

    private void OnBackTriggerEvent()
    {
        Back();
    }

    // UTILS

    private SelectorItem GetSelectedItem(UISelector i_Selector)
    {
        if (i_Selector == null)
        {
            return null;
        }

        SelectorItem item = i_Selector.currentItem;
        return item;
    }
}