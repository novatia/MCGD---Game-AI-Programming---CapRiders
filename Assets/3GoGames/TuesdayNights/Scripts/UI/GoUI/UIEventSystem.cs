using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;

namespace GoUI
{
    public class UIEventSystem : Singleton<UIEventSystem>
    {
        private EventSystem m_EventSystem = null;
        private InputModule m_InputModule = null;

        private List<UIEventTrigger> m_Triggers = new List<UIEventTrigger>();

        private bool m_Active = true;

        public delegate void OnFocusChanged(GameObject i_CurrFocus, GameObject i_PrevFocus);
        public delegate void OnTriggerAdded(UIEventTrigger i_Trigger);
        public delegate void OnTriggerRemoved(UIEventTrigger i_Trigger);

        // STATIC METHODS

        public static int triggersCountMain
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.triggersCount;
                }

                return 0;
            }
        }

        public static InputModule inputModuleMain
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.inputModule;
                }

                return null;
            }
        }

        public static void InitializeMain()
        {
            if (Instance != null)
            {
                Instance.Initialize();
            }
        }

        public static event OnFocusChanged onFocusChangedMain
        {
            add
            {
                if (Instance != null)
                {
                    Instance.onFocusChanged += value;
                }
            }

            remove
            {
                if (Instance != null)
                {
                    Instance.onFocusChanged -= value;
                }
            }
        }

        public static event OnTriggerAdded onTriggerAddedMain
        {
            add
            {
                if (Instance != null)
                {
                    Instance.onTriggerAdded += value;
                }
            }

            remove
            {
                if (Instance != null)
                {
                    Instance.onTriggerAdded -= value;
                }
            }
        }

        public static event OnTriggerRemoved onTriggerRemovedMain
        {
            add
            {
                if (Instance != null)
                {
                    Instance.onTriggerRemoved += value;
                }
            }

            remove
            {
                if (Instance != null)
                {
                    Instance.onTriggerRemoved -= value;
                }
            }
        }

        public static GameObject focusMain
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.focus;
                }

                return null;
            }
        }

        public static void SetFocusMain(GameObject i_Focus)
        {
            if (Instance != null)
            {
                Instance.SetFocus(i_Focus);
            }
        }

        public static void RegisterTriggerMain(UIEventTrigger i_Trigger)
        {
            if (Instance != null)
            {
                Instance.RegisterTrigger(i_Trigger);
            }
        }

        public static void UnregisterTriggerMain(UIEventTrigger i_Trigger)
        {
            if (Instance != null)
            {
                Instance.UnregisterTrigger(i_Trigger);
            }
        }

        public static UIEventTrigger GetUIEventTriggerMain(int i_Index)
        {
            if (Instance != null)
            {
                return Instance.GetUIEventTrigger(i_Index);
            }

            return null;
        }

        public static void SetActiveMain(bool i_Active)
        {
            if (Instance != null)
            {
                Instance.SetActive(i_Active);
            }
        }

        // BUSINESS LOGIC

        public int triggersCount
        {
            get
            {
                return m_Triggers.Count;
            }
        }

        public InputModule inputModule
        {
            get
            {
                return m_InputModule;
            }
        }

        public void Initialize()
        {
            GameObject root = new GameObject("Root");
            root.transform.SetParent(transform);

            EventSystem eventSystem = root.AddComponent<EventSystem>();
            eventSystem.firstSelectedGameObject = null;
            eventSystem.sendNavigationEvents = true;
            eventSystem.pixelDragThreshold = 5;

            m_EventSystem = eventSystem;

            /*

            StandaloneInputModule inputModule = uiEventSystem.AddComponent<StandaloneInputModule>();
            inputModule.horizontalAxis = "Horizontal";
            inputModule.verticalAxis = "Vertical";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
            inputModule.inputActionsPerSecond = 10f;
            inputModule.repeatDelay = 0.5f;
            inputModule.forceModuleActive = false;

            */

            InputModule inputModule = root.AddComponent<InputModule>();

            InputModuleConfig inputModuleConfig = Resources.Load<InputModuleConfig>("Input/InputModuleConfig");
            if (inputModuleConfig != null)
            {
                inputModule.horizontalAxis = inputModuleConfig.horizontalAxis;
                inputModule.verticalAxis = inputModuleConfig.verticalAxis;
                inputModule.submitButton = inputModuleConfig.submitButton;
                inputModule.cancelButton = inputModuleConfig.cancelButton;

                inputModule.wifiHorizontalAxis = inputModuleConfig.wifiHorizontalAxis;
                inputModule.wifiVerticalAxis = inputModuleConfig.wifiVerticalAxis;
                inputModule.wifiSubmitButton = inputModuleConfig.wifiSubmitButton;
                inputModule.wifiCancelButton = inputModuleConfig.wifiCancelButton;

                inputModule.inputActionsPerSecond = inputModuleConfig.inputActionsPerSecond;
                inputModule.moveOneElementPerAxisPress = inputModuleConfig.moveOneElementPerAxisPress;
                inputModule.repeatDelay = inputModuleConfig.repeatDelay;
                inputModule.axisDeadzone = inputModuleConfig.axisDeadzone;
                inputModule.allowMouseInput = inputModuleConfig.allowMouseInput;
                inputModule.allowMouseInputIfTouchSupported = inputModuleConfig.allowMouseInputIfTouchSupported;
                inputModule.allowActivationOnMobileDevice = inputModule.allowActivationOnMobileDevice;

                for (int index = 0; index < inputModuleConfig.defaultPlayersCount; ++index)
                {
                    string player = inputModuleConfig.GetPlayer(index);

                    if (StringUtils.IsNullOrEmpty(player))
                        continue;

                    inputModule.AddPlayer(player);
                }

                for (int index = 0; index < inputModuleConfig.defaultWifiPlayersCount; ++index)
                {
                    string player = inputModuleConfig.GetWifiPlayer(index);

                    if (StringUtils.IsNullOrEmpty(player))
                        continue;

                    inputModule.AddWifiPlayer(player);
                }
            }
            else
            {
                inputModule.horizontalAxis = "Horizontal";
                inputModule.verticalAxis = "Vertical";
                inputModule.submitButton = "Submit";
                inputModule.cancelButton = "Cancel";
                inputModule.inputActionsPerSecond = 4f;
                inputModule.moveOneElementPerAxisPress = true;
                inputModule.repeatDelay = 0f;
                inputModule.axisDeadzone = 0f;
            }

            m_InputModule = inputModule;

            // TODO : Configure PlayerInput designed to handle UI input.
        }

        public event OnFocusChanged onFocusChanged;
        public event OnTriggerAdded onTriggerAdded;
        public event OnTriggerRemoved onTriggerRemoved;

        public GameObject focus
        {
            get
            {
                if (m_EventSystem == null)
                {
                    return null;
                }

                return m_EventSystem.currentSelectedGameObject;
            }
        }

        public void SetFocus(GameObject i_Focus)
        {
            if (!m_Active)
                return;

            if (m_EventSystem == null)
                return;

            if (focus != i_Focus)
            {
                GameObject oldFocus = focus;

                m_EventSystem.SetSelectedGameObject(i_Focus);

                if (onFocusChanged != null)
                {
                    onFocusChanged(oldFocus, focus);
                }
            }
        }

        public void RegisterTrigger(UIEventTrigger i_Trigger)
        {
            if (!m_Triggers.Contains(i_Trigger))
            {
                m_Triggers.Add(i_Trigger);

                if (onTriggerAdded != null)
                {
                    onTriggerAdded(i_Trigger);
                }
            }
        }

        public void UnregisterTrigger(UIEventTrigger i_Trigger)
        {
            m_Triggers.Remove(i_Trigger);

            if (onTriggerRemoved != null)
            {
                onTriggerRemoved(i_Trigger);
            }
        }

        public UIEventTrigger GetUIEventTrigger(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Triggers.Count)
            {
                return null;
            }

            return m_Triggers[i_Index];
        }

        public void SetActive(bool i_Active)
        {
            if (m_Active != i_Active)
            {
                if (!i_Active)
                {
                    SetFocus(null);
                }

                m_Active = i_Active;
            }
        }

        // MonoBehaviour's INTERFACE

        void Update()
        {
            if (!m_Active)
                return;

            // Update Triggers.

            for (int triggerIndex = 0; triggerIndex < m_Triggers.Count; ++triggerIndex)
            {
                UIEventTrigger trigger = m_Triggers[triggerIndex];

                if (trigger == null)
                    continue;

                trigger.UpdateTrigger();
            }
        }
    }
}
