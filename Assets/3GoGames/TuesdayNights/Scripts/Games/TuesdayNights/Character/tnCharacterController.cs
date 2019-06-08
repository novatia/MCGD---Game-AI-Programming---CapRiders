using UnityEngine;

using System;

using TuesdayNights;

using TrueSync;

public delegate void OnDashTriggeredEvent();
public delegate void OnBallTouchedEvent();

[RequireComponent(typeof(TSRigidBody2D))]
public class tnCharacterController : TrueSyncBehaviour, tnISyncablePlayerInput
{
    // Serializable fields

    [Header("Movement")]

    [SerializeField]
    private FPRange m_MassRange = null;
    [SerializeField]
    private FPRange m_DragRange = null;
    [SerializeField]
    private FPRange m_MoveForceRange = null;
    [SerializeField]
    private FPRange m_MaxSpeedRange = null;

    [Header("Dash")]

    [SerializeField]
    private FPRange m_DashMassRange = null;
    [SerializeField]
    private FPRange m_DashDragRange = null;
    [SerializeField]
    private FPRange m_DashAppliedForceRange = null;
    [SerializeField]
    private FPRange m_DashCooldownRange = null;
    [SerializeField]
    private FPRange m_DashDurationRange = null;
    [SerializeField]
    private FPRange m_DashEnergyCostRange = null;
    [SerializeField]
    private int m_DashTickDelay = 0;

    [Header("Input")]

    [SerializeField]
    private bool m_EnableInputCompression = false;

    private static int s_InputPrecision = 32768;

    [Header("Effects")]

    [SerializeField]
    private Effect m_HitEffect = null;
    [SerializeField]
    private Effect m_BallHitEffect = null;
    [SerializeField]
    private float m_HitEffectInterval = 0.5f;

    [Header("Editor")]

    [SerializeField]
    private bool m_DrawGizmos = false;

    // Fields

    private int m_OriginalLayer = -1;

    private FP m_OriginalMass = FP.Zero;
    private FP m_OriginalDrag = FP.Zero;

    private FP m_MoveForce = FP.Zero;
    private FP m_MaxSpeed = FP.Zero;

    private FP m_DashMass = FP.Zero;
    private FP m_DashDrag = FP.Zero;
    private FP m_DashAppliedForce = FP.Zero;
    private FP m_DashCooldown = FP.Zero;
    private FP m_DashDuration = FP.Zero;
    private FP m_DashEnergyCost = FP.Zero;

    private int m_DashLayer = -1;

    [AddTracking]
    private int m_CurrentLayer = -1;
    [AddTracking]
    private FP m_CurrentMass = FP.FromFloat(1f);
    [AddTracking]
    private FP m_CurrentDrag = FP.Zero;

    [AddTracking]
    private FP m_DurationTimer = FP.Zero;
    [AddTracking]
    private FP m_CooldownTimer = FP.Zero;

    private float m_HitEffectTimer = 0f;

    [AddTracking]
    private bool m_RunSyncedUpdate = true;

    private float m_HorizontalAxis = 0f;
    private float m_VerticalAxis = 0f;

    [AddTracking]
    private int m_DashTickRequest = 0;
    private bool m_DashRequested = false;

    private byte m_HorizontalAxisCode = 0;
    private byte m_VerticalAxisCode = 0;
    private byte m_DashRequestedCode = 0;

    // COMPONENTS

    private TSRigidBody2D m_Rigidbody2D = null;
    private TSCollider2D m_Collider2D = null;

    private tnCharacterInput m_CharacterInput = null;
    private tnStatsContainer m_StatsContainer = null;
    private tnEnergy m_Energy = null;
    private tnRespawn m_Respawn = null;

    // ACCESSORS

    public bool runSyncedUpdate
    {
        get { return m_RunSyncedUpdate; }
        set { m_RunSyncedUpdate = value; }
    }

    public int currentLayer
    {
        get { return m_CurrentLayer; }
    }

