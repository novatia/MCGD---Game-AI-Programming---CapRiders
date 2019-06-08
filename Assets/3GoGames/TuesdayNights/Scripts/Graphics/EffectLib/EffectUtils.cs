using UnityEngine;

public static class EffectUtils
{
    // Effect utilities.

    public static Effect PlayEffect(Effect i_Prefab)
    {
        return PlayEffect(i_Prefab, Vector3.zero, Quaternion.identity, null, null);
    }

    public static Effect PlayEffect(Effect i_Prefab, Vector3 i_WorldPosition)
    {
        return PlayEffect(i_Prefab, i_WorldPosition, Quaternion.identity, null, null);
    }

    public static Effect PlayEffect(Effect i_Prefab, Vector3 i_WorldPosition, Quaternion i_WorldRotation, AnimCompletedCallback i_CompletedCallback = null)
    {
        return PlayEffect(i_Prefab, i_WorldPosition, i_WorldRotation, null, i_CompletedCallback);
    }

    public static Effect PlayEffect(Effect i_Prefab, Transform i_Target)
    {
        return PlayEffect(i_Prefab, i_Target.position, i_Target.rotation, i_Target, null);
    }

    public static Effect PlayEffect(Effect i_Prefab, Vector3 i_WorldPostion, Quaternion i_WorldRotation, Transform i_Anchor, AnimCompletedCallback i_CompletedCallback = null)
    {
        if (i_Prefab == null)
        {
            if (i_CompletedCallback != null)
            {
                i_CompletedCallback();
            }

            return null;
        }

        Effect instance = ObjectPool.SpawnMain<Effect>(i_Prefab);

        instance.SetTargetTransform(i_WorldPostion, i_WorldRotation, i_Anchor);
        instance.Play(i_CompletedCallback);

        return instance;
    }
    
    // Particle effect utilities.

    public static ParticleEffect PlayParticleEffect(ParticleEffect i_Prefab, uint i_Seed, Vector3 i_WorldPosition)
    {
        return PlayParticleEffect(i_Prefab, i_Seed, i_WorldPosition, Quaternion.identity, null, null);
    }

    public static ParticleEffect PlayParticleEffect(ParticleEffect i_Prefab, uint i_Seed, Vector3 i_WorldPosition, Quaternion i_WorldRotation, AnimCompletedCallback i_CompletedCallback = null)
    {
        return PlayParticleEffect(i_Prefab, i_Seed, i_WorldPosition, i_WorldRotation, null, i_CompletedCallback);
    }

    public static ParticleEffect PlayParticleEffect(ParticleEffect i_Prefab, uint i_Seed, Transform i_Target)
    {
        return PlayParticleEffect(i_Prefab, i_Seed, i_Target.position, i_Target.rotation, i_Target, null);
    }

    public static ParticleEffect PlayParticleEffect(ParticleEffect i_Prefab, uint i_Seed, Vector3 i_WorldPosition, Quaternion i_WorldRotation, Transform i_Anchor, AnimCompletedCallback i_CompletedCallback = null)
    {
        if (i_Prefab == null)
        {
            if (i_CompletedCallback != null)
            {
                i_CompletedCallback();
            }

            return null;
        }

        ParticleEffect instance = ObjectPool.SpawnMain<ParticleEffect>(i_Prefab);

        instance.SetParticleSeed(i_Seed);

        instance.SetTargetTransform(i_WorldPosition, i_WorldRotation, i_Anchor);
        instance.Play(i_CompletedCallback);

        return instance;
    }

    // Effect utilities - 2D

    public static Effect PlayEffect2D(Effect i_Prefab, Vector2 i_WorldPosition)
    {
        return PlayEffect2D(i_Prefab, i_WorldPosition, 0f, null, null);
    }

    public static Effect PlayEffect2D(Effect i_Prefab, Vector2 i_WorldPosition, float i_WorldRotation, AnimCompletedCallback i_CompletedCallback = null)
    {
        return PlayEffect2D(i_Prefab, i_WorldPosition, i_WorldRotation, null, i_CompletedCallback);
    }

