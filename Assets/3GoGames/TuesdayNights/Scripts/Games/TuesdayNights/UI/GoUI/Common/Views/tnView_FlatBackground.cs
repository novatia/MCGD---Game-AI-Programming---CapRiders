using UnityEngine;
using UnityEngine.UI;

using GoUI;

public class tnView_FlatBackground : GoUI.UIView
{
    // Serializable fields

    [Header("Widgets")]

    [SerializeField]
    private Image m_Background = null;

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // LOGIC

    public void SetBackgroundColor(Color i_Color)
    {
        if (m_Background != null)
        {
            m_Background.color = i_Color;
        }
    }

    public void SetBackgroundColorWithoutAlpha(Color i_Color)
    {
        if (m_Background != null)
        {
            m_Background.SetColorWithoutAlpha(i_Color);
        }
    }

    public void SetBackgroundAlpha(float i_Alpha)
    {
        if (m_Background != null)
        {
            m_Background.SetColorAlpha(i_Alpha);
        }
    }
}