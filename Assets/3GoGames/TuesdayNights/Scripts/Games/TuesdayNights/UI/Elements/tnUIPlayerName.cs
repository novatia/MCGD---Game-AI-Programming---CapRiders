using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class tnUIPlayerName : MonoBehaviour
{
    private Text m_Text = null;

    void Awake()
    {
        m_Text = GetComponent<Text>();
    }

    void Start()
    {
        tnCharacterInfo characterInfo = GetComponentInParent<tnCharacterInfo>();
        if (characterInfo != null)
        {
            int characterId = characterInfo.characterId;
            tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);
            if (characterData != null)
            {
                string s = characterData.displayName;
                m_Text.text = s;
            }
        }
    }

    // BUSINESS LOGIC

    public void SetVisible(bool i_Visible)
    {
        m_Text.enabled = i_Visible;
    }

    public void SetText(string i_Text)
    {
        // TODO: Bind text set here.
    }
}
