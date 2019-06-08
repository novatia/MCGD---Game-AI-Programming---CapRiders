using UnityEngine;

using TuesdayNights;

public class tnCharacterData
{
    private string m_FirstName = "";
    private string m_LastName = "";

    private string m_DisplayName = "";

    private CharacterRole m_Role = CharacterRole.Midfielder;
    private int m_Number = 1;

    private bool m_HasSpecificPrefab = false;
    private string m_PrefabPath = "";
    private RuntimeAnimatorController m_AnimatorController = null;

    private Sprite m_UIIconFacingRight = null;
    private Sprite m_UIIconFacingLeft = null;

    public string firstName
    {
        get { return m_FirstName; }
    }

    public string lastName
    {
        get { return m_LastName; }
    }

    public string displayName
    {
        get { return m_DisplayName; }
    }

    public CharacterRole role
    {
        get { return m_Role; }
    }

    public int number
    {
        get { return m_Number; }
    }

    public bool hasSpecificPrefab
    {
        get { return m_HasSpecificPrefab; }
    }

    public string prefabPath
    {
        get { return m_PrefabPath; }
    }

    public RuntimeAnimatorController animatorController
    {
        get { return m_AnimatorController; }
    }

    public Sprite uiIconFacingRight
    {
        get { return m_UIIconFacingRight; }
    }

    public Sprite uiIconFacingLeft
    {
        get { return m_UIIconFacingLeft; }
    }

    // LOGIC

    public GameObject LoadAndGetPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>(m_PrefabPath);
        return prefab;
    }

    // CTOR

    public tnCharacterData(tnCharacterDataDescriptor i_Descriptor)
    {
        if (i_Descriptor != null)
        {
            m_FirstName = i_Descriptor.firstName;
            m_LastName = i_Descriptor.lastName;

            m_DisplayName = i_Descriptor.displayName;

            m_Role = i_Descriptor.role;
            m_Number = i_Descriptor.number;

            m_HasSpecificPrefab = i_Descriptor.useDifferentPrefab;
            m_PrefabPath = i_Descriptor.prefabPath;
            m_AnimatorController = i_Descriptor.animatorController;

            m_UIIconFacingRight = i_Descriptor.uiIconFacingRight;
            m_UIIconFacingLeft = i_Descriptor.uiIconFacingLeft;
        }
    }
}
