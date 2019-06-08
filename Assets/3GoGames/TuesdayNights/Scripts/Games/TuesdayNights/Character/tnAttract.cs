using UnityEngine;

using System;

using TuesdayNights;

using TrueSync;

public class tnAttract : TrueSyncBehaviour, tnISyncablePlayerInput
{
    // Serializable fields
    
    [Header("Params")]

    [SerializeField]
    private LayerMask m_LayerMask = 0;

    [SerializeField]
    private FPRange m_EnergyCostRange = null;
    [SerializeField]
    private FPRange m_ForceMagnitudeRange = null;
    [SerializeField]
    private FPRange m_RadiusRange = null;

    [Header("Graphics")]

    [SerializeField]
    private GameObject m_Graphics = null;

    // Fields

    private FP m_EnergyCostRate = FP.FromFloat(0.165f);
    private FP m_ForceMagnitude = FP.FromFloat(-195f);
    private FP m_Radius = FP.FromFloat(0.85f);

    [AddTracking]
    private bool m_RunSyncedUpdate = true;

    [AddTracking]
    private bool m_Running = false;

    private bool m_ActionRequested = false;
    private bool m_ButtonPressed = false;

    private byte m_ButtonDownCode = 0;
    private byte m_ButtonPressedCode = 0;

    // COMPONENTS

    private tnCharacterInput m_CharacterInput = null;
    private tnStatsContainer m_StatsContainer = null;
    private tnEnergy m_Energy = null;
    private tnRespawn m_Respawn = null;

    // ACCESSORS

    public bool isActive
    {
        get
        {
            return m_Running;
        }
    }

    public bool runSyncedUpdate
    {
        get { return m_RunSyncedUpdate; }
        set { m_RunSyncedUpdate = value; }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        // Cache components.

        {
            m_CharacterInput = GetComponent<tnCharacterInput>();
            m_StatsContainer = GetComponent<tnStatsContainer>();
            m_Energy = GetComponent<tnEnergy>();
            m_Respawn = GetComponent<tnRespawn>();
        }


        // Force values refresh.

        {
            ComputeEnergyCostRate(FP.Zero);
            ComputeForceMagnitude(FP.Zero);
            ComputeRadius(FP.Zero);
        }

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Attract;
    }

    void OnEnable()
    {
        // Force stop.

        {
            m_Running = false;
        }

        // Bind callbacks.

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }

        if (m_StatsContainer != null)
        {
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_AttractEnergyCostRate_StatId, OnEnergyCostRateStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_AttractForce_StatId, OnForceMagnitudeStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_AttractRadius_StatId, OnRadiusStatChanged);
        }
    }

    void OnDisable()
    {
        // Force stop.

        {
            m_Running = false;
        }

        // Release callbacks.

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }

        if (m_StatsContainer != null)
        {
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_AttractEnergyCostRate_StatId, OnEnergyCostRateStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_AttractForce_StatId, OnForceMagnitudeStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_AttractRadius_StatId, OnRadiusStatChanged);
        }
    }

    void Update()
    {
        if (isMine)
        {
            if (!m_ActionRequested)
            {
                if (m_CharacterInput != null)
                {
                    m_ActionRequested = m_CharacterInput.GetButtonDown(InputActions.s_AttractButton);
                }
                else
                {
                    m_ActionRequested = false;
                }
            }

            if (m_CharacterInput != null)
            {
                m_ButtonPressed = m_CharacterInput.GetButton(InputActions.s_AttractButton);
            }
            else
            {
                m_ButtonPressed = false;
            }
        }

        if (m_Graphics != null)
        {
            m_Graphics.SetActive(m_Running);
        }
    }

    // tnISyncablePlayerInput's interface

    public void SyncedInput(tnInput o_Input)
    {
        if (m_ActionRequested)
        {
            o_Input.SetByte(m_ButtonDownCode, (byte)1);
            m_ActionRequested = false;
        }

        o_Input.SetByte(m_ButtonPressedCode, m_ButtonPressed ? (byte)1 : (byte)0);
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        tnCharacterInfo characterInfo = GetComponent<tnCharacterInfo>();
        int characterIndex = (characterInfo != null) ? characterInfo.characterIndex : 0;
        int inputBase = characterIndex * 10;

        m_ButtonDownCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_Standard_Attract_ButtonDown);
        m_ButtonPressedCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_Standard_Attract_ButtonPressed);
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        if (!m_RunSyncedUpdate)
            return;

        // Get delta time.

        FP deltaTime = TrueSyncManager.deltaTimeMain;

        // Handle attract.

        if (m_Running)
        {
            bool buttonPressed = TrueSyncInput.GetByte(m_ButtonPressedCode) > 0;
            if (buttonPressed)
            {
                FP cost = m_EnergyCostRate * deltaTime;
                cost = MathFP.Max(cost, FP.Zero);

                if (m_Energy != null && m_Energy.CanSpend(cost))
                {
                    m_Energy.Consume(cost);
                }
                else
                {
                    m_Running = false;
                }
            }
            else
            {
                m_Running = false;
            }
        }
        else
        {
            bool buttonDown = TrueSyncInput.HasByte(m_ButtonDownCode);
            if (buttonDown)
            {
                FP cost = m_EnergyCostRate * deltaTime;
                cost = MathFP.Max(cost, FP.Zero);

                if (m_Energy != null && m_Energy.CanSpend(cost))
                {
                    m_Energy.Consume(cost);

                    m_Running = true;
                }
            }
        }

        // Update effector.

        if (m_Running)
        {
            UpdateEffector();
        }
    }

    public override void OnGameEnded()
    {
        base.OnGameEnded();

        m_Running = false;
    }

    // EVENTS

    private void OnRespawnOccurred()
    {
        m_Running = false;
    }

    private void OnEnergyCostRateStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeEnergyCostRate(i_Value);
    }

    private void OnForceMagnitudeStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeForceMagnitude(i_Value);
    }

    private void OnRadiusStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeRadius(i_Value);
    }

    // STATS

    private void ComputeEnergyCostRate(FP i_StatValue)
    {
        m_EnergyCostRate = m_EnergyCostRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeForceMagnitude(FP i_StatValue)
    {
        m_ForceMagnitude = m_ForceMagnitudeRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeRadius(FP i_StatValue)
    {
        m_Radius = m_RadiusRange.GetValueAt(i_StatValue / (FP)100);
    }

    // INTERNALS

    public void UpdateEffector()
    {
        TSCollider2D[] colliders = TSPhysics2D.OverlapCircleAll(tsTransform2D.position, m_Radius, m_LayerMask);

        if (colliders != null)
        {
            if (colliders.Length > 0)
            {
                for (int index = 0; index < colliders.Length; ++index)
                {
                    TSCollider2D currentCollider = colliders[index];

                    if (currentCollider == null)
                        continue;

                    TSTransform2D transform2d = currentCollider.tsTransform;

                    TSVector2 direction = transform2d.position - tsTransform2D.position;
                    direction = direction.normalized;

                    TSRigidBody2D rigidbody = currentCollider.GetComponent<TSRigidBody2D>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(direction * m_ForceMagnitude);
                    }
                }
            }
        }
    }
}
