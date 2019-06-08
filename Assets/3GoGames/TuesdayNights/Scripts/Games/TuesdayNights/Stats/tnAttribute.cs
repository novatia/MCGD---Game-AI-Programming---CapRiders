using UnityEngine;

using System;
using System.Collections.Generic;

using TrueSync;

public delegate void tnStatChangedCallback(FP i_BaseValue, FP i_Value);

[Serializable]
public class tnAttribute
{
    [SerializeField]
    private FP m_BaseValue;

    private List<tnAttributeModifier> m_Modifiers = null;

    public tnStatChangedCallback statChangedEvent;

    // ACCESSORS

    public FP baseValue
    {
        get { return m_BaseValue; }
        set
        {
            m_BaseValue = value;
            OnStatChangedEvent();
        }
    }

    public FP value
    {
        get
        {
            return EvaluateStat();
        }
    }

    public int modifiersCount
    {
        get
        {
            return m_Modifiers.Count;
        }
    }

    // BUSINESS LOGIC

    public tnAttributeModifier GetModifier(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Modifiers.Count)
        {
            return null;
        }

        return m_Modifiers[i_Index];
    }

    public void AddModifier(tnAttributeModifier i_Modifier)
    {
        if (m_Modifiers == null)
            return;

        m_Modifiers.Add(i_Modifier);
        OnStatChangedEvent();
    }

    public void RemoveModifier(tnAttributeModifier i_Modifier)
    {
        if (i_Modifier == null)
            return;

        m_Modifiers.Remove(i_Modifier);
        OnStatChangedEvent();
    }

    // EVENTS

    private void OnStatChangedEvent()
    {
        if (statChangedEvent != null)
        {
            statChangedEvent(baseValue, value);
        }
    }

    // INTERNALS

    private FP EvaluateStat()
    {
        FP currentValue = m_BaseValue;

        // Additive modifiers.

        for (int modifierIndex = 0; modifierIndex < m_Modifiers.Count; ++modifierIndex)
        {
            tnAttributeModifier modifier = m_Modifiers[modifierIndex];
            if (modifier.type == ModifierType.Add)
            {
                currentValue += modifier.modifierValue;
            }
        }

        // Multiplicative modifiers.

        for (int modifierIndex = 0; modifierIndex < m_Modifiers.Count; ++modifierIndex)
        {
            tnAttributeModifier modifier = m_Modifiers[modifierIndex];
            if (modifier.type == ModifierType.Multiply)
            {
                currentValue *= modifier.modifierValue;
            }
        }

        return currentValue;
    }

    // CTOR

    public tnAttribute(FP i_BaseValue)
    {
        m_BaseValue = i_BaseValue;
        m_Modifiers = new List<tnAttributeModifier>();
    }
}
