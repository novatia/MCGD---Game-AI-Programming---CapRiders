using UnityEngine;

using System;
using System.Collections.Generic;

using GoUI;

public class tnPanel_TeamSelection : UIPanel<tnView_TeamSelection>
{
    private List<int> m_TeamsIds = new List<int>();

    private static int s_MaxPlayers = 2;
    private bool[] m_TeamConfirmed = new bool[s_MaxPlayers];

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();
        Internal_RegisterEvent();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
        Internal_UnregisterEvent();
    }

    // LOGIC

    public void ClearAll()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearAll();
        }

        m_TeamsIds.Clear();

        for (int index = 0; index < s_MaxPlayers; ++index)
        {
            m_TeamConfirmed[index] = false;
        }
    }

    public void SetPlayerColor(int i_Index, Color i_PlayerColor)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetPlayerColor(i_Index, i_PlayerColor);
    }

    public void AddTeam(int i_TeamId)
    {
        if (viewInstance == null)
            return;

        tnTeamData teamData = tnGameData.GetTeamDataMain(i_TeamId);

        if (teamData == null)
            return;

        viewInstance.AddTeamFlag(teamData);
        m_TeamsIds.Add(i_TeamId);
    }

    public void ForceTeamSelection(int i_TeamIndex, int i_SlotIndex)
    {
        if(viewInstance != null)
        {
            viewInstance.ForceTeamSelection(i_TeamIndex, i_SlotIndex);
        }
    }

    public void MoveSelection(int i_TeamIndex, UINavigationDirection i_Direction)
    {
        if (viewInstance != null)
        {
            if (i_TeamIndex >= 0 && i_TeamIndex < s_MaxPlayers)
            {
                if (!m_TeamConfirmed[i_TeamIndex])
                {
                    viewInstance.Move(i_TeamIndex, i_Direction);
                }
            }
        }
    }

    public void ConfirmTeam(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            if (i_TeamIndex >= 0 && i_TeamIndex < s_MaxPlayers)
            {
                viewInstance.ConfirmTeam(i_TeamIndex);
                m_TeamConfirmed[i_TeamIndex] = true;
            }
        }
    }

    public void CancelConfirmedTeam(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            if (i_TeamIndex >= 0 && i_TeamIndex < s_MaxPlayers)
            {
                viewInstance.CancelConfirmedTeam(i_TeamIndex);
                m_TeamConfirmed[i_TeamIndex] = false;
            }
        }
    }

    public bool GetTeamConfirm(int i_TeamIndex)
    {
        if (i_TeamIndex < 0 || i_TeamIndex >= s_MaxPlayers)
        {
            return false;
        }

        return m_TeamConfirmed[i_TeamIndex];
    }

    public int GetSelectedTeamId(int i_TeamIndex)
    {
        if(viewInstance != null)
        {
            int flagIndex = viewInstance.GetSelectedFlagIndex(i_TeamIndex);

            if (flagIndex < 0 || flagIndex >= m_TeamsIds.Count)
            {
                return -1;
            }

            return m_TeamsIds[flagIndex];
        }

        return Hash.s_NULL;
    }

    public void SetTimer(float i_Time)
    {
        if (viewInstance != null)
        {
            viewInstance.SetTimer(i_Time);
        }
    }

    public void SetTimer(double i_Time)
    {
        if (viewInstance != null)
        {
            viewInstance.SetTimer(i_Time);
        }
    }

    public void SetConfirmTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance == null)
            return;

        viewInstance.SetConfirmTriggerCanSend(i_CanSend);
    }

    // INTERNAL

    private void Internal_RegisterEvent()
    {

    }

    private void Internal_UnregisterEvent()
    {

    }
}
