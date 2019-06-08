using UnityEngine;

using System;

[Serializable]
public class UserStatDescriptor
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private UserStatType m_Type = UserStatType.Invalid;

    [SerializeField]
    private bool m_LowLimit = false;
    [SerializeField]
    private bool m_HighLimit = false;

    [SerializeField]
    private int m_DefaultIntValue = 0;
    [SerializeField]
    private int m_MinIntValue = 0;
    [SerializeField]
    private int m_MaxIntValue = 0;

    [SerializeField]
    private bool m_DefaultBoolValue = false;

    [SerializeField]
    private float m_DefaultFloatValue = 0f;
    [SerializeField]
    private float m_MinFloatValue = 0f;
    [SerializeField]
    private float m_MaxFloatValue = 0f;

    [SerializeField]
    private string m_DefaultStringValue = "";

    [SerializeField]
    private NumericCombineFunction m_NumericCombineFunction = NumericCombineFunction.Set;
    [SerializeField]
    private BooleanCombineFunction m_BooleanCombineFunction = BooleanCombineFunction.Set;
    [SerializeField]
    private StringCombineFunction m_StringCombineFunction = StringCombineFunction.Set;

    public string id
    {
        get { return m_Id; }
    }

    public UserStatType type
    {
        get { return m_Type; }
    }

    public bool usingHighLimit
    {
        get { return m_HighLimit; }
    }

    public bool usingLowLimit
    {
        get { return m_LowLimit; }
    }

    public int defaultIntValue
    {
        get { return m_DefaultIntValue; }
    }

    public int minIntValue
    {
        get { return m_MinIntValue; }
    }

    public int maxIntValue
    {
        get { return m_MaxIntValue; }
    }

    public bool defaultBoolValue
    {
        get { return m_DefaultBoolValue; }
    }

    public float defaultFloatValue
    {
        get { return m_DefaultFloatValue; }
    }

    public float minFloatValue
    {
        get { return m_MinFloatValue; }
    }

    public float maxFloatValue
    {
        get { return m_MaxFloatValue; }
    }

    public string defaultStringValue
    {
        get { return m_DefaultStringValue; }
    }

    public NumericCombineFunction numericCombineFunction
    {
        get { return m_NumericCombineFunction; }
    }

    public BooleanCombineFunction booleanCombineFunction
    {
        get { return m_BooleanCombineFunction; }
    }

    public StringCombineFunction stringCombineFunction
    {
        get { return m_StringCombineFunction; }
    }
}
