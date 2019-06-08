using UnityEngine;

public static class RectTransformUtils
{
    public static Vector3 GetPosition3(this RectTransform i_RectTransform)
    {
        Vector3 position = i_RectTransform.position;
        return position;
    }

    public static Vector2 GetPosition2(this RectTransform i_RectTransform)
    {
        Vector3 position = i_RectTransform.position;
        Vector2 v2 = new Vector2(position.x, position.y);
        return v2;
    }

    public static void SetPosition(this RectTransform i_RectTransform, Vector2 i_Position)
    {
        Vector3 position = i_RectTransform.GetPosition3();
        Vector3 targetPosition = new Vector3(i_Position.x, i_Position.y, position.z);
        i_RectTransform.SetPosition(targetPosition);
    }

    public static void SetPosition(this RectTransform i_RectTransform, Vector3 i_Position)
    {
        i_RectTransform.position = i_Position;
    }

    public static void SetAnchor(this RectTransform i_RectTransform, float i_MinX, float i_MinY, float i_MaxX, float i_MaxY)
    {
        Vector4 anchor = new Vector4(i_MinX, i_MinY, i_MaxX, i_MaxY);
        i_RectTransform.SetAnchor(anchor);
    }

    public static void SetAnchor(this RectTransform i_RectTransform, Vector4 i_Anchor)
    {
        i_RectTransform.anchorMin = new Vector2(i_Anchor.x, i_Anchor.y);
        i_RectTransform.anchorMax = new Vector2(i_Anchor.z, i_Anchor.w);
    }
}
