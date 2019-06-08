using UnityEngine;

using TuesdayNights;

public class tnRefereeInputFiller : tnAIInputFiller
{
    private tnBall m_Ball = null;

    private float m_FleeMinThreshold = 0.5f;
    private float m_FleeMaxThreshold = 1.5f;
    private float m_SeekMinThreshold = 4.5f;
    private float m_SeekMaxThreshold = 6.5f;

    // tnInputFiller's INTERFACE

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        if (m_Ball == null || self == null)
        {
            return;
        }

        Transform ballTransform = m_Ball.transform;
        Transform myTransform = self.transform;

        Vector2 steering = Vector2.zero;
        float distance = 0f;
        float deltaFleeThreshold = 0f;
        float deltaSeekThreshold = 0f;
        float perc = 0f;

        steering = ballTransform.position - myTransform.position;
        steering.Normalize();
        distance = Vector2.Distance(myTransform.position, ballTransform.position);
        deltaFleeThreshold = m_FleeMaxThreshold - m_FleeMinThreshold;
        deltaSeekThreshold = m_SeekMaxThreshold - m_SeekMinThreshold;

        if (distance < m_SeekMaxThreshold)
        {
            if (distance < m_FleeMinThreshold)
            {
                // zone -1
                perc = -1;
            }
            if (distance < m_FleeMaxThreshold && distance > m_FleeMinThreshold)
            {
                // zone (0 ; -1)
                perc = (distance - m_FleeMinThreshold) / deltaFleeThreshold * -1;
            }
            if (distance <= m_SeekMinThreshold && distance >= m_FleeMaxThreshold)
            {
                // zone 0
                perc = 0f;
            }
            if (distance > m_SeekMinThreshold)
            {
                // zone (1 ; 0)
                perc = (distance - m_SeekMinThreshold) / deltaSeekThreshold;
            }
        }
        else
        {
            // zone 1
            perc = 1f;
        }

        steering *= perc;

        // Fill Input

        i_Data.SetAxis(InputActions.s_HorizontalAxis, steering.x);
        i_Data.SetAxis(InputActions.s_VerticalAxis, steering.y);
    }

    public override void Clear()
    {

    }

    public override void DrawGizmos()
    {
        if (m_Ball == null)
            return;

        Transform ballTransform = m_Ball.transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ballTransform.position, m_FleeMinThreshold);
        Gizmos.DrawWireSphere(ballTransform.position, m_FleeMaxThreshold);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ballTransform.position, m_SeekMinThreshold);
        Gizmos.DrawWireSphere(ballTransform.position, m_SeekMaxThreshold);
    }

    public override void DrawGizmosSelected()
    {

    }

    // LOGIC

    public void SetThresholds(float i_FleeMinThreshold, float i_FleeMaxThershold, float i_SeekMinThreshold, float i_SeekMaxThershold)
    {
        m_FleeMinThreshold = i_FleeMinThreshold;
        m_FleeMaxThreshold = i_FleeMaxThershold;
        m_SeekMinThreshold = i_SeekMinThreshold;
        m_SeekMaxThreshold = i_SeekMaxThershold;
    }

    public void SetBall(tnBall i_BallInstance)
    {
        m_Ball = i_BallInstance;
    }

    // CTOR

    public tnRefereeInputFiller(GameObject i_Self)
        : base (i_Self)
    {

    }
}
