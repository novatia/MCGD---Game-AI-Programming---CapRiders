using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using GoUI;

public class tnView_MatchInfo : GoUI.UIView
{
    [SerializeField]
    private Image m_StadiumImage = null;
    [SerializeField]
    private Text m_StadiumName = null;
    [SerializeField]
    private Text m_StadiumMinPlayers = null;

    [SerializeField]
    private Text m_GameMode = null;
    [SerializeField]
    private Text m_OtherSettings = null;

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

    public void SetStadiumImage(Sprite i_Sprite)
    {
        if (m_StadiumImage == null)
            return;

        m_StadiumImage.sprite = i_Sprite;
    }

    public void SetStadiumName(string i_Name)
    {
        if (m_StadiumName == null)
            return;

        m_StadiumName.text = i_Name;
    }

    public void SetStadiumMinPlayers(int i_MinPlayers)
    {
        if (m_StadiumMinPlayers == null)
            return;

        m_StadiumMinPlayers.text = "min " + i_MinPlayers + " players";
    }

    public void SetGameMode(string i_GameMode)
    {
        if (m_GameMode == null)
            return;

        m_GameMode.text = i_GameMode;
    }

    public void SetOtherSettings(bool i_GoldenGoal, bool i_Referee, float i_MatchDuration)
    {
        if (m_OtherSettings == null)
            return;

        string otherSettings = (i_GoldenGoal == false) ? "no " : "";
        otherSettings += "golden goal, ";
        otherSettings += (i_Referee == true) ? "referee, " : "";
        otherSettings += TimeUtils.TimeToString(i_MatchDuration, true, true);

        m_OtherSettings.text = otherSettings;
    }
}
