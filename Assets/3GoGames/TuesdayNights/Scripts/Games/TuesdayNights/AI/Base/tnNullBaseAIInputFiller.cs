using UnityEngine;
using System.Collections;

public class tnNullBaseAIInputFiller : tnBaseAIInputFiller
{
    // tnInputFiller's INTERFACE

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {

    }

    public override void Clear()
    {

    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();
    }

    public override void DrawGizmosSelected()
    {
        base.DrawGizmosSelected();
    }

    // CTOR

    public tnNullBaseAIInputFiller(GameObject i_Self)
        : base (i_Self)
    {

    }
}
