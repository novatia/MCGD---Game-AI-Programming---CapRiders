using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class tnStatsPanel : MonoBehaviour
{
    [SerializeField]
    private Text m_PlayerName = null;

    [SerializeField]
    private Image m_Portrait = null;

    [SerializeField]
    private Text m_StatLabel = null;
    [SerializeField]
    private Text m_StatValue = null;

    [SerializeField]
    private Text m_PartecipationLabel = null;
    [SerializeField]
    private Text m_PartecipationValue = null;

    // BUSINESS LOGIC

    public void SetPlayerName(string i_Name)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.text = i_Name;
        }
    }

    public void SetPortrait(Sprite i_Portrait)
    {
        if (m_Portrait != null)
        {
            m_Portrait.sprite = i_Portrait;
        }
    }

    public void SetStatLabel(string i_Label)
    {
        if (m_StatLabel != null)
        {
            m_StatLabel.text = i_Label;
        }
    }

    public void SetStatValue(string i_Value)
    {
        if (m_StatLabel != null)
        {
            m_StatValue.text = i_Value;
        }
    }

    public void SetPartecipationLabel(string i_Label)
    {
        if (m_PartecipationLabel != null)
        {
            m_PartecipationLabel.text = i_Label;
        }
    }

    public void SetPartecipationValue(string i_Value)
    {
        if (m_PartecipationValue != null)
        {
            m_PartecipationValue.text = i_Value;
        }
    }

    public void SetTeamColor(Color i_Color)
    {
        InternalSetPlayerNameColor(i_Color);
        InternalSetStatValueColor(i_Color);
        InternalSetPartecipationValueColor(i_Color);
    }

    // INTERNALS

    private void InternalSetPlayerNameColor(Color i_Color)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.color = i_Color;
        }
    }

    private void InternalSetStatValueColor(Color i_Color)
    {
        if (m_StatValue != null)
        {
            m_StatValue.color = i_Color;
        }
    }

    private void InternalSetPartecipationValueColor(Color i_Color)
    {
        if (m_PartecipationValue != null)
        {
            m_PartecipationValue.color = i_Color;
        }
    }
}
