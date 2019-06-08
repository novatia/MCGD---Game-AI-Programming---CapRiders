using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class tnCreditsPortrait : MonoBehaviour
{
    [SerializeField]
    private Image m_CharacterImage = null;
    [SerializeField]
    private Text m_CharacterName = null;
    [SerializeField]
    private Text m_CharacterRole = null;

    // BUSINESS LOGIC

    public void SetCharacterSprite(Sprite i_Sprite)
    {
        InternalSetCharacterSprite(i_Sprite);
    }

    public void SetName(string i_Name)
    {
        InternalSetCharacterName(i_Name);
    }

    public void SetRole(string i_Role)
    {
        InternalSetCharacterRole(i_Role);
    }

    public void Clear()
    {
        InternalClear();
    }

    // INTERNALS

    private void InternalSetCharacterSprite(Sprite i_Sprite)
    {
        if (m_CharacterImage == null)
            return;

        m_CharacterImage.sprite = i_Sprite;
    }

    private void InternalSetCharacterName(string i_Name)
    {
        if (m_CharacterName == null)
            return;

        m_CharacterName.text = i_Name;
    }

    private void InternalSetCharacterRole(string i_Role)
    {
        if (m_CharacterRole == null)
            return;

        m_CharacterRole.text = i_Role;
    }

    private void InternalClear()
    {
        InternalSetCharacterSprite(null);
        InternalSetCharacterName("");
        InternalSetCharacterRole("");
    }
}
