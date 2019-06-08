using UnityEngine;

namespace GoUI
{
    public enum UINavigationDirection
    {
        Down = 0,
        Left = 1,
        Up = 2,
        Right = 3,
    }

    public enum UIGroup
    {
        Group0 = 0,
        Group1 = 1,
        Group2 = 2,
        Group3 = 3,
        Group4 = 4,
        Group5 = 5,
        Group6 = 6,
        Group7 = 7,
        GroupsCount = 8,
    }

    public static class UIPivot
    {
        public static Vector2 s_TopLeft = new Vector2(0f, 1f);
        public static Vector2 s_TopCenter = new Vector2(0.5f, 1f);
        public static Vector2 s_TopRight = new Vector2(1f, 1f);

        public static Vector2 s_MiddleLeft = new Vector2(0f, 0.5f);
        public static Vector2 s_MiddleCenter = new Vector2(0.5f, 0.5f);
        public static Vector2 s_MiddleRight = new Vector2(1f, 0.5f);

        public static Vector2 s_BottomLeft = new Vector2(0f, 0f);
        public static Vector2 s_BottomCenter = new Vector2(0.5f, 0f);
        public static Vector2 s_BottomRight = new Vector2(1f, 0f);
    }

    public static class UIAnchor
    {
        // Top

        public static Vector4 s_TopLeft = new Vector4(0f, 1f, 0f, 1f);
        public static Vector4 s_TopCenter = new Vector4(0.5f, 1f, 0.5f, 1f);
        public static Vector4 s_TopRight = new Vector4(1f, 1f, 1f, 1f);

        // Middle

        public static Vector4 s_MiddleLeft = new Vector4(0f, 0.5f, 0f, 0.5f);
        public static Vector4 s_MiddleCenter = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        public static Vector4 s_MiddleRight = new Vector4(1f, 0.5f, 1f, 0.5f);

        // Left

        public static Vector4 s_BottomLeft = new Vector4(0f, 0f, 0f, 0f);
        public static Vector4 s_BottomCenter = new Vector4(0.5f, 0f, 0.5f, 0f);
        public static Vector4 s_BottomRight = new Vector4(1f, 0f, 1f, 0f);

        // Stretch

        // Horizontal

        public static Vector4 s_HorizontalTop = new Vector4(0f, 1f, 1f, 1f);
        public static Vector4 s_HorizontalCenter = new Vector4(0f, 0.5f, 1f, 0.5f);
        public static Vector4 s_HorizontalBottom = new Vector4(0f, 0f, 1f, 0f);

        // Vertical

        public static Vector4 s_VerticalLeft = new Vector4(0f, 0f, 0f, 1f);
        public static Vector4 s_VerticalCenter = new Vector4(0.5f, 0f, 0.5f, 1f);
        public static Vector4 s_VerticalRight = new Vector4(1f, 0f, 1f, 1f);

        // Total

        public static Vector4 s_Stretch = new Vector4(0f, 0f, 1f, 1f);
    }
}