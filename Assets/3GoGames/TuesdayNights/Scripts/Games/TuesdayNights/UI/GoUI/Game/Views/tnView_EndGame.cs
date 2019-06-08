using UnityEngine;
using UnityEngine.UI;

using System;

using GoUI;

public class tnView_EndGame : GoUI.UIView
{
    // Serializable fields

    [Header("Selection panel")]

    [SerializeField]
    private GameObject m_SelectionPanel_Root = null;

    [SerializeField]
    private Button m_RematchButton = null;
    [SerializeField]
    private Button m_MainMenuButton = null;
    [SerializeField]
    private RectTransform m_SelectionPanelTimerRoot = null;
    [SerializeField]
    private Text m_SelectionPanelTimer = null;

    [Header("Waiting for players panel")]

    [SerializeField]
    private GameObject m_WaitingPlayers_Root = null;
    [SerializeField]
    private Text m_WaitingPlayersLabel = null;
    [SerializeField]
    private Text m_WaitingPlayersTimer = null;

    // Fields

    private event Action m_RematchButtonClickedEvent = null;
    private event Action m_MainMenuButtonClickedEvent = null;

    public event Action rematchButtonClickedEvent
    {
        add
        {
            m_RematchButtonClickedEvent += value;
        }

        remove
        {
            m_RematchButtonClickedEvent -= value;
        }
    }

    public event Action mainMenuButtonClickedEvent
    {
        add
        {
            m_MainMenuButtonClickedEvent += value;
        }

        remove
        {
            m_MainMenuButtonClickedEvent -= value;
        }
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();

        if (m_RematchButton != null)
        {
            m_RematchButton.onClick.AddListener(OnRematchButtonClicked);
        }

        if (m_MainMenuButton != null)
        {
            m_MainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (m_RematchButton != null)
        {
            m_RematchButton.onClick.RemoveListener(OnRematchButtonClicked);
        }

        if (m_MainMenuButton != null)
        {
            m_MainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }
    }

    // LOGIC

    public void ShowSelectionPanel()
    {
        Internal_SetSelectionPanelActive(true);
        Internal_SetWaitingForPlayersPanelActive(false);
    }

    public void ShowWaitingForPlayersPanel()
    {
        Internal_SetSelectionPanelActive(false);
        Internal_SetWaitingForPlayersPanelActive(true);
    }

    public void SetRematchButtonActive(bool i_Active)
    {
        Internal_SetRematchButtonActive(i_Active);
    }

    public void SetMainMenuButtonActive(bool i_Active)
    {
        Internal_SetMainMenuButtonActive(i_Active);
    }

    public void SetTimerActive(bool i_Active)
    {
        Internal_SetTimerActive(i_Active);
    }

    public void SetTimer(float i_Seconds)
    {
        int seconds = Mathf.FloorToInt(i_Seconds);
        Internal_SetTimer(seconds.ToString());
    }

    public void SetReadyPlayers(int i_ReadyPlayers, int i_TotalPlayers)
    {
        Internal_SetReadyPlayers(i_ReadyPlayers, i_TotalPlayers);
    }

    public void FocusMainMenuButton()
    {
        if (m_MainMenuButton != null)
        {
            UIEventSystem.SetFocusMain(m_MainMenuButton.gameObject);
        }
    }

    public void FocusRematchButton()
    {
        if (m_RematchButton != null)
        {
            UIEventSystem.SetFocusMain(m_RematchButton.gameObject);
        }
    }

    // INTERNALS

    private void Internal_SetRematchButtonActive(bool i_Active)
    {
        if (m_RematchButton != null)
        {
            m_RematchButton.gameObject.SetActive(i_Active);
        }
    }

    private void Internal_SetMainMenuButtonActive(bool i_Active)
    {
        if (m_MainMenuButton != null)
        {
            m_MainMenuButton.gameObject.SetActive(i_Active);
        }
    }

    private void Internal_SetTimerActive(bool i_Active)
    {
        if (m_SelectionPanelTimerRoot != null)
        {
            m_SelectionPanelTimerRoot.gameObject.SetActive(i_Active);
        }
    }

    private void Internal_SetTimer(string i_Timer)
    {
        if (m_SelectionPanelTimer != null)
        {
            m_SelectionPanelTimer.text = i_Timer;
        }

        if (m_WaitingPlayersTimer != null)
        {
            m_WaitingPlayersTimer.text = i_Timer;
        }
    }

    private void Internal_SetSelectionPanelActive(bool i_Active)
    {
        if (m_SelectionPanel_Root != null)
        {
            m_SelectionPanel_Root.SetActive(i_Active);
        }
    }

    private void Internal_SetWaitingForPlayersPanelActive(bool i_Active)
    {
        if (m_WaitingPlayers_Root != null)
        {
            m_WaitingPlayers_Root.SetActive(i_Active);
        }
    }

    private void Internal_SetReadyPlayers(int i_ReadyPlayers, int i_TotalPlayers)
    {
        if (m_WaitingPlayersLabel != null)
        {
            string label = "Waiting for players... (" + i_ReadyPlayers + "/" + i_TotalPlayers + ")";
            m_WaitingPlayersLabel.text = label;
        }
    }

    // EVENTS

    private void OnRematchButtonClicked()
    {
        if (m_RematchButtonClickedEvent != null)
        {
            m_RematchButtonClickedEvent();
        }
    }

    private void OnMainMenuButtonClicked()
    {
        if (m_MainMenuButtonClickedEvent != null)
        {
            m_MainMenuButtonClickedEvent();
        }
    }
}