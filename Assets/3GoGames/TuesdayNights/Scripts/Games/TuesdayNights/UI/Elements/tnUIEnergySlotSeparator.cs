using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class tnUIEnergySlotSeparator : MonoBehaviour
{
    private Image m_Image = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    // LOGIC

    public void SetColor(Color i_Color)
    {
        m_Image.color = i_Color;
    }
}
