using UnityEngine;

using System;

using GoUI;

public enum tnEndGamePanelState
{
    None,
    OnlineSelection,
    OfflineSelection,
    WaitingForPlayers,
    TimedOut,
}

public class tnPanel_EndGame : UIPanel<tnView_EndGame>
{
    // Fields

    private tnEndGamePanelState m_State = tnEndGamePanelState.None;

    private event Action m_RematchRequestedEvent = null;
    private event Action m_MainMenuRequestedEvent = null;

    public event Action rematchRequestedEvent
    {
        add
        {
            m_RematchRequestedEvent += value;
        }

        remove
        {
            m_RematchRequestedEvent -= value;
        }
    }

    public event Action mainMenuRequestedEvent
    {
        add
        {
            m_MainMenuRequestedEvent += value;
        }

        remove
        {
            m_MainMenuRequestedEvent -= value;
        }
    }

    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (viewInstance != null)
        {
            viewInstance.rematchButtonClickedEvent += OnViewRematch;
            viewInstance.mainMenuButtonClickedEvent += OnViewMainMenu;
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (viewInstance != null)
        {
            viewInstance.rematchButtonClickedEvent -= OnViewRematch;
            viewInstance.mainMenuButtonClickedEvent -= OnViewMainMenu;
        }
    }

    // LOGIC

    public void SetTimer(float i_Seconds)
    {
        float seconds = Mathf.Max(i_Seconds, 0f);

        if (viewInstance != null)
        {
            viewInstance.SetTimer(seconds);
        }
    }

    public void SetState(tnEndGamePanelState i_State)
    {
        Internal_SetState(i_State);
    }

    public void SetReadyPlayers(int i_ReadyPlayers, int i_TotalPlayers)
    {
        if (viewInstance != null)
        {
            viewInstance.SetReadyPlayers(i_ReadyPlayers, i_TotalPlayers);
        }
    }

    // INTERNALS

    private void Internal_SetState(tnEndGamePanelState i_State)
    {
        if (i_State == tnEndGamePanelState.None)
            return;

        if (i_State == m_State)
            return;

        if (viewInstance != null)
        {
            switch (i_State)
            {
                case tnEndGamePanelState.OnlineSelection:

                    viewInstance.ShowSelectionPanel();
                    viewInstance.SetTimerActive(true);
                    viewInstance.SetRematchButtonActive(true);
                    viewInstance.SetMainMenuButtonActive(true);

                    break;

                case tnEndGamePanelState.OfflineSelection:

                    viewInstance.ShowSelectionPanel();
                    viewInstance.SetTimerActive(false);
                    viewInstance.SetRematchButtonActive(true);
                    viewInstance.SetMainMenuButtonActive(true);

                    break;

                case tnEndGamePanelState.WaitingForPlayers:

                    viewInstance.ShowWaitingForPlayersPanel();

                    break;

                case tnEndGamePanelState.TimedOut:

                    viewInstance.ShowSelectionPanel();
                    viewInstance.SetTimerActive(false);
                    viewInstance.SetRematchButtonActive(false);
                    viewInstance.SetMainMenuButtonActive(true);

                    viewInstance.FocusMainMenuButton();

                    break;
            }
        }

        m_State = i_State;
    }

    // EVENTS

    private void OnViewRematch()
    {
        if (m_RematchRequestedEvent != null)
        {
            m_RematchRequestedEvent();
        }
    }

    private void OnViewMainMenu()
    {
        if (m_MainMenuRequestedEvent != null)
        {
            m_MainMenuRequestedEvent();
        }
    }
}
