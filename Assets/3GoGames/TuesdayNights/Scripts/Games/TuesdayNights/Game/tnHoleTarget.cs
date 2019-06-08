using UnityEngine;

using System.Collections.Generic;

using TuesdayNights;

using TrueSync;

[RequireComponent(typeof(TSTransform2D))]
[RequireComponent(typeof(TSCollider2D))]
public class tnHoleTarget : TrueSyncBehaviour
{
    // Fields

    [AddTracking]
    private int m_EnteringHoleIndex = -1;

    [AddTracking]
    private bool m_Teleporting = false;
    [AddTracking]
    private FP m_Timer = FP.Zero;
    [AddTracking]
    private int m_TeleportTick = 0;

    [AddTracking]
    private bool m_PrevCollidingWithHole = false;
    [AddTracking]
    private bool m_CollidingWithHole = false;

    [AddTracking]
    private FP m_Delay = FP.Zero;
    [AddTracking]
    private TSVector2 m_OutForce = TSVector2.zero;
    [AddTracking]
    private TSVector2 m_RespawnPosition = TSVector2.zero;

    [AddTracking]
    private FP m_CachedMass = FP.Zero;
    [AddTracking]
    private bool m_CachedKinematic = false;

    private List<int> m_OutEffectTicks = new List<int>();

    private Effect m_OutEffect = null;
    private int m_TeleportRequestTick = 0;

    // Components

    private TSCollider2D m_Collider = null;
    private TSRigidBody2D m_Rigidbody = null;

    // ACCESSORS

    public bool isTeleporting
    {
        get { return m_Teleporting; }
    }

    public int enteringHoleIndex
    {
        get
        {
            return m_EnteringHoleIndex;
        }
    }

    public TSCollider2D tsCollider2D
    {
        get
        {
            return m_Collider;
        }
    }

    public bool canEnterHole
    {
        get
        {
            return !m_PrevCollidingWithHole;
        }
    }

    private Vector3 currentPosition
    {
        get
        {
            Vector2 pos2 = tsTransform2D.position.ToVector();
            float z = transform.position.z;
            Vector3 pos3 = new Vector3(pos2.x, pos2.y, z);

            return pos3;
        }
    }

    // MonoBehaviour's interface

    private void Awake()
    {
        m_Collider = GetComponent<TSCollider2D>();
        m_Rigidbody = GetComponent<TSRigidBody2D>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_HoleTarget;
    }

    // TrueSyncBehaviour's interface

    public override void OnPreSyncedUpdate()
    {
        base.OnPreSyncedUpdate();

        m_PrevCollidingWithHole = m_CollidingWithHole;
        m_CollidingWithHole = false;
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        int currentTick = TrueSyncManager.ticksMain;
        int rollbackWindow = TrueSyncManager.rollbackWindowMain;

        // Get delta time.

        FP deltaTime = TrueSyncManager.deltaTimeMain;

        // If Teleport tick, check if it is safe. If teleporting, update timer.

        if (m_Teleporting && m_TeleportTick == 0)
        {
            m_Timer += deltaTime;

            if (m_Timer >= m_Delay)
            {
                m_TeleportTick = TrueSyncManager.ticksMain;
            }
        }

        if (m_TeleportTick != 0)
        {
            if (m_TeleportTick + rollbackWindow == currentTick)
            {
                Respawn();
            }
        }

        // Clear out effect ticks cache.

        for (int index = 0; index < m_OutEffectTicks.Count; ++index)
        {
            int tick = m_OutEffectTicks[index];
            if (TrueSyncManager.IsTickOutOfRollbackMain(tick))
            {
                m_OutEffectTicks.RemoveAt(index);
                index = -1;
            }
        }
    }

    // LOGIC

    public void Teleport(int i_HoleIndex, TSVector2 i_Position, TSVector2 i_OutForce, FP i_Delay, Effect i_InEffect = null, Effect i_OutEffect = null)
    {
        if (m_Teleporting)
            return;

        int tick = TrueSyncManager.ticksMain;

        // Play effect.

        if (m_TeleportRequestTick != tick)
        {
            EffectUtils.PlayEffect(i_InEffect, currentPosition);
        }

        // Cache rigidbody data and set it kinematic.

        if (m_Rigidbody != null)
        {
            m_CachedMass = m_Rigidbody.mass;
            m_CachedKinematic = m_Rigidbody.isKinematic;

            m_Rigidbody.isKinematic = true;
        }

        // Setup class variables.

        m_EnteringHoleIndex = i_HoleIndex;

        m_Teleporting = true;
        m_Timer = FP.Zero;
        m_TeleportRequestTick = tick;

        m_Delay = i_Delay;
        m_OutForce = i_OutForce;
        m_RespawnPosition = i_Position;

        m_OutEffect = i_OutEffect;

        // If no delay, teleport instant.

        if (i_Delay == FP.Zero)
        {
            Respawn();
        }
    }

    public void CancelTeleport()
    {
        if (!m_Teleporting)
            return;

        if (m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = m_CachedKinematic;
            m_Rigidbody.mass = m_CachedMass;
        }

        m_EnteringHoleIndex = -1;
        m_TeleportTick = 0;

        m_Teleporting = false;
        m_Timer = FP.Zero;

        m_Delay = FP.Zero;
        m_OutForce = TSVector2.zero;
        m_RespawnPosition = TSVector2.zero;

        m_OutEffect = null;
        m_TeleportRequestTick = 0;
    }

    public void CollidingWithHole()
    {
        m_CollidingWithHole = true;
    }

    // INTERNALS

    private void Respawn()
    {
        int tick = TrueSyncManager.ticksMain;

        // If rigidbody, re-config it. Move otherwise.

        if (m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = m_CachedKinematic;
            m_Rigidbody.mass = m_CachedMass;

            m_Rigidbody.MovePosition(m_RespawnPosition);
            m_Rigidbody.AddForce(m_OutForce);
        }
        else
        {
            tsTransform2D.position = m_RespawnPosition;
        }

        // Play Effect.

        if (!m_OutEffectTicks.Contains(tick))
        {
            Effect outEffect = m_OutEffect;
            EffectUtils.PlayEffect(outEffect, currentPosition);

            m_OutEffectTicks.Add(tick);
        }

        // Clear class variables.

        m_EnteringHoleIndex = -1;
        m_TeleportTick = 0;

        m_Teleporting = false;
        m_Timer = FP.Zero;

        m_Delay = FP.Zero;
        m_OutForce = TSVector2.zero;
        m_RespawnPosition = TSVector2.zero;

        m_OutEffect = null;
        m_TeleportRequestTick = 0;
    }
}