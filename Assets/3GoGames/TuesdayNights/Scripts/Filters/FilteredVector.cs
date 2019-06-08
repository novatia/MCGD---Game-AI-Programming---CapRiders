using UnityEngine;

public struct FilteredVector
{
    private float m_StepFactor;
    private Vector3 m_Position;

    public Vector3 position
    {
        get { return m_Position; }
        set { m_Position = value; }
    }

    public float stepFactor
    {
        get { return m_StepFactor; }
        set { m_StepFactor = value; }
    }

    public Vector3 Step(Vector3 i_Target, float i_FrameTime)
    {
        float smoothFactor = (m_StepFactor > 0.0f) ? 1.0f - Mathf.Pow(0.5f, i_FrameTime / m_StepFactor) : 1.0f;
        m_Position += (i_Target - m_Position) * smoothFactor;
        return m_Position;
    }

    public void Reset()
    {
        m_Position = Vector3.zero;
    }

    public void Reset(Vector3 i_Position)
    {
        m_Position = i_Position;
    }

    // CTOR

    public FilteredVector(float i_StepFactor = 0f)
    {
        m_Position = Vector3.zero;
        m_StepFactor = i_StepFactor;
    }

    public FilteredVector(Vector3 i_StartPosition, float i_StepFactor = 0f)
        : this (i_StepFactor)
    {
        m_Position = i_StartPosition;
    }
}
