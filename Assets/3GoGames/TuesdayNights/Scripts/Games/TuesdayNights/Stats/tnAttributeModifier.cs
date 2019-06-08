using UnityEngine;

using TrueSync;

public class tnAttributeModifier
{
    [SerializeField]
    private int m_AttributeId;
    [SerializeField]
    private ModifierType m_Type;
    [SerializeField]
    private FP m_ModifierValue;

    public int attributeId
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

    // CTOR

    public tnAttributeModifier(tnAttributeModifierDescriptor i_Descriptor)
    {
        if (i_Descriptor == null)
            return;

        m_AttributeId = StringUtils.GetHashCode(i_Descriptor.attributeId);
        m_Type = i_Descriptor.type;
        m_ModifierValue = i_Descriptor.modifierValue;
    }
}
