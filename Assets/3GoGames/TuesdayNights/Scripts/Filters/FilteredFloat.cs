using UnityEngine;

public struct FilteredFloat
{
    private float m_RaiseStepFactor;
    private float m_LowerStepFactor;

    private float m_Position;

    public float position
    {
        get { return m_Position; }
        set { m_Position = value; }
    }

    public float Step(float i_Target, float i_FrameTime)
    {
        float smoothStepFactor = (i_Target < m_Position) ? m_LowerStepFactor : m_RaiseStepFactor;
        float smoothFactor = (smoothStepFactor > 0.0f) ? 1.0f - Mathf.Pow(0.5f, i_FrameTime / smoothStepFactor) : 1.0f;

        m_Position += (i_Target - m_Position) * smoothFactor;

        return m_Position;
    }

    public void Reset(float i_Position = 0f)
    {
        m_Position = i_Position;
    }

    public FilteredFloat(float i_RaiseStepFactor = 0f, float i_LowerStepFactor = 0f)
    {
        m_Position = 0.0f;

        m_RaiseStepFactor = Mathf.Max(i_RaiseStepFactor, 0f);
        m_LowerStepFactor = Mathf.Max(i_LowerStepFactor, 0f);
    }
}
