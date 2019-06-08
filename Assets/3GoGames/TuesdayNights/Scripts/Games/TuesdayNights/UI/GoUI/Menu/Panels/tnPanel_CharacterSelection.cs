using UnityEngine;

using System.Collections.Generic;

using GoUI;

public class tnPanel_CharacterSelection : UIPanel<tnView_CharacterSelection>
{
    // UIPanel's interface

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

    // LOGIC

    public void ClearAll()
    {
        if (viewInstance != null)
        {
            viewInstance.ClearAll();
        }
    }

    public void Setup()
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

        if (teamsModule == null)
            return;

        for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
        {
            tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

            if (teamDescription == null)
                continue;

            Color teamColor = teamDescription.teamColor;

            if (viewInstance != null)
            {
                viewInstance.SetTeamColor(teamIndex, teamColor);
            }

            int captainPlayerIndex = teamDescription.captainOnlinePlayerIndex;

            for (int characterIndex = 0; characterIndex < teamDescription.charactersCount; ++characterIndex)
            {
                tnCharacterDescription characterDescription = teamDescription.GetCharacterDescription(characterIndex);

                if (characterDescription == null)
                    continue;

                List<int> onlinePlayersKeys = tnGameData.GetOnlinePlayersKeysMain();

                if (onlinePlayersKeys == null)
                    continue;

                int onlinePlayerIndex = characterDescription.onlinePlayerIndex;

                if (onlinePlayerIndex < 0 || onlinePlayerIndex >= onlinePlayersKeys.Count)
                    continue;

                int onlinePlayerKey = onlinePlayersKeys[onlinePlayerIndex];

                if (Hash.IsNullOrEmpty(onlinePlayerKey))
                    continue;

                tnOnlinePlayerData onlinePlayerData = tnGameData.GetOnlinePlayerDataMain(onlinePlayerKey);

                if (onlinePlayerData == null)
                    continue;

                Color playerColor = onlinePlayerData.color;

                if (viewInstance != null)
                {
                    viewInstance.SetPlayerIsHuman(teamIndex, characterIndex, true); // For now AI isnt' present online.
                    viewInstance.SetPlayerColor(teamIndex, characterIndex, playerColor);

                    if (onlinePlayerIndex == captainPlayerIndex)
                    {
                        viewInstance.SetCaptainColor(teamIndex, playerColor);
                    }
                }
            }
        }

        for (int teamIndex = 0; teamIndex < teamsModule.teamsCount; ++teamIndex)
        {
            tnTeamDescription teamDescription = teamsModule.GetTeamDescription(teamIndex);

            if (teamDescription == null)
                continue;

            if (viewInstance != null)
            {
                viewInstance.SetupTeam(teamIndex, teamDescription);
            }
        }
    }

    public void Move(int i_TeamIndex, UINavigationDirection i_Direction)
    {
        if(viewInstance != null)
        {
            viewInstance.Move(i_TeamIndex, i_Direction);
        }
    }

    public void Confirm(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            viewInstance.Confirm(i_TeamIndex);
        }
    }

    public void Cancel(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            viewInstance.Cancel(i_TeamIndex);
        }
    }

    public void Ready(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            viewInstance.Ready(i_TeamIndex);
        }
    }

    public void ForceSelection(int i_TeamIndex, int i_SlotIndex)
    {
        if(viewInstance != null)
        {
            viewInstance.ForceSelection(i_TeamIndex, i_SlotIndex);
        }
    }

    public bool GetTeamReady(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            return viewInstance.GetTeamReady(i_TeamIndex);
        }

        return false;
    }

    public bool HasCharacterSelected(int i_TeamIndex)
    {
        if (viewInstance != null)
        {
            return viewInstance.HasCharacterSelected(i_TeamIndex);
        }

        return false;
    }

    public List<int> GetLineUpIds(int i_TeamIndex)
    {
        if(viewInstance != null)
        {
            return viewInstance.GetLineUpIds(i_TeamIndex);
        }

        return null;
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

    public void SetProceedTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance != null)
        {
            viewInstance.SetProceedTriggerCanSend(i_CanSend);
        }
    }

    public void SetBackTriggerCanSend(bool i_CanSend)
    {
        if (viewInstance != null)
        {
            viewInstance.SetBackTriggerCanSend(i_CanSend);
        }
    }
}
