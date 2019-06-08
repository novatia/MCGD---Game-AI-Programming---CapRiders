using UnityEngine;
using System.Collections;

public class tnCreditsDataDescriptor : ScriptableObject
{
    [SerializeField]
    private string m_FirstName = "";
    [SerializeField]
    private string m_LastName = "";

    [SerializeField]
    private string m_Nickname = "";

    [SerializeField]
    private string m_Role = "";

    [SerializeField]
    private RuntimeAnimatorController m_AnimatorController = null;

    [SerializeField]
    private Sprite m_BaseSprite = null;
    [SerializeField]
    private Sprite m_CharacterSprite = null;

    public string firstName
    {
        get
        {
            return m_FirstName;
        }
    }

    public string lastName
    {
        get
        {
            return m_LastName;
        }
    }

    public string nickname
    {
        get
        {
            return m_Nickname;
        }
    }

    public string role
    {
        get
        {
            return m_Role;
        }
    }

    public RuntimeAnimatorController animatorController
    {
        get
        {
            return m_AnimatorController;
        }
    }

    public Sprite baseSprite
    {
        get
        {
            return m_BaseSprite;
        }
    }

    public Sprite characterSprite
    {
        get
        {
            return m_CharacterSprite;
        }
    }
}
