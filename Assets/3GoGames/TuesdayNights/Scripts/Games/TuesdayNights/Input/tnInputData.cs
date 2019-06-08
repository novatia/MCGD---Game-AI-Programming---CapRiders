using System.Collections.Generic;

public class tnInputData
{
    private class ButtonData
    {
        // Fields

        private bool m_Value;
        private bool m_PrevValue;

        // ACCESSORS

        public bool value
        {
            get { return m_Value; }
        }

        public bool prevValue
        {
            get { return m_PrevValue; }
        }

        public bool buttonDown
        {
            get { return m_Value && !m_PrevValue; }
        }

        public bool buttonUp
        {
            get { return !m_Value && m_PrevValue; }
        }

        // LOGIC

        public void SetValue(bool i_Value)
        {
            m_PrevValue = m_Value;
            m_Value = i_Value;
        }

        public void Clear()
        {
            m_Value = false;
            m_PrevValue = false;
        }

        // CTOR

        public ButtonData()
        {
            m_Value = false;
            m_PrevValue = false;
        }
    }

    private Dictionary<int, float> m_Axes = null;
    private Dictionary<int, ButtonData> m_Buttons = null;

    // LOGIC

    public void Clear()
    {
        m_Axes.Clear();

        foreach (ButtonData buttonData in m_Buttons.Values)
        {
            buttonData.Clear();
        }

        // m_Buttons.Clear();
    }

    // AXES

    public float GetAxis(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetAxis(hash);
    }

    public float GetAxis(int i_Id)
    {
        float axisValue;
        if (m_Axes.TryGetValue(i_Id, out axisValue))
        {
            return axisValue;
        }

        return 0f;
    }

    public void SetAxis(string i_Id, float i_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetAxis(hash, i_Value);
    }

    public void SetAxis(int i_Id, float i_Value)
    {
        m_Axes[i_Id] = i_Value;
    }

    // BUTTONS

    public bool GetButton(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetButton(hash);
    }

    public bool GetButton(int i_Id)
    {
        ButtonData buttonData = null;
        if (m_Buttons.TryGetValue(i_Id, out buttonData))
        {
            return buttonData.value;
        }

        return false;
    }

    public bool GetButtonDown(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetButtonDown(hash);
    }

    public bool GetButtonDown(int i_Id)
    {
        ButtonData buttonData = null;
        if (m_Buttons.TryGetValue(i_Id, out buttonData))
        {
            return buttonData.buttonDown;
        }

        return false;
    }

    public bool GetButtonUp(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetButtonUp(hash);
    }

    public bool GetButtonUp(int i_Id)
    {
        ButtonData buttonData = null;
        if (m_Buttons.TryGetValue(i_Id, out buttonData))
        {
            return buttonData.buttonUp;
        }

        return false;
    }

    public void SetButton(string i_Id, bool i_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetButton(hash, i_Value);
    }

    public void SetButton(int i_Id, bool i_Value)
    {
        ButtonData buttonData = null;
        if (m_Buttons.TryGetValue(i_Id, out buttonData))
        {
            buttonData.SetValue(i_Value);
        }
        else
        {
            buttonData = new ButtonData();
            buttonData.SetValue(i_Value);

            m_Buttons.Add(i_Id, buttonData);
        }
    }

    // CTOR

    public tnInputData()
    {
        m_Axes = new Dictionary<int, float>();
        m_Buttons = new Dictionary<int, ButtonData>();
    }
}
