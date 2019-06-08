using UnityEngine;

using TuesdayNights;

using TrueSync;

public class tnEnergy : TrueSyncBehaviour
{
    [Header("Params")]

    [SerializeField]
    private FPRange m_StartingValueRange = null;
    [SerializeField]
    private FPRange m_RecoveryRateRange = null;

    [SerializeField]
    [AddTracking]
    [DisallowEditInPlayMode]
    private bool m_AutoRecovery = false;

    [AddTracking]
    private FP m_Energy = FP.FromFloat(1f);

    private FP m_StartingValue = FP.FromFloat(0.5f);
    private FP m_RecoveryRate = FP.FromFloat(0.0825f);

    private FP m_ConsumedThisFrame = FP.Zero;

    // COMPONENTS

    private tnStatsContainer m_StatsContainer = null;
    private tnRespawn m_Respawn = null;

    // GETTERS

    public FP energy
    {
        get { return m_Energy; }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_StatsContainer = GetComponent<tnStatsContainer>();
        m_Respawn = GetComponent<tnRespawn>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Energy;
    }

    void Start()
    {
        Reset();
    }

    void OnEnable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccured;
        }

        if (m_StatsContainer != null)
        {
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_EnergyInitialValue_StatId, OnEnergyInitialValueStatChanged);
            m_StatsContainer.RegisterHandler(tnTeamStatsId.s_EnergyRecoveryRate_StatId, OnEnergyRecoveryRateStatChanged);
        }
    }

    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccured;
        }

        if (m_StatsContainer != null)
        {
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_EnergyInitialValue_StatId, OnEnergyInitialValueStatChanged);
            m_StatsContainer.UnregisterHandler(tnTeamStatsId.s_EnergyRecoveryRate_StatId, OnEnergyRecoveryRateStatChanged);
        }
    }

    // TrueSyncBehaviour's INTERFACE

    public override void OnPreSyncedUpdate()
    {
        base.OnPreSyncedUpdate();

        if (m_AutoRecovery)
        {
            FP deltaTime = TrueSyncManager.deltaTimeMain;
            m_Energy = MathFP.Clamp01(m_Energy + m_RecoveryRate * deltaTime);
        }

        m_ConsumedThisFrame = FP.Zero;
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        m_Energy = MathFP.Clamp01(m_Energy - m_ConsumedThisFrame);
    }

    // LOGIC

    public void Reset()
    {
        m_Energy = m_StartingValue;
    }

    public void Consume(FP i_Amount)
    {
        m_ConsumedThisFrame += i_Amount; // m_Energy = MathFP.Clamp01(m_Energy - i_Amount);
    }

    public void Restore(FP i_Amount)
    {
        m_Energy = MathFP.Clamp01(m_Energy + i_Amount);
    }

    public bool CanSpend(FP i_Amount)
    {
        return (m_Energy >= i_Amount);
    }

    public void SetAutorecoveryEnabled(bool i_Enabled)
    {
        m_AutoRecovery = i_Enabled;
    }

    // INTERNALS

    private void OnRespawnOccured()
    {
        Reset();
    }

    // EVENT

    private void OnEnergyInitialValueStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeEnergyInitialValue(i_Value);
    }

    private void OnEnergyRecoveryRateStatChanged(FP i_BaseValue, FP i_Value)
    {
        ComputeEnergyRecoveryRate(i_Value);
    }

    // STATS

    private void ComputeEnergyInitialValue(FP i_StatValue)
    {
        m_StartingValue = m_StartingValueRange.GetValueAt(i_StatValue / (FP)100);
    }

    private void ComputeEnergyRecoveryRate(FP i_StatValue)
    {
        m_RecoveryRate = m_RecoveryRateRange.GetValueAt(i_StatValue / (FP)100);
    }
}
