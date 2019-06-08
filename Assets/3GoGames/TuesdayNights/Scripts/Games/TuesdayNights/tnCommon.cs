using UnityEngine;

namespace TuesdayNights
{
    public enum CharacterRole
    {
        Goalkeeper = 0,
        Defender = 1,
        Midfielder = 2,
        Striker = 3,
    }

    public enum AIRole
    {
        Null = 0,
        Defender = 1,
        Midfielder = 2,
        Striker = 3,
    }

    public static class tnTeamStatsId
    {
        public static int s_Mass_StatId = StringUtils.GetHashCode("MOV_MASS");
        public static int s_Drag_StatId = StringUtils.GetHashCode("MOV_DRAG");
        public static int s_MovForce_StatId = StringUtils.GetHashCode("MOV_FORCE");
        public static int s_MaxSpeed_StatId = StringUtils.GetHashCode("MOV_MAX_SPEED");

        public static int s_EnergyInitialValue_StatId = StringUtils.GetHashCode("ENERGY_INITIAL_VALUE");
        public static int s_EnergyRecoveryRate_StatId = StringUtils.GetHashCode("ENERGY_RECOVERY_RATE");

        public static int s_DashMass_StatId = StringUtils.GetHashCode("DASH_MASS");
        public static int s_DashDrag_StatId = StringUtils.GetHashCode("DASH_DRAG");
        public static int s_DashForce_StatId = StringUtils.GetHashCode("DASH_FORCE");
        public static int s_DashCooldown_StatId = StringUtils.GetHashCode("DASH_COOLDOWN");
        public static int s_DashDuration_StatId = StringUtils.GetHashCode("DASH_DURATION");
        public static int s_DashEnergyCost_StatId = StringUtils.GetHashCode("DASH_ENERGY_COST");

        public static int s_KickEnergyCost_StatId = StringUtils.GetHashCode("KICK_ENERGY_COST");
        public static int s_KickRadius_StatId = StringUtils.GetHashCode("KICK_RADIUS");
        public static int s_KickForce_StatId = StringUtils.GetHashCode("KICK_FORCE");
        public static int s_KickTackleForce_StatId = StringUtils.GetHashCode("TACKLE_FORCE");
        public static int s_KickCooldown_StatId = StringUtils.GetHashCode("KICK_COOLDOWN");

        public static int s_AttractRadius_StatId = StringUtils.GetHashCode("ATTRACT_RADIUS");
        public static int s_AttractEnergyCostRate_StatId = StringUtils.GetHashCode("ATTRACT_ENERGY_COST_RATE");
        public static int s_AttractForce_StatId = StringUtils.GetHashCode("ATTRACT_FORCE");
    }

    public static class tnUserStatsId
    {
        public static int s_IbrahimovicStatId = StringUtils.GetHashCode("IBRAHIMOVIC_GOALS");
        public static int s_VardyStatId = StringUtils.GetHashCode("VARDY_GOALS");
        public static int s_RonaldoStatId = StringUtils.GetHashCode("RONALDO_GOALS");
        public static int s_ElSharaawyStatId = StringUtils.GetHashCode("ELSHARAAWY_GOALS");
        public static int s_IniestaStatId = StringUtils.GetHashCode("INIESTA_GOALS");
        public static int s_GotzeStatId = StringUtils.GetHashCode("GOTZE_GOALS");
        public static int s_HazardStatId = StringUtils.GetHashCode("HAZARD_GOALS");
        public static int s_PogbaStatId = StringUtils.GetHashCode("POGBA_GOALS");

        public static int s_MatchesPlayedStatId = StringUtils.GetHashCode("MATCHES_PLAYED");
        public static int s_MatchesPlayedWithRefereeStatId = StringUtils.GetHashCode("MATCHES_PLAYED_WITH_REFEREE");
        public static int s_MinSuffuredGoalsStatId = StringUtils.GetHashCode("MIN_SUFFERED_GOALS");
        public static int s_MaxScoredGoalsStatId = StringUtils.GetHashCode("MAX_SCORED_GOALS");
        public static int s_BiggerTeamSizeStatId = StringUtils.GetHashCode("BIGGER_TEAM_SIZE");

        public static int s_Result71StatId = StringUtils.GetHashCode("RESULT_71");
        public static int s_RealPoolStatId = StringUtils.GetHashCode("REAL_POOL");
        public static int s_ItaBeatsFranceStatId = StringUtils.GetHashCode("ITA_BEATS_FRA");
    }

