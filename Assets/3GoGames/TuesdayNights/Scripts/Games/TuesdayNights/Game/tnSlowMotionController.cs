using UnityEngine;

using System.Collections.Generic;

using TuesdayNights;

using TrueSync;

public class tnSlowMotionController : TrueSyncBehaviour
{
    // Serializable fields

    [SerializeField]
    private FP m_TimeToReach = 0.25f;

    [SerializeField]
    private FP m_MinDistance = 0.5f;
    [SerializeField]
    private FP m_MaxDistance = 1.5f;

    [SerializeField]
    private FP m_MinReferenceSpeed = 2f;
    [SerializeField]
    private FP m_DistanceThreshold = 1f;

    [SerializeField]
    private FP m_MinTimeScale = 0.1f;

    [SerializeField]
    private bool m_UseRaycast = false;

    [SerializeField]
    private LayerMask m_RaycastMask = 0;

    // Fields

    [AddTracking]
    private bool m_IsEnabled = true;

    private TSRigidBody2D m_Reference = null;
    private TSTransform2D m_ReferenceTransform = null;

    private List<TSVector2> m_Segments = new List<TSVector2>();
    private int m_SegmentCount = 0;

    // ACCESSORS

    public bool isEnabled
    {
        get
        {
            return m_IsEnabled;
        }
    }

    // LOGIC

    public void SetEnabled(bool i_Enabled)
    {
        m_IsEnabled = i_Enabled;

        TrueSyncManager.ForceTimeScaleMain(FP.One);
    }

    public void SetTarget(TSRigidBody2D i_Target)
    {
        m_Reference = i_Target;
        m_ReferenceTransform = (m_Reference != null) ? m_Reference.GetComponent<TSTransform2D>() : null;
    }

    public void AddSegment(TSVector2 i_A, TSVector2 i_B)
    {
        TSVector2 d = i_B - i_A;

        if (d.magnitude > FP.Epsilon)
        {
            m_Segments.Add(i_A);
            m_Segments.Add(i_B);

            m_SegmentCount = m_SegmentCount + 1;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        sortOrder = BehaviourSortOrder.s_SortOrder_SlowMotionController; // Set sort order.
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        TrueSyncManager.ForceTimeScaleMain(FP.One);
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedUpdate()
    {
        if (!m_IsEnabled)
            return;

        // Update time scale.

        if (TrueSyncManager.timeScaleMain == FP.Zero )
            return;

        FP newTimeScale;
        ComputeTimeScale(out newTimeScale);

        TrueSyncManager.ForceTimeScaleMain(newTimeScale);
    }

    // INERNALS

    private void ComputeTimeScale(out FP o_TimeScale)
    {
        o_TimeScale = FP.One;

        //bool slowMotionEnabled;
        //GameSettings.TryGetBoolMain(Settings.s_SlowMotionSetting, out slowMotionEnabled);

        if (m_Reference == null || m_ReferenceTransform == null /*|| !slowMotionEnabled*/)
        {
            return;
        }

        TSVector2 velocity = m_Reference.velocity;

        if (velocity.LengthSquared() < m_MinReferenceSpeed * m_MinReferenceSpeed)
        {
            o_TimeScale = 1f;
            return;
        }

        if (m_UseRaycast)
        {
            TSVector2 rayOrigin = m_Reference.position;
            TSVector2 rayDirection = velocity.normalized;

            FP speed = velocity.magnitude;

            FP rayDistance = speed * m_TimeToReach;
            rayDistance = MathFP.Max(rayDistance, m_DistanceThreshold);

            TSRaycastHit2D[] raycastHit = TSPhysics2D.Raycast(rayOrigin, rayDirection, rayDistance, m_RaycastMask);

            if (raycastHit == null || raycastHit.Length == 0)
            {
                o_TimeScale = 1f;
                return;
            }

            TSRaycastHit2D hitResult = raycastHit[0];

            FP timeScale = MathFP.GetClampedPercentage(hitResult.distance, m_MinDistance, m_MaxDistance);
            timeScale = MathFP.Max(timeScale, m_MinTimeScale);

            o_TimeScale = timeScale;
        }
        else // Do a simple line check.
        {
            TSVector2 delta = velocity * m_TimeToReach;
            if (delta.magnitude > m_DistanceThreshold)
            {
                delta = delta.normalized * m_DistanceThreshold;
            }

            TSVector2 rayStart = m_Reference.position;
            TSVector2 rayEnd = rayStart + delta;

            // Check each stored segment.

            for (int segmentIndex = 0; segmentIndex < m_SegmentCount; ++segmentIndex)
            {
                TSVector2 pointA = m_Segments[segmentIndex * 2];
                TSVector2 pointB = m_Segments[segmentIndex * 2 + 1];

                FP t;
                TSVector2 intersectionPoint;

                if (Test2DSegmentSegment(pointA, pointB, rayStart, rayEnd, out t, out intersectionPoint))
                {
                    TSVector2 distance = intersectionPoint - rayStart;

                    FP timeScale = MathFP.GetClampedPercentage(distance.magnitude, m_MinDistance, m_MaxDistance);
                    timeScale = MathFP.Max(timeScale, m_MinTimeScale);

                    o_TimeScale = timeScale;

                    return;
                }
            }
        }
    }

    // MATH UTILS

    // Returns 2 times the signed triangle area. The result is positive if abc is ccw, negative if abc is cw, zero if abc is degenerate.

    private FP Signed2DTriArea(TSVector2 i_A, TSVector2 i_B, TSVector2 i_C)
    {
        return (i_A.x - i_C.x) * (i_B.y - i_C.y) - (i_A.y - i_C.y) * (i_B.x - i_C.x);
    }

    // Test if segment ab and cd overlap. If they do, compute and return intersection t value along ab and intersection positon p.

    private bool Test2DSegmentSegment(TSVector2 i_A, TSVector2 i_B, TSVector2 i_C, TSVector2 i_D, out FP o_Time, out TSVector2 o_Point)
    {
        // Sign of areas correspond to which side of ab points c and d are.

        FP a1 = Signed2DTriArea(i_A, i_B, i_D); // Compute winding of abd (+ or -).
        FP a2 = Signed2DTriArea(i_A, i_B, i_C); // To intersect, must have sign opposite of a1.

        // If c and d are on different sides of ab, areas have different signs.

        if (a1 * a2 < FP.Zero)
        {
            // Compute signs for a and b with respect to segment cd.

            FP a3 = Signed2DTriArea(i_C, i_D, i_A); // Compute winding of cda (+ or -).

            // Since area is constant a1 - a2 = a3 - a4, or a4 = a3 + a2 - a1.

            // FP a4 = Signed2DTriArea(c, d, b); // Must have opposite sign of a3.
            FP a4 = a3 + a2 - a1;

            // Points a and b on different sides of cd if areas have different signs.

            if (a3 * a4 < FP.Zero)
            {
                // Segments intersect. Find intersection point along L(t) = a + t * (b - a).
                // Given height h1 of an over cd and height h2 of b over cd,
                // t = h1 / (h1 - h2) = (b*h1/2) / (b*h1/2 - b*h2/2) = a3 / (a3 - a4),
                // where b (the base of the triangles cda and cdb, i.e., the length of cd) cancels out.

                o_Time = a3 / (a3 - a4);
                o_Point = i_A + o_Time * (i_B - i_A);

                return true;
            }
        }

        // Segments not intersecting (or collinear).

        o_Time = FP.Zero;
        o_Point = TSVector2.zero;

        return false;
    }
}