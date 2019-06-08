using UnityEngine;
using UnityEngine.UI;

public class tnUIChargingForceBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Root = null;

    [SerializeField]
    private Slider m_Slider = null;
    [SerializeField]
    private Image m_FillImage = null;

    private bool m_Visible = true;

    private tnSubbuteoController m_SubbuteoController = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_SubbuteoController = GetComponentInParent<tnSubbuteoController>();

        if (m_Slider != null)
        {
            m_Slider.minValue = 0f;
            m_Slider.maxValue = 1f;
        }
    }

    void Start()
    {
        UpdateVisual();
    }

    void Update()
    {
        UpdateVisual();
    }

    // LOGIC

    public void SetVisible(bool i_Visile)
    {
        m_Visible = i_Visile;
    }

    public void SetColor(Color i_Color)
    {
        if (m_FillImage != null)
        {
            m_FillImage.color = i_Color;
        }
    }

    // INTERNALS

    private void UpdateVisual()
    {
        if (m_SubbuteoController == null)
            return;

        if (m_Root == null || m_Slider == null)
            return;

        if (m_SubbuteoController.isCharging && m_Visible)
        {
            m_Root.gameObject.SetActive(true);

            m_Slider.value = (float)m_SubbuteoController.chargeLevel;

            if (m_Slider.value == m_Slider.maxValue)
            {
                if (m_FillImage != null)
                {
                    m_FillImage.enabled = true;
                }
            }
        }
        else
        {
            m_Slider.value = 0f;

            if (m_FillImage != null)
            {
                m_FillImage.enabled = false;
            }

            m_Root.gameObject.SetActive(false);
        }
    }
}
