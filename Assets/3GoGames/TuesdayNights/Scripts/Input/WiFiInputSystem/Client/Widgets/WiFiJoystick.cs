using UnityEngine;
using UnityEngine.UI;

namespace WiFiInput.Client
{
    public class WiFiJoystick : MonoBehaviour
    {
        [SerializeField]
        private string m_XControlName = "";
        [SerializeField]
        private string m_YControlName = "";
        [SerializeField]
        private bool m_SendXAsButton = false;
        [SerializeField]
        private string m_XPositiveButtonControlName = "";
        [SerializeField]
        private string m_XNegativeButtonControlName = "";
        [SerializeField]
        [Range(0f, 1f)]
        private float m_XButtonThreshold = 0.5f;
        [SerializeField]
        private bool m_SendYAsButton = false;
        [SerializeField]
        private string m_YPositiveButtonControlName = "";
        [SerializeField]
        private string m_YNegativeButtonControlName = "";
        [SerializeField]
        [Range(0f, 1f)]
        private float m_YButtonThreshold = 0.5f;
        [SerializeField]
        private RectTransform m_BackPanel = null;
        [SerializeField]
        private Image m_BackImage = null;
        [SerializeField]
        private Image m_NubImage = null;
        [SerializeField]
        [Range(0f, 1f)]
        private float m_LowThreshold = 0.1f;
        [SerializeField]
        [Range(0f, 1f)]
        private float m_HighThreshold = 0.6f;

        private JoystickClientController m_Controller = null;

        // MonoBehaviour's interface

        void Awake()
        {
            CreateController();
        }

        void OnEnable()
        {
            m_Controller.Initialize(m_BackPanel, m_BackImage, m_NubImage, m_LowThreshold, m_HighThreshold, m_XButtonThreshold, m_YButtonThreshold);
        }

        void OnDisable()
        {
            m_Controller.Clear();
        }

        void Update()
        {
            m_Controller.OnUpdate();
        }

        // INTERNALS

        private void CreateController()
        {
            m_Controller = new JoystickClientController(m_XControlName, m_YControlName, m_SendXAsButton, m_XPositiveButtonControlName, m_XNegativeButtonControlName, m_SendYAsButton, m_YPositiveButtonControlName, m_YNegativeButtonControlName);
        }
    }
}