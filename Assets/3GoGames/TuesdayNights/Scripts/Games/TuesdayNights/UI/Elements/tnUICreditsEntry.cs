using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class tnUICreditsEntry : MonoBehaviour
{
    [SerializeField]
    private Image m_BaseImage = null;

    [SerializeField]
    private Animator m_CharacterAnimator = null;
    [SerializeField]
    private Text m_CharacterName = null;

    [SerializeField]
    private Image m_Highlight = null;

    [SerializeField]
    private Text m_Role = null;

    private int m_Index = -1;

    public int index
    {
        get
        {
            return m_Index;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {

    }

    // BUSINESS LOGIC

    public void SetIndex(int i_Index)
    {
        m_Index = i_Index;
    }

    public void SetBaseImage(Sprite i_Base)
    {
        if (m_BaseImage != null)
        {
            m_BaseImage.sprite = i_Base;
        }
    }

    public void SetCharacterAnimator(RuntimeAnimatorController i_AnimatorController)
    {
        if (m_CharacterAnimator != null)
        {
            m_CharacterAnimator.runtimeAnimatorController = i_AnimatorController;
        }
    }

    public void SetCharacterName(string i_Name)
    {
        if (m_CharacterName != null)
        {
            m_CharacterName.text = i_Name;
        }
    }

    public void SetRole(string i_Role)
    {
        if (m_Role != null)
        {
            m_Role.text = i_Role;
        }
    }

    public void SetHighlightColor(Color i_Color)
    {
        if (m_Highlight != null)
        {
            m_Highlight.color = i_Color;
        }
    }

    public void SetHighlighted(bool i_Highlighted)
    {
        if (i_Highlighted)
        {
            if (m_Highlight != null)
            {
                m_Highlight.enabled = true;
            }
        }
        else
        {
            if (m_Highlight != null)
            {
                m_Highlight.enabled = false;
            }
        }
    }
}
