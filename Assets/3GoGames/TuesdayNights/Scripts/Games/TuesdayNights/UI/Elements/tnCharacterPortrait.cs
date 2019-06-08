using UnityEngine;
using UnityEngine.UI;

public class tnCharacterPortrait : MonoBehaviour
{
    [SerializeField]
    private Image m_CharacterPortrait = null;
    [SerializeField]
    private Text m_CharacterName = null;
    [SerializeField]
    private Text m_Info1 = null;
    [SerializeField]
    private Text m_Info2 = null;

    // LOGIC

    public void SetCharacterPortrait(Sprite i_Sprite)
    {
        InternalSetCharacterPortrait(i_Sprite);
    }

    public void SetName(string i_Name)
    {
        InternalSetCharacterName(i_Name);
    }

    public void SetInfo1(string i_Info1)
    {
        InternalSetInfo1(i_Info1);
    }

    public void SetInfo2(string i_Info2)
    {
        InternalSetInfo2(i_Info2);
    }

    public void Clear()
    {
        InternalClear();
    }

    // INTERNALS

    private void InternalSetCharacterPortrait(Sprite i_Sprite)
    {
        if (m_CharacterPortrait == null)
            return;

        m_CharacterPortrait.sprite = i_Sprite;
    }

    private void InternalSetCharacterName(string i_Name)
    {
        if (m_CharacterName == null)
            return;

        m_CharacterName.text = i_Name;
    }

    private void InternalSetInfo1(string i_Info1)
    {
        if (m_Info1 == null)
            return;

        m_Info1.text = i_Info1;
    }

    private void InternalSetInfo2(string i_Info2)
    {
        if (m_Info2 == null)
            return;

        m_Info2.text = i_Info2;
    }

    private void InternalClear()
    {
        InternalSetCharacterPortrait(null);

        InternalSetCharacterName("");

        InternalSetInfo1("");
        InternalSetInfo2("");
    }
}
