using UnityEngine;

using WiFiInput.Server;

public delegate void SwapButtonClicked();

public class tnUISwapMenu : MonoBehaviour
{
    private static string s_PlayerInput_Up = "VerticalUp";
    private static string s_PlayerInput_Down = "VerticalDown";
    private static string s_PlayerInput_Submit = "Submit";

    private static string s_WiFiPlayerInput_Vertical = "Vertical";
    private static string s_WiFiPlayerInput_Submit = "Submit";

    [SerializeField]
    private GameObject m_SwapCharacterButton = null;
    [SerializeField]
    private GameObject m_SwapColorButton = null;

    private GameObject m_SelectedEntry = null;

    public SwapButtonClicked onSwapCharacterEvent;
    public SwapButtonClicked onSwapColorEvent;

    private static int s_SelectedBoolId = Animator.StringToHash("Selected");

    // MonoBehaviour's INTERFACE

    void OnEnable()
    {
        SetSelected(m_SwapCharacterButton, false);
        SetSelected(m_SwapColorButton, false);

        m_SelectedEntry = null;
    }

    void OnDisable()
    {

    }

    // LOGIC

    public void Frame(PlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
            return;

        bool move = false;

        move |= i_PlayerInput.GetButtonDown(s_PlayerInput_Down);
        move |= i_PlayerInput.GetButtonDown(s_PlayerInput_Up);

        bool submit = i_PlayerInput.GetButtonDown(s_PlayerInput_Submit);

        InternalUpdate(move, submit);
    }

    public void Frame(WiFiPlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
            return;

        bool move = false;

        move |= i_PlayerInput.GetNegativeButtonDown(s_WiFiPlayerInput_Vertical);
        move |= i_PlayerInput.GetPositiveButtonDown(s_WiFiPlayerInput_Vertical);

        bool submit = i_PlayerInput.GetButtonDown(s_WiFiPlayerInput_Submit);

        InternalUpdate(move, submit);
    }

    public void SetFocus()
    {
        Select(m_SwapCharacterButton);
    }

    public void ClearFocus()
    {

    }

    // INTERNALS

    private void InternalUpdate(bool i_Move, bool i_Submit)
    {
        if (i_Move)
        {
            if (m_SwapCharacterButton != null && (m_SelectedEntry == m_SwapCharacterButton))
            {
                Select(m_SwapColorButton);
            }
            else
            {
                if (m_SwapColorButton != null && (m_SelectedEntry == m_SwapColorButton))
                {
                    Select(m_SwapCharacterButton);
                }
            }
        }

        if (i_Submit)
        {
            if (m_SwapCharacterButton != null && (m_SelectedEntry == m_SwapCharacterButton))
            {
                if (onSwapCharacterEvent != null)
                {
                    onSwapCharacterEvent();
                }
            }
            else
            {
                if (m_SwapColorButton != null && (m_SelectedEntry == m_SwapColorButton))
                {
                    if (onSwapColorEvent != null)
                    {
                        onSwapColorEvent();
                    }
                }
            }
        }
    }

    // UTILS

    private void Select(GameObject i_Entry)
    {
        // Deselect previous entry.

        SetSelected(m_SelectedEntry, false);

        // Select new entry.

        SetSelected(i_Entry, true);

        m_SelectedEntry = i_Entry;
    }

    private void SetSelected(GameObject i_Entry, bool i_Value)
    {
        if (i_Entry != null)
        {
            Animator entryAnimator = i_Entry.GetComponent<Animator>();

            if (entryAnimator != null)
            {
                entryAnimator.SetBool(s_SelectedBoolId, i_Value);
            }
        }
    }
}
