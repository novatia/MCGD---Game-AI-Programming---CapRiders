using UnityEngine;
using UnityEngine.UI;

public enum tnUIOnlinePlayerSlotState
{
    None,
    Disabled,
    Empty,
    Occupied,
}

public class tnUIOnlinePlayerSlot : MonoBehaviour
{
    [SerializeField]
    private Text m_OverlayText = null;
    [SerializeField]
    private Image m_ControllerImage = null;
    [SerializeField]
    private Text m_PlayerNameText = null;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_OverlayDisabledAlpha = 0.5f;

    private tnUIOnlinePlayerSlotState m_State = tnUIOnlinePlayerSlotState.None;

    // ACCESSORS

    public tnUIOnlinePlayerSlotState state
    {
        get
        {
            return m_State;
        }
    }

    // LOGIC

    public void Clear()
    {
        SetColor(Color.white);
        SetState(tnUIOnlinePlayerSlotState.Disabled);
    }

    public void SetPlayerName(string i_Text)
    {
        SetPlayerNameText(i_Text);
    }

    public void SetColor(Color i_Color)
    {
        //if (m_OverlayText != null)
        //{
        //    m_OverlayText.SetColorWithoutAlpha(i_Color);
        //}

        if (m_ControllerImage != null)
        {
            m_ControllerImage.SetColorWithoutAlpha(i_Color);
        }

        if (m_PlayerNameText != null)
        {
            m_PlayerNameText.SetColorWithoutAlpha(i_Color);
        }
    }

    public void SetState(tnUIOnlinePlayerSlotState i_State)
    {
        Internal_SetState(i_State);
    }

    // INTERNALS

    private void SetOverlayText(string i_Text)
    {
        if (m_OverlayText != null)
        {
            m_OverlayText.text = i_Text;
        }
    }

    private void SetOverlayTextColorAlpha(float i_Alpha)
    {
        if (m_OverlayText != null)
        {
            m_OverlayText.SetColorAlpha(i_Alpha);
        }
    }

    private void SetOverlayTextEnabled(bool i_Enabled)
    {
        if (m_OverlayText != null)
        {
            m_OverlayText.enabled = i_Enabled;
        }
    }

    private void SetControllerImageEnabled(bool i_Enabled)
    {
        if (m_ControllerImage != null)
        {
            m_ControllerImage.enabled = i_Enabled;
        }
    }

    private void SetPlayerNameText(string i_Text)
    {
        if (m_PlayerNameText != null)
        {
            m_PlayerNameText.text = i_Text;
        }
    }

    private void SetPlayerNameEnabled(bool i_Enabled)
    {
        if (m_PlayerNameText != null)
        {
            m_PlayerNameText.enabled = i_Enabled;
        }
    }

    private void Internal_SetState(tnUIOnlinePlayerSlotState i_State)
    {
        if (i_State == tnUIOnlinePlayerSlotState.None)
            return;

        switch (i_State)
        {
            case tnUIOnlinePlayerSlotState.Disabled:

                SetOverlayText("CLOSED");
                SetOverlayTextEnabled(true);
                SetOverlayTextColorAlpha(m_OverlayDisabledAlpha);
                SetControllerImageEnabled(false);
                SetPlayerNameEnabled(false);

                break;

            case tnUIOnlinePlayerSlotState.Empty:

                SetOverlayText("EMPTY");
                SetOverlayTextEnabled(true);
                SetOverlayTextColorAlpha(1f);
                SetControllerImageEnabled(false);
                SetPlayerNameEnabled(false);

                break;

            case tnUIOnlinePlayerSlotState.Occupied:

                SetOverlayText("");
                SetOverlayTextEnabled(false);
                SetOverlayTextColorAlpha(1f);
                SetControllerImageEnabled(true);
                SetPlayerNameEnabled(true);

                break;
        }

        m_State = i_State;
    }
}
