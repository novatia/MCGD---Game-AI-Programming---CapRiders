using UnityEngine;

using WiFiInput.Client;

public class WiFiAxis : MonoBehaviour
{
    [SerializeField]
    private string m_ControlName = "";

    private AxisClientController m_Controller = null;

    // MonoBehaviour's interface

    void Awake()
    {
        CreateController();
    }

    void Update()
    {
        if (m_Controller == null)
            return;

        m_Controller.OnUpdate();
    }

    // INTERNALS

    private void CreateController()
    {
        m_Controller = new AxisClientController(m_ControlName);
        m_Controller.Initialize();
    }
}
