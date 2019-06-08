using UnityEngine;

using TuesdayNights;

using System.Collections.Generic;

public class tnBaseStandardMatchAIFactory
{
    // LOGIC

    public void Configure(tnTeamDescription i_TeamDescription)
    {
        OnConfigure(i_TeamDescription);
    }

    public tnStandardAIInputFillerBase CreateAI(int i_Index, GameObject i_Character)
    {
        return OnCreateAI(i_Index, i_Character);
    }

    // Protected - Virtuals.

    protected virtual void OnConfigure(tnTeamDescription i_TeamDescription) { }
    protected virtual tnStandardAIInputFillerBase OnCreateAI(int i_Index, GameObject i_Character) { return new tnStandardAIInputFillerNull(i_Character); }

    // CTOR

    public tnBaseStandardMatchAIFactory()
    {

    }
}

public class tnStandardMatchAIFactory : tnBaseStandardMatchAIFactory
{
    // STATIC

    private static AIRole[] s_Roles_1 = new AIRole[]
    {
        AIRole.Midfielder,
    };

    private static AIRole[] s_Roles_2 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
    };

    private static AIRole[] s_Roles_3 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
    };

    private static AIRole[] s_Roles_4 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
    };

    private static AIRole[] s_Roles_5 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
    };

    private static AIRole[] s_Roles_6 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
    };

    private static AIRole[] s_Roles_7 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
    };

    private static AIRole[] s_Roles_8 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
    };

    private static AIRole[] s_Roles_9 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
    };

    private static AIRole[] s_Roles_10 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
    };

    private static AIRole[] s_Roles_11 = new AIRole[]
    {
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
        AIRole.Midfielder,
        AIRole.Defender,
        AIRole.Striker,
    };

    private static AIRole[][] s_Roles = new AIRole[][]
    {
        s_Roles_1,
        s_Roles_2,
        s_Roles_3,
        s_Roles_4,
        s_Roles_5,
        s_Roles_6,
        s_Roles_7,
        s_Roles_8,
        s_Roles_9,
        s_Roles_10,
        s_Roles_11,
    };

    private static AIRole s_DefaultRole = AIRole.Midfielder;

    // Fields

    private List<AIRole> m_Roles = null;
    private int m_AICreated = 0;

    // tnBaseStandardMatchAIFactory's interface

    protected override void OnConfigure(tnTeamDescription i_TeamDescription)
    {
        if (i_TeamDescription == null)
            return;

        int charactersCount = i_TeamDescription.charactersCount;

        if (charactersCount <= 0 || charactersCount >= s_Roles.Length)
            return;

        AIRole[] roles = s_Roles[charactersCount - 1];

        if (roles == null || roles.Length == 0 || roles.Length != charactersCount)
            return;

        int aiIndex = 0;

        for (int characterIndex = 0; characterIndex < charactersCount; ++characterIndex)
        {
            tnCharacterDescription characterDescription = i_TeamDescription.GetCharacterDescription(characterIndex);

            if (characterDescription == null)
                continue;

            int playerId = characterDescription.playerId;
            tnPlayerData playerData = tnGameData.GetPlayerDataMain(playerId);

            if (playerData == null)
            {
                AIRole role = roles[aiIndex++];
                m_Roles.Add(role);
            }
        }

        m_Roles.Sort();
    }

    protected override tnStandardAIInputFillerBase OnCreateAI(int i_Index, GameObject i_Character)
    {
        if (m_Roles.Count == 0 || m_AICreated >= m_Roles.Count)
        {
            return CreateInputFiller(s_DefaultRole, i_Character);
        }

        AIRole role = m_Roles[m_AICreated++];
        return CreateInputFiller(role, i_Character);
    }

    // INTERNALS

    private tnStandardAIInputFillerBase CreateInputFiller(AIRole i_Role, GameObject i_Character)
    {
        //return new tnStandardAIInputFillerNull(i_Character);
        return new tnStandardAIInputFiller(i_Character, i_Role);
    }

    // CTOR

    public tnStandardMatchAIFactory()
        : base()
    {
        m_Roles = new List<AIRole>();
    }
}