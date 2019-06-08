using UnityEngine;
using UnityEngine.UI;

public class tnUIEnergySlot : MonoBehaviour
{
    [SerializeField]
    private Slider m_Slider = null;
    [SerializeField]
    private Image m_FillImage = null;
    
    // GETTERS

    public bool isFull
    {
        get
        {
            if (m_Slider == null)
            {
                return false;
            }

            return (m_Slider.value == m_Slider.maxValue);
        }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        if (m_Slider == null)
            return;

        m_Slider.minValue = 0f;
        m_Slider.maxValue = 1f;
    }

    // LOGIC

    public void Clear()
    {
        if (m_Slider == null)
            return;

        SetValue(m_Slider.minValue);   
    }

    public void Fill()
    {
        if (m_Slider == null)
            return;

        SetValue(m_Slider.maxValue);
    }

    public void SetValue(float i_Value)
    {
        InternalSetValue(i_Value);
    }

    public void SetColor(Color i_Color)
    {
        InternalSetColor(i_Color);
    }

    // INTERNALS

    private void InternalSetValue(float i_Value)
    {
        if (m_Slider != null)
        {
            float value = Mathf.Clamp(i_Value, m_Slider.minValue, m_Slider.maxValue);
            m_Slider.value = value;

            SetFillImageEnable(m_Slider.maxValue == m_Slider.value);
        }
    }

    private void InternalSetColor(Color i_Color)
    {
        if (m_FillImage != null)
        {
            m_FillImage.color = i_Color;
        }
    }

    private void SetFillImageEnable(bool i_Enable)
    {
        if (m_FillImage != null)
        {
            m_FillImage.enabled = i_Enable;
        }
    }
}
