using UnityEngine;
using System.Collections;

using GoUI;

public class tnPanel_MatchInfo : UIPanel<tnView_MatchInfo>
{
    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        ShowInfo();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // INTERNALS

    private void ShowInfo()
    {
        tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (matchSettingsModule == null || viewInstance == null)
            return;

        // stadium

        int stadiumId = matchSettingsModule.stadiumId;
        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

        if (stadiumData != null)
        {
            viewInstance.SetStadiumImage(stadiumData.icon);
            viewInstance.SetStadiumName(stadiumData.name);
            viewInstance.SetStadiumMinPlayers(stadiumData.onlineTeamSize.min * 2);
        }

        // game mode

        int gameMode = matchSettingsModule.gameModeId;
        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameMode);

        if(gameModeData != null)
        {
            viewInstance.SetGameMode(gameModeData.name);
        }

        // golden goal, referee, match duration

        string goldengol;
        tnGameData.TryGetGoldenGoalValueMain(matchSettingsModule.goldenGoalOption, out goldengol);

        string referee;
        tnGameData.TryGetGoldenGoalValueMain(matchSettingsModule.refereeOption, out referee);

        float matchDuration;
        tnGameData.TryGetMatchDurationValueMain(matchSettingsModule.matchDurationOption, out matchDuration);

        viewInstance.SetOtherSettings(goldengol == "ON", referee == "ON", matchDuration);
    }
}
