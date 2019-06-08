using UnityEngine;

public class tnStandardAIInputFillerNull : tnStandardAIInputFillerBase
{
    // tnInputFiller's INTERFACE

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        ResetInputData(i_Data);
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

    public tnStandardAIInputFillerNull(GameObject i_Self)
        : base (i_Self)
    {

    }
}
