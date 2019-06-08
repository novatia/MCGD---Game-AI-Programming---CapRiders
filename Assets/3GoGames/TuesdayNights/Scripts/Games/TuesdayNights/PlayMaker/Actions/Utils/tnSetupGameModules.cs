using UnityEngine;

using System.Collections.Generic;

using TuesdayNights;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Setup Modules with a specific GameMode default values.")]
    public class tnSetupGameModules : FsmStateAction
    {
        [RequiredField]
        public FsmString gameMode;

        public FsmBool forceTeamSize;

        public FsmInt forcedMinTeamSize;
        public FsmInt forcedMaxTeamSize;

        public override void Reset()
        {
            gameMode = "";

            forceTeamSize = false;

            forcedMinTeamSize = -1;
            forcedMaxTeamSize = -1;
        }

        public override void OnEnter()
        {
            SetupTeamsModule();
            SetupMatchSettingsModule();

            Finish();
        }

        private void SetupTeamsModule()
        {
            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

            if (teamsModule == null)
            {
                return;
            }

            teamsModule.Clear();

            tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameMode.Value);
            if (gameModeData == null)
            {
                return;
            }

            int numTeams = Random.Range(gameModeData.teamsRange.min, gameModeData.teamsRange.max);

            int minTeamSize = gameModeData.playersPerTeamRange.min;
            int maxTeamSize = gameModeData.playersPerTeamRange.max;

            if (forceTeamSize != null && forceTeamSize.Value)
            {
                if (forcedMinTeamSize != null && forcedMinTeamSize.Value > 0)
                {
                    minTeamSize = forcedMinTeamSize.Value;
                }

                if (forcedMaxTeamSize != null && forcedMaxTeamSize.Value > 0)
                {
                    maxTeamSize = forcedMaxTeamSize.Value;
                }
            }

            int[] teamIds = SelectTeams(numTeams);

            if (teamIds != null)
            {
                int teamSize = Random.Range(minTeamSize, maxTeamSize);

                Color[] teamColors = Utils.ComputeTeamColors(teamIds);

                for (int teamIndex = 0; teamIndex < teamIds.Length; ++teamIndex)
                {
                    int teamId = teamIds[teamIndex];
                    Color teamColor = teamColors[teamIndex];

                    tnTeamDescription teamDescription = CreateTeam(teamId, teamSize, teamColor);
                    teamsModule.AddTeamDescription(teamDescription);
                }
            }
        }

        private void SetupMatchSettingsModule()
        {
            tnMatchSettingsModule module = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

            if (module == null)
            {
                return;
            }

            module.Clear();

            // Game mode.

            {
                module.SetGameModeId(gameMode.Value);
            }

            // Stadium.

            {
                int stadiumKey = -1;
                GetRandomStadiumKey(gameMode.Value, out stadiumKey);

                module.SetStadiumId(stadiumKey);
            }

            // Ball.

            {
                int ballKey = -1;
                GetRandomBallKey(out ballKey);

                module.SetBallId(ballKey);
            }

            // Ai level.

            {
                module.SetAILevelIndex(tnGameData.aiLevelCountMain - 1);
            }

            // Options.

            {
                tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameMode.Value);
                if (gameModeData != null)
                {
                    tnGameModeConfig gameModeConfig = tnGameData.GetConfigDataMain(gameModeData.optionsConfigId);
                    if (gameModeConfig != null)
                    {
                        module.SetMatchDurationOption(gameModeConfig.matchDurationOption);
                        module.SetRefereeOption(gameModeConfig.refereeOption);
                    }
                }
            }
        }

        private int[] SelectTeams(int i_NumTeams)
        {
            List<int> teamKeys = tnGameData.GetTeamsKeysMain();

            List<int> teamIdsList = new List<int>();

            if (i_NumTeams >= teamKeys.Count)
            {
                // You've requested to many teams. Extract them randomly.

                for (int j = 0; j < i_NumTeams; ++j)
                {
                    int teamIndex = Random.Range(0, teamKeys.Count);
                    int teamId = teamKeys[teamIndex];

                    teamIdsList.Add(teamId);
                }
            }
            else
            {
                // Extract i_NumTeams different teams.

                for (int j = teamKeys.Count - i_NumTeams; j < teamKeys.Count; ++j)
                {
                    int teamIndex = Random.Range(0, j + 1);
                    int teamId = teamKeys[teamIndex];

                    if (teamIdsList.Contains(teamId))
                    {
                        teamIdsList.Add(teamKeys[j]);
                    }
                    else
                    {
                        teamIdsList.Add(teamId);
                    }
                }
            }

            int[] teamIds = teamIdsList.ToArray();
            return teamIds;
        }

        private void GetRandomStadiumKey(string i_GameModeId, out int o_StadiumKey)
        {
            List<int> allowedStadiumKeys = new List<int>();

            int minTeamSize;
            int maxTeamSize;
            GetTeamSize(out minTeamSize, out maxTeamSize);

            List<int> stadiyumKeys = tnGameData.GetStadiumsKeysMain();

            tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(i_GameModeId);

            for (int stadiumIndex = 0; stadiumIndex < stadiyumKeys.Count; ++stadiumIndex)
            {
                int currentStadiumKey = stadiyumKeys[stadiumIndex];

                tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(currentStadiumKey);

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

                IntRange allowedTeamSize = stadiumData.teamSize;

                bool locked = excludedByTag || !allowedTeamSize.IsValueValid(maxTeamSize) || !allowedTeamSize.IsValueValid(minTeamSize);
                if (!locked)
                {
                    allowedStadiumKeys.Add(currentStadiumKey);
                }
            }

            int stadiumKey = Hash.s_NULL;

            if (allowedStadiumKeys.Count > 0)
            {
                int randomIndex = Random.Range(0, allowedStadiumKeys.Count);
                stadiumKey = allowedStadiumKeys[randomIndex];
            }

            o_StadiumKey = stadiumKey;
        }

        private void GetRandomBallKey(out int o_BallKey)
        {
            List<int> ballKeys = tnGameData.GetBallsKeysMain();

            int randomIndex = Random.Range(0, ballKeys.Count);
            o_BallKey = ballKeys[randomIndex];
        }

        private void GetTeamSize(out int o_MinSize, out int o_MaxSize)
        {
            int minSize = int.MaxValue;
            int maxSize = int.MinValue;

            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

            if (teamsModule != null)
            {
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

                    if (teamDescription != null)
                    {
                        minSize = Mathf.Min(minSize, teamDescription.charactersCount);
                        maxSize = Mathf.Max(maxSize, teamDescription.charactersCount);
                    }
                }
            }

            o_MinSize = minSize;
            o_MaxSize = maxSize;
        }

        private tnTeamDescription CreateTeam(int i_TeamKey, int i_TeamSize, Color i_TeamColor)
        {
            tnTeamData teamData = tnGameData.GetTeamDataMain(i_TeamKey);

            if (teamData == null || i_TeamSize <= 0)
            {
                return null;
            }

            tnTeamDescription teamDescription = new tnTeamDescription();
            teamDescription.SetTeamId(i_TeamKey);
            teamDescription.SetTeamColor(i_TeamColor);

            List<int> teamLineup = teamData.GetDefaultLineUp(i_TeamSize);

            for (int characterIndex = 0; characterIndex < teamLineup.Count; ++characterIndex)
            {
                int characterKey = teamLineup[characterIndex];

                tnCharacterDescription characterDescription = new tnCharacterDescription();
                characterDescription.SetCharacterId(characterKey);
                characterDescription.SetPlayerId(StringUtils.s_NULL);
                characterDescription.SetSpawnOrder(characterIndex);

                teamDescription.AddCharacterDescription(characterDescription);
            }

            return teamDescription;
        }
    }
}