using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupPanel : MonoBehaviour
{
    public RectTransform panel = null;
    public float popupTimer = 2f;

    private float m_ElapsedTime = 0f;
    private bool m_Active = false;

    private Text m_Text = null;

    public bool isActive
    {
        get
        {
            return m_Active;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        if (panel != null)
        {
            m_Text = panel.GetComponentInChildren<Text>();
        }
    }

    void OnEnable()
    {
        Hide();

        Popup.RegisterPanelMain(this);
    }

    void OnDisable()
    {
        Popup.UnregisterPanelMain(this);

        Hide();
    }

    void Update()
    {
        if (m_Active)
        {
            m_ElapsedTime += Time.deltaTime;

            if (m_ElapsedTime > popupTimer)
            {
                Hide();
            }
        }
    }

    // BUSINESS LOGIC

    public void ShowMessage(string i_Message)
    {
        Show(i_Message);
    }

    // INTERNALS

    private void Show(string i_Message)
    {
        if (m_Text != null)
        {
            m_Text.text = i_Message;
        }

        if (panel != null)
        {
            panel.gameObject.SetActive(true);
        }

        m_Active = true;
        m_ElapsedTime = 0f;
    }

    private void Hide()
    {
        m_Active = false;

        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }

        if (m_Text != null)
        {
            m_Text.text = "";
        }
    }
}