    public static class InputActions
    {
        public static int s_HorizontalAxis = StringUtils.GetHashCode("MoveHorizontal");
        public static int s_VerticalAxis = StringUtils.GetHashCode("MoveVertical");

        public static int s_DashButton = StringUtils.GetHashCode("Dash");

        public static int s_ShotButton = StringUtils.GetHashCode("Shot");
        public static int s_PassButton = StringUtils.GetHashCode("Pass");
        public static int s_AttractButton = StringUtils.GetHashCode("Attract");

        public static int s_TauntButton = StringUtils.GetHashCode("Taunt");

        public static int s_StartButton = StringUtils.GetHashCode("Start");
    }

    public static class InputButtons
    {
        public static string s_HorizontalLeft = "HorizontalLeft";
        public static string s_HorizontalRight = "HorizontalRight";
        public static string s_VerticalDown = "VerticalDown";
        public static string s_VerticalUp = "VerticalUp";

        public static string s_Submit = "Submit";
        public static string s_Cancel = "Cancel";
        public static string s_Start = "Start";
    }

    public static class WifiInputButtons
    {
        public static string s_HorizontalLeft = "Horizontal";
        public static string s_HorizontalRight = "Horizontal";
        public static string s_VerticalDown = "Vertical";
        public static string s_VerticalUp = "Vertical";

        public static string s_Submit = "Submit";
        public static string s_Cancel = "Cancel";
        public static string s_Start = "Start";
    }

    public static class Settings
    {
        // Volumes

        public static int s_SfxVolumeSetting = StringUtils.GetHashCode("sfx_volume");
        public static int s_MusicVolumeSetting = StringUtils.GetHashCode("music_volume");

        // Visual effects

        public static int s_ScreenshakeSetting = StringUtils.GetHashCode("screen_shake");
        //public static int s_SlowMotionSetting = StringUtils.GetHashCode("slow_motion");
        public static int s_CameraMovementSetting = StringUtils.GetHashCode("camera_movement");

        // Input.

        public static int s_UseXInput = StringUtils.GetHashCode("use_xinput");
        public static int s_UseRumble = StringUtils.GetHashCode("use_rumble");
    }

    public static class GlobalSettings
    {
        public static int s_EnableAI = StringUtils.GetHashCode("ENABLE_AI");
        public static int s_TimeRestriction = StringUtils.GetHashCode("TIME_RESTRICTION");

        public static int s_WorkInProgress = StringUtils.GetHashCode("WORK_IN_PROGRESS");
    }

    public static class ParamKeys
    {
        public static int s_ScorerId = StringUtils.GetHashCode("SCORER_ID");
        public static int s_ScorerTeamId = StringUtils.GetHashCode("SCORER_TEAM_ID");
        public static int s_HasValidScorer = StringUtils.GetHashCode("HAS_VALID_SCORER");
        public static int s_IsOwnGoal = StringUtils.GetHashCode("IS_OWN_GOAL");
    }

    public static class Tags
    {
        public static string s_Ball = "Ball";
        public static string s_Character = "Character";
    }

    public static class Utils
    {
        public static Color[] ComputeTeamColors(int[] i_TeamIds)
        {
            if (i_TeamIds == null || i_TeamIds.Length == 0)
            {
                return null;
            }

            Color[] teamColors = new Color[i_TeamIds.Length];
                 
            for (int teamIndex = 0; teamIndex < i_TeamIds.Length; ++teamIndex)
            {
                teamColors[teamIndex] = Color.white;

                int teamId = i_TeamIds[teamIndex];

                tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);
                if (teamData != null)
                {
                    Color firstColor = teamData.firstColor;
                    Color secondColor = teamData.secondColor;

                    teamColors[teamIndex] = firstColor;

                    for (int prevIndex = 0; prevIndex < teamIndex; ++prevIndex)
                    {
                        if (ColorUtils.ApproximatelyEqual(teamColors[prevIndex], teamColors[teamIndex], 0.1f, 0.1f, 0.1f, 1f))
                        {
                            teamColors[teamIndex] = secondColor;
                            break;
                        }
                    }
                }
            }

