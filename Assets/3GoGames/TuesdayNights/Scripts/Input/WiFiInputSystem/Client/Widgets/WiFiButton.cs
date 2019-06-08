using UnityEngine;
using UnityEngine.UI;

namespace WiFiInput.Client
{
    [RequireComponent(typeof(Button))]
    public class WiFiButton : MonoBehaviour
    {
        [SerializeField]
        private string m_ControlName = "";

        private Button m_Button = null;

        private ButtonClientController m_Controller = null;

        // MonoBehaviour's interface

        void Awake()
        {
            m_Button = GetComponent<Button>();

            CreateController();
        }

        void OnEnable()
        {
            m_Controller.Initialize(m_Button);
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
            m_Controller = new ButtonClientController(m_ControlName);
        }
    }
}
