using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using GoUI;

public class tnView_SubbuteoMatchResults : GoUI.UIView
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

    // UIView's interface

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

    public void SetTeam0(string i_TeamName, int i_TeamScore, Sprite i_TeamFlag)
    {
        SetTeam0Name(i_TeamName);
        SetTeam0Score(i_TeamScore);
        SetTeam0Sprite(i_TeamFlag);
    }

    public void SetTeam1(string i_TeamName, int i_TeamScore, Sprite i_TeamFlag)
    {
        SetTeam1Name(i_TeamName);
        SetTeam1Score(i_TeamScore);
        SetTeam1Sprite(i_TeamFlag);
    }

    // INTERNALS

    private void SetTeam0Name(string i_TeamName)
    {
        if (m_Team0Name != null)
        {
            m_Team0Name.text = i_TeamName;
        }
    }

    private void SetTeam1Name(string i_TeamName)
    {
        if (m_Team1Name != null)
        {
            m_Team1Name.text = i_TeamName;
        }
    }

    private void SetTeam0Score(int i_TeamScore)
    {
        if (m_Team0Score != null)
        {
            m_Team0Score.text = i_TeamScore.ToString();
        }
    }

    private void SetTeam1Score(int i_TeamScore)
    {
        if (m_Team1Score != null)
        {
            m_Team1Score.text = i_TeamScore.ToString();
        }
    }

    private void SetTeam0Sprite(Sprite i_TeamSprite)
    {
        if (m_Team0Flag != null)
        {
            m_Team0Flag.sprite = i_TeamSprite;
        }
    }

    private void SetTeam1Sprite(Sprite i_TeamSprite)
    {
        if (m_Team1Flag != null)
        {
            m_Team1Flag.sprite = i_TeamSprite;
        }
    }
}