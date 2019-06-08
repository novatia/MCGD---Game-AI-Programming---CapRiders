using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TypeReferences;

public class tnSubbuteoMatchAIFactoryComponent : MonoBehaviour
{
    // Serializable fields

    [SerializeField]
    [ClassExtends(typeof(tnBaseSubbuteoMatchAIFactory))]
    private ClassTypeReference m_EvenTeamAIFactoryType = null;
    [SerializeField]
    [ClassExtends(typeof(tnBaseSubbuteoMatchAIFactory))]
    private ClassTypeReference m_OddTeamAIFactoryType = null;

    // Fields

    private bool m_SetupDone = false;

    private tnBaseSubbuteoMatchAIFactory[] m_AIFactories = null;
    private int[] m_TeamSizes = null;

    // LOGIC

    public void Setup(int i_TeamCount)
    {
        if (m_SetupDone)
            return;

        if (i_TeamCount < 0)
        {
            return;
        }

        m_AIFactories = new tnBaseSubbuteoMatchAIFactory[i_TeamCount];
        for (int index = 0; index < m_AIFactories.Length; ++index)
        {
            m_AIFactories[index] = null;
        }

        m_TeamSizes = new int[i_TeamCount];
        for (int index = 0; index < m_TeamSizes.Length; ++index)
        {
            m_TeamSizes[index] = 0;
        }

        m_SetupDone = true;
    }

    public void CreateTeamAIFactory(int i_TeamIndex, tnTeamDescription i_TeamDescription)
    {
        if (!m_SetupDone)
            return;

        if (i_TeamIndex < 0 || i_TeamIndex >= m_AIFactories.Length || i_TeamIndex >= m_TeamSizes.Length)
            return;

        if (i_TeamDescription == null)
            return;

        ClassTypeReference newAIFactoryType = (i_TeamIndex % 2 == 0) ? m_EvenTeamAIFactoryType : m_OddTeamAIFactoryType;

        tnBaseSubbuteoMatchAIFactory newAIFactory = CSharpUtils.Cast<tnBaseSubbuteoMatchAIFactory>(Activator.CreateInstance(newAIFactoryType));
        if (newAIFactory != null)
        {
            newAIFactory.Configure(i_TeamDescription);
        }

        m_AIFactories[i_TeamIndex] = newAIFactory;
        m_TeamSizes[i_TeamIndex] = i_TeamDescription.charactersCount;
    }

    public tnBaseAIInputFiller CreateAIInputFiller(int i_TeamIndex, int i_Index, GameObject i_Character)
    {
        if (!m_SetupDone)
        {
            return new tnNullBaseAIInputFiller(i_Character);
        }

        if (i_TeamIndex < 0 || i_TeamIndex >= m_AIFactories.Length)
        {
            return new tnNullBaseAIInputFiller(i_Character);
        }

        tnBaseSubbuteoMatchAIFactory aiFactory = m_AIFactories[i_TeamIndex];
        if (aiFactory != null)
        {
            return aiFactory.CreateAI(i_Index, i_Character);
        }

        return new tnNullBaseAIInputFiller(i_Character);
    }
}