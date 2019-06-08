using UnityEngine;

using BaseMatchEvents;

using TuesdayNights;

using TrueSync;

[RequireComponent(typeof(TSCollider2D))]
public class tnGoal : TrueSyncBehaviour
{
    [Header("Graphics")]

    [SerializeField]
    private GameObject m_Base = null;
    [SerializeField]
    private SpriteRenderer[] m_SpriteRenderers = null;

    [Header("Logic")]

    [SerializeField]
    private TSVector2 m_TopPostDelta = TSVector2.zero;
    [SerializeField]
    private TSVector2 m_BottomPostDelta = TSVector2.zero;

    [Header("Slow motion")]

    [SerializeField]
    private TSVector2 m_SlowMotionDeltaPivotA = TSVector2.zero;
    [SerializeField]
    private TSVector2 m_SlowMotionDeltaPivotB = TSVector2.zero;

    [Header("Debug")]

    [SerializeField]
    private bool m_DrawGizmos = false;

    // Fields

    private int m_TeamId = Hash.s_NULL;

    private TSCollider2D m_Collider2D = null;

    // ACCESSORS

    public TSVector2 topPostPosition
    {
        get
        {
            return tsTransform2D.position + m_TopPostDelta;
        }
    }

    public TSVector2 bottomPostPosition
    {
        get
        {
            return tsTransform2D.position + m_BottomPostDelta;
        }
    }

    public TSVector2 postToPost
    {
        get
        {
            return topPostPosition - bottomPostPosition;
        }
    }

    public FP goalWidth
    {
        get
        {
            return TSVector2.Distance(topPostPosition, bottomPostPosition);
        }
    }

    public TSVector2 slowMotionPivotA
    {
        get
        {
            return topPostPosition + m_SlowMotionDeltaPivotA;
        }
    }

    public TSVector2 slowMotionPivotB
    {
        get
        {
            return bottomPostPosition + m_SlowMotionDeltaPivotB;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Collider2D = GetComponent<TSCollider2D>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Goal;
    }

    void Start()
    {
        if (m_Base == null)
            return;

        GameObject fieldGo = GameObject.FindGameObjectWithTag("Field");

        if (fieldGo == null)
            return;

        Vector3 basePosition = m_Base.transform.position;

        basePosition.z = fieldGo.transform.position.z;
        basePosition.z -= 0.005f; // Add a small offset to prevent z-fighting.

        m_Base.transform.position = basePosition;
    }

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(topPostPosition.ToVector(), 0.1f);
        Gizmos.DrawSphere(bottomPostPosition.ToVector(), 0.1f);

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(slowMotionPivotA.ToVector(), 0.1f);
        Gizmos.DrawSphere(slowMotionPivotB.ToVector(), 0.1f);
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedTriggerEnter(TSCollision2D i_Collision)
    {
        if (!i_Collision.collider.CompareTag(Tags.s_Ball))
            return;

        if (m_TeamId == Hash.s_NULL)
            return;

        tnGoalEventParams goalEventParams = new tnGoalEventParams();
        goalEventParams.SetTeamId(m_TeamId);

        goalEventParams.SetHasValidScorer(false);

        {
            // Try to assign a valid scorer.

            tnKickable kickable = i_Collision.gameObject.GetComponent<tnKickable>();
            if (kickable != null)
            {
                for (int touchIndex = 0; touchIndex < kickable.touchesCount; ++touchIndex)
                {
                    tnTouch ballTouch = kickable.GetTouch(touchIndex);

                    if (m_TeamId != ballTouch.teamId)
                    {
                        goalEventParams.SetScorerId(ballTouch.characterId);
                        goalEventParams.SetScorerTeamId(ballTouch.teamId);

                        goalEventParams.SetIsHumanScorer(ballTouch.isHuman);
                        goalEventParams.SetIsLocalScorer(ballTouch.isLocal);

                        goalEventParams.SetHasValidScorer(true);

                        break;
                    }
                }
            }
        }

        // Send event.

        Messenger.Broadcast<tnGoalEventParams>("Goal", goalEventParams);
    }

    // LOGIC

    public void SetTeamId(int i_TeamId)
    {
        m_TeamId = i_TeamId;
    }

    public void Rotate()
    {
        // Flip sprite renderers

        if (m_SpriteRenderers != null)
        {
            for (int index = 0; index < m_SpriteRenderers.Length; ++index)
            {
                SpriteRenderer spriteRenderer = m_SpriteRenderers[index];

                if (spriteRenderer == null)
                    continue;

                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }

        // Invert collider center.

        TSVector2 colliderCenter = m_Collider2D.center;
        colliderCenter *= -FP.One;
        m_Collider2D.center = colliderCenter;

        // Invert posts.

        m_TopPostDelta.x *= -FP.One;
        m_BottomPostDelta.x *= -FP.One;

        // Invert posts.

        m_SlowMotionDeltaPivotA.x *= -FP.One;
        m_SlowMotionDeltaPivotB.x *= -FP.One;
    }
}