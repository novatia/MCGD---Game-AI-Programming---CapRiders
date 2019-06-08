using UnityEngine;

public class tnCharacterInput : MonoBehaviour
{
    [SerializeField]
    private bool m_InputEnabled = true;
    [SerializeField]
    private bool m_DrawInputFillerGizmos = false;

    private tnInputController m_InputController = null;

    private tnRespawn m_Respawn = null;

    public bool isHumanPlayer
    {
        get
        {
            if (m_InputController != null)
            {
                return m_InputController.isHumanPlayer;
            }

            return false;
        }
    }

    public bool isLocalPlayer
    {
        get
        {
            return (m_InputController != null);
        }
    }

    // MonoBehaviour's INTERFACE

    void OnDrawGizmos()
    {
        if (!m_DrawInputFillerGizmos)
            return;

        if (m_InputController != null)
        {
            m_InputController.DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!m_DrawInputFillerGizmos)
            return;

        if (m_InputController != null)
        {
            m_InputController.DrawGizmosSelected();
        }
    }

    void Awake()
    {
        m_Respawn = GetComponent<tnRespawn>();
    }

    void OnEnable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccured;
        }
    }
    
    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccured;
        }
    }

    // BUSINESS LOGIC

    public void Bind(tnInputController i_InputController)
    {
        m_InputController = i_InputController;
    }

    public void Clear()
    {
        if (m_InputController != null)
        {
            m_InputController.Clear();
        }
    }

    // RUMBLE

    public void SetVibration(float i_Left, float i_Right)
    {
        if (m_InputController != null)
        {
            m_InputController.SetVibration(i_Left, i_Right);
        }
    }

    public void StopVibration()
    {
        if (m_InputController != null)
        {
            m_InputController.StopVibration();
        }
    }

    // AXIS

    public float GetAxis(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetAxis(hash);
    }

    public float GetAxis(int i_ActionId)
    {
        if (!m_InputEnabled || m_InputController == null)
        {
            return 0f;
        }

        return m_InputController.GetAxis(i_ActionId);
    }

    // BUTTONS

    public bool GetButton(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetButton(hash);
    }

    public bool GetButton(int i_ActionId)
    {
        if (!m_InputEnabled || m_InputController == null)
        {
            return false;
        }

        return m_InputController.GetButton(i_ActionId);
    }

    public bool GetButtonDown(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetButtonDown(hash);
    }

    public bool GetButtonDown(int i_ActionId)
    {
        if (!m_InputEnabled || m_InputController == null)
        {
            return false;
        }

        return m_InputController.GetButtonDown(i_ActionId);
    }

    public bool GetButtonUp(string i_ActionName)
    {
        int hash = StringUtils.GetHashCode(i_ActionName);
        return GetButtonUp(hash);
    }

    public bool GetButtonUp(int i_ActionId)
    {
        if (!m_InputEnabled || m_InputController == null)
        {
            return false;
        }

        return m_InputController.GetButtonUp(i_ActionId);
    }

    // EVENTS

    private void OnRespawnOccured()
    {
        Clear();
    }
}