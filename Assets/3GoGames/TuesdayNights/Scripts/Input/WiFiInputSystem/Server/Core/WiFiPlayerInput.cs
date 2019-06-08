using UnityEngine;

using System.Collections.Generic;

namespace WiFiInput.Server
{
    public class WiFiButtonData
    {
        private bool m_PrevValue;
        private bool m_CurrentValue;

        public bool value
        {
            get { return m_CurrentValue; }
            set
            {
                m_PrevValue = m_CurrentValue;
                m_CurrentValue = value;
            }
        }

        public bool prevValue
        {
            get { return m_PrevValue; }
        }

        // LOGIC

        public void Clear()
        {
            m_PrevValue = false;
            m_CurrentValue = false;
        }

        // CTOR

        public WiFiButtonData()
        {
            m_PrevValue = false;
            m_CurrentValue = false;
        }
    }

    public class WiFiAxisData
    {
        private float m_PrevValue;
        private float m_CurrentValue;

        private float m_ButtonThreshold;

        public float value
        {
            get { return m_CurrentValue; }
            set
            {
                m_PrevValue = m_CurrentValue;
                m_CurrentValue = value;
            }
        }

        public float prevValue
        {
            get { return m_PrevValue; }
        }

        public float buttonThreshold
        {
            get { return m_ButtonThreshold; }
        }

        public bool prevPressed
        {
            get { return (prevPositivePressed || prevNegativePressed); }
        }

        public bool pressed
        {
            get { return (positivePressed || negativePressed); }
        }

        public bool prevPositivePressed
        {
            get { return m_PrevValue > m_ButtonThreshold; }
        }

        public bool positivePressed
        {
            get { return m_CurrentValue > m_ButtonThreshold; }
        }

        public bool prevNegativePressed
        {
            get { return m_PrevValue < -m_ButtonThreshold; }
        }

        public bool negativePressed
        {
            get { return m_CurrentValue < -m_ButtonThreshold; }
        }

        // LOGIC

        public void Clear()
        {
            m_CurrentValue = 0f;
            m_PrevValue = 0f;
        }

        // CTOR

        public WiFiAxisData()
        {
            m_PrevValue = 0f;
            m_CurrentValue = 0f;

            m_ButtonThreshold = 0.1f;
        }
    }

    public class WiFiPlayerInput
    {
        private string m_PlayerName = "";

        private Dictionary<int, AxisServerController> m_Axes = null;
        private List<int> m_AxesKeys = null;
        private Dictionary<int, WiFiAxisData> m_AxesData = null;

        private Dictionary<int, ButtonServerController> m_Buttons = null;
        private List<int> m_ButtonsKeys = null;
        private Dictionary<int, WiFiButtonData> m_ButtonsData = null;

        private Dictionary<int, int> m_ActionsMap = null;

        private List<WiFiServerController> m_Controllers = null;

        private bool m_IsActive = false;

        public bool isActive
        {
            get { return m_IsActive; }
        }

        public string playerName
        {
            get { return m_PlayerName; }
        }

        // LOGIC

        public void Initialize()
        {
            WiFiActionsDatabase database = Resources.Load<WiFiActionsDatabase>("Input/WiFi/WiFiActionsDatabase");
            if (database != null)
            {
                foreach (string actionId in database.keys)
                {
                    string controlName = "";
                    if (database.GetButton(actionId, out controlName))
                    {
                        int hashId = StringUtils.GetHashCode(actionId);
                        int hashControl = StringUtils.GetHashCode(controlName);

                        m_ActionsMap.Add(hashId, hashControl);
                    }
                }
            }
        }

        public void Update()
        {
            // Update Axes

            {
                for (int axisIndex = 0; axisIndex < m_AxesKeys.Count; ++axisIndex)
                {
                    int key = m_AxesKeys[axisIndex];
                    AxisServerController serverController = null;
                    if (m_Axes.TryGetValue(key, out serverController))
                    {
                        float value = serverController.GetValue();
                        m_AxesData[key].value = value;
                    }
                }
            }

            // Update Buttons

            {
                for (int buttonIndex = 0; buttonIndex < m_ButtonsKeys.Count; ++buttonIndex)
                {
                    int key = m_ButtonsKeys[buttonIndex];
                    ButtonServerController serverController = null;
                    if (m_Buttons.TryGetValue(key, out serverController))
                    {
                        bool newValue = serverController.GetButton();
                        m_ButtonsData[key].value = newValue;
                    }
                }
            }
        }

        public void OnConnectionsChanged()
        {
            for (int controllerIndex = 0; controllerIndex < m_Controllers.Count; ++controllerIndex)
            {
                WiFiServerController controller = m_Controllers[controllerIndex];
                if (controller != null)
                {
                    controller.OnConnectionsChanged();
                }
            }
        }

        public void SetActive(bool i_Active)
        {
            m_IsActive = i_Active;
        }

        // Buttons

        public void AddButton(string i_ControlName, ButtonServerController i_Controller)
        {
            int hash = StringUtils.GetHashCode(i_ControlName);
            AddButton(hash, i_Controller);
        }

        public void AddButton(int i_ControlId, ButtonServerController i_Controller)
        {
            if (!m_Buttons.ContainsKey(i_ControlId))
            {
                m_Buttons.Add(i_ControlId, i_Controller);
                m_ButtonsKeys.Add(i_ControlId);

                m_ButtonsData.Add(i_ControlId, new WiFiButtonData());

                m_Controllers.Add(i_Controller);
            }
        }

