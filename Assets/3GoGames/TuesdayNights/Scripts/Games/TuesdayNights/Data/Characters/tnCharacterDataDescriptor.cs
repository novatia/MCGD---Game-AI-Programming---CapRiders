using UnityEngine;

using TuesdayNights;

public class tnCharacterDataDescriptor : ScriptableObject
{
    [SerializeField]
    private string m_FirstName = "";
    [SerializeField]
    private string m_LastName = "";

    [SerializeField]
    private string m_DisplayName = "";

    [SerializeField]
    private CharacterRole m_Role = CharacterRole.Midfielder;
    [SerializeField]
    private int m_Number = 1;

    [SerializeField]
    private bool m_SpecifyPrefab = false;
    [SerializeField]
    private ResourcePath m_PrefabPath = null;

    [SerializeField]
    private Sprite[] m_Left = null;
    [SerializeField]
    private Sprite[] m_Right = null;

    [SerializeField]
    private RuntimeAnimatorController m_AnimatorController = null;

    [SerializeField]
    private Sprite m_UIIconFacingRight = null;
    [SerializeField]
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

    public bool useDifferentPrefab
    {
        get { return m_SpecifyPrefab; }
    }

    public string prefabPath
    {
        get { return m_PrefabPath; }
    }

    public int leftFramesCount
    {
        get { return m_Left.Length; }
    }

    public Sprite[] leftFrames
    {
        get { return m_Left; }
    }

    public int rightFramesCount
    {
        get { return m_Right.Length; }
    }

    public Sprite[] rightFrames
    {
        get { return m_Right; }
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

    public Sprite GetLeftFrame(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Left.Length)
        {
            return null;
        }

        return m_Left[i_Index];
    }

    public Sprite GetRightFrame(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Right.Length)
        {
            return null;
        }

        return m_Right[i_Index];
    }
}
