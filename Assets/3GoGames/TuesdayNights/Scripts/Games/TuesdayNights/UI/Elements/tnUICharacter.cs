using UnityEngine;
using UnityEngine.UI;

public class tnUICharacter : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Graphics = null;

    [SerializeField]
    private Image m_BaseImage = null;
    [SerializeField]
    private Image m_FlagImage = null;
    [SerializeField]
    private Image m_CharacterImage = null;
    [SerializeField]
    private Text m_CharacterName = null;
    [SerializeField]
    private Image m_Highlight = null;
    [SerializeField]
    private Image m_PlayerColor = null;

    [SerializeField]
    private float m_BlinkTimeOn = 0.5f;
    [SerializeField]
    private float m_BlinkTimeOff = 0.5f;

    private float m_Timer = 0f;

    private bool m_Blinking = false;

    public bool hasPlayerColor
    {
        get
        {
            return (m_PlayerColor != null && m_PlayerColor.enabled);
        }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        Clear();
    }

    void Update()
    {
        if (!m_Blinking)
            return;

        if (m_Graphics == null || m_CharacterName == null)
            return;

        m_Timer -= Time.deltaTime;

        if (m_Timer < 0f)
        {
            m_Graphics.SetActive(!m_Graphics.activeSelf);
            m_CharacterName.enabled = !m_CharacterName.enabled;

            if (m_Graphics.activeSelf)
            {
                m_Timer = m_BlinkTimeOn;
            }
            else
            {
                m_Timer = m_BlinkTimeOff;
            }
        }
    }

    // LOGIC

    public void SetBaseColor(Color i_Color)
    {
        if (m_BaseImage != null)
        {
            m_BaseImage.color = i_Color;
        }
    }

    public void SetFlagSprite(Sprite i_Flag)
    {
        if (m_FlagImage != null)
        {
            m_FlagImage.sprite = i_Flag;
        }
    }

    public void SetCharacterSprite(Sprite i_Character)
    {
        if (m_CharacterImage != null)
        {
            m_CharacterImage.sprite = i_Character;
        }
    }

    public void SetName(string i_Name)
    {
        if (m_CharacterName != null)
        {
            m_CharacterName.text = i_Name;
        }
    }

    public void SetPlayerColor(Color i_PlayerColor)
    {
        if (m_PlayerColor != null)
        {
            m_PlayerColor.color = i_PlayerColor;
            m_PlayerColor.enabled = true;
        }
    }

    public void ClearPlayerColor()
    {
        if (m_PlayerColor != null)
        {
            m_PlayerColor.color = Color.white;
            m_PlayerColor.enabled = false;
        }
    }

    public void SetHighlighted(Color i_Color)
    {
        if (m_Highlight != null)
        {
            m_Highlight.enabled = true;
            m_Highlight.color = i_Color;
        }
    }

    public void SetAvailable()
    {
        if (m_Highlight != null)
        {
            m_Highlight.enabled = false;
            m_Highlight.color = Color.white;
        }
    }

    public void Select()
    {
        m_Blinking = true;
    }

    public void Deselect()
    {
        if (m_Graphics != null)
        {
            m_Graphics.SetActive(true);
        } 

        if (m_CharacterName != null)
        {
            m_CharacterName.enabled = true;
        }

        m_Timer = 0f;
        m_Blinking = false;
    }

    public void Clear()
    {
        SetBaseColor(Color.white);

        SetFlagSprite(null);
        SetCharacterSprite(null);

        SetName("");

        ClearPlayerColor();

        SetAvailable();

        m_Timer = 0f;
        m_Blinking = false;
    }
}