        public bool GetButton(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetButton(hash);
        }

        public bool GetButton(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiButtonData buttonData = null;
            if (m_ButtonsData.TryGetValue(controlId, out buttonData))
            {
                bool buttonValue = buttonData.value;
                return buttonValue;
            }
            else
            {
                WiFiAxisData axisData = null;
                if (m_AxesData.TryGetValue(controlId, out axisData))
                {
                    bool axisPressed = axisData.pressed;
                    return axisPressed; 
                }
            }

            return false;
        }

        public bool GetButtonDown(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetButtonDown(hash);
        }

        public bool GetButtonDown(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiButtonData buttonData = null;
            if (m_ButtonsData.TryGetValue(controlId, out buttonData))
            {
                bool buttonValue = buttonData.value;
                bool prevButtonValue = buttonData.prevValue;

                return (buttonValue && !prevButtonValue);
            }
            else
            {
                WiFiAxisData axisData = null;
                if (m_AxesData.TryGetValue(controlId, out axisData))
                {
                    bool axisPressed = axisData.pressed;
                    bool axisPrevPressed = axisData.prevPressed;

                    return (axisPressed && !axisPrevPressed);
                }
            }

            return false;
        }

        public bool GetButtonUp(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetButtonUp(hash);
        }

        public bool GetButtonUp(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiButtonData buttonData = null;
            if (m_ButtonsData.TryGetValue(controlId, out buttonData))
            {
                bool buttonValue = buttonData.value;
                bool prevButtonValue = buttonData.prevValue;

                return (!buttonValue && prevButtonValue);
            }
            else
            {
                WiFiAxisData axisData = null;
                if (m_AxesData.TryGetValue(controlId, out axisData))
                {
                    bool axisPressed = axisData.pressed;
                    bool axisPrevPressed = axisData.prevPressed;

                    return (!axisPressed && axisPrevPressed);
                }
            }

            return false;
        }

        // Axes

        public void AddAxis(string i_ControlName, AxisServerController i_Controller)
        {
            int hash = StringUtils.GetHashCode(i_ControlName);
            AddAxis(hash, i_Controller);
        }

        public void AddAxis(int i_ControlId, AxisServerController i_Controller)
        {
            if (!m_Axes.ContainsKey(i_ControlId))
            {
                m_Axes.Add(i_ControlId, i_Controller);
                m_AxesKeys.Add(i_ControlId);

                m_AxesData.Add(i_ControlId, new WiFiAxisData());

                m_Controllers.Add(i_Controller);
            }
        }

        public float GetAxis(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetAxis(hash);
        }

        public float GetAxis(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return 0f;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return 0f;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                float axisValue = axisData.value;
                return axisValue;
            }

            return 0f;
        }

        public bool GetPositiveButton(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetPositiveButton(hash);
        }

        public bool GetPositiveButton(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                bool value = axisData.positivePressed;
                return value;
            }

            return false;
        }

        public bool GetPositiveButtonDown(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetPositiveButtonDown(hash);
        }

        public bool GetPositiveButtonDown(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                bool value = axisData.positivePressed;
                bool prevValue = axisData.prevPositivePressed;

                return (value && !prevValue);
            }

            return false;
        }

        public bool GetPositiveButtonUp(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetPositiveButtonUp(hash);
        }

        public bool GetPositiveButtonUp(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                bool value = axisData.positivePressed;
                bool prevValue = axisData.prevPositivePressed;

                return (!value && prevValue);
            }

            return false;
        }

        public bool GetNegativeButton(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetNegativeButton(hash);
        }

        public bool GetNegativeButton(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                bool value = axisData.negativePressed;
                return value;
            }

            return false;
        }

        public bool GetNegativeButtonDown(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetNegativeButtonDown(hash);
        }

        public bool GetNegativeButtonDown(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                bool value = axisData.negativePressed;
                bool prevValue = axisData.prevNegativePressed;

                return (value && !prevValue);
            }

            return false;
        }

        public bool GetNegativeButtonUp(string i_ActionName)
        {
            int hash = StringUtils.GetHashCode(i_ActionName);
            return GetNegativeButtonUp(hash);
        }

        public bool GetNegativeButtonUp(int i_ActionId)
        {
            if (!m_IsActive)
            {
                return false;
            }

            int controlId = Hash.s_NULL;

            if (!GetControlId(i_ActionId, out controlId))
            {
                return false;
            }

            WiFiAxisData axisData = null;
            if (m_AxesData.TryGetValue(controlId, out axisData))
            {
                bool value = axisData.negativePressed;
                bool prevValue = axisData.prevNegativePressed;

                return (!value && prevValue);
            }

            return false;
        }

        // INTERNALS

        private bool GetControlId(int i_ActionId, out int i_ControlId)
        {
            return m_ActionsMap.TryGetValue(i_ActionId, out i_ControlId);
        }

        // CTOR

        public WiFiPlayerInput(WiFiPlayerDescriptor i_Descriptor)
        {
            m_PlayerName = i_Descriptor.name;

            m_Axes = new Dictionary<int, AxisServerController>();
            m_AxesKeys = new List<int>();
            m_AxesData = new Dictionary<int, WiFiAxisData>();

            m_Buttons = new Dictionary<int, ButtonServerController>();
            m_ButtonsKeys = new List<int>();
            m_ButtonsData = new Dictionary<int, WiFiButtonData>();

            m_ActionsMap = new Dictionary<int, int>();

            m_Controllers = new List<WiFiServerController>();
        }
    }
}
