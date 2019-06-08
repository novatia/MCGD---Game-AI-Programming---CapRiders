using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using GoUI;

public class tnMatchSettingsController : UIViewController
{
    private UISelector m_GameModeSelector = null;

    private UISelector m_MatchDurationSelector = null;
    private UISelector m_RefereeSelector = null;
    private UISelector m_GoldenGoalSelector = null;
    private UISelector m_AILevelSelector = null;

    private UISelector m_BallSelector = null;
    private UISelector m_StadiumSelector = null;

    private UIEventTrigger m_TriggerProceed = null;
    private UIEventTrigger m_TriggerCancel = null;

    private static string s_WidgetId_GameModeSelector = "WIDGET_SELECTOR_GAMEMODE";
    private static string s_WidgetId_DurationSelector = "WIDGET_SELECTOR_DURATION";
    private static string s_WidgetId_RefereeSelector = "WIDGET_SELECTOR_REFEREE";
    private static string s_WidgetId_BallSelector = "WIDGET_SELECTOR_BALL";
    private static string s_WidgetId_StadiumSelector = "WIDGET_SELECTOR_STADIUM";
    private static string s_WidgetId_GoldenGoalSelector = "WIDGET_SELECTOR_GOLDENGOAL";
    private static string s_WidgetId_AILevelSelector = "WIDGET_SELECTOR_AILEVEL";
    private static string s_WidgetId_TriggerProceed = "WIDGET_TRIGGER_PROCEED";
    private static string s_WidgetId_TriggerCancel = "WIDGET_TRIGGER_CANCEL";

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        UIPageDescriptor pageDescriptor = GetComponentInChildren<UIPageDescriptor>();

        if (pageDescriptor == null)
            return;

        m_TriggerProceed = pageDescriptor.GetWidget<UIEventTrigger>(s_WidgetId_TriggerProceed);
        m_TriggerCancel = pageDescriptor.GetWidget<UIEventTrigger>(s_WidgetId_TriggerCancel);

