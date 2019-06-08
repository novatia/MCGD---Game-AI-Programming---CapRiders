using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TypeReferences;

public class tnStandardMatchAIFactoryComponent : MonoBehaviour
{
    // Serializable fields

    [SerializeField]
    [ClassExtends(typeof(tnBaseStandardMatchAIFactory))]
    private ClassTypeReference m_EvenTeamAIFactoryType = null;
    [SerializeField]
    [ClassExtends(typeof(tnBaseStandardMatchAIFactory))]
    private ClassTypeReference m_OddTeamAIFactoryType = null;

    // Fields

    private bool m_SetupDone = false;

    private tnBaseStandardMatchAIFactory[] m_AIFactories = null;
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

        m_AIFactories = new tnBaseStandardMatchAIFactory[i_TeamCount];
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

        tnBaseStandardMatchAIFactory newAIFactory = CSharpUtils.Cast<tnBaseStandardMatchAIFactory>(Activator.CreateInstance(newAIFactoryType));
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

        tnBaseStandardMatchAIFactory aiFactory = m_AIFactories[i_TeamIndex];
        if (aiFactory != null)
        {
            return aiFactory.CreateAI(i_Index, i_Character);
        }

        return new tnNullBaseAIInputFiller(i_Character);
    }
}