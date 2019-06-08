using UnityEngine;
using UnityEngine.EventSystems;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Setup InputModule for menu context.")]
    public class tnSetupInputModuleMenu : FsmStateAction
    {
        public override void OnEnter()
        {
            // Clear input module.

            ClearInputModule();

            // Process teams.

            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
            if (teamsModule != null)
            {
                for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
                {
                    tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);
                    ProcessTeam(teamDescription);
                }
            }

            Finish();
        }

        private void ClearInputModule()
        {
            InputModule inputModule = UIEventSystem.inputModuleMain;
            if (inputModule == null)
            {
                return;
            }

            inputModule.Clear();
        }

        private void ProcessTeam(tnTeamDescription i_TeamDescription)
        {
            if (i_TeamDescription == null)
            {
                return;
            }

            InputModule inputModule = UIEventSystem.inputModuleMain;
            if (inputModule == null)
            {
                return;
            }

            tnCharacterDescription characterDescription = i_TeamDescription.GetCharacterDescription(0);
            if (characterDescription != null)
            {
                int playerId = characterDescription.playerId;
                tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

                if (playerData != null)
                {
                    string playerInputName = playerData.playerInputName;
                    string wifiPlayerInputName = playerData.wifiPlayerInputName;

                    if (!StringUtils.IsNullOrEmpty(playerInputName))
                    {
                        inputModule.AddPlayer(playerInputName);
                    }
                    else
                    {
                        if (!StringUtils.IsNullOrEmpty(wifiPlayerInputName))
                        {
                            inputModule.AddWifiPlayer(wifiPlayerInputName);
                        }
                    }
                }
            }
        }
    }
}