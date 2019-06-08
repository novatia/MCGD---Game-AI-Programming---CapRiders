using UnityEngine;
using System.Collections;

public class tnCreditsData
{
    private string m_FirstName = "";
    private string m_LastName = "";

    private string m_Nickname = "";

    private string m_Role = "";

    private RuntimeAnimatorController m_AnimatorController = null;

    private Sprite m_BaseSprite = null;
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

    public tnCreditsData(tnCreditsDataDescriptor i_Descriptor)
    {
        if (i_Descriptor != null)
        {
            m_FirstName = i_Descriptor.firstName;
            m_LastName = i_Descriptor.lastName;

            m_Nickname = i_Descriptor.nickname;

            m_Role = i_Descriptor.role;

            m_AnimatorController = i_Descriptor.animatorController;

            m_BaseSprite = i_Descriptor.baseSprite;
            m_CharacterSprite = i_Descriptor.characterSprite;
        }
    }
}
