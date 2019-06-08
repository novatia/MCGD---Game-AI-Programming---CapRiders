using UnityEngine;

using TuesdayNights;

using TrueSync;

public delegate void PreRespawnCallback();
public delegate void PostRespawnCallback();

public delegate void RespawnOccurredCallback();

[RequireComponent(typeof(TSTransform2D))]
public class tnRespawn : TrueSyncBehaviour
{
    private TSRigidBody2D m_Rigidbody2d = null;

    private TSVector2 m_StartPosition = TSVector2.zero;
    private FP m_StartRotation = 0;

    private event PreRespawnCallback m_PreRespawnEvent;
    public event PreRespawnCallback preRespawnEvent
    {
        add
        {
            m_PreRespawnEvent += value;
        }

        remove
        {
            m_PreRespawnEvent -= value;
        }
    }

    private event PostRespawnCallback m_PostRespawnEvent;
    public event PostRespawnCallback postRespawnEvent
    {
        add
        {
            m_PostRespawnEvent += value;
        }

        remove
        {
            m_PostRespawnEvent -= value;
        }
    }

    private event RespawnOccurredCallback m_RespawnOccurredEvent;
    public event RespawnOccurredCallback respawnOccurredEvent
    {
        add
        {
            m_RespawnOccurredEvent += value;
        }

        remove
        {
            m_RespawnOccurredEvent -= value;
        }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_Rigidbody2d = GetComponent<TSRigidBody2D>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Respawn;
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        m_StartPosition = tsTransform2D.position;
        m_StartRotation = tsTransform2D.rotation;
    }

    // LOGIC

    public void Respawn()
    {
        RespawnOn(m_StartPosition, m_StartRotation);
    }

    public void RespawnOn(TSTransform2D i_Transform)
    {
        RespawnOn(i_Transform.position, i_Transform.rotation);
    }

    public void RespawnOn(TSVector2 i_Position, FP i_Rotation)
    {
        InternalRespawn(i_Position, i_Rotation);
    }

    // INTERNALS

    private void InternalRespawn(TSVector2 i_Position, FP i_Rotation)
    {
        // Pre-Respawn.

        if (m_PreRespawnEvent != null)
        {
            m_PreRespawnEvent();
        }

        // Respawn - Move Rigidbody.

        if (m_Rigidbody2d != null)
        {
            FP mass = m_Rigidbody2d.mass;

            bool kinematic = m_Rigidbody2d.isKinematic;
            m_Rigidbody2d.isKinematic = true;

            m_Rigidbody2d.velocity = TSVector2.zero;
            m_Rigidbody2d.MovePosition(i_Position);
            m_Rigidbody2d.MoveRotation(i_Rotation);

            m_Rigidbody2d.isKinematic = kinematic;
            m_Rigidbody2d.mass = mass;
        }
        else
        {
            tsTransform2D.position = i_Position;
            tsTransform2D.rotation = i_Rotation;
        }

        // Notify listeners.

        if (m_RespawnOccurredEvent != null)
        {
            m_RespawnOccurredEvent();
        }

        // Post-Respawn.

        if (m_PostRespawnEvent != null)
        {
            m_PostRespawnEvent();
        }
    }
}
