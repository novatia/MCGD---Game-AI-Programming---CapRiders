using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float GetAngle(this Vector2 v)
    {
        float angle = Vector2.Angle(Vector2.up, v);
        if (v.x > 0f)
        {
            angle *= -1;
        }

        return angle;
    }

    public static float GetAngle(this Vector2 i_From, Vector2 i_To)
    {
        float firstAngle = i_From.GetAngle();
        float secondAngle = i_To.GetAngle();

        return Mathf.Abs(firstAngle - secondAngle);
    }

    public static Vector2 GetDirection(this Vector2 i_Vector)
    {
        Vector2 direction = Vector2.up;
        if (i_Vector.sqrMagnitude > Mathf.Epsilon)
        {
            direction = i_Vector.normalized;
        }

        return direction;
    }

    public static bool ApproximatelyEqual(this Vector2 i_A, Vector2 i_B, float i_XTolerance, float i_YTolerance)
    {
        i_XTolerance = Mathf.Clamp01(i_XTolerance);
        i_YTolerance = Mathf.Clamp01(i_YTolerance);
        
        float minBx = i_B.x - i_B.x * i_XTolerance;
        float maxBx = i_B.x + i_B.x * i_XTolerance;

        float minBy = i_B.y - i_B.y * i_YTolerance;
        float maxBy = i_B.y + i_B.y * i_YTolerance;

        return (!(i_A.x < minBx || i_A.x > maxBx)) && (!(i_A.y < minBy || i_A.y > maxBy));
    }

	public static Vector2 ClampMagnitude(this Vector2 v, float i_Magnitude)
    {
        Vector2 clampedV = Vector2.ClampMagnitude(v, i_Magnitude);
        return clampedV;
    }
}
