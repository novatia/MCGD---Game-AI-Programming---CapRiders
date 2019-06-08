using UnityEngine;

using GoUI;

public class tnPanel_StandardMatchResults : UIPanel<tnView_StandardMatchResults>
{
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

    public void Config(tnStandardMatchController i_Controller)
    {
        if(viewInstance != null)
        {
            if (i_Controller == null)
                return;

            // Team 0

            tnStandardMatchTeamResults teamResults0 = (tnStandardMatchTeamResults)i_Controller.GetTeamResultsByIndex(0);
            if (teamResults0 != null)
            {
                int teamId0 = teamResults0.id;
                tnTeamData teamData0 = tnGameData.GetTeamDataMain(teamId0);

                string name = teamData0.name;
                int score = teamResults0.score;
                Sprite flag = teamData0.flag;

                viewInstance.SetTeam0(name, score, flag);
            }

            // Team 1

            tnStandardMatchTeamResults teamResults1 = (tnStandardMatchTeamResults)i_Controller.GetTeamResultsByIndex(1);
            if (teamResults1 != null)
            {
                int teamId1 = teamResults1.id;
                tnTeamData teamData1 = tnGameData.GetTeamDataMain(teamId1);

                string name = teamData1.name;
                int score = teamResults1.score;
                Sprite flag = teamData1.flag;

                viewInstance.SetTeam1(name, score, flag);
            }
        }
    }
}
