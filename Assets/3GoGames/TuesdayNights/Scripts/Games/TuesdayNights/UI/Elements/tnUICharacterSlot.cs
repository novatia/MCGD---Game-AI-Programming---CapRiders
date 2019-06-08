using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class tnUICharacterSlot : MonoBehaviour
{
    private int m_CharacterId = Hash.s_NULL;

    private tnUICharacter m_Character = null;

    private bool m_Selected = false;

    private bool m_Highlighted = false;
    private Color m_HighlightColor = Color.white;

    private bool m_HasPlayerColor = false;
    private Color m_PlayerColor = Color.white;

    private Selectable m_Selectable = null;

    public int characterId
    {
        get { return m_CharacterId; }
        set { m_CharacterId = value; }
    }

    public tnUICharacter character
    {
        get { return m_Character; }
        set
        {
            m_Character = value;

            if (m_Character != null)
            {
                m_Character.transform.position = transform.position;

                if (m_Highlighted)
                {
                    m_Character.SetHighlighted(m_HighlightColor);
                }
                else
                {
                    m_Character.SetAvailable();
                }

                if (m_Selected)
                {
                    m_Character.Select();
                }
                else
                {
                    m_Character.Deselect();
                }

                if (m_HasPlayerColor)
                {
                    m_Character.SetPlayerColor(m_PlayerColor);
                }
                else
                {
                    m_Character.ClearPlayerColor();
                }
            }
        }
    }

    public bool isSelected
    {
        get { return m_Selected; }
    }

    public bool isHighlighted
    {
        get { return m_Highlighted; }
    }

    public bool hasPlayerColor
    {
        get { return m_HasPlayerColor; }
    }

    public Color playerColor
    {
        get { return m_PlayerColor; }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_Selectable = GetComponent<Selectable>();
        m_Selectable.transition = Selectable.Transition.None;

        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Automatic;
        m_Selectable.navigation = navigation;
    }

    // LOGIC

    public void SetHighlighted(Color i_Color)
    {
        if (m_Character != null)
        {
            m_Character.SetHighlighted(i_Color);
        }

        m_HighlightColor = i_Color;
        m_Highlighted = true;
    }

    public void SetAvailable()
    {
        if (m_Character != null)
        {
            m_Character.SetAvailable();
        }

        m_HighlightColor = Color.white;
        m_Highlighted = false;
    }

    public void Select()
    {
        if (m_Character != null)
        {
            m_Character.Select();
        }

        m_Selected = true;
    }

    public void Deselect()
    {
        if (m_Character != null)
        {
            m_Character.Deselect();
        }

        m_Selected = false;
    }

    public void SetPlayerColor(Color i_Color)
    {
        if (m_Character != null)
        {
            m_Character.SetPlayerColor(i_Color);
        }

        m_HasPlayerColor = true;
        m_PlayerColor = i_Color;
    }

    public void ClearPlayerColor()
    {
        if (m_Character != null)
        {
            m_Character.ClearPlayerColor();
        }

        m_HasPlayerColor = false;
        m_PlayerColor = Color.white;
    }

    public void Clear()
    {
        m_CharacterId = Hash.s_NULL;

        m_Character = null;

        m_Selected = false;

        m_Highlighted = false;
        m_HighlightColor = Color.white;

        m_HasPlayerColor = false;
        m_PlayerColor = Color.white;
    }
}
