using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using WiFiInput.Common;

namespace WiFiInput.Client
{
    using EventTrigger = UnityEngine.EventSystems.EventTrigger;

    public class JoystickClientController : WiFiClientController
    {
        private string m_XControlName = "";
        private string m_YControlName = "";

        private bool m_SendXAsButton = false;
        private string m_XPositiveButtonControlName = "";
        private string m_XNegativeButtonControlName = "";
        private float m_XAxisThreshold = 0f;
        private bool m_SendYAsButton = false;
        private string m_YPositiveButtonControlName = "";
        private string m_YNegativeButtonControlName = "";
        private float m_YAxisThreshold = 0f;

        private RectTransform m_BackPanel = null;
        private Image m_BackImage = null;
        private Image m_Nub = null;

        private Vector2 m_OldPos = Vector2.zero;

        private float m_LowThreshold = 0.1f;
        private float m_HighThreshold = 0.6f;

        private Vector2 m_DefaultNubPosition = Vector2.zero;

        private AxisControllerType m_XController = null;
        private AxisControllerType m_YController = null;

        private ButtonControllerType m_XPositiveButtonController = null;
        private ButtonControllerType m_XNegativeButtonController = null;
        private ButtonControllerType m_YPositiveButtonController = null;
        private ButtonControllerType m_YNegativeButtonController = null;

        private float m_Ray = 10f;

        private float m_XAxis = 0f;
        private float m_YAxis = 0f;

        public string xControlName
        {
            get { return m_XControlName; }
        }

        public string yControlName
        {
            get { return m_YControlName; }
        }

        // LOGIC

        public void Initialize(RectTransform i_BackPanel, Image i_BackImage, Image i_Nub, float i_LowThreshold = 0.1f, float i_HighThreshold = 0.6f, float i_XButtonThreshold = 0.5f, float i_YButtonThreshold = 0.5f)
        {
            m_LowThreshold = Mathf.Clamp01(i_LowThreshold);
            m_HighThreshold = Mathf.Clamp01(i_HighThreshold);

            m_XAxisThreshold = Mathf.Clamp01(i_XButtonThreshold);
            m_YAxisThreshold = Mathf.Clamp01(i_YButtonThreshold);

            m_BackPanel = i_BackPanel;
            m_BackImage = i_BackImage;
            m_Nub = i_Nub;

            if (m_BackImage != null && m_BackPanel != null && m_Nub != null)
            {
                // Apply custom scale for tablet or big screens

                float scale = 1f;

                float screenDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
                float displayInches = (screenDiagonal / Screen.dpi);

                float refDisplayInches = 5f;

                float screenRatio = refDisplayInches / displayInches;
                if (screenRatio < 1f)
                {
                    scale = screenRatio;
                }

                m_BackImage.transform.localScale = new Vector3(scale, scale, scale);

                // Evaluate ray

                {
                    Vector2 centerPosition = m_BackImage.rectTransform.GetPosition2();

                    float ray = (m_BackImage.rectTransform.rect.width / 2f);
                    m_BackImage.rectTransform.anchoredPosition += Vector2.right * ray;

                    Vector2 p = m_BackImage.rectTransform.GetPosition2();

                    m_Ray = (p - centerPosition).magnitude * scale;

                    m_BackImage.rectTransform.SetPosition(centerPosition);
                }

                m_DefaultNubPosition = m_Nub.rectTransform.anchoredPosition;

                EventTrigger eventTrigger = m_BackPanel.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    eventTrigger = m_BackPanel.gameObject.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener(OnPointerDown);

                eventTrigger.triggers.Add(pointerDown);

                EventTrigger.Entry pointerDrag = new EventTrigger.Entry();
                pointerDrag.eventID = EventTriggerType.Drag;
                pointerDrag.callback.AddListener(OnPointerDrag);

                eventTrigger.triggers.Add(pointerDrag);

                EventTrigger.Entry pointerEndDrag = new EventTrigger.Entry();
                pointerEndDrag.eventID = EventTriggerType.EndDrag;
                pointerEndDrag.callback.AddListener(OnPointerEndDrag);

                eventTrigger.triggers.Add(pointerEndDrag);

                EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                pointerUp.eventID = EventTriggerType.PointerUp;
                pointerUp.callback.AddListener(OnPointerUp);

                eventTrigger.triggers.Add(pointerUp);
            }

            if (WiFiInputController.controllerDataDictionary != null)
            {
                string xControllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_AXIS, xControlName);
                string yControllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_AXIS, yControlName);

