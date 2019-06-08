using UnityEngine;
using System.Collections;

public class FilteredVector2
{
    private Vector3 m_Position;
    private Vector3 m_Velocity;

    private Vector3 m_Acceleration;

    private Vector3 m_PrevTarget;

    private float m_PiNaturalFrequency;
    private float m_ViscousFriction;
    private float m_Correction;

    public Vector3 position
    {
        get
        {
            return m_Position;
        }
    }

    public Vector3 velocity
    {
        get
        {
            return m_Velocity;
        }
    }

    public Vector3 acceleration
    {
        get
        {
            return m_Acceleration;
        }
    }

    public Vector3 Step(Vector3 i_Target, float i_FrameTime)
    {
        if (i_FrameTime < Mathf.Epsilon)
            return m_Position;

        Vector3 targetVelocity = (i_Target - m_PrevTarget) / i_FrameTime;
        return Step(i_Target, targetVelocity, i_FrameTime);
    }

    public Vector3 Step(Vector3 i_Target, Vector3 i_TargetVelocity, float i_FrameTime)
    {
        SingleStep(i_Target, i_TargetVelocity, i_FrameTime);
        m_PrevTarget = i_Target;
        return m_Position;
    }

    public void Reset(Vector3 i_Position, Vector3 i_Velocity)
    {
        m_Position = i_Position;
        m_Velocity = i_Velocity;
        m_Acceleration = ComputeAcceleration(i_Position, i_Velocity);

        m_PrevTarget = i_Position;
    }

    public FilteredVector2(float i_NaturalFrquency, float i_ViscousFactor, float i_CorrectionFactor)
    {
        m_PiNaturalFrequency = i_NaturalFrquency * Mathf.PI;
        m_ViscousFriction = i_ViscousFactor;
        m_Correction = i_CorrectionFactor;

        m_Position = Vector3.zero;
        m_Velocity = Vector3.zero;
        m_Acceleration = Vector3.zero;
        m_PrevTarget = Vector3.zero;
    }

    // INTERNALS

    private void SingleStep(Vector3 i_Target, Vector3 i_TargetVelocity, float i_FrameTime)
    {
        float halfTime = i_FrameTime * 0.5f;
        m_Position += m_Velocity * i_FrameTime + m_Acceleration * (i_FrameTime * halfTime);
        Vector3 acceleration = ComputeAcceleration(i_Target, i_TargetVelocity);
        m_Velocity += (m_Acceleration + acceleration) * halfTime;
        m_Acceleration = acceleration;
    }

    public Vector3 ComputeAcceleration(Vector3 i_Target, Vector3 i_TargetVelocity)
    {
        Vector3 error = m_Position - i_Target;
        Vector3 errorDerivative = m_Velocity - i_TargetVelocity;
        Vector3 acceleration = -4f * m_PiNaturalFrequency * (error * m_PiNaturalFrequency + m_Velocity * m_ViscousFriction + errorDerivative * m_Correction);
        return acceleration;
    }
}