    public int dashTickDelay
    {
        get
        {
            return m_DashTickDelay;
        }

        set
        {
            m_DashTickDelay = Mathf.Max(0, value);
        }
    }

    // EVENTS

    private event OnDashTriggeredEvent m_OnDashTriggered = null;
    public event OnDashTriggeredEvent onDashTriggered
    {
        add
        {
            m_OnDashTriggered += value;
        }

        remove
        {
            m_OnDashTriggered -= value;
        }
    }

    private event OnBallTouchedEvent m_OnBallTouched = null;
    public event OnBallTouchedEvent onBallTouched
    {
        add
        {
            m_OnBallTouched += value;
        }

        remove
        {
            m_OnBallTouched -= value;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        // Cache components.

        {
            m_Rigidbody2D = GetComponent<TSRigidBody2D>();
            m_Collider2D = GetComponent<TSCollider2D>();

            m_CharacterInput = GetComponent<tnCharacterInput>();
            m_StatsContainer = GetComponent<tnStatsContainer>();
            m_Energy = GetComponent<tnEnergy>();
            m_Respawn = GetComponent<tnRespawn>();
        }

        // Init data.

        {
            m_OriginalLayer = gameObject.layer;
            m_DashLayer = LayerMask.NameToLayer("Charging");
        }

        // Force values refresh.

        {
            ComputeAppliedForce(FP.Zero);
            ComputeMass(FP.Zero);
            ComputeDrag(FP.Zero);
            ComputeMaxSpeed(FP.Zero);

            ComputeDashMass(FP.Zero);
            ComputeDashDrag(FP.Zero);
            ComputeDashAppliedForce(FP.Zero);
            ComputeDashCooldown(FP.Zero);
            ComputeDashDuration(FP.Zero);
            ComputeDashEnergyCost(FP.Zero);
        }

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_CharacterController;
    }

    void OnEnable()
    {
        if (m_StatsContainer != null)
        {
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_Mass_StatId, OnMassStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_Drag_StatId, OnDragStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_MovForce_StatId, OnAppliedForceStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_MaxSpeed_StatId, OnMaxSpeedStatChanged);

            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_DashMass_StatId, OnDashMassStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_DashDrag_StatId, OnDragStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_DashForce_StatId, OnDashAppliedForceStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_DashCooldown_StatId, OnDashCooldownStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_DashDuration_StatId, OnDashDurationStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_DashEnergyCost_StatId, OnDashEnergyCostStatChanged);
        }

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }
    }

    void OnDisable()
    {
        if (m_StatsContainer != null)
        {
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_Mass_StatId, OnMassStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_Drag_StatId, OnDragStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_MovForce_StatId, OnAppliedForceStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_MaxSpeed_StatId, OnMaxSpeedStatChanged);

            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_DashMass_StatId, OnDashMassStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_DashDrag_StatId, OnDragStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_DashForce_StatId, OnDashAppliedForceStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_DashCooldown_StatId, OnDashCooldownStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_DashDuration_StatId, OnDashDurationStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_DashEnergyCost_StatId, OnDashEnergyCostStatChanged);
        }

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }
    }

    void Update()
    {
        if (isMine)
        {
            // Update axes.

            if (m_CharacterInput != null)
            {
                m_HorizontalAxis = m_CharacterInput.GetAxis(InputActions.s_HorizontalAxis);
                m_VerticalAxis = m_CharacterInput.GetAxis(InputActions.s_VerticalAxis);
            }
            else
            {
                m_HorizontalAxis = 0f;
                m_VerticalAxis = 0f;
            }

            // Update actions.

            if (!m_DashRequested)
            {
                float axisValue = Mathf.Max(Mathf.Abs(m_HorizontalAxis), Mathf.Abs(m_VerticalAxis));
                if (axisValue > 0.001f)
                {
                    if (m_CharacterInput != null)
                    {
                        bool dashTriggered = m_CharacterInput.GetButtonDown(InputActions.s_ShotButton);
                        if (dashTriggered)
                        {
                            m_DashRequested = true;
                        }
                    }
                }
            }
        }

        UpdateEffectTimer(Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        if (m_DurationTimer > FP.Zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        else
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        if (m_CooldownTimer > FP.Zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }

    // tnISyncablePlayerInput's interface

    public void SyncedInput(tnInput o_Input)
    {
        Vector2 input = new Vector2(m_HorizontalAxis, m_VerticalAxis);

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        if (m_EnableInputCompression)
        {
            int intX = (int)(input.x * s_InputPrecision);
            int intY = (int)(input.y * s_InputPrecision);

            o_Input.SetInt(m_HorizontalAxisCode, intX);
            o_Input.SetInt(m_VerticalAxisCode, intY);
        }
        else
        {
            o_Input.SetFP(m_HorizontalAxisCode, FP.FromFloat(input.x));
            o_Input.SetFP(m_VerticalAxisCode, FP.FromFloat(input.y));
        }

        if (m_DashRequested)
        {
            o_Input.SetByte(m_DashRequestedCode, (byte)1);
            m_DashRequested = false;
        }
    }

    // TrueSyncBehaviour's INTERFACE

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        m_CurrentLayer = m_OriginalLayer;
        m_CurrentMass = m_Rigidbody2D.mass;
        m_CurrentDrag = m_Rigidbody2D.linearDrag;

        tnCharacterInfo characterInfo = GetComponent<tnCharacterInfo>();
        int characterIndex = (characterInfo != null) ? characterInfo.characterIndex : 22; // First 22 (0 - 21) indices are for players. If there isn't character info, this is referee.
        int inputBase = characterIndex * 10;

        m_HorizontalAxisCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_HorizontalAxis);
        m_VerticalAxisCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_VerticalAxis);
        m_DashRequestedCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_Standard_CharacterController_DashRequested);
    }

    public override void OnPreSyncedUpdate()
    {
        base.OnPreSyncedUpdate();

        SetRigidbodyLayer(m_CurrentLayer);
        SetRigidbodyMass(m_CurrentMass);
        SetRigidbodyDrag(m_CurrentDrag);
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        if (!m_RunSyncedUpdate)
            return;

        // Read delta time.

        FP deltaTime = TrueSyncManager.deltaTimeMain;
        int currentTick = TrueSyncManager.ticksMain;

        // Update timers.

        UpdateTimers(deltaTime);

        // Read input.

        FP horizontalAxis;
        FP verticalAxis;

        if (m_EnableInputCompression)
        {
            int intX = TrueSyncInput.GetInt(m_HorizontalAxisCode);
            int intY = TrueSyncInput.GetInt(m_VerticalAxisCode);

            horizontalAxis = intX / (FP)s_InputPrecision;
            verticalAxis = intY / (FP)s_InputPrecision;
        }
        else
        {
            horizontalAxis = TrueSyncInput.GetFP(m_HorizontalAxisCode);
            verticalAxis = TrueSyncInput.GetFP(m_VerticalAxisCode);
        }

        // Handle actions.

        bool actionRequested = TrueSyncInput.HasByte(m_DashRequestedCode);
        bool cooldownOk = (m_CooldownTimer == FP.Zero);
        bool energyAvailable = (m_Energy != null && m_Energy.CanSpend(m_DashEnergyCost));

        if (actionRequested && m_DashTickRequest == 0 && energyAvailable  && cooldownOk)
        {
            // Cahce dash tick request.

            m_DashTickRequest = currentTick + m_DashTickDelay;

            // Consume energy.

            if (m_Energy != null)
            {
                m_Energy.Consume(m_DashEnergyCost);
            }
        }

        if ((m_DashTickRequest > 0) && (m_DashTickRequest == currentTick))
        {
            // Consume tick input.

            m_DashTickRequest = 0;

            // Override layer, mass and drag.

            int layerOverride = m_DashLayer;

            FP massOverride = m_DashMass;
            FP dragOverride = m_DashDrag;

            SetRigidbodyLayer(layerOverride);

            SetRigidbodyMass(massOverride);
            SetRigidbodyDrag(dragOverride);

            // Apply insant force.

            TSVector2 move = new TSVector2(horizontalAxis, verticalAxis);
            move.Normalize();

            TSVector2 moveForce = move * m_DashAppliedForce;
            m_Rigidbody2D.AddForce(moveForce);

            // Update timers.

            m_DurationTimer = m_DashDuration;
            m_CooldownTimer = MathFP.Max(m_DashCooldown, m_DurationTimer);

            // Raise event.

            if (m_OnDashTriggered != null)
            {
                m_OnDashTriggered();
            }
        }
        else // Handle movement.
        {
            if (m_DurationTimer == FP.Zero)
            {
                // Restore layer, mass and drag.

                SetRigidbodyLayer(m_OriginalLayer);

                SetRigidbodyMass(m_OriginalMass);
                SetRigidbodyDrag(m_OriginalDrag);

                // Compute movement force.

                TSVector2 move = new TSVector2(horizontalAxis, verticalAxis);
                TSVector2 moveForce = move * m_MoveForce;

                if (m_MaxSpeed > FP.Zero)
                {
                    TSVector2 currentVelocity = m_Rigidbody2D.velocity; 

                    TSVector2 moveAcceleration = moveForce / m_Rigidbody2D.mass;        // F = m * a    ==> a = F / m
                    TSVector2 deltaVelocity = moveAcceleration * deltaTime;             // a = dv / dt  ==> dv = a * dt

                    TSVector2 newVelocity = currentVelocity + deltaVelocity;

                    if (newVelocity.LengthSquared() > m_MaxSpeed * m_MaxSpeed)
                    {
                        // Modulate moveForce in order to reach a maximum speed of m_MaxSpeed.

                        TSVector2 maxVelocity = newVelocity.normalized * m_MaxSpeed;
                        TSVector2 maxAcceleration = (maxVelocity - currentVelocity) / deltaTime;

                        moveForce = maxAcceleration * m_Rigidbody2D.mass;
                    }
                }

                m_Rigidbody2D.AddForce(moveForce);
            }
        }
    }

    public override void OnSyncedCollisionEnter(TSCollision2D i_Collision)
    {
        if (i_Collision.collider.CompareTag(Tags.s_Ball))
        {
            if (m_OnBallTouched != null)
            {
                m_OnBallTouched();
            }
        }
    }

    public override void OnSyncedCollisionExit(TSCollision2D i_Collision)
    {
        if (m_HitEffectTimer == 0f)
        {
            if (!i_Collision.collider.CompareTag(Tags.s_Ball))
            {
                if (m_DurationTimer > 0f)
                {
                    EffectUtils.PlayEffect(m_HitEffect, transform.position, transform.rotation);
                    m_HitEffectTimer = m_HitEffectInterval;
                }
            }
            else
            {
                EffectUtils.PlayEffect(m_BallHitEffect, transform.position, transform.rotation);
                m_HitEffectTimer = m_HitEffectInterval;
            }
        }
    }

    // UTILS

    private void UpdateEffectTimer(float i_DeltaTime)
    {
        // Update hit effect timer.

        if (m_HitEffectTimer > 0f)
        {
            m_HitEffectTimer -= i_DeltaTime;

            if (m_HitEffectTimer < 0f)
            {
                m_HitEffectTimer = 0f;
            }
        }
    }

    private void UpdateTimers(FP i_DeltaTime)
    {
        // Update duration timer.

        if (m_DurationTimer > FP.Zero)
        {
            m_DurationTimer -= i_DeltaTime;

            if (m_DurationTimer < FP.Zero)
            {
                m_DurationTimer = FP.Zero;
            }
        }

        // Update cooldown timer.

        if (m_CooldownTimer > FP.Zero)
        {
            m_CooldownTimer -= i_DeltaTime;

            if (m_CooldownTimer < FP.Zero)
            {
                m_CooldownTimer = FP.Zero;
            }
        }
    }

    private void ForceStop()
    {
        // Restore layer, mass and drag.

        SetRigidbodyLayer(m_OriginalLayer);

        SetRigidbodyMass(m_OriginalMass);
        SetRigidbodyDrag(m_OriginalDrag);

        // Force zero velocity.

        SetRigidbodyVelocity(TSVector2.zero);

        // Consume pending action request.

        m_DurationTimer = FP.Zero;
        m_CooldownTimer = FP.Zero;

        m_HitEffectTimer = 0f;

        m_HorizontalAxis = 0f;
        m_VerticalAxis = 0f;

        // Clear action request

        m_DashTickRequest = 0;
        m_DashRequested = false;
    }

    private void SetRigidbodyLayer(int i_Layer)
    {
        if (!Layer.IsValidLayer(i_Layer))
            return;

        if (m_CurrentLayer != i_Layer)
        {
            m_Collider2D.SetCollisionLayer(i_Layer); // Apply changes to object's collider.
            m_CurrentLayer = i_Layer;
        }
    }

    private void SetRigidbodyMass(FP i_Mass)
    {
        if (m_Rigidbody2D.mass != i_Mass)
        {
            // Apply value to rigidbody.

            m_Rigidbody2D.mass = i_Mass;

            // Update cache.

            m_CurrentMass = i_Mass;
        }
    }

    private void SetRigidbodyDrag(FP i_Drag)
    {
        if (m_Rigidbody2D.linearDrag != i_Drag)
        {
            // Apply value to rigidbody.

            m_Rigidbody2D.linearDrag = i_Drag;

            // Update cache.

            m_CurrentDrag = i_Drag;
        }
    }

    private void SetRigidbodyVelocity(TSVector2 i_Velocity)
    {
        m_Rigidbody2D.velocity = i_Velocity;
    }

    // EVENTS

    private void OnRespawnOccurred()
    {
        ForceStop();
    }

    private void OnMassStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeMass(i_Value);
    }

    private void OnDragStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDrag(i_Value);
    }

    private void OnAppliedForceStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeAppliedForce(i_Value);
    }

    private void OnMaxSpeedStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeMaxSpeed(i_Value);
    }

    private void OnDashMassStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDashMass(i_Value);
    }

    private void OnDashDragStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDrag(i_Value);
    }

    private void OnDashAppliedForceStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDashAppliedForce(i_Value);
    }

    private void OnDashCooldownStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDashCooldown(i_Value);
    }

    private void OnDashDurationStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDashDuration(i_Value);
    }

    private void OnDashEnergyCostStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeDashEnergyCost(i_Value);
    }

    // STATS

    private void ComputeMass(FP i_StatValue)
    {
        m_OriginalMass = m_MassRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDrag(FP i_StatValue)
    {
        m_OriginalDrag = m_DragRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeAppliedForce(FP i_StatValue)
    {
        m_MoveForce = m_MoveForceRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeMaxSpeed(FP i_StatValue)
    {
        m_MaxSpeed = m_MaxSpeedRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDashMass(FP i_StatValue)
    {
        m_DashMass = m_DashMassRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDashDrag(FP i_StatValue)
    {
        m_DashDrag = m_DashDragRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDashAppliedForce(FP i_StatValue)
    {
        m_DashAppliedForce = m_DashAppliedForceRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDashCooldown(FP i_StatValue)
    {
        m_DashCooldown = m_DashCooldownRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDashDuration(FP i_StatValue)
    {
        m_DashDuration = m_DashDurationRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeDashEnergyCost(FP i_StatValue)
    {
        m_DashEnergyCost = m_DashEnergyCostRange.GetValueAt(i_StatValue / (FP)100);
    }
}