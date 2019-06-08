using UnityEngine;

using System;
using System.Collections.Generic;

using TrueSync;

using Random = UnityEngine.Random;

public class tnBaseMatchStatsController : tnMatchStatsController
{
    [SerializeField]
    private RectTransform m_StatsRootPanel = null;
    [SerializeField]
    private tnStatsPanel m_StatsPanelPrefab = null;

    private static int s_MaxPanels = 4;

    private tnStatsPanel[] m_StatsPanels = null;

    public enum StatType
    {
        Shots = 0,
        ShotsOnTarget = 1,
        DistanceRun = 2,
        DashCount = 3,
        BallTouches = 4,
        AttractTime = 5,
        Tackles = 6,
        GoalScored = 7,
    }

    private static string s_StatName_Shots = "Shots";
    private static string s_StatName_ShotsOnTarget = "Shots On Target";
    private static string s_StatName_DistanceRun = "Distance Run";
    private static string s_StatName_DashCount = "Dashes";
    private static string s_StatName_BallTouches = "Ball Touches";
    private static string s_StatName_AttractTime = "Attract Time";
    private static string s_StatName_Tackles = "Tackles";
    private static string s_StatName_GoalScored = "Goals Scored";

    private static string s_PartecipationLabel = "of Total";

    // tnMatchStatsController's INTERFACE

