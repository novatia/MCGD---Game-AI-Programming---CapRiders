using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TuesdayNights;


public class mcgd201819AIInputFiller : tnStandardAIInputFillerBase
{
    public override void Clear()
    {
    //    throw new System.NotImplementedException();
    }

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        //throw new System.NotImplementedException();
    }

    // CTOR
    public mcgd201819AIInputFiller(GameObject i_Self)
        : base(i_Self)
    {

    }
}

public class TestAIFactory : tnBaseStandardMatchAIFactory
{
    private List<AIRole> m_Roles = null;
    private int m_AICreated = 0;

    private static AIRole s_DefaultRole = AIRole.Midfielder;

    protected override void OnConfigure(tnTeamDescription i_TeamDescription)
    {
        base.OnConfigure(i_TeamDescription);
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

    public TestAIFactory()
        : base()
    {
        m_Roles = new List<AIRole>();
    }

    private tnStandardAIInputFillerBase CreateInputFiller(AIRole i_Role, GameObject i_Character)
    {
        //return new tnStandardAIInputFiller(i_Character, i_Role);
        return new mcgd201819AIInputFiller(i_Character);
    }
}