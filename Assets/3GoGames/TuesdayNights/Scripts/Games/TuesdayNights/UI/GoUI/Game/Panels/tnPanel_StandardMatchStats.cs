using UnityEngine;

using System;
using System.Collections.Generic;

using GoUI;

using TrueSync;

using Random = UnityEngine.Random;

public class tnPanel_StandardMatchStats : UIPanel<tnView_StandardMatchStats>
{
    // Types

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

    // STATIC

    private static string s_StatName_Shots = "Shots";
    private static string s_StatName_ShotsOnTarget = "Shots On Target";
    private static string s_StatName_DistanceRun = "Distance Run";
    private static string s_StatName_DashCount = "Dashes";
    private static string s_StatName_BallTouches = "Ball Touches";
    private static string s_StatName_AttractTime = "Attract Time";
    private static string s_StatName_Tackles = "Tackles";
    private static string s_StatName_GoalScored = "Goals Scored";

    private static string s_PartecipationLabel = "of Total";

    // UIPanel's interface

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

    public void Config(tnStandardMatchController i_MatchController)
    {
        Internal_Config(i_MatchController);
    }

    // INTERNALS

    private void Internal_Config(tnStandardMatchController i_MatchController)
    {
        if (viewInstance == null)
            return;

        viewInstance.DisableAllEntries();

        if (i_MatchController == null)
            return;

        int charactersCount = i_MatchController.charactersCount;
        int statsCount = Mathf.Max(1, charactersCount /* / 2*/);

        // Select N random stats.

        int[] statsIndices = SelectStats(statsCount);

        for (int index = 0; index < statsIndices.Length; ++index)
        {
            // Get target stat index.

            int statIndex = statsIndices[index];

            StatType statType = (StatType)statIndex;

            switch (statType)
            {
                case StatType.AttractTime:
                    FillAttractTimeData(index, i_MatchController);
                    break;

                case StatType.BallTouches:
                    FillBallTouchesData(index, i_MatchController);
                    break;

                case StatType.DashCount:
                    FillDashCountData(index, i_MatchController);
                    break;

                case StatType.DistanceRun:
                    FillDistanceRunData(index, i_MatchController);
                    break;

                case StatType.GoalScored:
                    FillGoalScoredData(index, i_MatchController);
                    break;

                case StatType.Shots:
                    FillShotsData(index, i_MatchController);
                    break;

                case StatType.ShotsOnTarget:
                    FillShotsOnTarget(index, i_MatchController);
                    break;

                case StatType.Tackles:
                    FillTacklesData(index, i_MatchController);
                    break;
            }
        }
    }

