using UnityEngine;

using TuesdayNights;

using TrueSync;

[RequireComponent(typeof(TSRigidBody2D))]
[RequireComponent(typeof(TSCollider2D))]
public class tnBall : TrueSyncBehaviour
{
    // Serializable fields

    [Header("Effects")]

    [SerializeField]
    private Effect m_HitEffect = null;
    [SerializeField]
    private Effect m_CharacterHitEffect = null;
    [SerializeField]
    private float m_HitEffectInterval = 0.2f;

    // Fields

    private bool m_CanRotate = true;
    private bool m_RotationEnabled = false;

    private float m_HitEffectTimer = 0f;

    private TSRigidBody2D m_Rigidbody2D = null;

    private MeshRenderer m_Mesh = null;

    private tnRespawn m_Respawn = null;

    private bool m_SafeRespownOutField = false;

    private TSVector2 m_MinBoundLimits = TSVector2.zero;
    private TSVector2 m_MaxBoundLimits = TSVector2.zero;

    // MonoBehaviour's INTERFACE

    void Awake()
    { 
        // Transform, Rigidbody and collider.

        m_Rigidbody2D = GetComponent<TSRigidBody2D>();

        // Mesh.

        m_Mesh = GetComponentInChildren<MeshRenderer>();

        // Extra components.

        m_Respawn = GetComponent<tnRespawn>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Ball;
    }

    void Start()
    {
        ForceStop();
    }

    void OnEnable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }
    }

    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }
    }

    void Update()
    {
        // Update effect timer.

        if (m_HitEffectTimer > 0f)
        {
            m_HitEffectTimer -= Time.deltaTime;

            if (m_HitEffectTimer < 0f)
            {
                m_HitEffectTimer = 0f;
            }
        }

        // Update rotation.

        if (m_Mesh != null && m_CanRotate && m_RotationEnabled)
        {
            TSVector2 velocity = m_Rigidbody2D.velocity;

            float localScaleX = m_Mesh.transform.localScale.x;
            float localScaelY = m_Mesh.transform.localScale.y;

            if (localScaleX > Mathf.Epsilon && localScaelY > Mathf.Epsilon)
            {
                float rotSpeedY = -(velocity.x.AsFloat()) / localScaleX;
                float rotSpeedX = (velocity.y.AsFloat()) / localScaelY;

                float rotAngleY = Mathf.Rad2Deg * rotSpeedY * Time.deltaTime;
                float rotAngleX = Mathf.Rad2Deg * rotSpeedX * Time.deltaTime;

                m_Mesh.transform.Rotate(new Vector3(rotAngleX, rotAngleY, 0f), Space.World);
            }
        }
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        m_RotationEnabled = true;
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        // Check if ball is out of field.

        if (m_SafeRespownOutField)
        {
            if (tsTransform2D.position.x < m_MinBoundLimits.x || tsTransform2D.position.x > m_MaxBoundLimits.x)
            {
                ForceRespawn();
            }
            else
            {
                if (tsTransform2D.position.y < m_MinBoundLimits.y || tsTransform2D.position.y > m_MaxBoundLimits.y)
                {
                    ForceRespawn();
                }
            }
        }
    }

    public override void OnSyncedCollisionExit(TSCollision2D i_Collision)
    {
        if (i_Collision.gameObject.CompareTag(Tags.s_Character))
        {
            if (m_HitEffectTimer == 0f)
            {
                EffectUtils.PlayEffect(m_CharacterHitEffect, transform.position, transform.rotation);

                m_HitEffectTimer = m_HitEffectInterval;
            }
        }
        else
        {
            if (m_HitEffectTimer == 0f)
            {
                EffectUtils.PlayEffect(m_HitEffect, transform.position, transform.rotation);

                m_HitEffectTimer = m_HitEffectInterval;
            }
        }
    }

    public override void OnGameEnded()
    {
        m_RotationEnabled = false;

        base.OnGameEnded();
    }

    // LOGIC

    public void SetBoundLimits(TSVector2 i_Min, TSVector2 i_Max)
    {
        m_MinBoundLimits = i_Min;
        m_MaxBoundLimits = i_Max;
    }

    public void SetSafeRespawnOutField(bool i_Active)
    {
        m_SafeRespownOutField = i_Active;
    }

    public void SetCanRotate(bool i_CanRotate)
    {
        m_CanRotate = i_CanRotate;
    }

    // EVENTS

    private void OnRespawnOccurred()
    {
        ForceStop();
    }

    // INTERNALS

    private void ForceMeshRotation(Quaternion i_Rotation)
    {
        if (m_Mesh == null)
            return;

        m_Mesh.transform.rotation = i_Rotation;
    }

    private void ForceStop()
    {
        ForceMeshRotation(Quaternion.identity);

        m_HitEffectTimer = 0f;
    }

    private void ForceRespawn()
    {
        if (m_Respawn != null)
        {
            m_Respawn.Respawn();
        }
    }
}