    public static Effect PlayEffect2D(Effect i_Prefab, Transform i_Target)
    {
        Vector3 worldPosition = new Vector3(i_Target.position.x, i_Target.position.y, 0f);
        Quaternion worldRotation = i_Target.rotation;

        return PlayEffect(i_Prefab, worldPosition, worldRotation, i_Target, null);
    }

    public static Effect PlayEffect2D(Effect i_Prefab, Vector2 i_WorldPosition, float i_WorldRotation, Transform i_Anchor, AnimCompletedCallback i_CompletedCallback = null)
    {
        Vector3 worldPosition = new Vector3(i_WorldPosition.x, i_WorldPosition.y, 0f);
        Quaternion worldRotation = Quaternion.AngleAxis(i_WorldRotation, Vector3.up);

        return PlayEffect(i_Prefab, worldPosition, worldRotation, i_Anchor, i_CompletedCallback);
    }

    public static Effect PlayRadialEffect2D(Effect i_Prefab, Vector2 i_WorldCenter, Vector2 i_Direction, float i_Radius, AnimCompletedCallback i_CompletedCallback = null)
    {
        return PlayRadialEffect2D(i_Prefab, i_WorldCenter, i_Direction, i_Radius, null, i_CompletedCallback);
    }

    public static Effect PlayRadialEffect2D(Effect i_Prefab, Vector2 i_WorldCenter, Vector2 i_Direction, float i_Radius, Transform i_Anchor, AnimCompletedCallback i_CompletedCallback = null)
    {
        if (i_Direction.sqrMagnitude > Mathf.Epsilon)
        {
            i_Direction.Normalize();
        }
        else
        {
            i_Direction = Vector2.up;
        }

        Vector2 worldPosition = i_WorldCenter + i_Direction * i_Radius;
        Quaternion worldRotation = Quaternion.FromToRotation(Vector2.up, i_Direction);

        return PlayEffect(i_Prefab, worldPosition, worldRotation, i_Anchor, i_CompletedCallback);
    }

    // Particle effect utilities - 2D

    public static ParticleEffect PlayParticleEffect2D(ParticleEffect i_Prefab, uint i_Seed, Vector2 i_WorldPosition)
    {
        return PlayParticleEffect2D(i_Prefab, i_Seed, i_WorldPosition, 0f, null, null);
    }

    public static ParticleEffect PlayParticleEffect2D(ParticleEffect i_Prefab, uint i_Seed, Vector2 i_WorldPosition, float i_WorldRotation, AnimCompletedCallback i_CompletedCallback = null)
    {
        return PlayParticleEffect2D(i_Prefab, i_Seed, i_WorldPosition, i_WorldRotation, null, i_CompletedCallback);
    }

    public static ParticleEffect PlayParticleEffect2D(ParticleEffect i_Prefab, uint i_Seed, Transform i_Target)
    {
        Vector3 worldPosition = new Vector3(i_Target.position.x, i_Target.position.y, 0f);
        Quaternion worldRotation = i_Target.rotation;

        return PlayParticleEffect(i_Prefab, i_Seed, worldPosition, worldRotation, null, null);
    }

    public static ParticleEffect PlayParticleEffect2D(ParticleEffect i_Prefab, uint i_Seed, Vector2 i_WorldPosition, float i_WorldRotation, Transform i_Anchor, AnimCompletedCallback i_CompletedCallback = null)
    {
        Vector3 worldPosition = new Vector3(i_WorldPosition.x, i_WorldPosition.y, 0f);
        Quaternion worldRotation = Quaternion.AngleAxis(i_WorldRotation, Vector3.up);

        return PlayParticleEffect(i_Prefab, i_Seed, worldPosition, worldRotation, i_Anchor, i_CompletedCallback);
    }
}

