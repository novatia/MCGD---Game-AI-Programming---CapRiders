using UnityEngine;

using System;
using System.Collections.Generic;

using TuesdayNights;

using TrueSync;

public class tnTaunt : TrueSyncBehaviour, tnISyncablePlayerInput
{
    // Serializable fields

    [Header("Params")]

    [SerializeField]
    private FP m_Cooldown = FP.Zero;

    [Header("Effects")]

    [SerializeField]
    private Transform m_Pivot = null;
    [SerializeField]
    private Effect m_Effect = null;

    // Fields

    [AddTracking]
    private FP m_Timer = FP.Zero;

    [AddTracking]
    private bool m_RunSyncedUpdate = true;

    private bool m_ActionRequested = false;

    private byte m_ButtonDownCode = 0;

    private List<int> m_EffectTicks = new List<int>();

    // COMPONENTS

    private tnCharacterInput m_CharacterInput = null;

    // ACCESSORS

    public bool runSyncedUpdate
    {
        get { return m_RunSyncedUpdate; }
        set { m_RunSyncedUpdate = value; }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_CharacterInput = GetComponent<tnCharacterInput>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Taunt;
    }

    void Update()
    {
        if (isMine)
        {
            if (!m_ActionRequested)
            {
                if (m_CharacterInput != null)
                {
                    bool buttonDown = m_CharacterInput.GetButtonDown(InputActions.s_TauntButton);
                    if (buttonDown)
                    {
                        m_ActionRequested = true;
                    }
                }
            }
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
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        tnCharacterInfo characterInfo = GetComponent<tnCharacterInfo>();
        int characterIndex = (characterInfo != null) ? characterInfo.characterIndex : 0;
        int inputBase = characterIndex * 10;

        m_ButtonDownCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_Taunt_ButtonDown);
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        if (!m_RunSyncedUpdate)
            return;

        // Read delta time.

        FP deltaTime = TrueSyncManager.deltaTimeMain;

        // Update timers.

        UpdateTimers(deltaTime);

        // Handle actions.

        bool buttonDown = TrueSyncInput.HasByte(m_ButtonDownCode);
        if (buttonDown && (m_Timer == FP.Zero))
        {
            int tick = TrueSyncManager.ticksMain;
            if (!m_EffectTicks.Contains(tick))
            {
                if (m_Pivot != null)
                {
                    EffectUtils.PlayEffect(m_Effect, m_Pivot);
                }

                m_EffectTicks.Add(tick);
            }

            m_Timer = m_Cooldown;
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

    private void UpdateTimers(FP i_DeltaTime)
    {
        if (m_Timer > FP.Zero)
        {
            m_Timer -= i_DeltaTime;

            if (m_Timer < FP.Zero)
            {
                m_Timer = FP.Zero;
            }
        }
    }
}
