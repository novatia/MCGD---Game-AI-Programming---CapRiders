using UnityEngine;

public class tnCharacterViewController : MonoBehaviour
{
    [SerializeField]
    private Animator m_CharacterAnimator = null;
    [SerializeField]
    private SpriteRenderer m_Base = null;
    [SerializeField]
    private SpriteRenderer m_Flag = null;
    [SerializeField]
    private SpriteRenderer m_PlayerColor = null;
    [SerializeField]
    private tnUIPlayerName m_PlayerName = null;
    [SerializeField]
    private tnUIPlayerArrow m_PlayerArrow = null;
    [SerializeField]
    private tnUIEnergyBar m_EnergyBar = null;
    [SerializeField]
    private tnUIChargingForceBar m_ChargingForceBar = null;

    // BUSINESS LOGIC

    public void SetBaseColor(Color i_Color)
    {
        InternalSetBaseColor(i_Color);
    }

    public void SetFlagSprite(Sprite i_Sprite)
    {
        InternalSetFlagSprite(i_Sprite);
    }

    public void SetAnimatorController(RuntimeAnimatorController i_AnimatorController)
    {
        InternalSetAnimatorController(i_AnimatorController);
    }

    public void SetFacingRight(bool i_Value)
    {
        InternalSetFacingRight(i_Value);
    }

    public void TurnOffColor()
    {
        InternalTurnOffColor();
    }

    public void SetPlayerColor(Color i_Color)
    {
        InternalSetPlayerColor(i_Color);
    }

    public void SetPlayerNameVisible(bool i_Visible)
    {
        InternalSetPlayerNameVisible(i_Visible);
    }

    public void SetPlayerNameText(string i_Text)
    {
        InternalSetPlayerNameText(i_Text);
    }

    public void SetArrowVisible(bool i_Visible)
    {
        InternalSetArrowVisible(i_Visible);
    }

    public void SetArrowColor(Color i_Color)
    {
        InternalSetArrowColor(i_Color);
    }

    public void SetEnergyBarVisible(bool i_Visible)
    {
        InternalSetEnergyBarVisible(i_Visible);
    }

    public void SetEnergyBarColor(Color i_Color)
    {
        InternalSetEnergyBarColor(i_Color);
    }

    public void SetChargingForceBarVisible(bool i_Visible)
    {
        InternalSetChargingForceBarVisible(i_Visible);
    }

    public void SetChargingForceBarColor(Color i_Color)
    {
        InternalSetChargingForceBarColor(i_Color);
    }

    public void Clear()
    {
        InternalClear();
    }

    // INTERNALS

    private void InternalSetBaseColor(Color i_Color)
    {
        if (m_Base != null)
        {
            m_Base.color = i_Color;
        }
    }

    private void InternalSetFlagSprite(Sprite i_Sprite)
    {
        if (m_Flag != null)
        {
            m_Flag.sprite = i_Sprite;
        }
    }

    private void InternalSetAnimatorController(RuntimeAnimatorController i_AnimatorController)
    {
        if (m_CharacterAnimator != null)
        {
            m_CharacterAnimator.runtimeAnimatorController = i_AnimatorController;

            for (int layerIndex = 0; layerIndex < m_CharacterAnimator.layerCount; ++layerIndex)
            {
                m_CharacterAnimator.SetLayerWeight(layerIndex, 1f);
            }
        }
    }

    private void InternalSetFacingRight(bool i_Value)
    {
        if (m_CharacterAnimator != null)
        {
            m_CharacterAnimator.SetBool("FacingRight", i_Value);
        }
    }

    private void InternalTurnOffColor()
    {
        if (m_PlayerColor != null)
        {
            m_PlayerColor.color = Color.white;
            m_PlayerColor.enabled = false;
        }
    }

    private void InternalSetPlayerColor(Color i_Color)
    {
        if (m_PlayerColor != null)
        {
            m_PlayerColor.enabled = true;
            m_PlayerColor.color = i_Color;
        }
    }

    private void InternalSetPlayerNameVisible(bool i_Visible)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.SetVisible(i_Visible);
        }
    }

    private void InternalSetPlayerNameText(string i_Name)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.SetText(i_Name);
        }
    }

    private void InternalSetArrowVisible(bool i_Visible)
    {
        if (m_PlayerArrow != null)
        {
            m_PlayerArrow.SetVisible(i_Visible);
        }
    }

    private void InternalSetArrowColor(Color i_Color)
    {
        if (m_PlayerArrow != null)
        {
            m_PlayerArrow.SetColor(i_Color);
        }
    }

    private void InternalSetEnergyBarVisible(bool i_Visible)
    {
        if (m_EnergyBar == null)
            return;

        m_EnergyBar.SetVisible(i_Visible);
    }

    private void InternalSetEnergyBarColor(Color i_Color)
    {
        if (m_EnergyBar == null)
            return;

        m_EnergyBar.SetColor(i_Color);
    }

    private void InternalSetChargingForceBarVisible(bool i_Visible)
    {
        if (m_ChargingForceBar == null)
            return;

        m_ChargingForceBar.SetVisible(i_Visible);
    }

    private void InternalSetChargingForceBarColor(Color i_Color)
    {
        if (m_ChargingForceBar == null)
            return;

        m_ChargingForceBar.SetColor(i_Color);
    }

    private void InternalClear()
    {
        InternalSetFlagSprite(null);
        InternalSetFacingRight(true);
        InternalSetAnimatorController(null);

        InternalTurnOffColor();

        InternalSetArrowColor(Color.white);
        InternalSetArrowVisible(false);

        InternalSetEnergyBarColor(Color.white);
        InternalSetChargingForceBarColor(Color.white);
    }
}
