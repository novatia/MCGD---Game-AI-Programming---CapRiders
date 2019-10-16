using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAIFactory : tnBaseStandardMatchAIFactory
{
    protected override void OnConfigure(tnTeamDescription i_TeamDescription)
    {
        base.OnConfigure(i_TeamDescription);
    }

    protected override tnStandardAIInputFillerBase OnCreateAI(int i_Index, GameObject i_Character)
    {


        return base.OnCreateAI(i_Index, i_Character);
    }

    public TestAIFactory()
        : base()
    {

    }
}