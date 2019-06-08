using System.Collections.Generic;

public class tnInputManager
{
    private List<tnInputController> m_Controllers = new List<tnInputController>();

    // BUSINESS LOGIC

    public int controllersCount
    {
        get
        {
            return m_Controllers.Count;
        }
    }

    public tnInputController GetController(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Controllers.Count)
        {
            return null;
        }

        return m_Controllers[i_Index];
    }

    public void AddController(tnInputController i_Controller)
    {
        m_Controllers.Add(i_Controller);
    }

    public void RemoveController(tnInputController i_Controller)
    {
        m_Controllers.Remove(i_Controller);
    }

    public void Clear()
    {
        m_Controllers.Clear();
    }

    public void SetControllerActive(int i_Index, bool i_Active)
    {
        InternalSetControllerActive(i_Index, i_Active);
    }

    public void ActivateAll()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            InternalSetControllerActive(controllerIndex, true);
        }
    }

    public void ActivateAllAIs()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];
            if (inputController != null)
            {
                if (!inputController.isHumanPlayer)
                {
                    InternalSetControllerActive(controllerIndex, true);
                }
            }
        }
    }

    public void ActivateAllHumans()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];
            if (inputController != null)
            {
                if (inputController.isHumanPlayer)
                {
                    InternalSetControllerActive(controllerIndex, true);
                }
            }
        }
    }

    public void DeactivateAll()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            InternalSetControllerActive(controllerIndex, false);
        }
    }

    public void DeactivateAllAIs()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];
            if (inputController != null)
            {
                if (!inputController.isHumanPlayer)
                {
                    InternalSetControllerActive(controllerIndex, false);
                }
            }
        }
    }

    public void DeactivateAllHumans()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];
            if (inputController != null)
            {
                if (inputController.isHumanPlayer)
                {
                    InternalSetControllerActive(controllerIndex, false);
                }
            }
        }
    }

    public void InhibitAll()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];

            if (inputController == null)
                continue;

            inputController.Inhibit();
        }
    }

    public void InhibitAllHumans()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];

            if (inputController == null)
                continue;

            if (inputController.isHumanPlayer)
            {
                inputController.Inhibit();
            }
        }
    }

    public void InhibitAllAIs()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];

            if (inputController == null)
                continue;

            if (!inputController.isHumanPlayer)
            {
                inputController.Inhibit();
            }
        }
    }

    public void Update()
    {
        for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
        {
            tnInputController inputController = m_Controllers[controllerIndex];
            if (inputController != null)
            {
                inputController.Update();
            }
        }
    }

    // INTERNALS

    private void InternalSetControllerActive(int i_Index, bool i_Active)
    {
        tnInputController inputController = GetController(i_Index);

        if (inputController != null)
        {
            inputController.SetActive(i_Active);
        }
    }
}
