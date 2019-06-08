using UnityEngine;
using UnityEngine.UI;

public class tnUIStandardScorePanel : MonoBehaviour
{
    [SerializeField]
    private Text m_Timer = null;

    [SerializeField]
    private Image m_ImageTeam0 = null;
    [SerializeField]
    private Image m_ImageTeam1 = null;

    [SerializeField]
    private Text m_ScoreTeam0 = null;
    [SerializeField]
    private Text m_ScoreTeam1 = null;

    private Animator m_Animator = null;

    private tnStandardMatchController m_MatchController = null;

    public void Bind(tnStandardMatchController i_MatchController)
    {
        m_MatchController = i_MatchController;

        // Team 0 - Flag.

        int team0Id = m_MatchController.GetTeamId(0);

        tnTeamData team0Data = tnGameData.GetTeamDataMain(team0Id);
        if (team0Data != null)
        {
            SetFlagTeam0(team0Data.icon);
        }

        // Team 1 - Flag.

        int team1Id = m_MatchController.GetTeamId(1);

        tnTeamData team1Data = tnGameData.GetTeamDataMain(team1Id);
        if (team1Data != null)
        {
            SetFlagTeam1(team1Data.icon);
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_MatchController == null)
            return;

        // Update animator parameters.

        if (m_Animator != null)
        {
            m_Animator.SetBool("GoldenGoal", m_MatchController.goldenGoal);
        }

        // Timer.

        float time = (float)m_MatchController.remainingTime;
        SetRemainingTime(time);

        // Team 0.

        tnStandardMatchTeamResults teamResults0 = (tnStandardMatchTeamResults)m_MatchController.GetTeamResultsByIndex(0);
        if (teamResults0 != null)
        {
            SetScoreTeam0(teamResults0.score);
        }

        // Team 1.

        tnStandardMatchTeamResults teamResults1 = (tnStandardMatchTeamResults)m_MatchController.GetTeamResultsByIndex(1);
        if (teamResults1 != null)
        {
            SetScoreTeam1(teamResults1.score);
        }
    }

    // INTERNALS

    private void SetRemainingTime(float i_Time)
    {
        if (m_Timer == null)
            return;

        m_Timer.text = TimeUtils.TimeToString(i_Time, true, true);
    }

    private void SetFlagTeam0(Sprite i_Flag)
    {
        if (m_ImageTeam0 == null)
            return;

        m_ImageTeam0.sprite = i_Flag;
    }

    private void SetFlagTeam1(Sprite i_Flag)
    {
        if (m_ImageTeam1 == null)
            return;

        m_ImageTeam1.sprite = i_Flag;
    }

    private void SetScoreTeam0(int i_Score)
    {
        if (m_ScoreTeam0 == null)
            return;

        int score = Mathf.Clamp(i_Score, 0, 99);
        m_ScoreTeam0.text = score.ToString();
    }

    private void SetScoreTeam1(int i_Score)
    {
        if (m_ScoreTeam1 == null)
            return;

        int score = Mathf.Clamp(i_Score, 0, 99);
        m_ScoreTeam1.text = score.ToString();
    }
}
