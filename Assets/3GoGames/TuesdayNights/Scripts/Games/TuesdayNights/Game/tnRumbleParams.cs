using UnityEngine;
using System.Collections;

public class tnRumbleParams : ScriptableObject
{
    [Range(0f, 1f)]
    [SerializeField]
    private float m_GoalIntensity = 0.80f;
    [SerializeField]
    private float m_GoalDuration = 0.35f;

    [Range(0f, 1f)]
    [SerializeField]
    private float m_KickBallIntensity = 0.50f;
    [SerializeField]
    private float m_KickBallDuration = 0.20f;

    [Range(0f, 1f)]
    [SerializeField]
    private float m_KickCharacterIntensity = 0.50f;
    [SerializeField]
    private float m_KickCharacterDuration = 0.20f;

    [Range(0f, 1f)]
    [SerializeField]
    private float m_KickReceivedIntensity = 0.70f;
    [SerializeField]
    private float m_KickReceivedDuration = 0.30f;

    [Range(0f, 1f)]
    [SerializeField]
    private float m_CollisionMinIntensity = 0.40f;
    [Range(0f, 1f)]
    [SerializeField]
    private float m_CollisionMaxIntensity = 0.80f;

    [SerializeField]
    private float m_CollisionMinDuration = 0.15f;
    [SerializeField]
    private float m_CollisionMaxDuration = 0.25f;

    [SerializeField]
    private float m_CollisionMinVelocity = 2.5f;
    [SerializeField]
    private float m_CollisionMaxVelocity = 7.5f;

    public float goalIntensity
    {
        get
        {
            return m_GoalIntensity;
        }
    }

    public float goalDuration
    {
        get
        {
            return m_GoalDuration;
        }
    }

    public float kickBallIntensity
    {
        get
        {
            return m_KickBallIntensity;
        }
    }

    public float kickBallDuration
    {
        get
        {
            return m_KickBallDuration;
        }
    }

    public float kickCharacterIntensity
    {
        get
        {
            return m_KickCharacterIntensity;
        }
    }

    public float kickCharacterDuration
    {
        get
        {
            return m_KickCharacterDuration;
        }
    }

    public float kickReceivedIntensity
    {
        get
        {
            return m_KickReceivedIntensity;
        }
    }

    public float kickReceivedDuration
    {
        get
        {
            return m_KickReceivedDuration;
        }
    }

    public float collisionMinIntensity
    {
        get
        {
            return m_CollisionMinIntensity;
        }
    }

    public float collisionMaxIntensity
    {
        get
        {
            return m_CollisionMaxIntensity;
        }
    }

    public float collisionMinDuration
    {
        get
        {
            return m_CollisionMinDuration;
        }
    }

    public float collisionMaxDuration
    {
        get
        {
            return m_CollisionMaxDuration;
        }
    }

    public float collisionMinVelocity
    {
        get
        {
            return m_CollisionMinVelocity;
        }
    }

    public float collisionMaxVelocity
    {
        get
        {
            return m_CollisionMaxVelocity;
        }
    }
}