    protected override void ShowStats(tnMatchController i_Controller)
    {
        base.ShowStats(i_Controller);

        if (i_Controller == null)
            return;

        tnBaseMatchController matchController = (tnBaseMatchController)i_Controller;
        InternalSetStats(matchController);
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        // Create stats panels.

        if (m_StatsRootPanel == null)
            return;

        if (m_StatsPanelPrefab == null)
            return;

        m_StatsPanels = new tnStatsPanel[s_MaxPanels];

        for (int panelIndex = 0; panelIndex < m_StatsPanels.Length; ++panelIndex)
        {
            tnStatsPanel statsPanel = Instantiate<tnStatsPanel>(m_StatsPanelPrefab);
            statsPanel.transform.SetParent(m_StatsRootPanel, false);

            m_StatsPanels[panelIndex] = statsPanel;
        }
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

    // INTERNALS

    private void InternalSetStats(tnBaseMatchController i_MathController)
    {
        if (m_StatsPanels == null)
            return;

        // Disable all slots.

        for (int slotIndex = 0; slotIndex < m_StatsPanels.Length; ++slotIndex)
        {
            tnStatsPanel statsPanel = m_StatsPanels[slotIndex];
            if (statsPanel != null)
            {
                statsPanel.gameObject.SetActive(false);
            }
        }

        if (i_MathController == null)
            return;

        int charactersCount = i_MathController.charactersCount;

        int statsCount = Mathf.Max(1, charactersCount/* / 2*/);

        // Select N random stats.

        int[] statsIndices = SelectStats(statsCount);

        for (int index = 0; index < statsIndices.Length; ++index)
        {
            // Get target stat index.

            int statIndex = statsIndices[index];

            // Select the best character for this stat.

            StatType statType = (StatType)statIndex;

            switch (statType)
            {
                case StatType.Shots:

                    FillShotsData(index, i_MathController);
                    break;

                case StatType.ShotsOnTarget:

                    FillShotsOnTarget(index, i_MathController);
                    break;

                case StatType.DistanceRun:

                    FillDistanceRunData(index, i_MathController);
                    break;

                case StatType.DashCount:

                    FillDashCountData(index, i_MathController);
                    break;

                case StatType.BallTouches:

                    FillBallTouchesData(index, i_MathController);
                    break;

                case StatType.AttractTime:

                    FillAttractTimeData(index, i_MathController);
                    break;

                case StatType.Tackles:

                    FillTacklesData(index, i_MathController);
                    break;

                case StatType.GoalScored:

                    FillGoalScoredData(index, i_MathController);
                    break;
            }
        }
    }

    private int[] SelectStats(int i_NumStats)
    {
        string[] statsNames = Enum.GetNames(typeof(StatType));

        int statsCount = statsNames.Length;
        int numStats = Mathf.Min(i_NumStats, statsCount);

        List<int> outList = new List<int>();

        for (int j = statsCount - numStats; j < statsCount; ++j)
        {
            int statIndex = Random.Range(0, j + 1);

            if (outList.Contains(statIndex))
            {
                outList.Add(j);
            }
            else
            {
                outList.Add(statIndex);
            }
        }

        int[] outArray = outList.ToArray();
        return outArray;
    }

    private bool IsValidSlotIndex(int i_SlotIndex)
    {
        return (m_StatsPanels != null && (i_SlotIndex >= 0 && i_SlotIndex < m_StatsPanels.Length));
    }

    private void FillShotsData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalShots = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalShots += characterResults.shots;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        int maxShots = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterShots = characterResults.shots;

                if (characterShots > maxShots)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxShots = characterShots;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxShots.ToString();

        float partecipationPercentage = 0f;

        if (totalShots > 0)
        {
            partecipationPercentage = (float)maxShots / (float)totalShots;
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_Shots, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillShotsOnTarget(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalShotsOnTarget = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalShotsOnTarget += characterResults.shotsOnTarget;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        int maxShotsOnTarget = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterShotsOnTarget = characterResults.shotsOnTarget;

                if (characterShotsOnTarget > maxShotsOnTarget)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxShotsOnTarget = characterShotsOnTarget;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxShotsOnTarget.ToString();

        float partecipationPercentage = 0f;

        if (totalShotsOnTarget > 0)
        {
            partecipationPercentage = (float)maxShotsOnTarget / (float)totalShotsOnTarget;
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_ShotsOnTarget, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillDistanceRunData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        FP totalDistanceRun = FP.Zero;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalDistanceRun += characterResults.distanceRun;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        FP maxDistanceRun = FP.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                FP characterDistanceRun = characterResults.distanceRun;

                if (characterDistanceRun > maxDistanceRun)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxDistanceRun = characterDistanceRun;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxDistanceRun.ToString(2);
        statValue += " mt";

        FP partecipationPercentage = FP.Zero;

        if (totalDistanceRun > FP.Zero)
        {
            partecipationPercentage = maxDistanceRun / totalDistanceRun;
            partecipationPercentage *= 100f;

            partecipationPercentage = MathFP.Clamp(partecipationPercentage, FP.Zero, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString(2);
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_DistanceRun, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillDashCountData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalDashCount = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalDashCount += characterResults.dashCount;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        int maxDashCount = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterDashCount = characterResults.dashCount;

                if (characterDashCount > maxDashCount)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxDashCount = characterDashCount;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxDashCount.ToString();

        float partecipationPercentage = 0f;

        if (totalDashCount > 0)
        {
            partecipationPercentage = (float)maxDashCount / (float)totalDashCount;
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_DashCount, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillBallTouchesData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalBallTouches = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalBallTouches += characterResults.ballTouches;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        int maxBallTouches = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterBallTouches = characterResults.ballTouches;

                if (characterBallTouches > maxBallTouches)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxBallTouches = characterBallTouches;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxBallTouches.ToString();

        float partecipationPercentage = 0f;

        if (totalBallTouches > 0)
        {
            partecipationPercentage = (float)maxBallTouches / (float)totalBallTouches;
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_BallTouches, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillAttractTimeData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        FP totalAttractTime = FP.Zero;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalAttractTime += characterResults.attractTime;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        FP maxAttractTime = FP.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                FP characterAttractTime = characterResults.attractTime;

                if (characterAttractTime > maxAttractTime)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxAttractTime = characterAttractTime;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxAttractTime.ToString(2);
        statValue += " s";

        FP partecipationPercentage = FP.Zero;

        if (totalAttractTime > FP.Zero)
        {
            partecipationPercentage = maxAttractTime / totalAttractTime;
            partecipationPercentage *= 100f;

            partecipationPercentage = MathFP.Clamp(partecipationPercentage, FP.Zero, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString(2);
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_AttractTime, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillTacklesData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalTackles = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalTackles += characterResults.tackles;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        int maxTackles = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterTackles = characterResults.tackles;

                if (characterTackles > maxTackles)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxTackles = characterTackles;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxTackles.ToString();

        float partecipationPercentage = 0f;

        if (totalTackles > 0)
        {
            partecipationPercentage = (float)maxTackles / (float)totalTackles;
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_Tackles, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillGoalScoredData(int i_SlotIndex, tnBaseMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalGoalScored = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalGoalScored += characterResults.goalScored;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;

        int selectedCharacterId = Hash.s_NULL;
        int selectedPlayerId = Hash.s_NULL;

        int maxGoalScored = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnBaseMatchCharacterResults characterResults = (tnBaseMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterGoalScored = characterResults.goalScored;

                if (characterGoalScored > maxGoalScored)
                {
                    selectedCharacterIndex = characterIndex;

                    selectedCharacterId = characterResults.id;
                    selectedPlayerId = characterResults.playerId;

                    maxGoalScored = characterGoalScored;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        Color playerColor = Color.white;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }

            tnPlayerData playerData = tnGameData.GetPlayerDataMain(selectedPlayerId);
            if (playerData != null)
            {
                playerColor = playerData.color;
            }
        }

        string statValue = maxGoalScored.ToString();

        float partecipationPercentage = 0f;

        if (totalGoalScored > 0)
        {
            partecipationPercentage = (float)maxGoalScored / (float)totalGoalScored;
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, playerColor, s_StatName_GoalScored, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillData(int i_SlotIndex, string i_PlayerName, Sprite i_Portrait, Color i_TeamColor, string i_StatLabel, string i_StatValue, string i_ParticipationLabel, string i_ParticipationValue)
    {
        if (!IsValidSlotIndex(i_SlotIndex))
            return;

        if (m_StatsPanels == null)
            return;

        tnStatsPanel statsPanel = m_StatsPanels[i_SlotIndex];
        if (statsPanel == null)
            return;

        statsPanel.gameObject.SetActive(true);

        statsPanel.SetPlayerName(i_PlayerName);
        statsPanel.SetPortrait(i_Portrait);

        statsPanel.SetTeamColor(i_TeamColor);

        statsPanel.SetStatLabel(i_StatLabel);
        statsPanel.SetStatValue(i_StatValue);

        statsPanel.SetPartecipationLabel(i_ParticipationLabel);
        statsPanel.SetPartecipationValue(i_ParticipationValue);
    }
}
