using UnityEngine;

using TuesdayNights;
using BaseMatchEvents;

public class tnUserStatsUpdater : MonoBehaviour
{
    private static int s_IbrahimovicHashedId = StringUtils.GetHashCode("IBRAHIMOVIC");
    private static int s_VardyHashedId = StringUtils.GetHashCode("VARDY");
    private static int s_RonaldoHashedId = StringUtils.GetHashCode("CRISTIANORONALDO");
    private static int s_ElSharaawyHashedId = StringUtils.GetHashCode("ELSHARAAWY");
    private static int s_IniestaHashedId = StringUtils.GetHashCode("INIESTA");
    private static int s_GotzeHashedId = StringUtils.GetHashCode("GOTZE");
    private static int s_HazardHashedId = StringUtils.GetHashCode("HAZARD");
    private static int s_PogbaHashedId = StringUtils.GetHashCode("POGBA");

    private static int s_ItaHashedId = StringUtils.GetHashCode("ITA");
    private static int s_FraHashedId = StringUtils.GetHashCode("FRA");

    private static int s_Ball8Id = StringUtils.GetHashCode("8ball");

    private static int s_PoolPalaceHashedId = StringUtils.GetHashCode("pool_palace");

    // MonoBehaviour's interface

    void OnEnable()
    {
        Messenger.AddListener<tnGoalEventParams>("ValidatedGoal", OnGoal);

        Messenger.AddListener<tnStartMatchEventParams>("MatchStarted", OnMatchStarted);
        Messenger.AddListener<tnEndMatchEventParams>("MatchEnded", OnMatchEnded);
    }

    void OnDisable()
    {
        Messenger.RemoveListener<tnEndMatchEventParams>("MatchEnded", OnMatchEnded);
        Messenger.RemoveListener<tnStartMatchEventParams>("MatchStarted", OnMatchStarted);

        Messenger.RemoveListener<tnGoalEventParams>("ValidatedGoal", OnGoal);
    }

    // EVENTS

    private void OnGoal(tnGoalEventParams i_Params)
    {
        CheckIbrahimovicGoal(i_Params);
        CheckVardyGoal(i_Params);
        CheckRonaldoGoal(i_Params);
        CheckElSharaawyGoal(i_Params);
        CheckIniestaGoal(i_Params);
        CheckGotzeGoal(i_Params);
        CheckHazardGoal(i_Params);
        CheckPogbaGoal(i_Params);
    }

    private void OnMatchStarted(tnStartMatchEventParams i_Params)
    {
        if (!IsValidMatch(i_Params))
            return;

        StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();

        if (statsModule == null)
            return;

        int humanPlayers = i_Params.teamAHumanPlayers + i_Params.teamBHumanPlayers;
        statsModule.UpdateIntStat(tnUserStatsId.s_BiggerTeamSizeStatId, humanPlayers, IntStatUpdateFunction.Max);

        statsModule.StoreStats();
    }

    private void OnMatchEnded(tnEndMatchEventParams i_Params)
    {
        if (!IsValidMatch(i_Params))
            return;

        StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();

        if (statsModule == null)
            return;

        statsModule.UpdateIntStat(tnUserStatsId.s_MatchesPlayedStatId, 1, IntStatUpdateFunction.Add);

        if (i_Params.hasReferee)
        {
            statsModule.UpdateIntStat(tnUserStatsId.s_MatchesPlayedWithRefereeStatId, 1, IntStatUpdateFunction.Add);
        }

        int minSufferedGoals = GetMinSufferedGoals(i_Params);
        int maxScoredGoals = GetMaxScoredGoals(i_Params);

        statsModule.UpdateIntStat(tnUserStatsId.s_MinSuffuredGoalsStatId, minSufferedGoals, IntStatUpdateFunction.Min);
        statsModule.UpdateIntStat(tnUserStatsId.s_MaxScoredGoalsStatId, maxScoredGoals, IntStatUpdateFunction.Max);

        bool result71 = (i_Params.scoreA == 7 && i_Params.scoreB == 1 && i_Params.teamAHumanPlayers > 0 && i_Params.isTeamALocal) || (i_Params.scoreA == 1 && i_Params.scoreB == 7 && i_Params.teamBHumanPlayers > 0 && i_Params.isTeamBLocal);
        statsModule.UpdateBoolStat(tnUserStatsId.s_Result71StatId, result71, BoolStatUpdateFunction.Add);

        bool realPool = (i_Params.ballId == s_Ball8Id && i_Params.stadiumId == s_PoolPalaceHashedId);
        statsModule.UpdateBoolStat(tnUserStatsId.s_RealPoolStatId, realPool, BoolStatUpdateFunction.Add);

        bool itaBeatsFrance = (i_Params.teamAId == s_ItaHashedId && i_Params.teamBId == s_FraHashedId && i_Params.teamAHumanPlayers > 0 && i_Params.isTeamALocal && i_Params.scoreA > i_Params.scoreB)
                                || (i_Params.teamAId == s_FraHashedId && i_Params.teamBId == s_ItaHashedId && i_Params.teamBHumanPlayers > 0 && i_Params.isTeamBLocal && i_Params.scoreB > i_Params.scoreA);
        statsModule.UpdateBoolStat(tnUserStatsId.s_ItaBeatsFranceStatId, itaBeatsFrance, BoolStatUpdateFunction.Add);

        statsModule.StoreStats();
    }