            return teamColors;
        }
    }
     
    public static class AppInfo
    {
        public static string s_GameVersion = "1.02a";
    }

    public static class SpawnPoints
    {
        private static string[] s_Spawn_Team0_0 = new string[]
        {
            "",
        };

        private static string[] s_Spawn_Team0_1 = new string[]
        {
            "Spawn_Team0_1_1",
        };

        private static string[] s_Spawn_Team0_2 = new string[]
        {
            "Spawn_Team0_2_1",
            "Spawn_Team0_2_2",
        };

        private static string[] s_Spawn_Team0_3 = new string[]
        {
            "Spawn_Team0_3_1",
            "Spawn_Team0_3_2",
            "Spawn_Team0_3_3",
        };

        private static string[] s_Spawn_Team0_4 = new string[]
        {
            "Spawn_Team0_4_1",
            "Spawn_Team0_4_2",
            "Spawn_Team0_4_3",
            "Spawn_Team0_4_4",
        };

        private static string[] s_Spawn_Team0_5 = new string[]
        {
            "Spawn_Team0_5_1",
            "Spawn_Team0_5_2",
            "Spawn_Team0_5_3",
            "Spawn_Team0_5_4",
            "Spawn_Team0_5_5",
        };

        private static string[] s_Spawn_Team0_6 = new string[]
        {
            "Spawn_Team0_6_1",
            "Spawn_Team0_6_2",
            "Spawn_Team0_6_3",
            "Spawn_Team0_6_4",
            "Spawn_Team0_6_5",
            "Spawn_Team0_6_6",
        };

        private static string[] s_Spawn_Team0_7 = new string[]
        {
            "Spawn_Team0_7_1",
            "Spawn_Team0_7_2",
            "Spawn_Team0_7_3",
            "Spawn_Team0_7_4",
            "Spawn_Team0_7_5",
            "Spawn_Team0_7_6",
            "Spawn_Team0_7_7",
        };

        private static string[] s_Spawn_Team0_8 = new string[]
        {
            "Spawn_Team0_8_1",
            "Spawn_Team0_8_2",
            "Spawn_Team0_8_3",
            "Spawn_Team0_8_4",
            "Spawn_Team0_8_5",
            "Spawn_Team0_8_6",
            "Spawn_Team0_8_7",
            "Spawn_Team0_8_8",
        };

        private static string[] s_Spawn_Team0_9 = new string[]
        {
            "Spawn_Team0_9_1",
            "Spawn_Team0_9_2",
            "Spawn_Team0_9_3",
            "Spawn_Team0_9_4",
            "Spawn_Team0_9_5",
            "Spawn_Team0_9_6",
            "Spawn_Team0_9_7",
            "Spawn_Team0_9_8",
            "Spawn_Team0_9_9",
        };

        private static string[] s_Spawn_Team0_10 = new string[]
        {
            "Spawn_Team0_10_1",
            "Spawn_Team0_10_2",
            "Spawn_Team0_10_3",
            "Spawn_Team0_10_4",
            "Spawn_Team0_10_5",
            "Spawn_Team0_10_6",
            "Spawn_Team0_10_7",
            "Spawn_Team0_10_8",
            "Spawn_Team0_10_9",
            "Spawn_Team0_10_10",
        };

        private static string[] s_Spawn_Team0_11 = new string[]
        {
            "Spawn_Team0_11_1",
            "Spawn_Team0_11_2",
            "Spawn_Team0_11_3",
            "Spawn_Team0_11_4",
            "Spawn_Team0_11_5",
            "Spawn_Team0_11_6",
            "Spawn_Team0_11_7",
            "Spawn_Team0_11_8",
            "Spawn_Team0_11_9",
            "Spawn_Team0_11_10",
            "Spawn_Team0_11_11",
        };

        private static string[] s_Spawn_Team1_0 = new string[]
        {
            "",
        };

        private static string[] s_Spawn_Team1_1 = new string[]
        {
            "Spawn_Team1_1_1",
        };

        private static string[] s_Spawn_Team1_2 = new string[]
        {
            "Spawn_Team1_2_1",
            "Spawn_Team1_2_2",
        };

        private static string[] s_Spawn_Team1_3 = new string[]
        {
            "Spawn_Team1_3_1",
            "Spawn_Team1_3_2",
            "Spawn_Team1_3_3",
        };

        private static string[] s_Spawn_Team1_4 = new string[]
        {
            "Spawn_Team1_4_1",
            "Spawn_Team1_4_2",
            "Spawn_Team1_4_3",
            "Spawn_Team1_4_4",
        };

        private static string[] s_Spawn_Team1_5 = new string[]
        {
            "Spawn_Team1_5_1",
            "Spawn_Team1_5_2",
            "Spawn_Team1_5_3",
            "Spawn_Team1_5_4",
            "Spawn_Team1_5_5",
        };

        private static string[] s_Spawn_Team1_6 = new string[]
        {
            "Spawn_Team1_6_1",
            "Spawn_Team1_6_2",
            "Spawn_Team1_6_3",
            "Spawn_Team1_6_4",
            "Spawn_Team1_6_5",
            "Spawn_Team1_6_6",
        };

        private static string[] s_Spawn_Team1_7 = new string[]
        {
            "Spawn_Team1_7_1",
            "Spawn_Team1_7_2",
            "Spawn_Team1_7_3",
            "Spawn_Team1_7_4",
            "Spawn_Team1_7_5",
            "Spawn_Team1_7_6",
            "Spawn_Team1_7_7",
        };

        private static string[] s_Spawn_Team1_8 = new string[]
        {
            "Spawn_Team1_8_1",
            "Spawn_Team1_8_2",
            "Spawn_Team1_8_3",
            "Spawn_Team1_8_4",
            "Spawn_Team1_8_5",
            "Spawn_Team1_8_6",
            "Spawn_Team1_8_7",
            "Spawn_Team1_8_8",
        };

        private static string[] s_Spawn_Team1_9 = new string[]
        {
            "Spawn_Team1_9_1",
            "Spawn_Team1_9_2",
            "Spawn_Team1_9_3",
            "Spawn_Team1_9_4",
            "Spawn_Team1_9_5",
            "Spawn_Team1_9_6",
            "Spawn_Team1_9_7",
            "Spawn_Team1_9_8",
            "Spawn_Team1_9_9",
        };

        private static string[] s_Spawn_Team1_10 = new string[]
        {
            "Spawn_Team1_10_1",
            "Spawn_Team1_10_2",
            "Spawn_Team1_10_3",
            "Spawn_Team1_10_4",
            "Spawn_Team1_10_5",
            "Spawn_Team1_10_6",
            "Spawn_Team1_10_7",
            "Spawn_Team1_10_8",
            "Spawn_Team1_10_9",
            "Spawn_Team1_10_10",
        };

        private static string[] s_Spawn_Team1_11 = new string[]
        {
            "Spawn_Team1_11_1",
            "Spawn_Team1_11_2",
            "Spawn_Team1_11_3",
            "Spawn_Team1_11_4",
            "Spawn_Team1_11_5",
            "Spawn_Team1_11_6",
            "Spawn_Team1_11_7",
            "Spawn_Team1_11_8",
            "Spawn_Team1_11_9",
            "Spawn_Team1_11_10",
            "Spawn_Team1_11_11",
        };

        private static string[][] s_Spawn_Team0 = new string[][]
        {
            s_Spawn_Team0_0,
            s_Spawn_Team0_1,
            s_Spawn_Team0_2,
            s_Spawn_Team0_3,
            s_Spawn_Team0_4,
            s_Spawn_Team0_5,
            s_Spawn_Team0_6,
            s_Spawn_Team0_7,
            s_Spawn_Team0_8,
            s_Spawn_Team0_9,
            s_Spawn_Team0_10,
            s_Spawn_Team0_11,
        };

        private static string[][] s_Spawn_Team1 = new string[][]
        {
            s_Spawn_Team1_0,
            s_Spawn_Team1_1,
            s_Spawn_Team1_2,
            s_Spawn_Team1_3,
            s_Spawn_Team1_4,
            s_Spawn_Team1_5,
            s_Spawn_Team1_6,
            s_Spawn_Team1_7,
            s_Spawn_Team1_8,
            s_Spawn_Team1_9,
            s_Spawn_Team1_10,
            s_Spawn_Team1_11,
        };

        public static string[][][] s_DefaultSpawnPoints = new string[][][]
        {
            s_Spawn_Team0,
            s_Spawn_Team1,
        };

        public static string[] GetSpawnPoints(int i_TeamIndex, int i_TeamSize)
        {
            if (i_TeamIndex < 0 || i_TeamIndex >= s_DefaultSpawnPoints.Length)
            {
                return null;
            }

            string[][] spawnPoints = s_DefaultSpawnPoints[i_TeamIndex];

            if (i_TeamSize < 0 || i_TeamSize >= spawnPoints.Length)
            {
                return null;
            }

            return spawnPoints[i_TeamSize];
        }
    }

    public static class BehaviourSortOrder
    {
        public static int s_SortOrder_SlowMotionController = 1;

        public static int s_SortOrder_CharacterController = 100;
        public static int s_SortOrder_SubbuteoController = 101;

        public static int s_SortOrder_Kick = 200;
        public static int s_SortOrder_Kickable = 201;
        public static int s_SortOrder_Attract = 202;
        public static int s_SortOrder_Taunt = 300;
        public static int s_SortOrder_Shake = 400;
        public static int s_SortOrder_Ball = 500;
        public static int s_SortOrder_BouncingPlatform = 600;
        public static int s_SortOrder_Hole = 601;
        public static int s_SortOrder_HoleTarget = 602;
        public static int s_SortOrder_Goal = 700;
        public static int s_SortOrder_Respawn = 800;
        public static int s_SortOrder_Energy = 900;

        public static int s_SortOrder_CharacterStats = 1100;
        public static int s_SortOrder_MatchController = 2000;
    }

    public static class BuildInfo
    {
        public static string s_ExecutableName_Win32 = "CapRiders_x86";
        public static string s_ExecutableName_Win64 = "CapRiders_x64";

		public static string s_BuildName_Mac_Intelx86 = "CapRiders_x86";
		public static string s_BuildName_Mac_Intelx64 = "CapRiders_x64";
		public static string s_BuildName_Mac_Universal = "CapRiders_Universal";

#if PHOTON

        public static string s_PhotonAppId_Debug = "c33687bb-c3cc-44e2-a668-c687fb45aaa9";
        public static string s_PhotonAppId_Test = "60076ec5-df51-4611-ac5e-32e5ec3a212e";
        public static string s_PhotonAppId_Release = "9ab04a82-e02b-4230-a180-12c34938a804";

#endif // PHOTON
    }

