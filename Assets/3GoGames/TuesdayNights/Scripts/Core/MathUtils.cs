using UnityEngine;

public static class MathUtils
{
    public static float GetClampedPercentage(float value, float min, float max)
    {
        if (Mathf.Abs(max - min) < 0.00001f)
        {
            return 0f;
        }
        else
        {
            return Mathf.Clamp01((value - min) / (max - min));
        }
    }

    public static bool ApproximatelyEqual(float i_A, float i_B, float i_Tolerance)
    {
        i_Tolerance = Mathf.Clamp01(i_Tolerance);
        float minB = i_B - i_B * i_Tolerance;
        float maxB = i_B + i_B * i_Tolerance;
        return !(i_A < minB || i_A > maxB);
    }

    public static float Square(float i_Value)
	{
		return (i_Value * i_Value);
    }

    public static float GetBellInterpolation2(float i_Value, float i_T0, float i_T1)
	{
		return (1f - Square(2f * (GetClampedPercentage(i_Value, i_T0, i_T1)) - 1f));
	}

    public static float GetBellInterpolation4(float i_Value, float i_T0, float i_T1)
    {
        return (1f - Square(GetBellInterpolation2(i_Value, i_T0, i_T1)));
    }

    public static float GetBellInterpolation8(float i_Value, float i_T0, float i_T1)
    {
        return (1f - Square(GetBellInterpolation4(i_Value, i_T0, i_T1)));
    }

    public static float InterpolateBetweenThresholds(float i_Value, float i_T0, float i_T1, float i_T2, float i_T3)
    {
        if (i_Value <= i_T0)
        {
            return 0f;
        }

        if (i_Value <= i_T1)
        {
            return GetClampedPercentage(i_Value, i_T0, i_T1);
        }

        if (i_Value <= i_T2)
        {
            return 1f;
        }

        if (i_Value <= i_T3)
        {
            return (1f - GetClampedPercentage(i_Value, i_T3, i_T2));
        }

        return 0f;
    }

    public static float GetNormalizedAngle(float i_Angle)
    {
        float normalizedAngle = i_Angle - Mathf.FloorToInt(i_Angle / 360f) * 360f;
        normalizedAngle = Mathf.Clamp(normalizedAngle, 0f, 360f);

        return normalizedAngle;
    }

    public static float ClampAngle(float i_Angle, float i_Min, float i_Max)
    {
        float result = GetNormalizedAngle(i_Angle);
        result = Mathf.Clamp(result, i_Min, i_Max);

        return result;
    }
}
