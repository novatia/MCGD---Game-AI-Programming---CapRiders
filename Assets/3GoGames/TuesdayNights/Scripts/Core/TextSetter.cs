using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextSetter : MonoBehaviour
{
    public enum ForcedCase
    {
        None,
        UpperCase,
        LowerCase,
    }

    [SerializeField]
    private string m_TextId = "";
    [SerializeField]
    private ForcedCase m_ForcedCase = ForcedCase.None;

    private Text m_Text = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_Text = GetComponent<Text>();

        string text;
        if (!TextManager.TryGetTextMain(m_TextId, out text))
        {
            text = StringUtils.s_INVALID;
        }

        if (m_ForcedCase != ForcedCase.None)
        {
            if (m_ForcedCase == ForcedCase.UpperCase)
            {
                text = text.ToUpper();
            }
            else // LowerCase
            {
                text = text.ToLower();
            }
        }

        m_Text.text = text;
    }
}
