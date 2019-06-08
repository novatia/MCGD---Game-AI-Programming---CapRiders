using UnityEngine;

using System;
using System.Collections.Generic;

using TuesdayNights;

using TrueSync;

public delegate void KickCallback(tnKickable i_Target);

[RequireComponent(typeof(TSTransform2D))]
public class tnKick : TrueSyncBehaviour, tnISyncablePlayerInput
{
    // Serializable fields

    [Header("Params")]

    [SerializeField]
    private FPRange m_RadiusRange = null;
    [SerializeField]
    private FPRange m_EnergyCostRange = null;
    [SerializeField]
    private FPRange m_CooldownRange = null;

    [SerializeField]
    private int m_KickTickDelay = 0;

    [Header("Kick")]

    [SerializeField]
    private FPRange m_KickForceRange = null;
    [SerializeField]
    private bool m_KickInvertVelocity = false;

    [SerializeField]
    private LayerMask m_KickLayerMask = 0;

    [Header("Tackle")]

    [SerializeField]
    private FPRange m_TackleForceRange = null;
    [SerializeField]
    private bool m_TackleInvertVelocity = false;

    [SerializeField]
    private LayerMask m_TackleLayerMask = 0;

    [Header("Effects")]

    [SerializeField]
    private Effect m_Effect = null;
    [SerializeField]
    private Transform m_EffectPivot = null;
    [SerializeField]
    private bool m_PlayEffectWhenNothingIsHit = false;

    [Header("Editor")]

    [SerializeField]
    private bool m_DrawGizmos = false;

    // Fields

    private FP m_Radius = FP.FromFloat(0.55f);
    private FP m_EnergyCost = FP.FromFloat(0.05f);
    private FP m_Cooldown = FP.Zero;

    private FP m_KickForce = FP.Zero;
    private FP m_TackleForce = FP.Zero;

    [AddTracking]
    private FP m_CooldownTimer = FP.Zero;

    [AddTracking]
    private bool m_RunSyncedUpdate = true;

    [AddTracking]
    private int m_KickTickRequest = 0;
    private bool m_KickRequested = false;

    private byte m_ButtonDownCode = 0;

    private List<int> m_EffectTicks = new List<int>();

    private int m_ChargingLayer = 0;

    // COMPONENTS

    private tnCharacterController m_CharacterController = null;
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

    public int kickTickDelay
    {
        get { return m_KickTickDelay; }
        set { m_KickTickDelay = Mathf.Max(0, value); }
    }

    // EVENTS

    private event KickCallback m_KickEvent;
    public event KickCallback kickEvent
    {
        add
        {
            m_KickEvent += value;
        }

        remove
        {
            m_KickEvent -= value;
        }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        // Cache components.

        {
            m_CharacterController = GetComponent<tnCharacterController>();
            m_CharacterInput = GetComponent<tnCharacterInput>();
            m_StatsContainer = GetComponent<tnStatsContainer>();
            m_Energy = GetComponent<tnEnergy>();
            m_Respawn = GetComponent<tnRespawn>();
        }

        // Force values refresh.

        {
            ComputeRadius(FP.Zero);
            ComputeEnergyCost(FP.Zero);
            ComputeCooldown(FP.Zero);
            ComputeKickForce(FP.Zero);
            ComputeTackleForce(FP.Zero);
        }

        // Compute charging layer.

        m_ChargingLayer = LayerMask.NameToLayer("Charging");

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Kick;
    }

