using UnityEngine;

using System;

using FullInspector;

[fiInspectorOnly]
public class tnMenuMatchController : tnBaseMatchController
{
    // Fields

    private tnStandardMatchAIFactory[] m_AIFactories = null;
    private int[] m_TeamSize = null;

    // tnMatchController's interface

    protected override void OnPreInit()
    {
        base.OnPreInit();

        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null || teamsModule.teamsCount == 0)
            return;

        m_AIFactories = new tnStandardMatchAIFactory[teamsModule.teamsCount];
        for (int index = 0; index < m_AIFactories.Length; ++index)
        {
            m_AIFactories[index] = null;
        }

        m_TeamSize = new int[teamsModule.teamsCount];
        for (int index = 0; index < m_TeamSize.Length; ++index)
        {
            m_TeamSize[index] = 0;
        }
    }

    protected override void OnCreateTeam(int i_TeamIndex, tnTeamDescription i_TeamDescription)
    {
        base.OnCreateTeam(i_TeamIndex, i_TeamDescription);

        if (i_TeamIndex < 0 || i_TeamIndex >= m_AIFactories.Length || i_TeamIndex >= m_TeamSize.Length)
            return;

        if (i_TeamDescription == null)
            return;

        tnStandardMatchAIFactory aiFactory = new tnStandardMatchAIFactory();
        aiFactory.Configure(i_TeamDescription);

        m_AIFactories[i_TeamIndex] = aiFactory;
        m_TeamSize[i_TeamIndex] = i_TeamDescription.charactersCount;
    }

    protected override tnCharacterResults CreateCharacterResults(int i_Id)
    {
        return new tnMenuMatchCharacterResults(i_Id);
    }

    protected override tnTeamResults CreateTeamResults(int i_Id)
    {
        return new tnMenuMatchTeamResults(i_Id);
    }

    protected override Comparison<tnTeamResults> GetSortDelegate()
    {
        return SortFunctor;
    }

    protected override void OnSetupMatch()
    {
        base.OnSetupMatch();

        RequestStartGame();
    }

    // tnBaseMatchController's interface

    protected override bool Draw()
    {
        return base.Draw();
    }

    protected override void OnKickOff()
    {
        base.OnKickOff();

        // Enable energy recovery.

        SetEnergyRecoveryEnabled(true);
    }

    protected override void OnResetField()
    {
        base.OnResetField();

        // Disable energy recovery.

        SetEnergyRecoveryEnabled(false);
    }

    protected override tnBaseAIInputFiller CreateBaseAIInputFiller(int i_TeamIndex, int i_Index, GameObject i_Character)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= m_AIFactories.Length)
        {
            return new tnNullBaseAIInputFiller(i_Character);
        }

        tnStandardMatchAIFactory aiFactory = m_AIFactories[i_TeamIndex];
        if (aiFactory != null)
        {
            return aiFactory.CreateAI(i_Index, i_Character);
        }

        return new tnNullBaseAIInputFiller(i_Character); // Invalid AI Factory, create null input filler.
    }

    // UTILS

    private void SetEnergyRecoveryEnabled(bool i_Enabled)
    {
        for (int index = 0; index < charactersCount; ++index)
        {
            GameObject character = GetCharacterByIndex(index);

            if (character == null)
                continue;

            tnEnergy energy = character.GetComponent<tnEnergy>();
            if (energy != null)
            {
                energy.SetAutorecoveryEnabled(i_Enabled);
            }
        }
    }

    // FUNCTORS

    private static int SortFunctor(tnTeamResults i_Res1, tnTeamResults i_Res2)
    {
        return 0;
    }
}