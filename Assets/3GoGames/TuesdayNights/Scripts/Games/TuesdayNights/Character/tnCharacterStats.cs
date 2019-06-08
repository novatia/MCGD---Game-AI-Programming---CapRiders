using UnityEngine;

using TuesdayNights;

using TrueSync;

[RequireComponent(typeof(TSTransform2D))]
public class tnCharacterStats : TrueSyncBehaviour
{
    // Serializable fields

    [SerializeField]
    private bool m_DrawGizmos = true;

    // Fields

    private tnBaseMatchCharacterResults m_Results = null;

    /*

    private tnGoal m_Goal = null;

    */

    private tnGoal m_OpponentGoal = null;

    private tnCharacterController m_CharacterController = null;
    private tnRespawn m_Respawn = null;

    private tnKick m_Kick = null;
    private tnKickable m_Kickable = null;

    private tnAttract m_AttractContoller = null;

    [AddTracking]
    private TSVector2 m_PrevPos = TSVector2.zero;
    [AddTracking]
    private bool m_Respawned = false;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_CharacterController = GetComponent<tnCharacterController>();
        m_Respawn = GetComponent<tnRespawn>();

        m_Kick = GetComponent<tnKick>();
        m_Kickable = GetComponent<tnKickable>();

        m_AttractContoller = GetComponent<tnAttract>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_CharacterStats;
    }

    void OnEnable()
    {
        // Reset previous position.

        {
            m_PrevPos = tsTransform2D.position;
            m_Respawned = true;
        }

        // Register events.

        if (m_CharacterController != null)
        {
            m_CharacterController.onDashTriggered += OnDashTriggered;
            m_CharacterController.onBallTouched += OnBallTouched;
        }

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }

        if (m_Kick != null)
        {
            m_Kick.kickEvent += OnKickOccurred;
        }

        if (m_Kickable != null)
        {
            m_Kickable.kickedEvent += OnKickReceived;
        }
    }

    void OnDisable()
    {
        // Unregister events.

        if (m_CharacterController != null)
        {
            m_CharacterController.onDashTriggered -= OnDashTriggered;
            m_CharacterController.onBallTouched -= OnBallTouched;
        }

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }

        if (m_Kick != null)
        {
            m_Kick.kickEvent -= OnKickOccurred;
        }
       
        if (m_Kickable != null)
        {
            m_Kickable.kickedEvent -= OnKickReceived;
        }
    }

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        if (m_OpponentGoal != null)
        {
            TSVector2 topPostPosition = m_OpponentGoal.topPostPosition;
            TSVector2 bottomPostPosition = m_OpponentGoal.bottomPostPosition;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, topPostPosition.ToVector());

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, bottomPostPosition.ToVector());
        }
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        UpdateDistance();
        UpdateAttractTime();
    }

    // BUSINESS LOGIC

    public void SetResults(tnBaseMatchCharacterResults i_Results)
    {
        m_Results = i_Results;
    }

    /*

    public void SetGoal(tnGoal i_Goal)
    {
        m_Goal = i_Goal;
    }

    */

    public void SetOpponentGoal(tnGoal i_OpponentGoal)
    {
        m_OpponentGoal = i_OpponentGoal;
    }

    // INTERNALS

    private void UpdateDistance()
    {
        if (!m_Respawned)
        {
            if (m_Results != null)
            {
                TSVector2 currentPosition = tsTransform2D.position;
                TSVector2 deltaPosition = currentPosition - m_PrevPos;

                FP deltaMovement = deltaPosition.magnitude;
                m_Results.distanceRun += deltaMovement;
            }
        }
        else
        {
            m_Respawned = false;
        }

        m_PrevPos = tsTransform2D.position;
    }

    private void UpdateAttractTime()
    {
        if (m_Results == null)
            return;

        if (m_AttractContoller == null)
            return;

        if (m_AttractContoller.isActive)
        {
            m_Results.attractTime += TrueSyncManager.deltaTimeMain;
        }
    }

    // EVENTS

    private void OnDashTriggered()
    {
        if (m_Results == null)
            return;

        m_Results.dashCount = m_Results.dashCount + 1;
    }

    private void OnBallTouched()
    {
        if (m_Results == null)
            return;

        m_Results.ballTouches = m_Results.ballTouches + 1;
    }

    private void OnKickOccurred(tnKickable i_Target)
    {
        if (m_Results == null)
            return;

        if (i_Target == null)
            return;

        if (i_Target.CompareTag(Tags.s_Ball))
        {
            m_Results.shots = m_Results.shots + 1;

            if (m_OpponentGoal != null)
            {
                TSVector2 topPostPosition = m_OpponentGoal.topPostPosition;
                TSVector2 bottomPostPosition = m_OpponentGoal.bottomPostPosition;

                TSVector2 currentPosition = tsTransform2D.position;

                TSVector2 topPostDirection = topPostPosition - currentPosition;
                topPostDirection.Normalize();

                TSVector2 bottomPostDirection = bottomPostPosition - currentPosition;
                bottomPostDirection.Normalize();

                TSVector2 targetPosition = i_Target.tsTransform2D.position;

                TSVector2 targetDirection = targetPosition - currentPosition;
                targetDirection.Normalize();

                TSVector2 goalPosition = m_OpponentGoal.tsTransform2D.position;

                TSVector2 goalDirection = goalPosition - currentPosition;
                goalDirection.Normalize();

                FP dot = TSVector2.Dot(targetDirection, goalDirection);

                if (dot > 0f)
                {
                    FP angleA = TSVector2.Angle(topPostDirection, TSVector2.up);
                    FP angleB = TSVector2.Angle(bottomPostDirection, TSVector2.up);

                    FP shotAngle = TSVector2.Angle(targetDirection, TSVector2.up);

                    if (shotAngle > angleA && shotAngle < angleB)
                    {
                        m_Results.shotsOnTarget = m_Results.shotsOnTarget + 1;
                    }
                }
            }
        }
        else
        {
            if (i_Target.CompareTag(Tags.s_Character))
            {
                m_Results.tackles = m_Results.tackles + 1;
            }
        }
    }

    private void OnKickReceived()
    {
        if (m_Results == null)
            return;

        m_Results.tacklesReceived = m_Results.tacklesReceived + 1;
    }

    private void OnRespawnOccurred()
    {
        m_Respawned = true;
    }
}
