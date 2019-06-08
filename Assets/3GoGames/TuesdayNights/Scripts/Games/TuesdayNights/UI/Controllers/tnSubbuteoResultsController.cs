using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class tnSubbuteoResultsController : tnBaseMatchResultsController
{
    [SerializeField]
    private Image m_Team0Flag = null;
    [SerializeField]
    private Image m_Team1Flag = null;

    [SerializeField]
    private Text m_Team0Score = null;
    [SerializeField]
    private Text m_Team1Score = null;

    [SerializeField]
    private Text m_Team0Name = null;
    [SerializeField]
    private Text m_Team1Name = null;

    // tnMatchResultsController's INTERFACE

    protected override void ShowResults(tnMatchController i_Controller)
    {
        base.ShowResults(i_Controller);

        if (i_Controller == null)
            return;

        tnSubbuteoMatchController matchController = (tnSubbuteoMatchController)i_Controller;
        InternalSetResults(matchController);
    }

    // INTERNALS

    private void InternalSetResults(tnSubbuteoMatchController i_MathController)
    {
        if (i_MathController == null)
            return;

        // Team 0

        tnSubbuteoMatchTeamResults teamResults0 = (tnSubbuteoMatchTeamResults)i_MathController.GetTeamResultsByIndex(0);

        if (teamResults0 != null)
        {
            int teamId0 = teamResults0.id;
            tnTeamData teamData0 = tnGameData.GetTeamDataMain(teamId0);

            if (m_Team0Flag != null)
            {
                m_Team0Flag.sprite = teamData0.flag;
            }

            if (m_Team0Name != null)
            {
                m_Team0Name.text = teamData0.name;
            }

            if (m_Team0Score != null)
            {
                m_Team0Score.text = teamResults0.score.ToString();
            }
        }

        // Team 1

        tnSubbuteoMatchTeamResults teamResults1 = (tnSubbuteoMatchTeamResults)i_MathController.GetTeamResultsByIndex(1);

        if (teamResults1 != null)
        {
            int teamId1 = teamResults1.id;
            tnTeamData teamData1 = tnGameData.GetTeamDataMain(teamId1);

            if (m_Team1Flag != null)
            {
                m_Team1Flag.sprite = teamData1.flag;
            }

            if (m_Team1Name != null)
            {
                m_Team1Name.text = teamData1.name;
            }

            if (m_Team1Score != null)
            {
                m_Team1Score.text = teamResults1.score.ToString();
            }
        }
    }
}
