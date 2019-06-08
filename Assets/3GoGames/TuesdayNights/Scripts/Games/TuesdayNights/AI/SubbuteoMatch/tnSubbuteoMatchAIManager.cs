using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class tnBaseSubbuteoMatchAIFactory
{
    // LOGIC

    public void Configure(tnTeamDescription i_TeamDescription)
    {
        OnConfigure(i_TeamDescription);
    }

    public tnSubbuteoAIInputFillerBase CreateAI(int i_Index, GameObject i_Character)
    {
        return OnCreateAI(i_Index, i_Character);
    }

    // Protected - Virtuals.

    protected virtual void OnConfigure(tnTeamDescription i_TeamDescription) { }
    protected virtual tnSubbuteoAIInputFillerBase OnCreateAI(int i_Index, GameObject i_Character) { return new tnSubbuteoAIInputFillerNull(i_Character); }

    // CTOR

    public tnBaseSubbuteoMatchAIFactory()
    {

    }
}

public class tnSubbuteoMatchAIFactory : tnBaseSubbuteoMatchAIFactory
{
    // tnBaseStandardMatchAIFactory's interface

    protected override void OnConfigure(tnTeamDescription i_TeamDescription)
    {

    }

    protected override tnSubbuteoAIInputFillerBase OnCreateAI(int i_Index, GameObject i_Character)
    {
        return new tnSubbuteoAIInputFiller(i_Character);
    }

    // CTOR

    public tnSubbuteoMatchAIFactory()
        : base()
    {

    }
}