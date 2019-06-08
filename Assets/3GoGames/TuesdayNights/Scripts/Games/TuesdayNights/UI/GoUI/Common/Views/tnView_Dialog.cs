using UnityEngine;
using UnityEngine.UI;

using System;

using GoUI;

public class tnView_Dialog : GoUI.UIView
{
    [SerializeField]
    private RectTransform m_MainPanel = null;

    [SerializeField]
    private Text m_Title = null;
    [SerializeField]
    private Text m_DetailText = null;
    [SerializeField]
    private Button m_ConfirmButton = null;

    private Action m_Callback = null;

    private bool m_IsVisible = false;

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        Internal_SetMainPanelActive(false);
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (m_ConfirmButton != null)
        {
            m_ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (m_ConfirmButton != null)
        {
            m_ConfirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        }
    }

    // LOGIC

    public void SetTitle(string i_Text)
    {
        Internal_SetTitle(i_Text);
    }

    public void SetDeatilText(string i_Text)
    {
        Internal_SetDeatilText(i_Text);
    }

    public void ShowDialog(string i_Title, string i_DetailText, Action i_Callback = null)
    {
        SetTitle(i_Title);
        SetDeatilText(i_DetailText);

        Show(i_Callback);
    }

    public void ShowDialog(Action i_Callback = null)
    {
        if (m_IsVisible)
            return;

        m_Callback = i_Callback;

        m_IsVisible = true;
        Internal_SetMainPanelActive(true);
    }

    // INTERNALS

    private void Internal_SetMainPanelActive(bool i_Active)
    {
        if (m_MainPanel != null)
        {
            m_MainPanel.gameObject.SetActive(i_Active);
        }
    }

    private void Internal_SetTitle(string i_Text)
    {
        if (m_Title != null)
        {
            m_Title.text = i_Text;
        }
    }

    private void Internal_SetDeatilText(string i_Text)
    {
        if (m_DetailText != null)
        {
            m_DetailText.text = i_Text;
        }
    }

    private void Callback()
    {
        if (m_Callback != null)
        {
            m_Callback();
            m_Callback = null;
        }
    }

    // EVENTS

    private void OnConfirmButtonClicked()
    {
        if (!m_IsVisible)
            return;

        Internal_SetMainPanelActive(false);
        m_IsVisible = false;

        Callback();
    }
}