using UnityEngine;

#if PHOTON_TRUE_SYNC

using TrueSync;

#endif // PHOTON_TRUE_SYNC

public static class AISteeringBehaviors
{
    // SEEK

    public static Vector2 Seek(GameObject i_Source, GameObject i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Seek(i_Source.transform, i_Target.transform);
    }

    public static Vector2 Seek(GameObject i_Source, Transform i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Seek(i_Source.transform, i_Target);
    }

    public static Vector2 Seek(GameObject i_Source, Vector2 i_TargetPosition)
    {
        if (i_Source == null)
        {
            return Vector2.zero;
        }

        return Seek(i_Source.transform, i_TargetPosition);
    }

    public static Vector2 Seek(Transform i_Source, GameObject i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Seek(i_Source, i_Target.transform);
    }

    public static Vector2 Seek(Transform i_Source, Transform i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Seek(i_Source.position, i_Target.position);
    }

    public static Vector2 Seek(Transform i_Source, Vector2 i_TargetPosition)
    {
        if (i_Source == null)
        {
            return Vector2.zero;
        }

        return Seek(i_Source.position, i_TargetPosition);
    }

    public static Vector2 Seek(Vector2 i_StartPosition, GameObject i_Target)
    {
        if (i_Target == null)
        {
            return Vector2.zero;
        }

        return Seek(i_StartPosition, i_Target.transform);
    }

    public static Vector2 Seek(Vector2 i_StartPosition, Transform i_Target)
    {
        if (i_Target == null)
        {
            return Vector2.zero;
        }

        return Seek(i_StartPosition, i_Target.position);
    }

    public static Vector2 Seek(Vector2 i_StartPosition, Vector2 i_TargetPosition)
    {
        Vector2 difference = i_TargetPosition - i_StartPosition;
        Vector2 direction = difference.normalized;

        return direction;
    }

    // FLEE

    public static Vector2 Flee(GameObject i_Source, GameObject i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Flee(i_Source.transform, i_Target.transform);
    }

    public static Vector2 Flee(GameObject i_Source, Transform i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Flee(i_Source.transform, i_Target);
    }

    public static Vector2 Flee(GameObject i_Source, Vector2 i_TargetPosition)
    {
        if (i_Source == null)
        {
            return Vector2.zero;
        }

        return Flee(i_Source.transform, i_TargetPosition);
    }

    public static Vector2 Flee(Transform i_Source, GameObject i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Flee(i_Source, i_Target.transform);
    }

