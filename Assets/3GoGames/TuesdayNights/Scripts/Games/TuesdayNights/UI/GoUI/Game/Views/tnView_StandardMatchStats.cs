using UnityEngine;

using System.Collections.Generic;

using GoUI;

public class tnView_StandardMatchStats : GoUI.UIView
{
    // Serializable fields

    [Header("Prefabs")]

    [SerializeField]
    private tnStatsPanel m_StatsEntryPrefab = null;

    [Header("Logic")]
    [SerializeField]
    [DisallowEditInPlayMode]
    private int m_MaxEntries = 4;
    [SerializeField]
    private RectTransform m_ContentRoot = null;

    // Fields

    private tnStatsPanel[] m_Entries = null;

    // ACCESSORS

    public int entryCount
    {
        get { return (m_Entries != null) ? m_Entries.Length : 0; }
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        if (m_ContentRoot != null)
        {
            if (m_StatsEntryPrefab != null)
            {
                int entryCount = Mathf.Max(1, m_MaxEntries);
                m_Entries = new tnStatsPanel[entryCount];

                for (int index = 0; index < m_Entries.Length; ++index)
                {
                    tnStatsPanel entry = Instantiate<tnStatsPanel>(m_StatsEntryPrefab);
                    entry.transform.SetParent(m_ContentRoot, false);

                    m_Entries[index] = entry;
                }

                DisableAllEntries();
            }
        }
    }

    // LOGIC

    public void SetEntry(int i_Index, string i_PlayerName, Sprite i_Portrait, string i_StatLabel, string i_StatValue, string i_PartecipationLabel, string i_PartecipationValue, Color i_TeamColor)
    {
        Internal_SetEntry(i_Index, i_PlayerName, i_Portrait, i_StatLabel, i_StatValue, i_PartecipationLabel, i_PartecipationValue, i_TeamColor);
    }

    public void ClearEntry(int i_Index)
    {
        Internal_ClearEntry(i_Index);
    }

    public void DisableAllEntries()
    {
        if (m_Entries == null)
            return;

        for (int index = 0; index < m_Entries.Length; ++index)
        {
            tnStatsPanel entry = m_Entries[index];

            if (entry == null)
                continue;

            entry.gameObject.SetActive(false);
        }
    }

    public void EnableAllEntries()
    {
        if (m_Entries == null)
            return;

        for (int index = 0; index < m_Entries.Length; ++index)
        {
            tnStatsPanel entry = m_Entries[index];

            if (entry == null)
                continue;

            entry.gameObject.SetActive(true);
        }
    }

    // INTERNALS

    private void Internal_SetEntry(int i_Index, string i_PlayerName, Sprite i_Portrait, string i_StatLabel, string i_StatValue, string i_PartecipationLabel, string i_PartecipationValue, Color i_TeamColor)
    {
        tnStatsPanel entry = GetEntry(i_Index);

        if (entry == null)
            return;

        entry.gameObject.SetActive(true);

        entry.SetPlayerName(i_PlayerName);
        entry.SetPortrait(i_Portrait);
        entry.SetStatLabel(i_StatLabel);
        entry.SetStatValue(i_StatValue);
        entry.SetPartecipationLabel(i_PartecipationLabel);
        entry.SetPartecipationValue(i_PartecipationValue);
        entry.SetTeamColor(i_TeamColor);
    }

    private void Internal_ClearEntry(int i_Index)
    {
        tnStatsPanel entry = GetEntry(i_Index);

        if (entry == null)
            return;

        entry.gameObject.SetActive(false);
    }

    // UTILS

    private tnStatsPanel GetEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Entries.Length)
        {
            return null;
        }

        return m_Entries[i_Index];
    }
}