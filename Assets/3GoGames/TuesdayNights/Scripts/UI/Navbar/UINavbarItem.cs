using UnityEngine;
using UnityEngine.UI;

public sealed class UINavbarItem : MonoBehaviour
{
    [SerializeField]
    private Image m_Icon = null;
    [SerializeField]
    private Text m_Label = null;

    private bool m_Visible = false;

    public bool isVisible
    {
        get { return m_Visible; }
        set { m_Visible = value; }
    }

    private bool m_Active = true;

    public bool isActive
    {
        get { return m_Active; }
        set { m_Active = value; }
    }

    private Animator m_Animator = null;

    private static int s_Visible = Animator.StringToHash("Visible");
    private static int s_Active = Animator.StringToHash("Active");

    // BUSINESS LOGIC

    public void SetIcon(Sprite i_Sprite)
    {
        InternalSetIcon(i_Sprite);
    }

    public void SetText(string i_Text)
    {
        InternalSetText(i_Text);
    }

    public void Clear()
    {
        SetIcon(null);
        SetText("");
    }

    // INTERNALS

    private void InternalSetIcon(Sprite i_Sprite)
    {
        if (m_Icon != null)
        {
            m_Icon.sprite = i_Sprite;
        }
    }

    private void InternalSetIconColor(Color i_Color)
    {
        if (m_Icon != null)
        {
            m_Icon.color = i_Color;
        }
    }

    private void InternalSetText(string i_Text)
    {
        if (m_Label != null)
        {
            m_Label.text = i_Text;
        }
    }

    private void InternalSetTextColor(Color i_Color)
    {
        if (m_Label != null)
        {
            m_Label.color = i_Color;
        }
    }

    private void InternalSetIconEnabled(bool i_Enabled)
    {
        if (m_Icon != null)
        {
            m_Icon.enabled = i_Enabled;
        }
    }

    private void InternalSetTextEnabled(bool i_Enabled)
    {
        if (m_Label != null)
        {
            m_Label.enabled = i_Enabled;
        }
    }

    private void InternalForceVisible(bool i_Visible)
    {
        InternalSetIconEnabled(i_Visible);
        InternalSetTextEnabled(i_Visible);
    }

    private void InternaForceActive(bool i_Active)
    {
        InternalSetIconColor(i_Active ? Color.white : Color.gray);
        InternalSetTextColor(i_Active ? Color.white : Color.gray);
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool(s_Visible, m_Visible);
            m_Animator.SetBool(s_Active, m_Active);
        }
        else // Update without animator.
        {
            bool isVisible = m_Visible;
            bool isActive = m_Visible;

            InternalForceVisible(isVisible);
            InternaForceActive(isActive);
        }
    }
}