                m_XController = (AxisControllerType)WiFiInputController.controllerDataDictionary[xControllerKey];
                m_YController = (AxisControllerType)WiFiInputController.controllerDataDictionary[yControllerKey];

                if (m_SendXAsButton)
                {
                    string xPositiveButtonControllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_BUTTON, m_XPositiveButtonControlName);
                    string xNegativeButtonControllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_BUTTON, m_XNegativeButtonControlName);

                    m_XPositiveButtonController = (ButtonControllerType)WiFiInputController.controllerDataDictionary[xPositiveButtonControllerKey];
                    m_XNegativeButtonController = (ButtonControllerType)WiFiInputController.controllerDataDictionary[xNegativeButtonControllerKey];
                }

                if (m_SendYAsButton)
                {
                    string yPositiveButtonControllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_BUTTON, m_YPositiveButtonControlName);
                    string yNegativeButtonControllerKey = WiFiInputController.registerControl(WiFiInputConstants.CONTROLLERTYPE_BUTTON, m_YNegativeButtonControlName);

                    m_YPositiveButtonController = (ButtonControllerType)WiFiInputController.controllerDataDictionary[yPositiveButtonControllerKey];
                    m_YNegativeButtonController = (ButtonControllerType)WiFiInputController.controllerDataDictionary[yNegativeButtonControllerKey];
                }
            }
        }

        public void Clear()
        {
            m_YController = null;
            m_XController = null;

            m_XPositiveButtonController = null;
            m_XNegativeButtonController = null;
            m_YPositiveButtonController = null;
            m_YNegativeButtonController = null;

            if (m_BackPanel != null && m_BackImage != null && m_Nub != null)
            {
                EventTrigger eventTrigger = m_BackPanel.GetComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    eventTrigger.triggers.Clear();
                }
            }

            m_Nub = null;
            m_BackImage = null;
            m_BackPanel = null;
        }

        // WiFiClientController's interface

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void mapInputToDataStream()
        {
            if (m_XController == null || m_YController == null)
                return;

            m_XController.AXIS_VALUE = m_XAxis;
            m_YController.AXIS_VALUE = m_YAxis;

            if (m_SendXAsButton)
            {
                m_XPositiveButtonController.BUTTON_STATE_IS_PRESSED = (m_XAxis > m_XAxisThreshold);
                m_XNegativeButtonController.BUTTON_STATE_IS_PRESSED = (m_XAxis < -m_XAxisThreshold);
            }

            if (m_SendYAsButton)
            {
                m_YPositiveButtonController.BUTTON_STATE_IS_PRESSED = (m_YAxis > m_YAxisThreshold);
                m_YNegativeButtonController.BUTTON_STATE_IS_PRESSED = (m_YAxis < -m_YAxisThreshold);
            }
        }

        // INTERNALS

        private void OnPointerDown(BaseEventData i_EventData)
        {
            PointerEventData pointerEventData = (PointerEventData)i_EventData;

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pointerEventData.position);

            if (m_BackImage != null)
            {
                m_BackImage.rectTransform.SetPosition(worldPoint);
                m_BackImage.gameObject.SetActive(true);

                if (m_Nub != null)
                {
                    m_Nub.rectTransform.SetPosition(worldPoint);
                    m_OldPos = worldPoint;

                    m_DefaultNubPosition = worldPoint;
                }
            }
        }

        private void OnPointerDrag(BaseEventData i_EventData)
        {
            if (!m_BackImage.gameObject.activeSelf)
                return;

            float xAxis = 0f;
            float yAxis = 0f;

            PointerEventData pointerEventData = (PointerEventData)i_EventData;

            Vector2 currentPos = Camera.main.ScreenToWorldPoint(pointerEventData.position);
            Vector2 deltaPos = currentPos - m_OldPos;

            if (m_Nub != null)
            {
                Vector2 nubPos = m_Nub.rectTransform.GetPosition2();
                Vector2 targetPosition = nubPos + deltaPos;

                m_Nub.rectTransform.SetPosition(targetPosition);

                Vector2 difference = m_Nub.rectTransform.GetPosition2() - m_DefaultNubPosition;
                float differenceMagnitude = difference.sqrMagnitude;

                if (differenceMagnitude > m_Ray * m_Ray)
                {
                    difference = difference.ClampMagnitude(m_Ray);

                    Vector3 oldNubPos1 = m_Nub.rectTransform.position;
                    Vector3 newNubPos1 = oldNubPos1;
                    newNubPos1.x = m_DefaultNubPosition.x + difference.x;
                    newNubPos1.y = m_DefaultNubPosition.y + difference.y;

                    m_Nub.rectTransform.position = newNubPos1;
                }

                m_OldPos = currentPos;

                float xDifference = difference.x;
                float yDifference = difference.y;

                // Check X

                {
                    float sign = (xDifference > 0f) ? 1f : -1f;
                    float diff = Mathf.Abs(xDifference);

                    float percentage = Mathf.Clamp01(diff / m_Ray);

                    float inThresholdValue = Mathf.InverseLerp(m_LowThreshold, m_HighThreshold, percentage);
                    percentage = Mathf.InverseLerp(m_LowThreshold, m_HighThreshold, inThresholdValue);

                    xAxis = sign * percentage;
                }

                // Check Y

                {
                    float sign = (yDifference > 0f) ? 1f : -1f;
                    float diff = Mathf.Abs(yDifference);

                    float percentage = Mathf.Clamp01(diff / m_Ray);

                    float inThresholdValue = Mathf.InverseLerp(m_LowThreshold, m_HighThreshold, percentage);
                    percentage = Mathf.InverseLerp(m_LowThreshold, m_HighThreshold, inThresholdValue);

                    yAxis = sign * percentage;
                }
            }

            m_XAxis = xAxis;
            m_YAxis = yAxis;
        }

        private void OnPointerEndDrag(BaseEventData i_EventData)
        {
            ClearJoystick();
        }

        private void OnPointerUp(BaseEventData i_EventData)
        {
            ClearJoystick();
        }

        private void ClearJoystick()
        {
            if (m_BackImage != null)
            {
                if (m_Nub != null)
                {
                    m_Nub.rectTransform.SetPosition(m_DefaultNubPosition);
                }

                m_BackImage.gameObject.SetActive(false);
            }

            m_XAxis = 0f;
            m_YAxis = 0f;
        }

        // CTOR

        public JoystickClientController(string i_XControlName, string i_YControlName, bool i_SendXAsButton = false, string i_XPositiveButtonControlName = "", string i_XNegativeButtonControlName = "", bool i_SendYAsButton = false, string i_YPositiveButtonControlName = "", string i_YNegativeButtonControlName = "")
        {
            m_XControlName = i_XControlName;
            m_YControlName = i_YControlName;

            m_SendXAsButton = i_SendXAsButton;
            m_SendYAsButton = i_SendYAsButton;

            if (m_SendXAsButton)
            {
                m_XPositiveButtonControlName = i_XPositiveButtonControlName;
                m_XNegativeButtonControlName = i_XNegativeButtonControlName;
            }

            if (m_SendYAsButton)
            {
                m_YPositiveButtonControlName = i_YPositiveButtonControlName;
                m_YNegativeButtonControlName = i_YNegativeButtonControlName;
            }
        }       
    }
}
