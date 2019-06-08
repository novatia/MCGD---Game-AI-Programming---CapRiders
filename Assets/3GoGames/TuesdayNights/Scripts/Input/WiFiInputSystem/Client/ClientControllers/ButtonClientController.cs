using UnityEngine.UI;
using UnityEngine.EventSystems;

using WiFiInput.Common;

namespace WiFiInput.Client
{
    using EventTrigger = UnityEngine.EventSystems.EventTrigger;

    public class ButtonClientController : WiFiClientController
    {
        private string m_ControlName = "";

        private Button m_Button = null;
        private bool m_ButtonPressed = false;

        private ButtonControllerType m_Controller = null;

        public string controlName
        {
            get { return m_ControlName; }
        }

        // LOGIC

        public void Initialize(Button i_Button)
        {
            m_Button = i_Button;

            if (m_Button != null)
            {
                EventTrigger eventTrigger = m_Button.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    eventTrigger = m_Button.gameObject.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
                pointerDownEntry.eventID = EventTriggerType.PointerDown;
                pointerDownEntry.callback.AddListener(OnButtonDown);

                eventTrigger.triggers.Add(pointerDownEntry);

                EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
                pointerUpEntry.eventID = EventTriggerType.PointerUp;
                pointerUpEntry.callback.AddListener(OnButtonUp);

                eventTrigger.triggers.Add(pointerUpEntry);
            }

            if (WiFiInputController.controllerDataDictionary != null)
            {
                string controllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_BUTTON, controlName);
                m_Controller = (ButtonControllerType)WiFiInputController.controllerDataDictionary[controllerKey];
            }
        }

        public void Clear()
        {
            m_Controller = null;

            if (m_Button != null)
            {
                EventTrigger eventTrigger = m_Button.GetComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    eventTrigger.triggers.Clear();
                }
            }

            m_Button = null;
        }

        // WiFiClientController's interface

        protected override void mapInputToDataStream()
        {
            if (m_Controller == null)
                return;

            m_Controller.BUTTON_STATE_IS_PRESSED = m_ButtonPressed;
        }

        // INTERNALS

        private void OnButtonDown(BaseEventData i_EventData)
        {
            m_ButtonPressed = true;
        }

        private void OnButtonUp(BaseEventData i_EventData)
        {
            m_ButtonPressed = false;
        }

        // CTOR

        public ButtonClientController(string i_ControlName)
        {
            m_ControlName = i_ControlName;
        }
    }
}