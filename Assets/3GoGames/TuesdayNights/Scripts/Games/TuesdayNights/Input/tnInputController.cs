using UnityEngine;

public class tnInputController
{
    private tnInputData m_Input = new tnInputData();

    private tnRumbleController m_RumbleController = null;
    private tnInputFiller m_Filler = null;
    private bool m_Active = true;

    private bool m_IsHumanPlayer = false;

    private int m_InhibitFrame = 0;

    // GETTERS

    public bool isHumanPlayer
    {
        get { return m_IsHumanPlayer; }
    }

    // BUSINESS LOGIC

    public void SetActive(bool i_Active)
    {
        if (m_Active != i_Active)
        {
            if (!i_Active)
            {
                Clear();
            }

            m_Active = i_Active;
        }
    }

    public void Update()
    {
        if (m_InhibitFrame > 0)
        {
            if (Time.frameCount > m_InhibitFrame)
            {
                m_InhibitFrame = 0;
            }
        }

        if (!m_Active || m_Filler == null)
        {
            return;
        }

        m_Filler.Fill(Time.deltaTime, m_Input);
    }

    public void Clear()
    {
        if (m_Filler != null)
        {
            m_Filler.Clear();
        }

        m_Input.Clear();
    }

    public void Inhibit()
    {
        m_InhibitFrame = Time.frameCount;
    }

    public void DrawGizmos()
    {
        if (m_Filler != null)
        {
            m_Filler.DrawGizmos();
        }
    }

    public void DrawGizmosSelected()
    {
        if (m_Filler != null)
        {
            m_Filler.DrawGizmosSelected();
        }
    }

    // RUMBLE

    public void SetRumbleController(tnRumbleController i_RumbleController)
    {
        m_RumbleController = i_RumbleController;
    }

    public void SetVibration(float i_Left, float i_Right)
    {
        if (m_RumbleController != null)
        {
            m_RumbleController.SetVibration(i_Left, i_Right);
        }
    }

    public void StopVibration()
    {
        if (m_RumbleController != null)
        {
            m_RumbleController.StopVibration();
        }
    }

    // AXES

    public float GetAxis(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetAxis(hash);
    }

    public float GetAxis(int i_ActionId)
    {
        if (!m_Active || m_InhibitFrame > 0)
        {
            return 0f;
        }

        return m_Input.GetAxis(i_ActionId);
    }

    // BUTTONS

    public bool GetButton(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetButton(hash);
    }

    public bool GetButton(int i_ActionId)
    {
        if (!m_Active || m_InhibitFrame > 0)
        {
            return false;
        }

        return m_Input.GetButton(i_ActionId);
    }

    public bool GetButtonDown(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetButtonDown(hash);
    }

    public bool GetButtonDown(int i_ActionId)
    {
        if (!m_Active || m_InhibitFrame > 0)
        {
            return false;
        }

        return m_Input.GetButtonDown(i_ActionId);
    }

    public bool GetButtonUp(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetButtonUp(hash);
    }

    public bool GetButtonUp(int i_ActionId)
    {
        if (!m_Active || m_InhibitFrame > 0)
        {
            return false;
        }

        return m_Input.GetButtonUp(i_ActionId);
    }

    // CTOR

    public tnInputController(tnInputFiller i_Filler)
    {
        m_Filler = i_Filler;

        m_IsHumanPlayer = !(i_Filler is tnAIInputFiller);
    }
}