    private int[] SelectStats(int i_StatsCount)
    {
        string[] statsNames = Enum.GetNames(typeof(StatType));

        int statsCount = statsNames.Length;
        int numStats = Mathf.Min(i_StatsCount, statsCount);

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

    private void FillShotsData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charactersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalShots = 0;

        for (int characterIndex = 0; characterIndex < charactersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalShots += characterResults.shots;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        int maxShots = int.MinValue;

        for (int characterIndex = 0; characterIndex < charactersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterShots = characterResults.shots;
                if (characterShots > maxShots)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxShots = characterShots;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
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

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_Shots, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillShotsOnTarget(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalShotsOnTarget = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalShotsOnTarget += characterResults.shotsOnTarget;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        int maxShotsOnTarget = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterShotsOnTarget = characterResults.shotsOnTarget;

                if (characterShotsOnTarget > maxShotsOnTarget)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxShotsOnTarget = characterShotsOnTarget;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
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

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_ShotsOnTarget, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillDistanceRunData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        FP totalDistanceRun = FP.Zero;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalDistanceRun += characterResults.distanceRun;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        FP maxDistanceRun = FP.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                FP characterDistanceRun = characterResults.distanceRun;

                if (characterDistanceRun > maxDistanceRun)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxDistanceRun = characterDistanceRun;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }
        }

        string statValue = maxDistanceRun.ToString(2);
        statValue += " mt";

        float partecipationPercentage = 0f;

        if (totalDistanceRun > 0f)
        {
            partecipationPercentage = maxDistanceRun.AsFloat() / totalDistanceRun.AsFloat();
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_DistanceRun, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillDashCountData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalDashCount = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalDashCount += characterResults.dashCount;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        int maxDashCount = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterDashCount = characterResults.dashCount;

                if (characterDashCount > maxDashCount)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxDashCount = characterDashCount;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
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

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_DashCount, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillBallTouchesData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalBallTouches = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalBallTouches += characterResults.ballTouches;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        int maxBallTouches = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterBallTouches = characterResults.ballTouches;

                if (characterBallTouches > maxBallTouches)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxBallTouches = characterBallTouches;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
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

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_BallTouches, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillAttractTimeData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        FP totalAttractTime = FP.Zero;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalAttractTime += characterResults.attractTime;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        FP maxAttractTime = FP.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                FP characterAttractTime = characterResults.attractTime;

                if (characterAttractTime > maxAttractTime)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxAttractTime = characterAttractTime;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
            }
        }

        string statValue = maxAttractTime.ToString(2);
        statValue += " s";

        float partecipationPercentage = 0f;

        if (totalAttractTime > 0f)
        {
            partecipationPercentage = maxAttractTime.AsFloat() / totalAttractTime.AsFloat();
            partecipationPercentage *= 100f;

            partecipationPercentage = Mathf.Clamp(partecipationPercentage, 0f, 100f);
        }

        string partecipationValue = partecipationPercentage.ToString("0.00");
        partecipationValue += "%";

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_AttractTime, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillTacklesData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalTackles = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalTackles += characterResults.tackles;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        int maxTackles = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterTackles = characterResults.tackles;

                if (characterTackles > maxTackles)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxTackles = characterTackles;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
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

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_Tackles, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillGoalScoredData(int i_SlotIndex, tnStandardMatchController i_MatchController)
    {
        if (i_MatchController == null)
            return;

        int charcatersCount = i_MatchController.charactersCount;

        // Compute total shots count.

        int totalGoalScored = 0;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                totalGoalScored += characterResults.goalScored;
            }
        }

        // Get best character for this stat.

        int selectedCharacterIndex = -1;
        int selectedCharacterId = Hash.s_NULL;

        int maxGoalScored = int.MinValue;

        for (int characterIndex = 0; characterIndex < charcatersCount; ++characterIndex)
        {
            tnStandardMatchCharacterResults characterResults = (tnStandardMatchCharacterResults)i_MatchController.GetCharacterResultsByIndex(characterIndex);
            if (characterResults != null)
            {
                int characterGoalScored = characterResults.goalScored;

                if (characterGoalScored > maxGoalScored)
                {
                    selectedCharacterIndex = characterIndex;
                    selectedCharacterId = characterResults.id;

                    maxGoalScored = characterGoalScored;
                }
            }
        }

        if (selectedCharacterIndex < 0)
            return;

        // Get team color.

        Color teamColor = Color.white;

        GameObject characterGo = i_MatchController.GetCharacterByIndex(selectedCharacterIndex);
        if (characterGo != null)
        {
            tnCharacterInfo characterInfo = characterGo.GetComponent<tnCharacterInfo>();
            if (characterInfo != null)
            {
                teamColor = characterInfo.teamColor;
            }
        }

        // Fill data.

        string playerName = "";
        Sprite playerPortrait = null;

        {
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(selectedCharacterId);
            if (characterData != null)
            {
                playerName = characterData.displayName;
                playerPortrait = characterData.uiIconFacingRight;
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

        FillData(i_SlotIndex, playerName, playerPortrait, teamColor, s_StatName_GoalScored, statValue, s_PartecipationLabel, partecipationValue);
    }

    private void FillData(int i_SlotIndex, string i_PlayerName, Sprite i_Portrait, Color i_TeamColor, string i_StatLabel, string i_StatValue, string i_PartecipationLabel, string i_PartecipationValue)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetEntry(i_SlotIndex, i_PlayerName, i_Portrait, i_StatLabel, i_StatValue, i_PartecipationLabel, i_PartecipationValue, i_TeamColor);
    }
}