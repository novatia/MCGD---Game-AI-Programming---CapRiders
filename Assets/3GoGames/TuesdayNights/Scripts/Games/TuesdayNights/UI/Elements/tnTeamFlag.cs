using UnityEngine;
using UnityEngine.UI;

using System.Collections;

[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Animator))]
public class tnTeamFlag : MonoBehaviour
{
    public enum SelectionStatus
    {
        None,
        Available,
        Selected,
        Highlighted,
    }

    [SerializeField]
    private Image m_Image = null;
    [SerializeField]
    private Image m_Highlight = null;
    [SerializeField]
    private Image m_Overlay = null;
    [SerializeField]
    private Text m_Label = null;

    private Selectable m_Selectable = null;
    private Animator m_Animator = null;

    private int m_TeamId = Hash.s_NULL;

    private SelectionStatus m_Status = SelectionStatus.None;

    private static int s_Normal = Animator.StringToHash("Normal");
    private static int s_Highlighted = Animator.StringToHash("Highlighted");
    private static int s_Selected = Animator.StringToHash("Selected");

    // ACCESSORS

    public int teamId
    {
        get
        {
            return m_TeamId;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_Selectable = GetComponent<Selectable>();
        m_Selectable.transition = Selectable.Transition.None;

        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Automatic;
        m_Selectable.navigation = navigation; 
    }

    void Update()
    {
        {
            bool normal = (m_Status == SelectionStatus.Available);
            m_Animator.SetBool(s_Normal, normal);

            bool highlighted = (m_Status == SelectionStatus.Highlighted);
            m_Animator.SetBool(s_Highlighted, highlighted);

            bool selected = (m_Status == SelectionStatus.Selected);
            m_Animator.SetBool(s_Selected, selected);
        }
    }

    // BUSINESS LOGIC

    public void SetTeamId(int i_TeamId)
    {
        m_TeamId = i_TeamId;
    }

    public void SetImage(Sprite i_Image)
    {
        if (m_Image == null)
        {
            return;
        }

        m_Image.sprite = i_Image;
    }

    public void SetLabel(string i_Label)
    {
        if (m_Label == null)
        {
            return;
        }

        m_Label.text = i_Label;
    }

    public void SetColor(Color i_Color)
    {
        if (m_Status == SelectionStatus.None)
            return;

        if (m_Status == SelectionStatus.Highlighted || m_Status == SelectionStatus.Selected)
        {
            SetHighlightColor(i_Color);
            SetOverlayColor(i_Color);
        }
    }

    public bool isAvailable
    {
        get
        {
            return (m_Status == SelectionStatus.Available);
        }
    }

    public bool isHighlighted
    {
        get
        {
            return (m_Status == SelectionStatus.Highlighted);
        }
    }

    public bool isSelected
    {
        get
        {
            return (m_Status == SelectionStatus.Selected);
        }
    }

    public void SetAvailable()
    {
        // Update status.

        m_Status = SelectionStatus.Available;

        // Update appearence.

        SetHighlightColor(Color.white);
        SetOverlayColor(Color.white);

        SetOverlayAlpha(0f);
    }

    public void SetHighlighted(Color i_Color)
    {
        // Update status.

        m_Status = SelectionStatus.Highlighted;

        // Update appearence.

        SetHighlightColor(i_Color);
        SetOverlayColor(i_Color);

        SetOverlayAlpha(0f);
    }

    public void SetSelected(Color i_Color)
    {
        // Update status.

        m_Status = SelectionStatus.Selected;

        // Update appearence.

        SetHighlightColor(i_Color);
        SetOverlayColor(i_Color);

        SetOverlayAlpha(0.5f);
    }

    // INTERNALS

    private void SetHighlightColor(Color i_Color)
    {
        if (m_Highlight == null)
        {
            return;
        }

        m_Highlight.color = i_Color;
    }

    private void SetOverlayColor(Color i_Color)
    {
        if (m_Overlay == null)
        {
            return;
        }

        m_Overlay.SetColorWithoutAlpha(i_Color);
    }

    public void SetOverlayAlpha(float i_Alpha)
    {
        if (m_Overlay == null)
        {
            return;
        }

        m_Overlay.SetColorAlpha(i_Alpha);
    }
}
