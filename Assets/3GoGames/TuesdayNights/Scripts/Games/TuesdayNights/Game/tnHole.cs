using UnityEngine;

using System;
using System.Collections.Generic;

using TuesdayNights;

using TrueSync;

public delegate void HoleCallback(TSCollider2D i_Entity);

[RequireComponent(typeof(TSTransform2D))]
public class tnHole : TrueSyncBehaviour
{
    // Types

    [Serializable]
    public class RespawnPoint
    {
        [SerializeField]
        private TSTransform2D m_Transform = null;
        [SerializeField]
        private TSVector2 m_ForceDirection = TSVector2.zero;

        [SerializeField]
        private FP m_ErrorAngle = FP.Zero;

        [SerializeField]
        private FP m_ForceIntensity = FP.Zero;

        public TSTransform2D transform
        {
            get { return m_Transform; }
        }

        public TSVector2 forceDirection
        {
            get { return m_ForceDirection; }
        }

        public TSVector2 respawnPosition
        {
            get
            {
                if (m_Transform != null)
                {
                    return m_Transform.position;
                }

                return TSVector2.zero;
            }
        }

        public FP errorAngle
        {
            get { return m_ErrorAngle; }
        }

        public FP forceIntensity
        {
            get { return m_ForceIntensity; }
        }
    }

    // Serializable fields

    [Header("Logic")]

    [SerializeField]
    private int m_HoleIndex = -1;

    [SerializeField]
    private FP m_Threshold = FP.FromFloat(0.25f);
    [SerializeField]
    private FP m_RespawnTime = FP.FromFloat(2f);

    [SerializeField]
    private RespawnPoint[] m_RespawnPoints = null;

    [Header("Effects")]

    [SerializeField]
    private Effect m_InEffect = null;
    [SerializeField]
    private Effect m_OutEffect = null;

    // Fields

    [AddTracking]
    private bool m_IsActive = true;

    private List<tnHoleTarget> m_Targets = new List<tnHoleTarget>();

    private DictionaryList<int, List<tnHoleTarget>> m_Pending = new DictionaryList<int, List<tnHoleTarget>>();

    // ACCESSORS

    public FP threshold
    {
        get { return m_Threshold; }
        set { m_Threshold = value; }
    }

    // MonoBehaviour's interface

    private void Awake()
    {
        sortOrder = BehaviourSortOrder.s_SortOrder_Hole; // Set sort order.
    }

    private void OnEnable()
    {
        Messenger.AddListener("PreFieldReset", OnPreFieldReset);
        Messenger.AddListener("KickOff", OnKickOff);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener("PreFieldReset", OnPreFieldReset);
        Messenger.RemoveListener("KickOff", OnKickOff);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_Threshold.AsFloat());

