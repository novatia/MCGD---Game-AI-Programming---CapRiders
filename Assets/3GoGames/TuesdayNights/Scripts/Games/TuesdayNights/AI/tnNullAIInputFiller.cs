using UnityEngine;

public class tnAINullInputFiller : tnAIInputFiller
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

    }

    public override void DrawGizmosSelected()
    {

    }

    // CTOR

    public tnAINullInputFiller(GameObject i_Self)
        : base (i_Self)
    {

    }
}
