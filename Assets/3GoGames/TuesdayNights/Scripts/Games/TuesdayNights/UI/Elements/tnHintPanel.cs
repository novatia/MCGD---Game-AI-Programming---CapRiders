using UnityEngine;
using UnityEngine.UI;

using GoUI;

public class tnHintPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Root = null;

    [SerializeField]
    private Text m_Text = null;

    void OnEnable()
    {
        Hide();

        UIEventSystem.onFocusChangedMain += OnFocusChanged;
    }

    void OnDisable()
    {
        UIEventSystem.onFocusChangedMain -= OnFocusChanged;
    }

    // INTERNALS

    private void Show(string i_Message)
    {
        if (m_Root != null)
        {
            m_Root.gameObject.SetActive(true);
        }

        if (m_Text != null)
        {
            m_Text.text = i_Message;
        }
    }

    private void Hide()
    {
        if (m_Root != null)
        {
            m_Root.gameObject.SetActive(false);
        }
    }

    // EVENTS

    void OnFocusChanged(GameObject i_CurrFocus, GameObject i_PrevFocus)
    {
        if (i_CurrFocus != null)
        {
            tnHint hint = i_CurrFocus.GetComponent<tnHint>();

            if (hint != null)
            {
                Show(hint.text);
            }
            else
            {
                Hide();
            }
        }
    }
}
