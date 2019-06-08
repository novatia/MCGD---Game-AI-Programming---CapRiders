using UnityEngine;
using System.Collections;

public static class Vector3Utils
{
    public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 normal)
    {
        float dot = Vector3.Dot(v, normal);
        return v - dot * normal;
    }

    public static Vector3 GetDirection(this Vector3 v)
    {
        Vector3 dir = Vector3.up;

        if (v.sqrMagnitude > Mathf.Epsilon)
        {
            dir = v.normalized;
        }

        return dir;
    }

    public static Vector2 GetVector2XY(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 GetVector2XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector2 GetVector2YZ(this Vector3 v)
    {
        return new Vector2(v.y, v.z);
    }
}

