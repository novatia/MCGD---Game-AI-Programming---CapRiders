using UnityEngine;

using System;

using TrueSync;

[Serializable]
public class tnAttributeModifierDescriptor
{
    [SerializeField]
    private string m_AttributeId = "";
    [SerializeField]
    private ModifierType m_Type = ModifierType.Add;
    [SerializeField]
    private FP m_ModifierValue = 0f;

    public string attributeId
    {
        get { return m_AttributeId; }
    }

    public ModifierType type
    {
        get { return m_Type; }
    }

    public FP modifierValue
    {
        get { return m_ModifierValue; }
    }
}