    void OnEnable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccured;
        }

        if (m_StatsContainer != null)
        {
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_KickEnergyCost_StatId, OnEnergyCostStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_KickRadius_StatId, OnRadiusStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_KickCooldown_StatId, OnCooldownStatChanged);

            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_KickForce_StatId, OnKickForceStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_KickTackleForce_StatId, OnTackleForceStatChanged);
        }
    }

    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccured;
        }

        if (m_StatsContainer != null)
        {
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_KickEnergyCost_StatId, OnEnergyCostStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_KickRadius_StatId, OnRadiusStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_KickCooldown_StatId, OnCooldownStatChanged);

            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_KickForce_StatId, OnKickForceStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_KickTackleForce_StatId, OnTackleForceStatChanged);
        }
    }

    void Update()
    {
        if (isMine)
        {
            // Check input.

            if (!m_KickRequested)
            {
                if (m_CharacterInput != null)
                {
                    bool actionTriggered = m_CharacterInput.GetButtonDown(InputActions.s_PassButton);
                    if (actionTriggered)
                    {
                        m_KickRequested = true;
                    }
                }
            }            
        }
    }

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_Radius.AsFloat());
    }

    // tnISyncablePlayerInput's interface

    public void SyncedInput(tnInput o_Input)
    {
        if (m_KickRequested)
        {
            o_Input.SetByte(m_ButtonDownCode, (byte)1);
            m_KickRequested = false;
        }
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        tnCharacterInfo characterInfo = GetComponent<tnCharacterInfo>();
        int characterIndex = (characterInfo != null) ? characterInfo.characterIndex : 0;
        int inputBase = characterIndex * 10;

        m_ButtonDownCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_Standard_Kick_KickRequested);
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

        // Handle kick.

        bool kickRequested = TrueSyncInput.HasByte(m_ButtonDownCode);
        bool cooldownOk = (m_CooldownTimer == FP.Zero);
        bool energyAvailable = (m_Energy != null && m_Energy.CanSpend(m_EnergyCost));

        if (kickRequested && m_KickTickRequest == 0 && energyAvailable && cooldownOk)
        {
            // Cache kick tick request.

            m_KickTickRequest = currentTick + m_KickTickDelay;

            // Consume energy

            if (m_Energy != null)
            {
                m_Energy.Consume(m_EnergyCost);
            }
        }

        if ((m_KickTickRequest > 0) && (m_KickTickRequest == currentTick))
        {
            m_KickTickRequest = 0;

            bool kick = Kick(m_KickLayerMask, m_KickForce, deltaTime, m_KickInvertVelocity);
            bool tackle = Kick(m_TackleLayerMask, m_TackleForce, deltaTime, m_TackleInvertVelocity);

            bool kickedAnything = kick || tackle;

            if (kickedAnything || m_PlayEffectWhenNothingIsHit)
            {
                int tick = TrueSyncManager.ticksMain;
                if (!m_EffectTicks.Contains(tick))
                {
                    if (m_EffectPivot != null)
                    {
                        EffectUtils.PlayEffect(m_Effect, m_EffectPivot);
                    }

                    m_EffectTicks.Add(tick);
                }
            }

            if (!kickedAnything)
            {
                m_CooldownTimer = FP.Zero;
            }
            else
            {
                m_CooldownTimer = m_Cooldown;
            }
        }

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

    // INTERNALS

    private bool Kick(LayerMask i_Mask, FP i_AppliedForce, FP i_DeltaTime, bool i_InvertVelocity = false)
    {
        bool kickedAnything = false;

        TSCollider2D[] colliders = TSPhysics2D.OverlapCircleAll(tsTransform2D.position, m_Radius, i_Mask);

        if (colliders != null)
        {
            for (int colliderIndex = 0; colliderIndex < colliders.Length; ++colliderIndex)
            {
                TSCollider2D currentCollider = colliders[colliderIndex];

                if (currentCollider == null)
                    continue;

                GameObject currentGo = currentCollider.gameObject;

                if (m_CharacterController != null)
                {
                    int currentLayer = m_CharacterController.currentLayer;

                    if (currentLayer == m_ChargingLayer)
                        continue;
                }

                tnKickable kickable = currentGo.GetComponent<tnKickable>();

                if (kickable == null)
                    continue;

                TSTransform2D kickableTransform = kickable.tsTransform2D;

                if (kickableTransform == null)
                    continue;

                TSVector2 kickablePosition = kickableTransform.position;

                kickedAnything = true;

                // Evaluate force.

                TSVector2 direction = kickablePosition - tsTransform2D.position;
                direction.Normalize();

                TSVector2 force = direction * i_AppliedForce;

                if (i_InvertVelocity)
                {
                    if (i_DeltaTime > FP.Zero)
                    {
                        FP otherMass = kickable.mass;
                        TSVector2 otherVelocity = kickable.currentVelocity;

                        TSVector2 oppositeForce = (otherMass * otherVelocity) / i_DeltaTime;
                        oppositeForce = oppositeForce * -FP.One;
                        force += oppositeForce;
                    }
                }

                // Kick.

                kickable.Kick(force, gameObject);

                // Notify listeners.

                if (m_KickEvent != null)
                {
                    m_KickEvent(kickable);
                }
            }
        }

        return kickedAnything;
    }

    // UTILS

    private void UpdateTimers(FP i_DeltaTime)
    {
        // Update cooldown timer.

        if (m_CooldownTimer >= FP.Zero)
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
        m_CooldownTimer = FP.Zero;

        m_KickTickRequest = 0;
        m_KickRequested = false;
    }

    // STATS

    private void ComputeRadius(FP i_StatValue)
    {
        m_Radius = m_RadiusRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeEnergyCost(FP i_StatValue)
    {
        m_EnergyCost = m_EnergyCostRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeCooldown(FP i_StatValue)
    {
        m_Cooldown = m_CooldownRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeKickForce(FP i_StatValue)
    {
        m_KickForce = m_KickForceRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeTackleForce(FP i_StatValue)
    {
        m_TackleForce = m_TackleForceRange.GetValueAt(i_StatValue / (FP)100);
    }

    // EVENTS

    private void OnRespawnOccured()
    {
        ForceStop();
    }

    private void OnRadiusStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeRadius(i_Value);
    }

    private void OnEnergyCostStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeEnergyCost(i_Value);
    }

    private void OnCooldownStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeCooldown(i_Value);
    }

    private void OnKickForceStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeKickForce(i_Value);
    }

    private void OnTackleForceStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeTackleForce(i_Value);
    }
}
