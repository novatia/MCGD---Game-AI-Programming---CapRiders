using UnityEngine;
using System.Collections;

public class FilteredFloat2
{
    private float m_Position;
    private float m_Velocity;

    private float m_Acceleration;

    private float m_PrevTarget;

    private float m_PiNaturalFrequency;
    private float m_ViscousFriction;
    private float m_Correction;

    public float position
    {
        get
        {
            return m_Position;
        }
    }

    public float velocity
    {
        get
        {
            return m_Velocity;
        }
    }

    public float acceleration
    {
        get
        {
            return m_Acceleration;
        }
    }

    public float Step(float i_Target, float i_FrameTime)
    {
        if (i_FrameTime < Mathf.Epsilon)
            return m_Position;

        float targetVelocity = (i_Target - m_PrevTarget) / i_FrameTime;
        return Step(i_Target, targetVelocity, i_FrameTime);
    }

    public float Step(float i_Target, float i_TargetVelocity, float i_FrameTime)
    {
        SingleStep(i_Target, i_TargetVelocity, i_FrameTime);
        m_PrevTarget = i_Target;
        return m_Position;
    }

    public void Reset(float i_Position, float i_Velocity)
    {
        m_Position = i_Position;
        m_Velocity = i_Velocity;
        m_Acceleration = ComputeAcceleration(i_Position, i_Velocity);

        m_PrevTarget = i_Position;
    }

    public FilteredFloat2(float i_NaturalFrquency, float i_ViscousFactor, float i_CorrectionFactor)
    {
        m_PiNaturalFrequency = i_NaturalFrquency * Mathf.PI;
        m_ViscousFriction = i_ViscousFactor;
        m_Correction = i_CorrectionFactor;

        m_Position = 0f;
        m_Velocity = 0f;
        m_Acceleration = 0f;
        m_PrevTarget = 0f;
    }

    // INTERNALS

    private void SingleStep(float i_Target, float i_TargetVelocity, float i_FrameTime)
    {
        float halfTime = i_FrameTime * 0.5f;

        m_Position += m_Velocity * i_FrameTime + m_Acceleration * (i_FrameTime * halfTime);

        float error = m_Position - i_Target;
        float errorDerivative = m_Velocity - i_TargetVelocity;
        float acceleration = -4f + m_PiNaturalFrequency * (error * m_PiNaturalFrequency + m_Velocity * m_ViscousFriction + errorDerivative * m_Correction);

        m_Velocity += (m_Acceleration + acceleration) * halfTime;
        m_Acceleration = acceleration;
    }

    public float ComputeAcceleration(float i_Target, float i_TargetVelocity)
    {
        float error = m_Position - i_Target;
        float errorDerivative = m_Velocity - i_TargetVelocity;
        float acceleration = -4f + m_PiNaturalFrequency * (error * m_PiNaturalFrequency + m_Velocity * m_ViscousFriction + errorDerivative * m_Correction);
        return acceleration;
    }
}