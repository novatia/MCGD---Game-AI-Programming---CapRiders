using UnityEngine;

public static class ColorUtils
{
    public static bool ApproximatelyEqual(Color i_C1, Color i_C2, float i_RThreshold = 0f, float i_GThreshold = 0f, float i_BThreshold = 0f, float i_AThreshold = 0f)
    {
        i_RThreshold = Mathf.Clamp01(i_RThreshold);
        i_GThreshold = Mathf.Clamp01(i_GThreshold);
        i_BThreshold = Mathf.Clamp01(i_BThreshold);
        i_AThreshold = Mathf.Clamp01(i_AThreshold);

        float r1 = i_C1.r;
        float r2 = i_C2.r;

        float g1 = i_C1.g;
        float g2 = i_C2.g;

        float b1 = i_C1.b;
        float b2 = i_C2.b;

        float a1 = i_C1.a;
        float a2 = i_C2.a;

        float rDiff = Mathf.Abs(r1 - r2);
        float gDiff = Mathf.Abs(g1 - g2);
        float bDiff = Mathf.Abs(b1 - b2);
        float aDiff = Mathf.Abs(a1 - a2);

        bool equal = (rDiff <= i_RThreshold) && (gDiff <= i_GThreshold) && (bDiff <= i_BThreshold) && (aDiff <= i_AThreshold);

        return equal;
    }
}