    public static Vector2 Flee(Transform i_Source, Transform i_Target)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Flee(i_Source.position, i_Target.position);
    }

    public static Vector2 Flee(Transform i_Source, Vector2 i_TargetPosition)
    {
        if (i_Source == null)
        {
            return Vector2.zero;
        }

        return Flee(i_Source.position, i_TargetPosition);
    }

    public static Vector2 Flee(Vector2 i_StartPosition, GameObject i_Target)
    {
        if (i_Target == null)
        {
            return Vector2.zero;
        }

        return Flee(i_StartPosition, i_Target.transform);
    }

    public static Vector2 Flee(Vector2 i_StartPosition, Transform i_Target)
    {
        if (i_Target == null)
        {
            return Vector2.zero;
        }

        return Flee(i_StartPosition, i_Target.position);
    }

    public static Vector2 Flee(Vector2 i_StartPosition, Vector2 i_TargetPosition)
    {
        Vector2 difference = i_StartPosition - i_TargetPosition;
        Vector2 direction = difference.normalized;

        return direction;
    }

    // PURSUIT

    public static Vector2 Pursuit(GameObject i_Source, GameObject i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Pursuit(i_Source.transform, i_Target.transform, i_MaxLookAheadTime);
    }

    public static Vector2 Pursuit(GameObject i_Source, Transform i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Pursuit(i_Source.transform, i_Target, i_MaxLookAheadTime);
    }

    public static Vector2 Pursuit(Transform i_Source, GameObject i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Pursuit(i_Source, i_Target.transform, i_MaxLookAheadTime);
    }

    public static Vector2 Pursuit(Transform i_Source, Transform i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        Vector2 evaderPosition = i_Target.position;
        Vector2 myPosition = i_Source.position;

        Vector2 toEvader = evaderPosition - myPosition;
        float evaderDistance = toEvader.magnitude;

        Vector2 evaderVelocity = GetRigidbody2dVelocity(i_Target);

        float mySpeed = GetRigidbody2dSpeed(i_Source);
        float evaderSpeed = GetRigidbody2dSpeed(i_Target);

        float lookAheadTime = i_MaxLookAheadTime;
        if (mySpeed + evaderSpeed > 0f)
        {
            lookAheadTime = evaderDistance / (mySpeed + evaderSpeed);
        }

        return Seek(myPosition, evaderPosition + evaderVelocity * lookAheadTime);
    }

    // EVADE

    public static Vector2 Evade(GameObject i_Source, GameObject i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Evade(i_Source.transform, i_Target.transform, i_MaxLookAheadTime);
    }

    public static Vector2 Evade(GameObject i_Source, Transform i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Evade(i_Source.transform, i_Target, i_MaxLookAheadTime);
    }

    public static Vector2 Evade(Transform i_Source, GameObject i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        return Evade(i_Source, i_Target.transform, i_MaxLookAheadTime);
    }

    public static Vector2 Evade(Transform i_Source, Transform i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Source == null || i_Target == null)
        {
            return Vector2.zero;
        }

        Vector2 hunterPosition = i_Target.position;
        Vector2 myPosition = i_Source.position;

        Vector2 toHunter = hunterPosition - myPosition;
        float hunterDistance = toHunter.magnitude;

        Vector2 hunterVelocity = GetRigidbody2dVelocity(i_Target);

        float mySpeed = GetRigidbody2dSpeed(i_Source);
        float hunterSpeed = GetRigidbody2dSpeed(i_Target);

        float lookAheadTime = i_MaxLookAheadTime;
        if (mySpeed + hunterSpeed > Mathf.Epsilon)
        {
            lookAheadTime = hunterDistance / (mySpeed + hunterSpeed);
        }

        return Flee(myPosition, hunterPosition + hunterVelocity * lookAheadTime);
    }

    // INTERPOSE

    public static Vector2 Interpose(GameObject i_Source, GameObject i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A.transform, i_B.transform, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, GameObject i_A, Transform i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A.transform, i_B, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, Transform i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A, i_B.transform, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, Transform i_A, Transform i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A, i_B, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, GameObject i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source, i_A.transform, i_B.transform, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, GameObject i_A, Transform i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source, i_A.transform, i_B, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, Transform i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source, i_A, i_B.transform, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, Vector2 i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source, i_A, i_B.transform, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, Vector2 i_A, Transform i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source, i_B, i_A, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, GameObject i_A, Vector2 i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source, i_A.transform, i_B, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, Transform i_A, Vector2 i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A, i_B, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, Vector2 i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A, i_B.transform, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, Vector2 i_A, Transform i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_B == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A, i_B, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, GameObject i_A, Vector2 i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A.transform, i_B, i_Weight);
    }

    public static Vector2 Interpose(GameObject i_Source, Vector2 i_A, Vector2 i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null)
        {
            return Vector2.zero;
        }

        return Interpose(i_Source.transform, i_A, i_B, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, Vector2 i_A, Vector2 i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null)
        {
            return Vector2.zero;
        }

        Vector2 sourceVelocity = GetRigidbody2dVelocity(i_Source);

        return Interpose(i_Source.position, i_A, i_B, sourceVelocity, Vector2.zero, Vector2.zero, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, Transform i_A, Vector2 i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null)
        {
            return Vector2.zero;
        }

        Vector2 sourceVelocity = GetRigidbody2dVelocity(i_Source);
        Vector2 aVelocity = GetRigidbody2dVelocity(i_A);

        return Interpose(i_Source.position, i_A.position, i_B, sourceVelocity, aVelocity, Vector2.zero, i_Weight);
    }

    public static Vector2 Interpose(Transform i_Source, Transform i_A, Transform i_B, float i_Weight = 0.5f)
    {
        if (i_Source == null || i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        Vector2 sourceVelocity = GetRigidbody2dVelocity(i_Source);
        Vector2 aVelocity = GetRigidbody2dVelocity(i_A);
        Vector2 bVelocity = GetRigidbody2dVelocity(i_B);

        return Interpose(i_Source.position, i_A.position, i_B.position, sourceVelocity, aVelocity, bVelocity, i_Weight);
    }

    public static Vector2 Interpose(Vector2 i_Source, Vector2 i_A, Vector2 i_B, Vector2 i_SourceVelocity, Vector2 i_AVelocity, Vector2 i_BVelocity, float i_Weight = 0.5f)
    {
        float w = Mathf.Clamp01(i_Weight);

        float mySpeed = i_SourceVelocity.magnitude;

        Vector2 midPoint = Vector2.Lerp(i_A, i_B, w);

        Vector2 toMidPoint = midPoint - i_Source;
        float midPointDistance = toMidPoint.magnitude;

        float timeToReachMidPoint = 0f;
        if (mySpeed > Mathf.Epsilon)
        {
            timeToReachMidPoint = midPointDistance / mySpeed;
        }

        Vector2 aFuturePosition = i_A + i_AVelocity * timeToReachMidPoint;
        Vector2 bFuturePosition = i_B + i_BVelocity * timeToReachMidPoint;

        Vector2 futureMidPoint = Vector2.Lerp(aFuturePosition, bFuturePosition, w);

        return Seek(i_Source, futureMidPoint);
    }

    // UTILS

    private static Vector2 GetRigidbody2dVelocity(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return Vector2.zero;
        }

        return GetRigidbody2dVelocity(i_Transform.gameObject);
    }

    private static Vector2 GetRigidbody2dVelocity(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return Vector2.zero;
        }

#if PHOTON_TRUE_SYNC

        TSRigidBody2D tsRigidbody2d = i_Go.GetComponent<TSRigidBody2D>();
        if (tsRigidbody2d != null)
        {
            TSVector2 velocity = tsRigidbody2d.velocity;
            return velocity.ToVector();
        }

#else

        Rigidbody2D rigidbody2d = i_Go.GetComponent<Rigidbody2D>();
        if (rigidbody2d != null)
        {
            Vector2 velocity = rigidbody2d.velocity;
            return velocity;
        }

#endif // PHOTON_TRUE_SYNC

        return Vector2.zero;
    }

    private static float GetRigidbody2dSpeed(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return 0f;
        }

        return GetRigidbody2dSpeed(i_Transform.gameObject);
    }

    private static float GetRigidbody2dSpeed(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return 0f;
        }

        Vector2 velocity = GetRigidbody2dVelocity(i_Go);
        float speed = velocity.magnitude;

        return speed;
    }

    private static float GetRigidbody2dSpeed2(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return 0f;
        }

        return GetRigidbody2dSpeed2(i_Transform.gameObject);
    }

    private static float GetRigidbody2dSpeed2(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return 0f;
        }

        Vector2 velocity = GetRigidbody2dVelocity(i_Go);
        float speed = velocity.sqrMagnitude;

        return speed;
    }
}