#if PHOTON

    public static class PhotonPropertyKey
    {
        // Room.

        public static string s_RoomCustomPropertyKey_MatchStatus = "match_status";

        public static string s_RoomCustomPropertyKey_GameMode = "game_mode";
        public static string s_RoomCustomPropertyKey_MatchDuration = "match_duration";
        public static string s_RoomCustomPropertyKey_GoldenGoal = "golden_goal";
        public static string s_RoomCustomPropertyKey_Referee = "referee";
        public static string s_RoomCustomPropertyKey_Ball = "ball";
        public static string s_RoomCustomPropertyKey_Stadium = "stadium";

        public static string s_RoomCustomPropertyKey_AssignedIndices = "assigned_player_indices";
        public static string s_RoomCustomPropertyKey_PlayerSides = "selected_player_sides";

        public static string s_RoomCustomPropertyKey_HostName = "host_name";
        public static string s_RoomCustomPropertyKey_PlayerCount = "player_count";

        public static string s_RoomCustomPropertyKey_TeamSelectionStartTime = "team_selection_start_time";
        public static string s_RoomCustomPropertyKey_CharacterSelectionStartTime = "character_selection_start_time";

        public static string s_RoomCustomPropertyKey_AvgPing = "avg_ping";

        public static string s_RoomCustomPropertyKey_GameFinishedStartTimeAlreadySet = "game_finished_start_time_Already_Set";
        public static string s_RoomCustomPropertyKey_GameFinishedStartTime = "game_finished_start_time";

        // Player.

        public static string s_PlayerCustomPropertyKey_Ping = "ping";

        public static string s_PlayerCustomPropertyKey_Joined = "room_joined";
        public static string s_PlayerCustomPropertyKey_LocalPartySize = "local_party_size";
        public static string s_PlayerCustomPropertyKey_SideSelection_Ready = "side_selection_ready";
    }

#endif // PHOTON

#if PHOTON_TRUE_SYNC

    public static class TrueSyncInputKey
    {
        // Common

        public static int s_HorizontalAxis = 0;
        public static int s_VerticalAxis = 1;

        public static int s_Taunt_ButtonDown = 2;

        // Standard

        public static int s_Standard_CharacterController_DashRequested = 3;

        public static int s_Standard_Kick_KickRequested = 4;

        public static int s_Standard_Attract_ButtonDown = 5;
        public static int s_Standard_Attract_ButtonPressed = 6;

        // Subbuteo

        public static int s_Subbuteo_CharacterController_ButtonPressed = 3;
    }

#endif // PHOTON_TRUE_SYNC
}