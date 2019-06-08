using UnityEngine;
using UnityEngine.UI;

public class NonDrawableImage : Graphic, ICanvasRaycastFilter
{
    public override void SetMaterialDirty() { return; }
    public override void SetVerticesDirty() { return; }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        return;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }
}