        if (m_RespawnPoints != null)
        {
            for (int respawnPointIndex = 0; respawnPointIndex < m_RespawnPoints.Length; ++respawnPointIndex)
            {
                RespawnPoint respawnPoint = m_RespawnPoints[respawnPointIndex];

                if (respawnPoint == null)
                    continue;

                TSVector2 normalizedDir = respawnPoint.forceDirection.normalized;

                // Draw main direction.

                if (respawnPoint.transform != null)
                {
                    Gizmos.color = Color.yellow;

                    Vector3 from = respawnPoint.transform.position.ToVector();
                    Vector3 to = from + new Vector3(normalizedDir.x.AsFloat(), normalizedDir.y.AsFloat(), 0f);
                    Gizmos.DrawLine(from, to);
                }

                if (MathFP.Abs(respawnPoint.errorAngle) > FP.Zero)
                {
                    // Draw error range.

                    {
                        Gizmos.color = Color.red;

                        TSVector2 normalizedDirLeft = normalizedDir.Rotate(-respawnPoint.errorAngle);

                        Vector3 from = respawnPoint.transform.position.ToVector();
                        Vector3 to = from + new Vector3(normalizedDirLeft.x.AsFloat(), normalizedDirLeft.y.AsFloat(), 0f);
                        Gizmos.DrawLine(from, to);
                    }

                    {
                        Gizmos.color = Color.red;

                        TSVector2 normalizedDirRight = normalizedDir.Rotate(respawnPoint.errorAngle);

                        Vector3 from = respawnPoint.transform.position.ToVector();
                        Vector3 to = from + new Vector3(normalizedDirRight.x.AsFloat(), normalizedDirRight.y.AsFloat(), 0f);
                        Gizmos.DrawLine(from, to);
                    }
                }
            }
        }
    }

    // TrueSyncBehaviour's interface

    public override void OnPreSyncedUpdate()
    {
        base.OnPreSyncedUpdate();

        int currentTick = TrueSyncManager.ticksMain;

        // Clear cache for this tick.

        List<tnHoleTarget> targets = m_Pending.GetValue(currentTick);
        if (targets != null)
        {
            targets.Clear();
        }
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        if (!m_IsActive || m_HoleIndex < 0)
            return;

        // Get Simulation info.

        int currentTick = TrueSyncManager.ticksMain;
        int rollbackWindow = TrueSyncManager.rollbackWindowMain;

        // Check current collision

        TSVector2 myPosition = tsTransform2D.position;

        for (int targetIndex = 0; targetIndex < m_Targets.Count; ++targetIndex)
        {
            tnHoleTarget holeTarget = m_Targets[targetIndex];

            if (holeTarget == null)
                continue;

            TSTransform2D otherTransform = holeTarget.GetComponent<TSTransform2D>();

            if (otherTransform == null)
                continue;

            TSVector2 targetPosition = otherTransform.position;

            TSVector2 positionDelta = targetPosition - myPosition;

            FP distance2 = positionDelta.LengthSquared();
            if (distance2 < m_Threshold * m_Threshold)
            {
                // Notify collision.

                holeTarget.CollidingWithHole();

                // Add object to pending list.

                if (holeTarget.canEnterHole && !holeTarget.isTeleporting)
                {
                    Internal_CacheTarget(currentTick, holeTarget);
                }
            }
        }

        // Check pending objects.

        for (int index = 0; index < m_Pending.count; ++index)
        {
            int tick = m_Pending.GetKey(index);

            if (currentTick == tick + rollbackWindow)
            {
                List<tnHoleTarget> holeTargets = m_Pending.GetValue(tick);
                if (holeTargets != null)
                {
                    for (int targetIndex = 0; targetIndex < holeTargets.Count; ++targetIndex)
                    {
                        tnHoleTarget holeTarget = holeTargets[targetIndex];

                        if (holeTarget == null)
                            continue;

                        RespawnPoint respawnPoint = GetRandomSpawnPoint();

                        if (respawnPoint == null || respawnPoint.transform == null)
                            continue;

                        TSTransform2D targetTransform = holeTarget.tsTransform2D;
                        TSRigidBody2D targetRigidbody = holeTarget.GetComponent<TSRigidBody2D>();

                        // Snap position.

                        if (targetRigidbody != null)
                        {
                            targetRigidbody.MovePosition(tsTransform2D.position);
                        }
                        else
                        {
                            targetTransform.position = tsTransform2D.position;
                        }

                        // Set rigidbody velocity,

                        if (targetRigidbody != null)
                        {
                            targetRigidbody.velocity = TSVector2.zero;
                        }

                        // Eavluate force

                        TSVector2 forceDirection = respawnPoint.forceDirection;
                        forceDirection.Normalize();

                        if (MathFP.Abs(respawnPoint.errorAngle) > FP.Zero)
                        {
                            int random = TSRandom.Range(0, 101);
                            FP t = ((FP)random) / 100;
                            FP randomError = MathFP.Lerp(-respawnPoint.errorAngle, respawnPoint.errorAngle, t);
                            forceDirection = forceDirection.Rotate(randomError);
                        }

                        TSVector2 outForce = forceDirection * respawnPoint.forceIntensity;

                        // Teleport.

                        holeTarget.Teleport(m_HoleIndex, respawnPoint.respawnPosition, outForce, m_RespawnTime, m_InEffect, m_OutEffect);
                    }
                }
            }
        }

        // Remove old data from dictionary.

        for (int index = 0; index < m_Pending.count; ++index)
        {
            int tick = m_Pending.GetKey(index);
            int executionTick = tick + rollbackWindow;

            bool isSafeTick = TrueSyncManager.IsTickOutOfRollbackMain(executionTick);
            if (isSafeTick)
            {
                m_Pending.Remove(tick);
                index = -1;
            }
        }
    }

    // LOGIC

    public void SetIsActive(bool i_Active)
    {
        if (m_HoleIndex < 0)
            return;

        if (i_Active != m_IsActive)
        {
            Clear();
            m_IsActive = i_Active;
        }
    }

    public void Clear()
    {
        for (int targetIndex = 0; targetIndex < m_Targets.Count; ++targetIndex)
        {
            tnHoleTarget holeTarget = m_Targets[targetIndex];

            if (holeTarget == null)
                continue;

            if (holeTarget.isTeleporting && holeTarget.enteringHoleIndex == m_HoleIndex)
            {
                holeTarget.CancelTeleport();
            }
        }
    }

    public void RegisterTarget(tnHoleTarget i_Target)
    {
        if (i_Target == null)
            return;

        m_Targets.Add(i_Target);
    }

    // INTERNALS

    private void Internal_CacheTarget(int i_Tick, tnHoleTarget i_Target)
    {
        if (i_Target == null)
            return;

        List<tnHoleTarget> targets;
        bool alreadyPresent = m_Pending.TryGetValue(i_Tick, out targets);
        if (targets == null)
        {
            targets = new List<tnHoleTarget>();
        }

        if (!alreadyPresent)
        {
            m_Pending.Add(i_Tick, targets);
        }

        targets.Add(i_Target);
    }

    // UTILS

    private RespawnPoint GetRandomSpawnPoint()
    {
        if (m_RespawnPoints == null || m_RespawnPoints.Length == 0)
        {
            return null;
        }

        int randomIndex = TSRandom.Range(0, m_RespawnPoints.Length);
        return m_RespawnPoints[randomIndex];
    }

    // EVENTS

    private void OnPreFieldReset()
    {
        SetIsActive(false);
    }

    private void OnKickOff()
    {
        SetIsActive(true);
    }
}