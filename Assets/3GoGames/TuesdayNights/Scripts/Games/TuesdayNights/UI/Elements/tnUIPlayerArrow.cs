using UnityEngine;
using System.Collections;

public class tnUIPlayerArrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_Arrow = null;

    private bool m_Visible = true;

    private tnSubbuteoController m_SubbuteoController = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_SubbuteoController = GetComponentInParent<tnSubbuteoController>();
    }

    void Start()
    {
        UpdateVisual();
    }

    void Update()
    {
        UpdateVisual();
    }

    // BUSINESS LOGIC

    public void SetVisible(bool i_Visible)
    {
        m_Visible = i_Visible;
    }

    public void SetColor(Color i_Color)
    {
        if (m_Arrow != null)
        {
            m_Arrow.color = i_Color;
        }
    }

    // INTERNALS

    private void UpdateVisual()
    {
        if (m_SubbuteoController == null || m_Arrow == null)
            return;

        float moveDirectionX = Mathf.Clamp((float)(m_SubbuteoController.moveDirection.x), -1f, 1f);
        float moveDirectionY = Mathf.Clamp((float)(m_SubbuteoController.moveDirection.y), -1f, 1f);

        float absMoveDirectionX = Mathf.Abs(moveDirectionX);
        float absMoveDirectionY = Mathf.Abs(moveDirectionY);

        bool showArrow = m_Visible;
        // showArrow = showArrow && m_SubbuteoController.isCharging;
        showArrow = showArrow && !m_SubbuteoController.isInCooldown;
        showArrow = showArrow && ((absMoveDirectionX > 0.1f) || (absMoveDirectionY > 0.1f));

        if (showArrow)
        {
            if (m_Arrow != null)
            {
                m_Arrow.enabled = true;

                float arcSin = Mathf.Asin(moveDirectionY);
                float angle = Mathf.Rad2Deg * arcSin;

                if (moveDirectionX < 0f)
                {
                    angle = 180f - angle;
                }

                m_Arrow.transform.localEulerAngles = new Vector3(0f, 0f, angle);
            }
        }
        else
        {
            if (m_Arrow != null)
            {
                m_Arrow.enabled = false;
            }
        }
    }
}