        m_GameModeSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_GameModeSelector);

        m_MatchDurationSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_DurationSelector);
        m_RefereeSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_RefereeSelector);
        m_GoldenGoalSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_GoldenGoalSelector);
        m_AILevelSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_AILevelSelector);

        m_BallSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_BallSelector);
        m_StadiumSelector = pageDescriptor.GetWidget<UISelector>(s_WidgetId_StadiumSelector);

        InitMatchDurationSelector();
        InitRefereeSelector();
        InitGoldenGoalSelector();

        InitBallSelector();
    }

    void OnEnable()
    {
        // Refresh Game mode selector.

        SetupGameModeSelector();

        // Reset selectors to default values (specified into each tnGameModeConfig).

        SelectGameModeIndex(0);

        if (m_GameModeSelector != null)
        {
            SelectorItem currentGameMode = m_GameModeSelector.currentItem;
            if (currentGameMode != null)
            {
                int gameModeId = currentGameMode.id;
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

                SetupStadiumSelector(gameModeId);
            }
        }

        SelectBallIndex(0);
        SelectFirstUnlockedStadium();

        SetupAILevelSelector();
        SelectAILevel(tnGameData.aiLevelCountMain / 2);

        int aiCount = GetAICount();
        if (m_AILevelSelector != null)
        {
            m_AILevelSelector.interactable = (aiCount > 0);
        }

        // Adjust navigation.

        AdjustAISelectorNavigation(aiCount);

        // Register on events.

        if (m_GameModeSelector != null)
        {
            m_GameModeSelector.onChangeSelection.AddListener(OnGameModeSelectedChanged);
        }
    }

    void OnDisable()
    {
        if (m_GameModeSelector != null)
        {
            m_GameModeSelector.onChangeSelection.RemoveListener(OnGameModeSelectedChanged);
        }
    }

    void Update()
    {
        UpdateProceedTrigger();
        UpdateCancelTrigger();
    }

    // UIViewController's interface

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    // BUSINESS LOGIC

    public void UpdateModule()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
        if (matchSettingsModule == null)
            return;

        matchSettingsModule.Clear();

        SetGameModeId(matchSettingsModule);     // Set Game Mode.
        SetMatchDuration(matchSettingsModule);  // Set Match Duration.
        SetReferee(matchSettingsModule);        // Set Referee On/Off.
        SetGoldenGoal(matchSettingsModule);     // Set Golden Goal.
        SetAILevelIndex(matchSettingsModule);   // Set AI Level Index.
        SetBall(matchSettingsModule);           // Set Ball type.
        SetStadium(matchSettingsModule);        // Set Stadium.
    }

    // INTERNALS

    private void UpdateProceedTrigger()
    {
        if (m_TriggerProceed == null || m_StadiumSelector == null)
            return;

        m_TriggerProceed.canSend = true; // Default

        SelectorItem currentStadim = m_StadiumSelector.currentItem;
        if (currentStadim != null)
        {
            m_TriggerProceed.canSend = !currentStadim.locked;
        }
    }

    private void UpdateCancelTrigger()
    {
        if (m_TriggerCancel == null)
            return;

        m_TriggerCancel.canSend = true;
    }

    private void InitMatchDurationSelector()
    {
        if (m_MatchDurationSelector == null)
            return;

        SelectorData selectorData = new SelectorData();

        List<int> matchDurationKeys = tnGameData.GetMatchDurationOptionKeysMain();

        if (matchDurationKeys != null)
        {
            foreach (int key in matchDurationKeys)
            {
                float value;
                if (tnGameData.TryGetMatchDurationValueMain(key, out value))
                {
                    string time = TimeUtils.TimeToString(value, true, true);

                    SelectorItem selectorItem = new SelectorItem(key, time, "", null);
                    selectorData.AddItem(selectorItem);
                }
            }
        }

        m_MatchDurationSelector.SetData(selectorData);
    }

    private void InitRefereeSelector()
    {
        if (m_RefereeSelector == null)
            return;

        SelectorData selectorData = new SelectorData();

        List<int> refereeOptionKeys = tnGameData.GetRefereeOptionKeysMain();

        if (refereeOptionKeys != null)
        {
            for (int refereeOptionIndex = 0; refereeOptionIndex < refereeOptionKeys.Count; ++refereeOptionIndex)
            {
                int refereeOptionId = refereeOptionKeys[refereeOptionIndex];

                string refereeOption;
                if (tnGameData.TryGetRefereeValueMain(refereeOptionId, out refereeOption))
                {
                    SelectorItem selectorItem = new SelectorItem(refereeOptionId, refereeOption, "", null);
                    selectorData.AddItem(selectorItem);
                }
            }
        }

        m_RefereeSelector.SetData(selectorData);
    }

    private void InitGoldenGoalSelector()
    {
        if (m_GoldenGoalSelector == null)
            return;

        SelectorData selectorData = new SelectorData();

        List<int> goldenGoalOptionKeys = tnGameData.GetGoldenGoalOptionKeysMain();

        if (goldenGoalOptionKeys != null)
        {
            for (int goldenGoalOptionIndex = 0; goldenGoalOptionIndex < goldenGoalOptionKeys.Count; ++goldenGoalOptionIndex)
            {
                int goldenGoalOptionId = goldenGoalOptionKeys[goldenGoalOptionIndex];

                string goldenGoalOption;
                if (tnGameData.TryGetGoldenGoalValueMain(goldenGoalOptionId, out goldenGoalOption))
                {
                    SelectorItem selectorItem = new SelectorItem(goldenGoalOptionId, goldenGoalOption, "", null);
                    selectorData.AddItem(selectorItem);
                }
            }
        }

        m_GoldenGoalSelector.SetData(selectorData);
    }
        
    private void InitBallSelector()
    {
        if (m_BallSelector == null)
            return;

        SelectorData selectorData = new SelectorData();

        List<int> ballKeys = tnGameData.GetBallsKeysMain();

        if (ballKeys != null)
        {
            for (int ballIndex = 0; ballIndex < ballKeys.Count; ++ballIndex)
            {
                int ballId = ballKeys[ballIndex];
                tnBallData ballData = tnGameData.GetBallDataMain(ballId);

                SelectorItem selectorItem = new SelectorItem(ballId, ballData.name, "", ballData.icon);
                selectorData.AddItem(selectorItem);
            }
        }

        m_BallSelector.SetData(selectorData);
    }

    private void SetupGameModeSelector()
    {
        if (m_GameModeSelector == null)
            return;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        int maxTeamSize = 0;

        for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
        {
            tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);
            if (teamDescription != null)
            {
                maxTeamSize = Mathf.Max(teamDescription.charactersCount, maxTeamSize);
            }
        }

        SelectorData selectorData = new SelectorData();

        List<int> gameModesKeys = tnGameData.GetGameModesKeysMain();

        if (gameModesKeys != null)
        {
            for (int gameModeIndex = 0; gameModeIndex < gameModesKeys.Count; ++gameModeIndex)
            {
                int gameModeId = gameModesKeys[gameModeIndex];
                tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);

                if (gameModeData == null)
                    continue;

                if (!gameModeData.hidden)
                {
                    IntRange teamsRange = gameModeData.teamsRange;
                    IntRange teamSizeRange = gameModeData.playersPerTeamRange;

                    if (teamsRange.IsValueValid(teamsModule.teamsCount))
                    {
                        if (teamSizeRange.IsValueValid(maxTeamSize))
                        {
                            SelectorItem selectorItem = new SelectorItem(gameModeId, gameModeData.name, "", null);
                            selectorData.AddItem(selectorItem);
                        }
                    }
                }
            }
        }

        m_GameModeSelector.SetData(selectorData);
    }

    private void SetupStadiumSelector(int i_GameModeId)
    {
        if (m_StadiumSelector == null)
            return;

        tnTeamsModule teamModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamModule == null)
            return;

        int maxTeamSize = 0;

        for (int teamIndex = 0; teamIndex < teamModule.teamsCount; ++teamIndex)
        {
            tnTeamDescription teamDescription = teamModule.GetTeamDescription(teamIndex);
            if (teamDescription != null)
            {
                maxTeamSize = Mathf.Max(teamDescription.charactersCount, maxTeamSize);
            }
        }

        SelectorData selectorData = new SelectorData();

        List<int> stadiumKeys = tnGameData.GetStadiumsKeysMain();

        if (stadiumKeys != null)
        {
            tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(i_GameModeId);

            for (int stadiumIndex = 0; stadiumIndex < stadiumKeys.Count; ++stadiumIndex)
            {
                int stadiumId = stadiumKeys[stadiumIndex];
                tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

                if (stadiumData == null)
                    continue;

                bool excludedByTag = false;

                if (gameModeData != null)
                {
                    for (int excluderTagIndex = 0; excluderTagIndex < gameModeData.fieldsExcludersTagsCount; ++excluderTagIndex)
                    {
                        int excluderTag = gameModeData.GetFieldExcluderTag(excluderTagIndex);
                        if (excluderTag != Hash.s_EMPTY && excluderTag != Hash.s_NULL)
                        {
                            if (stadiumData.HasTag(excluderTag))
                            {
                                excludedByTag = true;
                            }
                        }
                    }
                }

                IntRange teamSizeRange = stadiumData.teamSize;

                bool locked = excludedByTag || !(teamSizeRange.IsValueValid(maxTeamSize));
                string lockedString = "";

                if (locked)
                {
                    if (excludedByTag)
                    {
                        lockedString = "Not available in this game mode.";
                    }
                    else
                    {
                        lockedString = "From " + teamSizeRange.min + " to " + teamSizeRange.max + " players";
                    }
                }

                SelectorItem selectorItem = new SelectorItem(stadiumId, stadiumData.name, stadiumData.description, stadiumData.icon, locked, lockedString);
                selectorData.AddItem(selectorItem);
            }
        }

        m_StadiumSelector.SetData(selectorData);
    }

    private void SetupAILevelSelector()
    {
        if (m_AILevelSelector == null)
            return;

        SelectorData selectorData = new SelectorData();

        for (int aiLevelIndex = 0; aiLevelIndex < tnGameData.aiLevelCountMain; ++aiLevelIndex)
        {
            tnAILevel aiLevel = tnGameData.GetAILevelMain(aiLevelIndex);

            if (aiLevel == null)
                continue;

            SelectorItem selectorItem = new SelectorItem(aiLevelIndex, aiLevel.label, "", null);
            selectorData.AddItem(selectorItem);
        }

        m_AILevelSelector.SetData(selectorData);
    }

    private void SelectGameMode(int i_Id)
    {
        if (m_GameModeSelector == null)
            return;

        m_GameModeSelector.SelectItem(i_Id);
    }

    private void SelectMatchDuration(int i_Id)
    {
        if (m_MatchDurationSelector == null)
            return;

        m_MatchDurationSelector.SelectItem(i_Id);
    }

    private void SelectReferee(int i_Id)
    {
        if (m_RefereeSelector == null)
            return;

        m_RefereeSelector.SelectItem(i_Id);
    }

    private void SelectGoldenGoal(int i_Id)
    {
        if (m_GoldenGoalSelector == null)
            return;

        m_GoldenGoalSelector.SelectItem(i_Id);
    }

    private void SelectAILevel(int i_Id)
    {
        if (m_AILevelSelector == null)
            return;

        m_AILevelSelector.SelectItem(i_Id);
    }

    private void SelectGameModeIndex(int i_Index)
    {
        if (m_GameModeSelector == null)
            return;

        m_GameModeSelector.SelectItemByIndex(i_Index);
    }

    private void SelectBallIndex(int i_Index)
    {
        if (m_BallSelector == null)
            return;

        m_BallSelector.SelectItemByIndex(i_Index);
    }

    private void SelectStadiumIndex(int i_Index)
    {
        if (m_StadiumSelector == null)
            return;

        m_StadiumSelector.SelectItemByIndex(i_Index);
    }

    private void SelectFirstUnlockedStadium()
    {
        if (m_StadiumSelector == null)
            return;

        int firstValidIndex = 0;
        for (int index = 0; index < m_StadiumSelector.itemsCount; ++index)
        {
            SelectorItem selectorItem = m_StadiumSelector.GetSelectorItem(index);
            if (selectorItem != null)
            {
                if (!selectorItem.locked)
                {
                    firstValidIndex = index;
                    break;
                }
            }
        }

        m_StadiumSelector.SelectItemByIndex(firstValidIndex);
    }

    private void SetGameModeId(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_GameModeSelector == null || m_GameModeSelector.currentItem == null)
            return;

        int gameModeId = m_GameModeSelector.currentItem.id;
        i_Module.SetGameModeId(gameModeId);

        LogManager.Log(this, LogContexts.FSM, "Game Mode : " + m_GameModeSelector.currentItem.label + " " + "[" + m_GameModeSelector.currentItem.id + "]");
    }

    private void SetMatchDuration(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_MatchDurationSelector == null || m_MatchDurationSelector.currentItem == null)
            return;

        int matchDurationKey = m_MatchDurationSelector.currentItem.id;
        i_Module.SetMatchDurationOption(matchDurationKey);

        LogManager.Log(this, LogContexts.FSM, "Match Duration : " + m_MatchDurationSelector.currentItem.label + " " + "[" + m_MatchDurationSelector.currentItem.id + "]");
    }

    private void SetReferee(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_RefereeSelector == null || m_RefereeSelector.currentItem == null)
            return;

        int refereeOptionId = m_RefereeSelector.currentItem.id;
        i_Module.SetRefereeOption(refereeOptionId);

        LogManager.Log(this, LogContexts.FSM, "Referee : " + m_RefereeSelector.currentItem.label + " " + "[" + m_RefereeSelector.currentItem.id + "]");
    }

    private void SetGoldenGoal(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_GoldenGoalSelector == null || m_GoldenGoalSelector.currentItem == null)
            return;

        int goldenGoalOptionId = m_GoldenGoalSelector.currentItem.id;
        i_Module.SetGoldenGoalOption(goldenGoalOptionId);

        LogManager.Log(this, LogContexts.FSM, "Golden Goal : " + m_GoldenGoalSelector.currentItem.label + " " + "[" + m_GoldenGoalSelector.currentItem.id + "]");
    }

    private void SetAILevelIndex(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_AILevelSelector == null || m_AILevelSelector.currentItem == null)
            return;

        int aiLevelIndex = m_AILevelSelector.currentItem.id;
        i_Module.SetAILevelIndex(aiLevelIndex);

        LogManager.Log(this, LogContexts.FSM, "Ai Level : " + m_AILevelSelector.currentItem.label + " " + "[" + m_AILevelSelector.currentItem.id + "]");
    }

    private void SetBall(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_BallSelector == null || m_BallSelector.currentItem == null)
            return;

        int ballId = m_BallSelector.currentItem.id;
        i_Module.SetBallId(ballId);

        LogManager.Log(this, LogContexts.FSM, "Ball : " + m_BallSelector.currentItem.label + " " + "[" + m_BallSelector.currentItem.id + "]");
    }

    private void SetStadium(tnMatchSettingsModule i_Module)
    {
        if (i_Module == null)
            return;

        if (m_StadiumSelector == null || m_StadiumSelector.currentItem == null)
            return;

        int stadiumId = m_StadiumSelector.currentItem.id;
        i_Module.SetStadiumId(stadiumId);

        LogManager.Log(this, LogContexts.FSM, "Stadium : " + m_StadiumSelector.currentItem.label + " " + "[" + m_StadiumSelector.currentItem.id + "]");
    }

    private void AdjustAISelectorNavigation(int i_AICount)
    {
        if (m_AILevelSelector == null)
            return;

        Selectable onUpAILevelSelector = m_AILevelSelector.navigation.selectOnUp;
        Selectable onDownAILevelSelector = m_AILevelSelector.navigation.selectOnDown;
        Selectable onRightAILevelSelector = m_AILevelSelector.navigation.selectOnRight;
        Selectable onLeftAILevelSelector = m_AILevelSelector.navigation.selectOnLeft;

        if (i_AICount > 0)
        {
            if (onUpAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnDown = m_AILevelSelector;

                navigation.selectOnUp = onUpAILevelSelector.navigation.selectOnUp;
                navigation.selectOnRight = onUpAILevelSelector.navigation.selectOnRight;
                navigation.selectOnLeft = onUpAILevelSelector.navigation.selectOnLeft;

                onUpAILevelSelector.navigation = navigation;
            }

            if (onDownAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnUp = m_AILevelSelector;

                navigation.selectOnDown = onDownAILevelSelector.navigation.selectOnDown;
                navigation.selectOnRight = onDownAILevelSelector.navigation.selectOnRight;
                navigation.selectOnLeft = onDownAILevelSelector.navigation.selectOnLeft;

                onDownAILevelSelector.navigation = navigation;
            }

            if (onRightAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnLeft = m_AILevelSelector;

                navigation.selectOnUp = onRightAILevelSelector.navigation.selectOnUp;
                navigation.selectOnRight = onRightAILevelSelector.navigation.selectOnRight;
                navigation.selectOnDown = onRightAILevelSelector.navigation.selectOnDown;

                onRightAILevelSelector.navigation = navigation;
            }

            if (onLeftAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnRight = m_AILevelSelector;

                navigation.selectOnUp = onLeftAILevelSelector.navigation.selectOnUp;
                navigation.selectOnDown = onLeftAILevelSelector.navigation.selectOnDown;
                navigation.selectOnLeft = onLeftAILevelSelector.navigation.selectOnLeft;

                onLeftAILevelSelector.navigation = navigation;
            }
        }
        else
        {
            if (onUpAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnDown = onDownAILevelSelector;

                navigation.selectOnUp = onUpAILevelSelector.navigation.selectOnUp;
                navigation.selectOnRight = onUpAILevelSelector.navigation.selectOnRight;
                navigation.selectOnLeft = onUpAILevelSelector.navigation.selectOnLeft;

                onUpAILevelSelector.navigation = navigation;
            }

            if (onDownAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnUp = onUpAILevelSelector;

                navigation.selectOnDown = onDownAILevelSelector.navigation.selectOnDown;
                navigation.selectOnRight = onDownAILevelSelector.navigation.selectOnRight;
                navigation.selectOnLeft = onDownAILevelSelector.navigation.selectOnLeft;

                onDownAILevelSelector.navigation = navigation;
            }

            if (onRightAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnLeft = onLeftAILevelSelector;

                navigation.selectOnUp = onRightAILevelSelector.navigation.selectOnUp;
                navigation.selectOnRight = onRightAILevelSelector.navigation.selectOnRight;
                navigation.selectOnDown = onRightAILevelSelector.navigation.selectOnDown;

                onRightAILevelSelector.navigation = navigation;
            }

            if (onLeftAILevelSelector != null)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnRight = onRightAILevelSelector;

                navigation.selectOnUp = onLeftAILevelSelector.navigation.selectOnUp;
                navigation.selectOnDown = onLeftAILevelSelector.navigation.selectOnDown;
                navigation.selectOnLeft = onLeftAILevelSelector.navigation.selectOnLeft;

                onLeftAILevelSelector.navigation = navigation;
            }
        }
    }

    // UTILS

    private int GetAICount()
    {
        int aiCount = 0;

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule != null)
        {
            for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
            {
                tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                if (teamDescription == null)
                    continue;

                for (int characterIndex = 0; characterIndex < teamDescription.charactersCount; ++characterIndex)
                {
                    tnCharacterDescription characterDescription = teamDescription.GetCharacterDescription(characterIndex);

                    if (characterDescription == null)
                        continue;

                    bool ai = (characterDescription.playerId == Hash.s_NULL);
                    aiCount += (ai) ? 1 : 0;
                }
            }
        }

        return aiCount;
    }

    // EVENTS

    private void OnGameModeSelectedChanged(SelectorItem i_SelectorItem)
    {
        if (i_SelectorItem == null)
            return;

        int gameModeId = i_SelectorItem.id;
        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);
        if (gameModeData != null)
        {
            tnGameModeConfig gameModeConfig = tnGameData.GetConfigDataMain(gameModeData.optionsConfigId);
            if (gameModeConfig != null)
            {
                // SelectMatchDuration(gameModeConfig.matchDurationOption);
                // SelectReferee(gameModeConfig.refereeOption);
                // SelectGoldenGoal(gameModeConfig.goldenGoalOption);
            }
        }

        SetupStadiumSelector(gameModeId);
        SelectFirstUnlockedStadium();
    }
}
