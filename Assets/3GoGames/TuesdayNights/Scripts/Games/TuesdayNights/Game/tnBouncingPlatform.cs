using UnityEngine;

using System.Collections.Generic;

using TuesdayNights;

using TrueSync;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(TSTransform2D))]
public class tnBouncingPlatform : TrueSyncBehaviour
{
    // Serializable fields

    [Header("Logic")]

    [SerializeField]
    private float m_Force = 10000f;

    [Header("Effects")]

    [SerializeField]
    private Effect m_Effect = null;

    // Fields

    private List<int> m_EffectTicks = new List<int>();

    // Components

    private Animator m_Animator = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Animator = GetComponent<Animator>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_BouncingPlatform;
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        // Clear effect ticks cache.

        for (int index = 0; index < m_EffectTicks.Count; ++index)
        {
            int tick = m_EffectTicks[index];

            if (TrueSyncManager.IsTickOutOfRollbackMain(tick))
            {
                m_EffectTicks.RemoveAt(index);
                index = -1;
            }
        }
    }

    public override void OnSyncedCollisionEnter(TSCollision2D i_Collision)
    {
        int tick = TrueSyncManager.ticksMain;

        // Apply forces.

        TSRigidBody2D rigidbody2D = i_Collision.gameObject.GetComponent<TSRigidBody2D>();
        if (rigidbody2D != null)
        {
            // Clear velocity.

            rigidbody2D.velocity = TSVector2.zero;

            // Add bounce force.

            TSVector2 otherPosition = rigidbody2D.position;
            TSVector2 myPostion = tsTransform2D.position;

            TSVector2 direction = otherPosition - myPostion;
            direction.Normalize();

            TSVector2 force = direction * m_Force;

            rigidbody2D.AddForce(force);
        }

        // Play effect.

        if (!m_EffectTicks.Contains(tick))
        {
            m_Animator.SetTrigger("Hit");
            EffectUtils.PlayEffect(m_Effect, transform);

            m_EffectTicks.Add(tick);
        }
    }   
}