    // INTERNALS

    private void CheckIbrahimovicGoal(tnGoalEventParams i_Params)
    {
         if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_IbrahimovicHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_IbrahimovicStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckVardyGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_VardyHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_VardyStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckRonaldoGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_RonaldoHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_RonaldoStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckElSharaawyGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_ElSharaawyHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_ElSharaawyStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckIniestaGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_IniestaHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_IniestaStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckGotzeGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_GotzeHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_GotzeStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckHazardGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_HazardHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_HazardStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    private void CheckPogbaGoal(tnGoalEventParams i_Params)
    {
        if (!i_Params.hasValidScorer)
            return;

        int scorerId = i_Params.scorerId;
        if (scorerId == s_PogbaHashedId)
        {
            bool humanScorer = i_Params.isHumanScorer;
            bool isLocal = i_Params.isLocalScorer;

            if (humanScorer && isLocal)
            {
                StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
                if (statsModule != null)
                {
                    statsModule.UpdateIntStat(tnUserStatsId.s_PogbaStatId, 1, IntStatUpdateFunction.Add);
                    statsModule.StoreStats();
                }
            }
        }
    }

    // UTILS

    private bool IsValidMatch(tnStartMatchEventParams i_Params)
    {
        int teamAHumanPlayers = i_Params.teamAHumanPlayers;
        int teamBHumanPlayers = i_Params.teamBHumanPlayers;

        return (teamAHumanPlayers > 0 || teamBHumanPlayers > 0);
    }

    private bool IsValidMatch(tnEndMatchEventParams i_Params)
    {
        int teamAHumanPlayers = i_Params.teamAHumanPlayers;
        int teamBHumanPlayers = i_Params.teamBHumanPlayers;

        return (teamAHumanPlayers > 0 || teamBHumanPlayers > 0);
    }

    private int GetMinSufferedGoals(tnEndMatchEventParams i_Params)
    {
        if (i_Params.scoreA == i_Params.scoreB)
        {
            return i_Params.scoreA;
        }

        if (i_Params.scoreA < i_Params.scoreB)
        {
            if (i_Params.teamAHumanPlayers > 0 && i_Params.isTeamALocal)
            {
                return i_Params.scoreA;
            }
            else
            {
                if (i_Params.teamBHumanPlayers > 0 && i_Params.isTeamBLocal)
                {
                    return i_Params.scoreB;
                }
                else
                {
                    return int.MaxValue;
                }
            }
        }
        else
        {
            // i_Params.scoreB < i_Params.scoreA

            if (i_Params.teamBHumanPlayers > 0 && i_Params.isTeamBLocal)
            {
                return i_Params.scoreB;
            }
            else
            {
                if (i_Params.teamAHumanPlayers > 0 && i_Params.isTeamALocal)
                {
                    return i_Params.scoreA;
                }
                else
                {
                    return int.MaxValue;
                }
            }
        }
    }

    private int GetMaxScoredGoals(tnEndMatchEventParams i_Params)
    {
        if (i_Params.scoreA == i_Params.scoreB)
        {
            return i_Params.scoreA;
        }

        if (i_Params.scoreA > i_Params.scoreB)
        {
            if (i_Params.teamAHumanPlayers > 0 && i_Params.isTeamALocal)
            {
                return i_Params.scoreA;
            }
            else
            {
                if (i_Params.teamBHumanPlayers > 0 && i_Params.isTeamBLocal)
                {
                    return i_Params.scoreB;
                }
                else
                {
                    return 0;
                }
            }
        }
        else
        {
            // i_Params.scoreB > i_Params.scoreA

            if (i_Params.teamBHumanPlayers > 0 && i_Params.isTeamBLocal)
            {
                return i_Params.scoreB;
            }
            else
            {
                if (i_Params.teamAHumanPlayers > 0 && i_Params.isTeamALocal)
                {
                    return i_Params.scoreA;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
